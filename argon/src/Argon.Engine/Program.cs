using System.Globalization;
using System.Text;
using System.Text.Json;
using Argon.Engine;

var jsonOptions = new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true,
    WriteIndented = true
};

var repoRoot = Directory.GetCurrentDirectory();
var profileJsonPath = args.Length > 0
    ? Path.GetFullPath(args[0])
    : Path.Combine(repoRoot, "data", "processed", "runs", "06-FCE-V-C110-007-C-profile", "profile.json");

var transcriptJsonPath = args.Length > 1
    ? Path.GetFullPath(args[1])
    : Path.Combine(repoRoot, "argon", "experiments", "EXP-001-insumision-dynamics", "transcript.aligned.json");

var outputDir = args.Length > 2
    ? Path.GetFullPath(args[2])
    : Path.Combine(repoRoot, "argon", "experiments", "EXP-001-insumision-dynamics");

if (!File.Exists(profileJsonPath))
    throw new FileNotFoundException($"Base profile not found: {profileJsonPath}");
if (!File.Exists(transcriptJsonPath))
    throw new FileNotFoundException($"Aligned transcript not found: {transcriptJsonPath}");

var profileDir = Path.GetDirectoryName(profileJsonPath)!;
using var baseProfileDoc = JsonDocument.Parse(await File.ReadAllTextAsync(profileJsonPath));
var root = baseProfileDoc.RootElement;
var durationSeconds = root.GetProperty("source").GetProperty("durationSeconds").GetDouble();
var wavRelative = root.GetProperty("audio").GetProperty("path").GetString()
                  ?? throw new InvalidDataException("Profile has no extracted audio WAV.");
var wavPath = Path.Combine(profileDir, wavRelative);

var frameList = new List<(int Index, double Timestamp, double Start, double End, string Image, string Raster)>();
var idx = 0;
foreach (var f in root.GetProperty("frames").EnumerateArray())
{
    frameList.Add((
        idx++,
        f.GetProperty("timestampSeconds").GetDouble(),
        f.GetProperty("windowStartSeconds").GetDouble(),
        f.GetProperty("windowEndSeconds").GetDouble(),
        f.GetProperty("image").GetString()!,
        f.GetProperty("colorRaster").GetString()!));
}

var transcriptDoc = JsonSerializer.Deserialize<TranscriptDocument>(
    await File.ReadAllTextAsync(transcriptJsonPath), jsonOptions)
    ?? throw new InvalidDataException("Failed to parse transcript.aligned.json");

Console.WriteLine($"[Argon.Engine] Analyzing {frameList.Count} windows ({durationSeconds:F2}s) from {Path.GetFileName(profileJsonPath)}...");

var windows = frameList.Select(f => (f.Start, f.End)).ToList();
var acousticMetrics = AcousticAnalyzer.AnalyzeWav(wavPath, windows);
var visualMetrics = VisualDynamicsAnalyzer.AnalyzeRasters(
    profileDir,
    frameList.Select(f => (f.Index, f.Timestamp, f.Image, f.Raster)).ToList());
var linguisticMetrics = LinguisticAnalyzer.AnalyzeWindows(transcriptDoc, windows);

var dynamicsProfile = NarrativeMetricsEngine.ComputeFullProfile(
    transcriptDoc,
    durationSeconds,
    acousticMetrics,
    visualMetrics,
    linguisticMetrics);

Directory.CreateDirectory(outputDir);

var outProfilePath = Path.Combine(outputDir, "dynamics.profile.json");
await File.WriteAllTextAsync(outProfilePath, JsonSerializer.Serialize(dynamicsProfile, jsonOptions));

var csv = new StringBuilder();
csv.AppendLine("window_index,start_s,end_s,speaker_id,dbfs,vad_ratio,visual_lum,visual_sat,visual_jsd_bits,scene_cut,wpm,norm_marker_density,resonance,narrative_energy,stance_entropy_bits,norm_entropy,narrative_inertia,narrative_complexity,x1_antimil,x2_legal,x3_pragmatic,x4_autonomy,polarization");
foreach (var p in dynamicsProfile.Timeline)
{
    csv.AppendLine(FormattableString.Invariant(
        $"{p.WindowIndex},{p.StartSeconds:F2},{p.EndSeconds:F2},{p.SpeakerId},{p.Acoustic.DbFs:F2},{p.Acoustic.SpeechActivityRatio:F3},{p.Visual.MeanLuminance:F4},{p.Visual.MeanSaturation:F4},{p.Visual.JensenShannonDivergenceFromPrev:F4},{p.Visual.IsSceneCutDetected},{p.Linguistic.WordsPerMinute:F1},{p.Linguistic.NormativeMarkerDensity:F4},{p.CrossModalResonance:F4},{p.NarrativeEnergy:F2},{p.CumulativeStanceEntropyBits:F4},{p.NormalizedNarrativeEntropy:F4},{p.NarrativeInertia:F4},{p.NarrativeComplexity:F2},{p.CollectiveStateX["B1_ANTIMILITARISM"]:F4},{p.CollectiveStateX["B2_LEGAL_COMPLIANCE"]:F4},{p.CollectiveStateX["B3_PRAGMATIC_SERVICE"]:F4},{p.CollectiveStateX["B4_INDIVIDUAL_AUTONOMY"]:F4},{p.PolarizationIndex:F4}"));
}
var outCsvPath = Path.Combine(outputDir, "telemetry.csv");
await File.WriteAllTextAsync(outCsvPath, csv.ToString());

var outHtmlPath = Path.Combine(outputDir, "dashboard.html");
await File.WriteAllTextAsync(outHtmlPath, DashboardRenderer.RenderHtml(dynamicsProfile, transcriptDoc, profileDir));

Console.WriteLine($"[Argon.Engine] Mean Energy: {dynamicsProfile.GlobalIndices.MeanNarrativeEnergy:F2} | Peak Energy: {dynamicsProfile.GlobalIndices.PeakNarrativeEnergy:F2} (Window #{dynamicsProfile.GlobalIndices.PeakEnergyWindowIndex})");
Console.WriteLine($"[Argon.Engine] Final Stance Entropy: {dynamicsProfile.GlobalIndices.FinalNarrativeEntropyBits:F4} bits ({dynamicsProfile.GlobalIndices.NormalizedNarrativeEntropy:P1})");
Console.WriteLine($"[Argon.Engine] Mean Inertia: {dynamicsProfile.GlobalIndices.MeanNarrativeInertia:F4} | Mean Complexity: {dynamicsProfile.GlobalIndices.MeanNarrativeComplexity:F2}");
Console.WriteLine($"[Argon.Engine] Scene Cuts Detected: {dynamicsProfile.GlobalIndices.DetectedSceneCutsCount} | Control Condition Kappa: {dynamicsProfile.ControlAnalysis.ConditionNumberKappa:F3}");
Console.WriteLine($"[Argon.Engine] Artifacts written to: {outputDir}");
return 0;
