using System.Text.Json;
using System.Text.RegularExpressions;

namespace Argon.Engine;

public sealed record TranscriptDocument(
    string SchemaVersion,
    string SourceVideo,
    string SourceSha256,
    string Language,
    string Context,
    List<BeliefDimensionDef> BeliefDimensions,
    List<TranscriptSegment> Segments);

public sealed record BeliefDimensionDef(string Id, string Name, string Description);

public sealed record TranscriptSegment(
    string Id,
    double StartSeconds,
    double EndSeconds,
    string SpeakerId,
    string SpeakerLabel,
    string Role,
    string Transcript,
    VisualEvidenceInfo VisualEvidence,
    List<ArchetypeEntry> Archetypes,
    Dictionary<string, double> StanceVector,
    double InstitutionalAnchoring);

public sealed record VisualEvidenceInfo(
    int[] FrameRange,
    string Setting,
    string Clothing,
    List<string> Objects);

public sealed record ArchetypeEntry(string Role, string Entity, string Evidence);

public sealed record LinguisticWindowMetrics(
    int WindowIndex,
    string SegmentId,
    string SpeakerId,
    string SpeakerLabel,
    double WordsPerMinute,
    double TypeTokenRatio,
    double NormativeMarkerDensity,
    double DialecticalConnectorDensity,
    double InstitutionalAnchoring,
    double NormalizedLinguisticIntensity,
    Dictionary<string, double> ActiveStanceVector);

public static partial class LinguisticAnalyzer
{
    private static readonly HashSet<string> NormativeLexicon = new(StringComparer.OrdinalIgnoreCase)
    {
        // Spanish normative/value markers (CORPUS-001)
        "insumisión", "insumiso", "insumisos", "guerra", "guerras", "conflicto", "conflictos",
        "ejército", "obligatorio", "obligaciones", "profesional", "profesionales", "eliminar",
        "intervención", "país", "militar", "social", "sustitutorio", "mili", "ley", "leyes",
        "impone", "impuestos", "consecuente", "problemas", "libre", "libertad", "objetores",
        "conciencia", "insolidario", "español", "positiva", "positivo", "rechazar", "presa", "presos", "juicios",
        // English normative/value/crisis markers (CORPUS-002 & CORPUS-003)
        "stakes", "world", "children", "live", "dark", "love", "die", "vote", "president",
        "union", "mourning", "tragedy", "pain", "country", "national", "loss", "courage",
        "dangers", "heroes", "nation", "daring", "brave", "grace", "spirit", "challenge",
        "truths", "serve", "pioneers", "exploration", "discovery", "future", "fainthearted",
        "faith", "respect", "freedom", "public", "hopes", "dedication", "honored", "god"
    };

    private static readonly HashSet<string> DialecticalConnectors = new(StringComparer.OrdinalIgnoreCase)
    {
        // Spanish & English dialectical/subordinating connectors
        "pero", "sino", "aunque", "porque", "embargo", "bien", "tampoco", "si", "como", "cuando",
        "but", "or", "either", "perhaps", "yet", "nor", "while", "if", "because", "though"
    };

    public static List<LinguisticWindowMetrics> AnalyzeWindows(
        TranscriptDocument doc,
        IReadOnlyList<(double StartSeconds, double EndSeconds)> windows)
    {
        var results = new List<LinguisticWindowMetrics>();

        for (var idx = 0; idx < windows.Count; idx++)
        {
            var (wStart, wEnd) = windows[idx];
            var mid = 0.5 * (wStart + wEnd);

            var seg = doc.Segments.FirstOrDefault(s => mid >= s.StartSeconds && mid <= s.EndSeconds)
                      ?? doc.Segments.OrderBy(s => Math.Abs(0.5 * (s.StartSeconds + s.EndSeconds) - mid)).First();

            var segDurationMin = Math.Max(0.05, (seg.EndSeconds - seg.StartSeconds) / 60.0);
            var words = TokenizeWords(seg.Transcript);
            var wordCount = Math.Max(1, words.Count);
            var wpm = words.Count / segDurationMin;

            var uniqueWords = new HashSet<string>(words, StringComparer.OrdinalIgnoreCase);
            var ttr = (double)uniqueWords.Count / wordCount;

            var normHits = words.Count(w => NormativeLexicon.Contains(w));
            var dialHits = words.Count(w => DialecticalConnectors.Contains(w));

            var normDensity = (double)normHits / wordCount;
            var dialDensity = (double)dialHits / wordCount;

            var normWpm = Math.Clamp(wpm / 210.0, 0.0, 1.0);
            var normMarkerScore = Math.Clamp(normDensity / 0.16, 0.0, 1.0);
            var normDialScore = Math.Clamp(dialDensity / 0.12, 0.0, 1.0);
            var stanceMag = Math.Sqrt(seg.StanceVector.Values.Select(v => v * v).DefaultIfEmpty(0).Average());

            var lingIntensity = Math.Clamp(
                0.35 * normWpm + 0.35 * normMarkerScore + 0.15 * normDialScore + 0.15 * stanceMag,
                0.0, 1.0);

            results.Add(new LinguisticWindowMetrics(
                idx,
                seg.Id,
                seg.SpeakerId,
                seg.SpeakerLabel,
                Math.Round(wpm, 1),
                Math.Round(ttr, 3),
                Math.Round(normDensity, 4),
                Math.Round(dialDensity, 4),
                Math.Round(seg.InstitutionalAnchoring, 3),
                Math.Round(lingIntensity, 4),
                new Dictionary<string, double>(seg.StanceVector)));
        }

        return results;
    }

    public static List<string> TokenizeWords(string text)
    {
        // Strip bracketed interviewer notes when counting speaker tokens, or keep words for density
        var cleaned = Regex.Replace(text, @"\[.*?\]", " ");
        var matches = WordRegex().Matches(cleaned);
        return matches.Select(m => m.Value.ToLowerInvariant()).ToList();
    }

    [GeneratedRegex(@"[\p{L}\p{N}]+")]
    private static partial Regex WordRegex();
}
