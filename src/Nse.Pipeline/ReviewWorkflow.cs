using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

static class ReviewWorkflow
{
    internal static readonly string[] Categories = ["objects", "clothing", "setting", "actions"];
    internal static readonly JsonSerializerOptions Json = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true,
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow
    };

    public static async Task<int> Run(string[] args)
    {
        var importing = args[0] == "import-review";
        var minimum = importing ? 3 : 2;
        if (args.Length != minimum && (args.Length != minimum + 2 || args[minimum] != "--output"))
            throw new ArgumentException("Usage: review <profile.json> [--output <new-directory>] or import-review <profile.json> <annotations.json> [--output <new-directory>]");
        var profilePath = Path.GetFullPath(args[1]);
        var profileBytes = await File.ReadAllBytesAsync(profilePath);
        var profile = JsonNode.Parse(profileBytes)?.AsObject() ?? throw new InvalidDataException("Invalid profile.");
        var profileHash = Convert.ToHexString(SHA256.HashData(profileBytes)).ToLowerInvariant();
        var sourceHash = profile["source"]?["sha256"]?.GetValue<string>() ?? throw new InvalidDataException("Profile has no source hash.");
        var frames = profile["frames"]?.Deserialize<List<FrameSample>>(Json) ?? throw new InvalidDataException("Profile has no frames.");
        if (frames.Count == 0 || frames.Select(f => f.Image).Distinct().Count() != frames.Count || frames.Any(f => !double.IsFinite(f.TimestampSeconds)))
            throw new InvalidDataException("Profile requires uniquely referenced frames with finite timestamps.");
        AnnotationDocument document;
        if (importing)
        {
            document = JsonSerializer.Deserialize<AnnotationDocument>(await File.ReadAllTextAsync(args[2]), Json)
                ?? throw new InvalidDataException("Invalid annotation document.");
            Validate(document, sourceHash, profileHash, frames);
        }
        else
        {
            document = new("0.1", "visual-0.1", sourceHash, profileHash, new("", "human"),
                frames.Select(f => new FrameReview(f.Image, f.TimestampSeconds,
                    Categories.Select(c => new CategoryReview(c, "not_reviewed", [])).ToList())).ToList());
        }
        var output = Path.GetFullPath(args.Length == minimum + 2 ? args[minimum + 1] : Path.Combine("data", "processed", "runs", $"review-{DateTime.UtcNow:yyyyMMdd-HHmmss}-{Guid.NewGuid():N}"));
        if (Directory.Exists(output) || File.Exists(output)) throw new ArgumentException("Output must be a new directory; existing reviews are never overwritten.");
        // Embed reviewed evidence so review/report pages remain usable when opened directly from disk.
        var images = new Dictionary<string, string>();
        var root = Path.GetDirectoryName(profilePath)!;
        foreach (var frame in frames)
        {
            var imagePath = ResolveImage(root, frame.Image);
            images[frame.Image] = "data:image/jpeg;base64," + Convert.ToBase64String(await File.ReadAllBytesAsync(imagePath));
        }
        Directory.CreateDirectory(output);
        await File.WriteAllTextAsync(Path.Combine(output, "status.json"), "{\"status\":\"running\"}");
        await File.WriteAllTextAsync(Path.Combine(output, "annotations.json"), JsonSerializer.Serialize(document, Json));
        await File.WriteAllTextAsync(Path.Combine(output, "review.html"), ReviewPage.Render(document, images, false));
        if (importing)
        {
            // Keep the extracted measurements and their paths intact; record the source profile location.
            profile["annotationReview"] = JsonSerializer.SerializeToNode(new
            {
                document.SchemaVersion, document.CodebookVersion, document.SourceSha256, document.ProfileSha256,
                document.Reviewer, importedUtc = DateTimeOffset.UtcNow,
                measurementProfilePath = profilePath,
                evidenceBaseDirectory = root,
                status = document.Frames.All(f => f.Categories.All(c => c.Status != "not_reviewed")) ? "complete" : "partial"
            }, Json);
            profile["annotations"] = JsonSerializer.SerializeToNode(document.Frames, Json);
            if (profile["limitations"] is JsonArray limitations)
                for (var i = 0; i < limitations.Count; i++)
                    if (limitations[i]?.GetValue<string>() == "No speech, object, scene, emotion, or semantic analysis has been performed.")
                        limitations[i] = "The measurement stage performed no speech, object, scene, emotion, or semantic analysis. Reviewed frame annotations are stored separately.";
            await File.WriteAllTextAsync(Path.Combine(output, "profile.reviewed.json"), profile.ToJsonString(Json));
            await File.WriteAllTextAsync(Path.Combine(output, "report.html"), ReviewPage.Render(document, images, true));
        }
        await File.WriteAllTextAsync(Path.Combine(output, "status.json"), "{\"status\":\"complete\"}");
        Console.WriteLine($"{(importing ? "Imported review" : "Review workspace ready")}: {output}");
        return 0;
    }

    static string ResolveImage(string root, string relative)
    {
        if (string.IsNullOrWhiteSpace(relative) || Path.IsPathRooted(relative)) throw new InvalidDataException("Frame image must be a relative path.");
        var full = Path.GetFullPath(Path.Combine(root, relative));
        var prefix = Path.GetFullPath(root) + Path.DirectorySeparatorChar;
        if (!full.StartsWith(prefix, OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal))
            throw new InvalidDataException("Frame image escapes the profile directory.");
        // Reject links so imported profiles cannot pull unrelated files into the embedded review page.
        for (var current = full; current != Path.GetFullPath(root); current = Path.GetDirectoryName(current)!)
            if ((File.GetAttributes(current) & FileAttributes.ReparsePoint) != 0) throw new InvalidDataException("Linked frame paths are unsupported.");
        return full;
    }

    static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidDataException(message);
    }

    static bool Text(string? value, int max) => !string.IsNullOrWhiteSpace(value) && value.Length <= max;

    static void Validate(AnnotationDocument doc, string sourceHash, string profileHash, List<FrameSample> frames)
    {
        Require(doc.SchemaVersion == "0.1" && doc.CodebookVersion == "visual-0.1", "Unsupported annotation schema or codebook version.");
        Require(doc.SourceSha256 == sourceHash && doc.ProfileSha256 == profileHash, "Annotations belong to a different source or profile. Open the matching review workspace.");
        Require(doc.Reviewer != null && Text(doc.Reviewer.Name, 200) && new[] { "human", "ai_assisted" }.Contains(doc.Reviewer.Kind), "Reviewer name and kind (human or ai_assisted) are required.");
        Require(doc.Frames != null && doc.Frames.Count == frames.Count, "Every sampled frame must appear exactly once.");
        var seen = new HashSet<string>();
        foreach (var frame in doc.Frames!)
        {
            Require(frame != null && frame.FrameImage != null && seen.Add(frame.FrameImage), "Duplicate or missing frame reference.");
            var expected = frames.SingleOrDefault(f => f.Image == frame!.FrameImage);
            Require(expected != null && frame!.TimestampSeconds == expected.TimestampSeconds, "Unknown frame or changed timestamp.");
            Require(frame!.Categories != null && frame.Categories.Count == Categories.Length, "All four categories are required for every frame.");
            var categories = new HashSet<string>();
            foreach (var category in frame.Categories!)
            {
                Require(category != null && Categories.Contains(category.Category) && categories.Add(category.Category), "Unknown or duplicate category.");
                Require(new[] { "not_reviewed", "none_visible", "observed", "uncertain" }.Contains(category!.Status), "Invalid review status.");
                Require(category.Observations != null && category.Observations.Count <= 100, "An observations array with at most 100 entries is required.");
                Require(category.Status is "observed" or "uncertain" ? category.Observations!.Count > 0 : category.Observations!.Count == 0,
                    "Observed/uncertain categories require evidence; not_reviewed/none_visible categories must have no observations.");
                foreach (var observation in category.Observations!)
                {
                    Require(observation != null && Text(observation.Label, 200) && Text(observation.Evidence, 2000), "Each observation needs a label and visible evidence.");
                    Require(new[] { "clear", "uncertain" }.Contains(observation!.Certainty), "Certainty must be clear or uncertain.");
                }
                if (category.Status == "uncertain") Require(category.Observations!.Any(o => o.Certainty == "uncertain"), "An uncertain category needs an uncertain observation.");
                if (category.Status == "observed") Require(category.Observations!.All(o => o.Certainty == "clear"), "Use uncertain status when any observation is uncertain.");
            }
        }
    }
}

sealed record AnnotationDocument(
    [property: JsonRequired] string SchemaVersion,
    [property: JsonRequired] string CodebookVersion,
    [property: JsonRequired] string SourceSha256,
    [property: JsonRequired] string ProfileSha256,
    [property: JsonRequired] Reviewer Reviewer,
    [property: JsonRequired] List<FrameReview> Frames);
sealed record Reviewer([property: JsonRequired] string Name, [property: JsonRequired] string Kind);
sealed record FrameReview([property: JsonRequired] string FrameImage, [property: JsonRequired] double TimestampSeconds, [property: JsonRequired] List<CategoryReview> Categories);
sealed record CategoryReview([property: JsonRequired] string Category, [property: JsonRequired] string Status, [property: JsonRequired] List<Observation> Observations);
sealed record Observation([property: JsonRequired] string Label, [property: JsonRequired] string Evidence, [property: JsonRequired] string Certainty);
