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

if (args.Length > 0 && string.Equals(args[0], "atlas", StringComparison.OrdinalIgnoreCase))
{
    var c1Path = Path.Combine(repoRoot, "argon", "experiments", "EXP-001-insumision-dynamics", "dynamics.profile.json");
    var c2Path = Path.Combine(repoRoot, "argon", "experiments", "EXP-002-public-domain-atlas", "daisy-run", "dynamics.profile.json");
    var c3Path = Path.Combine(repoRoot, "argon", "experiments", "EXP-002-public-domain-atlas", "challenger-run", "dynamics.profile.json");

    var p1 = JsonSerializer.Deserialize<NarrativeDynamicsProfile>(await File.ReadAllTextAsync(c1Path), jsonOptions)!;
    var p2 = JsonSerializer.Deserialize<NarrativeDynamicsProfile>(await File.ReadAllTextAsync(c2Path), jsonOptions)!;
    var p3 = JsonSerializer.Deserialize<NarrativeDynamicsProfile>(await File.ReadAllTextAsync(c3Path), jsonOptions)!;

    var entries = new List<(string WorkId, string Title, string Domain, string Language, string License, NarrativeDynamicsProfile Profile)>
    {
        ("CORPUS-001-INSUMISION-1994", "La Mili y la Insumisión — Public Broadcast Debate (Spain, 1994)", "Polyphonic Civic Debate", "es", "Academic Research Corpus (Spain, 1994)", p1),
        ("CORPUS-002-LBJ-DAISY-1964", "Lyndon B. Johnson 'Daisy' Nuclear Campaign Spot (USA, 1964)", "Acute Fear-Appeal Political Spot", "en", "Public Domain (US National Archives / LBJ Library)", p2),
        ("CORPUS-003-REAGAN-CHALLENGER-1986", "Ronald Reagan Address on the Space Shuttle Challenger Disaster (USA, 1986)", "Institutional Crisis & Consolation Address", "en", "Public Domain (17 U.S.C. § 105 / White House Television Office)", p3)
    };

    var atlas = AtlasComparator.BuildAtlas(entries);
    var atlasOutDir = Path.Combine(repoRoot, "argon", "experiments", "EXP-002-public-domain-atlas");
    Directory.CreateDirectory(atlasOutDir);

    var reportPath = Path.Combine(atlasOutDir, "atlas.report.json");
    var htmlPath = Path.Combine(atlasOutDir, "atlas-dashboard.html");

    await File.WriteAllTextAsync(reportPath, JsonSerializer.Serialize(atlas, jsonOptions));
    await File.WriteAllTextAsync(htmlPath, AtlasComparator.RenderAtlasHtml(atlas));

    Console.WriteLine($"[Argon.Engine Atlas] Synthesized Cross-Domain Narrative Phase-Space Atlas ({atlas.Works.Count} works, {atlas.PairwiseDistances.Count} pairwise distances).");
    foreach (var d in atlas.PairwiseDistances)
    {
        Console.WriteLine($"  -> {d.WorkA} vs. {d.WorkB}: 5D Metric Dist = {d.MetricSpaceEuclideanDistance:F4} | State End Dist = {d.StateTrajectoryEndDistance:F4} | Contrast = {d.RegimeContrastScore:F4}");
    }
    Console.WriteLine($"[Argon.Engine Atlas] Dashboard written to: {htmlPath}");
    return 0;
}

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
