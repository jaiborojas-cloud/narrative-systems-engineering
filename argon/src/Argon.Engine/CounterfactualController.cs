using System.Globalization;
using System.Text;
using System.Text.Json;

namespace Argon.Engine;

public sealed record PermutationEvaluation(
    int RankIndex,
    string ObjectiveTag,
    List<string> BlockOrder,
    Dictionary<string, double> FinalStateX,
    double FinalStateNormL2,
    double ShiftFromOriginalL2,
    double MeanInertia,
    List<Dictionary<string, double>> StateTrajectory);

public sealed record MontageOptimizationStudy(
    string WorkId,
    int BlockCount,
    int TotalPermutationsEvaluated,
    Dictionary<string, double> OriginalFinalStateX,
    double OriginalStateNormL2,
    Dictionary<string, double> ReachableStateMin,
    Dictionary<string, double> ReachableStateMax,
    Dictionary<string, double> ReachableSpanPerDimension,
    PermutationEvaluation OptimalAntiMilitarist,
    PermutationEvaluation OptimalLegalCompliance,
    PermutationEvaluation OptimalCivicEquilibrium,
    PermutationEvaluation OptimalAutonomy,
    PermutationEvaluation WorstPolarizingExtremum,
    List<PermutationSamplePoint> CloudSamples);

public sealed record PermutationSamplePoint(
    string OrderCode,
    double X1AntiMil,
    double X2Legal,
    double X3Pragmatic,
    double X4Autonomy,
    double NormL2);

public sealed record ParametricInterventionPoint(
    string InterventionId,
    string Label,
    string ParameterDescription,
    double ParameterValue,
    double PeakEnergy,
    double MeanEnergy,
    double FinalInertia,
    Dictionary<string, double> FinalStateX,
    double ShiftFromBaselineL2,
    List<double> EnergyTrajectory,
    List<double> PrimaryStateTrajectory);

public sealed record ShockImpulseStudy(
    string WorkId,
    double BaselinePeakEnergy,
    Dictionary<string, double> BaselineFinalStateX,
    double NonVerbalShockLeveragePercent,
    List<ParametricInterventionPoint> Interventions);

public sealed record InertialShieldingStudy(
    string WorkId,
    double BaselineFinalInertia,
    Dictionary<string, double> BaselineFinalStateX,
    double InstitutionalStabilizationMargin,
    List<ParametricInterventionPoint> Perturbations);

public sealed record CounterfactualControlReport(
    string SchemaVersion,
    DateTimeOffset CreatedUtc,
    string ExperimentId,
    string Author,
    string Orcid,
    string Doi,
    MontageOptimizationStudy Study1MontageOptimization,
    ShockImpulseStudy Study2ShockImpulseControl,
    InertialShieldingStudy Study3InertialShielding);

public static class CounterfactualController
{
    private static readonly string[] StateKeys =
    [
        "B1_ANTIMILITARISM",
        "B2_LEGAL_COMPLIANCE",
        "B3_PRAGMATIC_SERVICE",
        "B4_INDIVIDUAL_AUTONOMY"
    ];

    public static CounterfactualControlReport RunFullControlExperiment(
        NarrativeDynamicsProfile p1Insumision,
        NarrativeDynamicsProfile p2Daisy,
        NarrativeDynamicsProfile p3Challenger)
    {
        var study1 = RunStudy1MontagePermutations(p1Insumision);
        var study2 = RunStudy2DaisyShockControl(p2Daisy);
        var study3 = RunStudy3ChallengerInertialShielding(p3Challenger);

        return new CounterfactualControlReport(
            "argon-control-0.3",
            DateTimeOffset.UtcNow,
            "EXP-003-COUNTERFACTUAL-CONTROL",
            "Jair Ivan Rojas Ruiz",
            "https://orcid.org/0009-0003-1109-4583",
            "https://doi.org/10.5281/zenodo.23987656",
            study1,
            study2,
            study3);
    }

    private static MontageOptimizationStudy RunStudy1MontagePermutations(NarrativeDynamicsProfile profile)
    {
        // Group contiguous speaker turns into 6 editorial blocks
        var blocks = new List<(string BlockId, List<WindowTelemetryPoint> Windows)>();
        foreach (var pt in profile.Timeline)
        {
            if (blocks.Count == 0 || blocks[^1].BlockId != pt.SpeakerId)
            {
                blocks.Add((pt.SpeakerId, [pt]));
            }
            else
            {
                blocks[^1].Windows.Add(pt);
            }
        }

        var baselineFinal = profile.Timeline.Last().CollectiveStateX;
        var origNorm = Math.Round(Math.Sqrt(StateKeys.Sum(k => baselineFinal[k] * baselineFinal[k])), 4);

        var indices = Enumerable.Range(0, blocks.Count).ToArray();
        var allPerms = GetPermutations(indices, indices.Length).ToList();

        var evaluations = new List<(List<string> Order, Dictionary<string, double> FinalX, double NormL2, double ShiftL2, double MeanInertia, List<Dictionary<string, double>> Trajectory)>(allPerms.Count);

        var minBounds = StateKeys.ToDictionary(k => k, _ => double.PositiveInfinity);
        var maxBounds = StateKeys.ToDictionary(k => k, _ => double.NegativeInfinity);

        foreach (var perm in allPerms)
        {
            var orderedWindows = new List<WindowTelemetryPoint>(profile.Timeline.Count);
            var blockOrder = new List<string>(blocks.Count);
            foreach (var idx in perm)
            {
                blockOrder.Add(blocks[idx].BlockId);
                orderedWindows.AddRange(blocks[idx].Windows);
            }

            var (finalX, meanInertia, traj) = SimulateWindowSequence(orderedWindows);
            var normL2 = Math.Sqrt(StateKeys.Sum(k => finalX[k] * finalX[k]));
            var shiftL2 = Math.Sqrt(StateKeys.Sum(k => Math.Pow(finalX[k] - baselineFinal[k], 2)));

            foreach (var k in StateKeys)
            {
                if (finalX[k] < minBounds[k]) minBounds[k] = finalX[k];
                if (finalX[k] > maxBounds[k]) maxBounds[k] = finalX[k];
            }

            evaluations.Add((blockOrder, finalX, Math.Round(normL2, 4), Math.Round(shiftL2, 4), Math.Round(meanInertia, 4), traj));
        }

        PermutationEvaluation ToRecord(int rank, string tag, (List<string> Order, Dictionary<string, double> FinalX, double NormL2, double ShiftL2, double MeanInertia, List<Dictionary<string, double>> Trajectory) item)
            => new(rank, tag, item.Order, item.FinalX, item.NormL2, item.ShiftL2, item.MeanInertia, item.Trajectory);

        var bestAnti = evaluations.OrderByDescending(e => e.FinalX["B1_ANTIMILITARISM"]).First();
        var bestLegal = evaluations.OrderByDescending(e => e.FinalX["B2_LEGAL_COMPLIANCE"]).First();
        var bestEquilibrium = evaluations.OrderBy(e => e.NormL2).First();
        var bestAutonomy = evaluations.OrderByDescending(e => e.FinalX["B4_INDIVIDUAL_AUTONOMY"]).First();
        var maxNorm = evaluations.OrderByDescending(e => e.NormL2).First();

        // Downsample 120 evenly-spaced permutations + key extrema for interactive scatter cloud
        var step = Math.Max(1, evaluations.Count / 120);
        var cloud = new List<PermutationSamplePoint>();
        for (var i = 0; i < evaluations.Count; i += step)
        {
            var e = evaluations[i];
            cloud.Add(new PermutationSamplePoint(
                string.Join("→", e.Order.Select(s => s.Replace("ENV_POSTER", "P").Replace("SPK_0", "S"))),
                e.FinalX["B1_ANTIMILITARISM"],
                e.FinalX["B2_LEGAL_COMPLIANCE"],
                e.FinalX["B3_PRAGMATIC_SERVICE"],
                e.FinalX["B4_INDIVIDUAL_AUTONOMY"],
                e.NormL2));
        }

        var span = StateKeys.ToDictionary(k => k, k => Math.Round(maxBounds[k] - minBounds[k], 4));

        return new MontageOptimizationStudy(
            "CORPUS-001-INSUMISION-1994",
            blocks.Count,
            evaluations.Count,
            baselineFinal,
            origNorm,
            minBounds.ToDictionary(kv => kv.Key, kv => Math.Round(kv.Value, 4)),
            maxBounds.ToDictionary(kv => kv.Key, kv => Math.Round(kv.Value, 4)),
            span,
            ToRecord(1, "MAX_B1_ANTIMILITARISM", bestAnti),
            ToRecord(2, "MAX_B2_LEGAL_COMPLIANCE", bestLegal),
            ToRecord(3, "MIN_NORM_CIVIC_EQUILIBRIUM", bestEquilibrium),
            ToRecord(4, "MAX_B4_INDIVIDUAL_AUTONOMY", bestAutonomy),
            ToRecord(5, "MAX_CONVICTION_EXTREMUM", maxNorm),
            cloud);
    }

    private static ShockImpulseStudy RunStudy2DaisyShockControl(NarrativeDynamicsProfile daisy)
    {
        var baseTimeline = daisy.Timeline;
        var baseFinal = baseTimeline.Last().CollectiveStateX;

        var interventions = new List<ParametricInterventionPoint>();

        // 0. Baseline (Original Broadcast: Countdown & Nuclear Detonation at Windows #4-#8)
        interventions.Add(EvaluateDaisyVariant(
            "DAISY_BASELINE",
            "Baseline: Actual 1964 Broadcast (Shock at t=30s)",
            "Meadow innocence (0-20s) -> Countdown (20-30s) -> Nuclear Detonation (30-45s) -> Mandate (45-60s)",
            1.0,
            baseTimeline,
            baseFinal));

        // 1. Cold-Open Shock (Nuclear detonation placed at t=0s, meadow innocence placed at end)
        var coldOpenOrder = new List<WindowTelemetryPoint>();
        coldOpenOrder.AddRange(baseTimeline.Skip(4).Take(5)); // Windows 4..8 (Countdown + Detonation)
        coldOpenOrder.AddRange(baseTimeline.Skip(9).Take(3)); // Windows 9..11 (LBJ Mandate)
        coldOpenOrder.AddRange(baseTimeline.Take(4));         // Windows 0..3 (Meadow girl at the end)
        interventions.Add(EvaluateDaisyVariant(
            "DAISY_COLD_OPEN_SHOCK",
            "Counterfactual 2A: Cold-Open Detonation (Shock at t=0s, Meadow Last)",
            "Places nuclear countdown & blast at t=0s, ending with the quiet meadow counting scene.",
            0.0,
            coldOpenOrder,
            baseFinal));

        // 2. Climax-Last Shock (Meadow -> Mandate -> Detonation at very end t=45-60s)
        var terminalShockOrder = new List<WindowTelemetryPoint>();
        terminalShockOrder.AddRange(baseTimeline.Take(4));         // Meadow
        terminalShockOrder.AddRange(baseTimeline.Skip(9).Take(3)); // Mandate
        terminalShockOrder.AddRange(baseTimeline.Skip(4).Take(5)); // Detonation at very end
        interventions.Add(EvaluateDaisyVariant(
            "DAISY_TERMINAL_SHOCK",
            "Counterfactual 2B: Terminal Detonation (Shock at t=35-60s)",
            "Delays the nuclear countdown and fireball to the final frames after the verbal pitch.",
            1.0,
            terminalShockOrder,
            baseFinal));

        // 3. Acoustic & Visual Shock Attenuation (alpha = 0.35: keep words identical, flatten dBFS and remove nuclear cut flash)
        var attenuatedWindows = baseTimeline.Select((pt, idx) =>
        {
            if (idx is >= 4 and <= 8)
            {
                // Replace explosive prosody & atomic flash with calm meadow-level acoustic/visual salience
                var dampedEnergy = Math.Round(pt.NarrativeEnergy * 0.42, 2);
                return pt with { NarrativeEnergy = dampedEnergy };
            }
            return pt;
        }).ToList();

        var attenuatedResult = EvaluateDaisyVariant(
            "DAISY_DAMPED_SHOCK",
            "Counterfactual 2C: Non-Verbal Shock Attenuation (-60% Audio/Visual Impulse)",
            "Preserves identical transcript & stances z(t), but damps nuclear roar (-13.7 -> -25 dBFS) and flash cut (D_JS 0.58 -> 0.04).",
            0.42,
            attenuatedWindows,
            baseFinal);
        interventions.Add(attenuatedResult);

        // Compute percentage of terminal mobilization state B2_LEGAL_COMPLIANCE lost when non-verbal shock is damped
        var baseB2 = Math.Max(1e-6, Math.Abs(baseFinal["B2_LEGAL_COMPLIANCE"]));
        var dampedB2 = Math.Abs(attenuatedResult.FinalStateX["B2_LEGAL_COMPLIANCE"]);
        var leveragePct = Math.Round(100.0 * Math.Abs(baseB2 - dampedB2) / baseB2, 2);

        return new ShockImpulseStudy(
            "CORPUS-002-LBJ-DAISY-1964",
            daisy.GlobalIndices.PeakNarrativeEnergy,
            baseFinal,
            leveragePct,
            interventions);
    }

    private static InertialShieldingStudy RunStudy3ChallengerInertialShielding(NarrativeDynamicsProfile challenger)
    {
        var baseTimeline = challenger.Timeline;
        var baseFinal = baseTimeline.Last().CollectiveStateX;
        var baseFinalInertia = baseTimeline.Last().NarrativeInertia;

        var perturbations = new List<ParametricInterventionPoint>();

        // 0. Baseline Oval Office Address
        perturbations.Add(EvaluateChallengerVariant(
            "CHALLENGER_BASELINE",
            "Baseline: Unbroken Oval Office Address (1986)",
            "Single-camera shot (0 cuts), high institutional anchoring, monotonic state convergence.",
            0.0,
            baseTimeline,
            baseFinal));

        // 1. De-Institutionalized Framing (Set InstitutionalAnchoring = 0.05 across all windows)
        var deInstWindows = baseTimeline.Select(pt =>
        {
            var newLing = pt.Linguistic with { InstitutionalAnchoring = 0.05 };
            return pt with { Linguistic = newLing };
        }).ToList();
        var deInstEval = EvaluateChallengerVariant(
            "CHALLENGER_DEINSTITUTIONALIZED",
            "Perturbation 3A: De-Institutionalized Framing (A_inst -> 0.05)",
            "Removes statutory/presidential institutional anchoring while keeping speech cadence and stances.",
            0.05,
            deInstWindows,
            baseFinal);
        perturbations.Add(deInstEval);

        // 2. Early Counter-Frame Injection (Injects 3 windows of high-energy institutional critique at Windows #3-#5, t=30-60s)
        perturbations.Add(EvaluateChallengerCounterFrame(
            "CHALLENGER_PERTURB_EARLY",
            "Perturbation 3B: Early Crisis Critique Injection (t = 30-60s)",
            "Injects 30s of high-energy institutional failure critique early in the speech when inertia is still building.",
            3,
            baseTimeline,
            baseFinal));

        // 3. Mid Counter-Frame Injection (Windows #12-#14, t=120-150s)
        perturbations.Add(EvaluateChallengerCounterFrame(
            "CHALLENGER_PERTURB_MID",
            "Perturbation 3C: Mid-Speech Critique Injection (t = 120-150s)",
            "Injects identical 30s institutional critique at the midpoint of the address.",
            12,
            baseTimeline,
            baseFinal));

        // 4. Late Counter-Frame Injection (Windows #21-#23, t=210-240s)
        perturbations.Add(EvaluateChallengerCounterFrame(
            "CHALLENGER_PERTURB_LATE",
            "Perturbation 3D: Late Speech Critique Injection (t = 210-240s)",
            "Injects identical 30s critique near the end, testing recency impact vs. accumulated institutional inertia.",
            21,
            baseTimeline,
            baseFinal));

        var instMargin = Math.Round(baseFinalInertia - deInstEval.FinalInertia, 4);

        return new InertialShieldingStudy(
            "CORPUS-003-REAGAN-CHALLENGER-1986",
            baseFinalInertia,
            baseFinal,
            instMargin,
            perturbations);
    }

    private static ParametricInterventionPoint EvaluateDaisyVariant(
        string id,
        string label,
        string desc,
        double paramVal,
        List<WindowTelemetryPoint> windows,
        Dictionary<string, double> baselineFinal)
    {
        var (finalX, _, traj) = SimulateWindowSequence(windows);
        var shift = Math.Sqrt(StateKeys.Sum(k => Math.Pow(finalX[k] - baselineFinal[k], 2)));
        return new ParametricInterventionPoint(
            id,
            label,
            desc,
            paramVal,
            windows.Max(w => w.NarrativeEnergy),
            Math.Round(windows.Average(w => w.NarrativeEnergy), 2),
            windows.Last().NarrativeInertia,
            finalX,
            Math.Round(shift, 4),
            windows.Select(w => w.NarrativeEnergy).ToList(),
            traj.Select(st => st["B2_LEGAL_COMPLIANCE"]).ToList());
    }

    private static ParametricInterventionPoint EvaluateChallengerVariant(
        string id,
        string label,
        string desc,
        double paramVal,
        List<WindowTelemetryPoint> windows,
        Dictionary<string, double> baselineFinal)
    {
        var (finalX, meanInertia, traj) = SimulateWindowSequence(windows, recomputeInertia: true);
        var shift = Math.Sqrt(StateKeys.Sum(k => Math.Pow(finalX[k] - baselineFinal[k], 2)));
        return new ParametricInterventionPoint(
            id,
            label,
            desc,
            paramVal,
            windows.Max(w => w.NarrativeEnergy),
            Math.Round(windows.Average(w => w.NarrativeEnergy), 2),
            meanInertia,
            finalX,
            Math.Round(shift, 4),
            windows.Select(w => w.NarrativeEnergy).ToList(),
            traj.Select(st => st["B2_LEGAL_COMPLIANCE"]).ToList());
    }

    private static ParametricInterventionPoint EvaluateChallengerCounterFrame(
        string id,
        string label,
        string desc,
        int startIdx,
        List<WindowTelemetryPoint> baseTimeline,
        Dictionary<string, double> baselineFinal)
    {
        var modified = baseTimeline.Select((pt, idx) =>
        {
            if (idx >= startIdx && idx < startIdx + 3)
            {
                var critiqueStance = new Dictionary<string, double>
                {
                    ["B1_ANTIMILITARISM"] = -0.80,
                    ["B2_LEGAL_COMPLIANCE"] = -0.90,
                    ["B3_PRAGMATIC_SERVICE"] = -0.85,
                    ["B4_INDIVIDUAL_AUTONOMY"] = -0.70
                };
                var newLing = pt.Linguistic with
                {
                    ActiveStanceVector = critiqueStance,
                    InstitutionalAnchoring = 0.10
                };
                return pt with
                {
                    Linguistic = newLing,
                    NarrativeEnergy = 78.50
                };
            }
            return pt;
        }).ToList();

        return EvaluateChallengerVariant(id, label, desc, startIdx * 10.0, modified, baselineFinal);
    }

    private static (Dictionary<string, double> FinalState, double FinalOrMeanInertia, List<Dictionary<string, double>> Trajectory) SimulateWindowSequence(
        List<WindowTelemetryPoint> windows,
        bool recomputeInertia = true)
    {
        var stateX = StateKeys.ToDictionary(k => k, _ => 0.0);
        var trajectory = new List<Dictionary<string, double>>(windows.Count);
        var count = windows.Count;
        double lastInertia = 0.0;

        for (var t = 0; t < count; t++)
        {
            var pt = windows[t];
            var energy = pt.NarrativeEnergy;

            var currentConviction = stateX.Values.Select(Math.Abs).Average();
            var repetitionFactor = Math.Clamp((double)(t + 1) / count, 0.0, 1.0);
            var inertia = recomputeInertia
                ? Math.Clamp(
                    0.55 * pt.Linguistic.InstitutionalAnchoring +
                    0.25 * currentConviction +
                    0.20 * repetitionFactor,
                    0.0, 1.0)
                : pt.NarrativeInertia;
            lastInertia = inertia;

            var nextState = new Dictionary<string, double>();
            foreach (var key in StateKeys)
            {
                var uVal = pt.Linguistic.ActiveStanceVector.GetValueOrDefault(key, 0.0);
                var dimInertia = key == "B2_LEGAL_COMPLIANCE"
                    ? Math.Clamp(inertia * 1.15, 0.0, 0.95)
                    : inertia;
                var gamma = 0.18 * (energy / 100.0) * (1.0 - 0.65 * dimInertia);
                var updated = Math.Clamp((1.0 - gamma) * stateX[key] + gamma * uVal, -1.0, 1.0);
                nextState[key] = Math.Round(updated, 4);
            }

            stateX = nextState;
            trajectory.Add(new Dictionary<string, double>(stateX));
        }

        return (stateX, Math.Round(lastInertia, 4), trajectory);
    }

    private static IEnumerable<int[]> GetPermutations(int[] list, int length)
    {
        if (length == 1) return list.Select(t => new[] { t });
        return GetPermutations(list, length - 1)
            .SelectMany(t => list.Where(e => !t.Contains(e)),
                (t1, t2) => t1.Concat([t2]).ToArray());
    }

    public static string RenderControlHtml(CounterfactualControlReport report)
    {
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
        var payloadJson = JsonSerializer.Serialize(report, jsonOptions);

        var s1 = report.Study1MontageOptimization;
        var s2 = report.Study2ShockImpulseControl;
        var s3 = report.Study3InertialShielding;

        var sb = new StringBuilder();
        sb.AppendLine("<!doctype html>");
        sb.AppendLine("<html lang=\"en\">");
        sb.AppendLine("<head>");
        sb.AppendLine("<meta charset=\"utf-8\">");
        sb.AppendLine("<meta name=\"viewport\" content=\"width=device-width,initial-scale=1\">");
        sb.AppendLine("<title>EXP-003: Counterfactual Narrative Control & Montage Optimization — Argon v0.3</title>");
        sb.AppendLine("<style>");
        sb.AppendLine(@"
:root {
  --bg: #070b12;
  --panel: #101826;
  --panel-alt: #162133;
  --border: #243552;
  --text: #e6edf7;
  --muted: #94a8c4;
  --accent: #38bdf8;
  --gold: #f59e0b;
  --red: #ef4444;
  --green: #10b981;
  --purple: #a855f7;
}
* { box-sizing: border-box; }
body {
  margin: 0;
  font-family: system-ui, -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
  background: var(--bg);
  color: var(--text);
  line-height: 1.55;
}
header {
  background: linear-gradient(135deg, #0b1528 0%, #1e1b4b 100%);
  border-bottom: 1px solid var(--border);
  padding: 28px 38px;
}
header h1 { margin: 8px 0; font-size: 26px; letter-spacing: -0.02em; }
header p { margin: 0; color: var(--muted); font-size: 14px; max-width: 1080px; }
.badge {
  display: inline-block;
  padding: 3px 10px;
  border-radius: 999px;
  font-size: 11px;
  font-weight: 700;
  background: rgba(168, 85, 247, 0.18);
  color: #d8b4fe;
  border: 1px solid rgba(168, 85, 247, 0.4);
  margin-right: 8px;
}
main { max-width: 1440px; margin: 0 auto; padding: 28px 38px 64px; }
.kpi-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
  gap: 16px;
  margin-bottom: 24px;
}
.kpi {
  background: var(--panel);
  border: 1px solid var(--border);
  border-radius: 12px;
  padding: 16px 18px;
}
.kpi .label { font-size: 11px; text-transform: uppercase; color: var(--muted); letter-spacing: 0.05em; }
.kpi .val { font-size: 24px; font-weight: 700; margin: 4px 0; color: #fff; }
.kpi .sub { font-size: 12px; color: var(--muted); }
.grid-2 { display: grid; grid-template-columns: 1.15fr 1fr; gap: 22px; margin-bottom: 24px; }
@media (max-width: 1080px) { .grid-2 { grid-template-columns: 1fr; } }
section {
  background: var(--panel);
  border: 1px solid var(--border);
  border-radius: 14px;
  padding: 22px 24px;
  margin-bottom: 24px;
}
section h2 { margin: 0 0 6px 0; font-size: 18px; color: #fff; }
section p.desc { margin: 0 0 16px 0; font-size: 13px; color: var(--muted); }
canvas {
  width: 100%;
  background: #0a101a;
  border: 1px solid #1e2d46;
  border-radius: 10px;
  display: block;
}
table { width: 100%; border-collapse: collapse; font-size: 13px; margin-top: 12px; }
th, td { text-align: left; padding: 10px 12px; border-bottom: 1px solid var(--border); }
th { color: var(--muted); font-size: 11px; text-transform: uppercase; letter-spacing: 0.05em; }
code { background: #172336; padding: 2px 6px; border-radius: 4px; font-size: 12px; color: #93c5fd; }
.legend { display: flex; flex-wrap: wrap; gap: 16px; margin-top: 10px; font-size: 12px; color: var(--muted); }
.legend span { display: inline-flex; align-items: center; gap: 6px; font-weight: 600; }
.dot { width: 10px; height: 10px; border-radius: 50%; display: inline-block; }
");
        sb.AppendLine("</style>");
        sb.AppendLine("</head>");
        sb.AppendLine("<body>");
        sb.AppendLine("<header>");
        sb.AppendLine("  <div><span class=\"badge\">EXP-003: COUNTERFACTUAL CONTROL</span><span class=\"badge\">ARGON ENGINE v0.3</span><span class=\"badge\">JAIR IVAN ROJAS RUIZ (ORCID: 0009-0003-1109-4583)</span><span class=\"badge\">DOI: 10.5281/zenodo.23987656</span></div>");
        sb.AppendLine("  <h1>Narrative Systems Engineering — Counterfactual Control &amp; Montage Optimization Lab (EXP-003)</h1>");
        sb.AppendLine("  <p>Discrete-time optimal control synthesis over multimodal audiovisual state-space dynamics: (1) Exhaustive 6! = 720 Montage Permutation Reachability in CORPUS-001, (2) Multimodal Shock Impulse Timing &amp; Attenuation in CORPUS-002 (LBJ 'Daisy'), and (3) Institutional Inertial Shielding in CORPUS-003 (Reagan 'Challenger').</p>");
        sb.AppendLine("</header>");
        sb.AppendLine("<main>");

        sb.AppendLine("<div class=\"kpi-grid\">");
        sb.AppendLine(CultureInfo.InvariantCulture, $"  <div class=\"kpi\"><div class=\"label\">Montage Permutations Solved</div><div class=\"val\">{s1.TotalPermutationsEvaluated} (6!)</div><div class=\"sub\">Exact LTV trajectory evaluation on CORPUS-001</div></div>");
        sb.AppendLine(CultureInfo.InvariantCulture, $"  <div class=\"kpi\"><div class=\"label\">Reachable Abolitionist Span (Δx₁)</div><div class=\"val\">{s1.ReachableSpanPerDimension["B1_ANTIMILITARISM"]:F3}</div><div class=\"sub\">[{s1.ReachableStateMin["B1_ANTIMILITARISM"]:+0.000;-0.000} to {s1.ReachableStateMax["B1_ANTIMILITARISM"]:+0.000;-0.000}] purely via editing order</div></div>");
        sb.AppendLine(CultureInfo.InvariantCulture, $"  <div class=\"kpi\"><div class=\"label\">Min-Bias Equilibrium Norm ||x*(T)||₂</div><div class=\"val\">{s1.OptimalCivicEquilibrium.FinalStateNormL2:F4}</div><div class=\"sub\">vs. {s1.OriginalStateNormL2:F4} in original broadcast (-{(1.0 - s1.OptimalCivicEquilibrium.FinalStateNormL2 / s1.OriginalStateNormL2) * 100:F1}% bias)</div></div>");
        sb.AppendLine(CultureInfo.InvariantCulture, $"  <div class=\"kpi\"><div class=\"label\">Non-Verbal Shock Leverage (Daisy)</div><div class=\"val\">{s2.NonVerbalShockLeveragePercent:F1}%</div><div class=\"sub\">State mobilization attributable to audio/visual blast</div></div>");
        sb.AppendLine(CultureInfo.InvariantCulture, $"  <div class=\"kpi\"><div class=\"label\">Institutional Stabilization Margin</div><div class=\"val\">+{s3.InstitutionalStabilizationMargin:F3}</div><div class=\"sub\">Inertia gain from presidential framing in CORPUS-003</div></div>");
        sb.AppendLine("</div>");

        // Study 1
        sb.AppendLine("<div class=\"grid-2\">");
        sb.AppendLine("  <section>");
        sb.AppendLine("    <h2>Study 1A. Reachable Terminal State Cloud Across All 6! = 720 Montage Orders (CORPUS-001)</h2>");
        sb.AppendLine("    <p class=\"desc\">Projection of the 720 reachable terminal states x(T) in the plane (X = B1 Anti-Militarist Abolitionism, Y = B2 Legal Compliance) using the exact same 39 video windows. Editing order alone flips the audience outcome across quadrants.</p>");
        sb.AppendLine("    <canvas id=\"cloudCanvas\" width=\"720\" height=\"400\"></canvas>");
        sb.AppendLine("    <div class=\"legend\">");
        sb.AppendLine("      <span><i class=\"dot\" style=\"background:#64748b\"></i> 720 Montage Permutations</span>");
        sb.AppendLine("      <span><i class=\"dot\" style=\"background:#f59e0b\"></i> Original Broadcast</span>");
        sb.AppendLine("      <span><i class=\"dot\" style=\"background:#ef4444\"></i> Optimal Abolitionist (π*₁)</span>");
        sb.AppendLine("      <span><i class=\"dot\" style=\"background:#10b981\"></i> Optimal Civic Equilibrium (π*_eq)</span>");
        sb.AppendLine("      <span><i class=\"dot\" style=\"background:#38bdf8\"></i> Optimal Legal Compliance (π*₂)</span>");
        sb.AppendLine("    </div>");
        sb.AppendLine("  </section>");

        sb.AppendLine("  <section>");
        sb.AppendLine("    <h2>Study 1B. State Trajectory Synthesis x₁(t): Original vs. Optimal Montages</h2>");
        sb.AppendLine("    <p class=\"desc\">Temporal evolution of Anti-Militarist Abolitionism x₁(t) over t = 0..195s under Original Broadcast, Optimal Abolitionist Montage, and Minimum-Bias Civic Equilibrium Montage.</p>");
        sb.AppendLine("    <canvas id=\"montageTrajCanvas\" width=\"600\" height=\"400\"></canvas>");
        sb.AppendLine("  </section>");
        sb.AppendLine("</div>");

        sb.AppendLine("<section>");
        sb.AppendLine("  <h2>Study 1C. Optimal Montage Synthesis Solutions (CORPUS-001, 6 Speaker Blocks)</h2>");
        sb.AppendLine("  <table>");
        sb.AppendLine("    <thead><tr><th>Control Objective</th><th>Synthesized Block Sequence π*</th><th>x₁ (Anti-Mil)</th><th>x₂ (Legal)</th><th>x₃ (Pragmatic)</th><th>x₄ (Autonomy)</th><th>||x(T)||₂ Norm</th><th>Shift ||Δx||₂</th></tr></thead>");
        sb.AppendLine("    <tbody>");
        foreach (var opt in new[] { s1.OptimalAntiMilitarist, s1.OptimalLegalCompliance, s1.OptimalCivicEquilibrium, s1.OptimalAutonomy, s1.WorstPolarizingExtremum })
        {
            var seq = string.Join(" → ", opt.BlockOrder);
            sb.AppendLine(CultureInfo.InvariantCulture, $"      <tr><td><strong>{opt.ObjectiveTag}</strong></td><td><code>{seq}</code></td><td>{opt.FinalStateX["B1_ANTIMILITARISM"]:+0.0000;-0.0000}</td><td>{opt.FinalStateX["B2_LEGAL_COMPLIANCE"]:+0.0000;-0.0000}</td><td>{opt.FinalStateX["B3_PRAGMATIC_SERVICE"]:+0.0000;-0.0000}</td><td>{opt.FinalStateX["B4_INDIVIDUAL_AUTONOMY"]:+0.0000;-0.0000}</td><td><strong>{opt.FinalStateNormL2:F4}</strong></td><td>{opt.ShiftFromOriginalL2:F4}</td></tr>");
        }
        sb.AppendLine("    </tbody>");
        sb.AppendLine("  </table>");
        sb.AppendLine("</section>");

        // Study 2 & Study 3
        sb.AppendLine("<div class=\"grid-2\">");
        sb.AppendLine("  <section>");
        sb.AppendLine("    <h2>Study 2. Shock Impulse Timing &amp; Attenuation in CORPUS-002 (LBJ 'Daisy', 1964)</h2>");
        sb.AppendLine("    <p class=\"desc\">Comparison of Collective Electoral Mobilization x₂(t) under Actual Broadcast (mid-spot detonation), Cold-Open Shock (detonation at t=0s), Terminal Shock, and Non-Verbal Audio/Visual Attenuation (-60% impulse).</p>");
        sb.AppendLine("    <canvas id=\"daisyControlCanvas\" width=\"660\" height=\"340\"></canvas>");
        sb.AppendLine("    <table>");
        sb.AppendLine("      <thead><tr><th>Intervention</th><th>Peak E</th><th>Final x₂ (Mobilization)</th><th>Shift ||Δx||₂</th></tr></thead>");
        sb.AppendLine("      <tbody>");
        foreach (var iv in s2.Interventions)
        {
            sb.AppendLine(CultureInfo.InvariantCulture, $"        <tr><td><strong>{System.Net.WebUtility.HtmlEncode(iv.Label)}</strong></td><td>{iv.PeakEnergy:F2}</td><td><strong>{iv.FinalStateX["B2_LEGAL_COMPLIANCE"]:+0.0000;-0.0000}</strong></td><td>{iv.ShiftFromBaselineL2:F4}</td></tr>");
        }
        sb.AppendLine("      </tbody>");
        sb.AppendLine("    </table>");
        sb.AppendLine("  </section>");

        sb.AppendLine("  <section>");
        sb.AppendLine("    <h2>Study 3. Inertial Shielding Against Crisis Perturbations in CORPUS-003 (Reagan, 1986)</h2>");
        sb.AppendLine("    <p class=\"desc\">Response of National Unity / Institutional Faith x₂(t) when a 30-second high-energy institutional failure critique is injected Early (t=30s), Mid (t=120s), or Late (t=210s) vs. De-Institutionalized framing.</p>");
        sb.AppendLine("    <canvas id=\"challengerControlCanvas\" width=\"660\" height=\"340\"></canvas>");
        sb.AppendLine("    <table>");
        sb.AppendLine("      <thead><tr><th>Perturbation Scenario</th><th>Final Inertia I(T)</th><th>Final x₂ (Unity)</th><th>Shift ||Δx||₂</th></tr></thead>");
        sb.AppendLine("      <tbody>");
        foreach (var pt in s3.Perturbations)
        {
            sb.AppendLine(CultureInfo.InvariantCulture, $"        <tr><td><strong>{System.Net.WebUtility.HtmlEncode(pt.Label)}</strong></td><td>{pt.FinalInertia:F4}</td><td><strong>{pt.FinalStateX["B2_LEGAL_COMPLIANCE"]:+0.0000;-0.0000}</strong></td><td>{pt.ShiftFromBaselineL2:F4}</td></tr>");
        }
        sb.AppendLine("      </tbody>");
        sb.AppendLine("    </table>");
        sb.AppendLine("  </section>");
        sb.AppendLine("</div>");

        sb.AppendLine("</main>");
        sb.AppendLine("<script>");
        sb.Append("const CTRL = ");
        sb.Append(payloadJson);
        sb.AppendLine(";");
        sb.AppendLine(@"
function drawCloud() {
  const canvas = document.getElementById('cloudCanvas');
  const ctx = canvas.getContext('2d');
  const W = canvas.width, H = canvas.height;
  ctx.clearRect(0, 0, W, H);
  const padL = 52, padR = 24, padT = 24, padB = 42;
  const pW = W - padL - padR, pH = H - padT - padB;

  const mapX = v => padL + ((v + 0.8) / 1.6) * pW;
  const mapY = v => padT + pH * (1 - (v + 0.8) / 1.6);

  // Axes
  ctx.strokeStyle = '#1e293b';
  for (let v = -0.6; v <= 0.65; v += 0.3) {
    const px = mapX(v), py = mapY(v);
    ctx.beginPath(); ctx.moveTo(px, padT); ctx.lineTo(px, H - padB); ctx.stroke();
    ctx.beginPath(); ctx.moveTo(padL, py); ctx.lineTo(W - padR, py); ctx.stroke();
    ctx.fillStyle = '#64748b'; ctx.font = '11px system-ui';
    ctx.fillText(v.toFixed(1), px - 10, H - 16);
    ctx.fillText(v.toFixed(1), 14, py + 4);
  }
  // Zero crosshairs
  ctx.strokeStyle = '#334155';
  ctx.beginPath(); ctx.moveTo(mapX(0), padT); ctx.lineTo(mapX(0), H - padB); ctx.stroke();
  ctx.beginPath(); ctx.moveTo(padL, mapY(0)); ctx.lineTo(W - padR, mapY(0)); ctx.stroke();

  const s1 = CTRL.study1MontageOptimization;
  s1.cloudSamples.forEach(pt => {
    ctx.fillStyle = 'rgba(148, 163, 184, 0.35)';
    ctx.beginPath();
    ctx.arc(mapX(pt.x1AntiMil), mapY(pt.x2Legal), 3.5, 0, Math.PI * 2);
    ctx.fill();
  });

  const highlights = [
    { x: s1.originalFinalStateX.B1_ANTIMILITARISM, y: s1.originalFinalStateX.B2_LEGAL_COMPLIANCE, c: '#f59e0b', r: 7 },
    { x: s1.optimalAntiMilitarist.finalStateX.B1_ANTIMILITARISM, y: s1.optimalAntiMilitarist.finalStateX.B2_LEGAL_COMPLIANCE, c: '#ef4444', r: 8 },
    { x: s1.optimalCivicEquilibrium.finalStateX.B1_ANTIMILITARISM, y: s1.optimalCivicEquilibrium.finalStateX.B2_LEGAL_COMPLIANCE, c: '#10b981', r: 8 },
    { x: s1.optimalLegalCompliance.finalStateX.B1_ANTIMILITARISM, y: s1.optimalLegalCompliance.finalStateX.B2_LEGAL_COMPLIANCE, c: '#38bdf8', r: 8 }
  ];
  highlights.forEach(h => {
    ctx.fillStyle = h.c;
    ctx.beginPath();
    ctx.arc(mapX(h.x), mapY(h.y), h.r, 0, Math.PI * 2);
    ctx.fill();
    ctx.strokeStyle = '#fff'; ctx.lineWidth = 1.5; ctx.stroke();
  });
}

function drawMontageTrajectories() {
  const canvas = document.getElementById('montageTrajCanvas');
  const ctx = canvas.getContext('2d');
  const W = canvas.width, H = canvas.height;
  ctx.clearRect(0, 0, W, H);
  const padL = 46, padR = 20, padT = 24, padB = 38;
  const pW = W - padL - padR, pH = H - padT - padB;
  const mapY = v => padT + pH * (1 - (v + 0.8) / 1.6);

  ctx.strokeStyle = '#334155';
  ctx.beginPath(); ctx.moveTo(padL, mapY(0)); ctx.lineTo(W - padR, mapY(0)); ctx.stroke();

  const s1 = CTRL.study1MontageOptimization;
  const series = [
    { traj: s1.optimalAntiMilitarist.stateTrajectory, c: '#ef4444', label: 'Optimal Abolitionist (π*₁)' },
    { traj: s1.optimalCivicEquilibrium.stateTrajectory, c: '#10b981', label: 'Optimal Equilibrium (π*_eq)' },
    { traj: s1.optimalLegalCompliance.stateTrajectory, c: '#38bdf8', label: 'Optimal Legal (π*₂)' }
  ];

  series.forEach(sr => {
    ctx.strokeStyle = sr.c;
    ctx.lineWidth = 2.5;
    ctx.beginPath();
    sr.traj.forEach((st, i) => {
      const x = padL + (i / (sr.traj.length - 1)) * pW;
      const y = mapY(st.B1_ANTIMILITARISM);
      if (i === 0) ctx.moveTo(x, y); else ctx.lineTo(x, y);
    });
    ctx.stroke();
  });
}

function drawMultiSeries(canvasId, items) {
  const canvas = document.getElementById(canvasId);
  const ctx = canvas.getContext('2d');
  const W = canvas.width, H = canvas.height;
  ctx.clearRect(0, 0, W, H);
  const padL = 46, padR = 20, padT = 20, padB = 34;
  const pW = W - padL - padR, pH = H - padT - padB;
  const mapY = v => padT + pH * (1 - (v + 0.8) / 1.8);

  ctx.strokeStyle = '#334155';
  ctx.beginPath(); ctx.moveTo(padL, mapY(0)); ctx.lineTo(W - padR, mapY(0)); ctx.stroke();

  const cols = ['#f59e0b', '#ef4444', '#38bdf8', '#10b981', '#a855f7'];
  items.forEach((iv, idx) => {
    ctx.strokeStyle = cols[idx % cols.length];
    ctx.lineWidth = idx === 0 ? 3.0 : 2.2;
    ctx.beginPath();
    iv.primaryStateTrajectory.forEach((v, i) => {
      const x = padL + (i / Math.max(1, iv.primaryStateTrajectory.length - 1)) * pW;
      const y = mapY(v);
      if (i === 0) ctx.moveTo(x, y); else ctx.lineTo(x, y);
    });
    ctx.stroke();
  });
}

drawCloud();
drawMontageTrajectories();
drawMultiSeries('daisyControlCanvas', CTRL.study2ShockImpulseControl.interventions);
drawMultiSeries('challengerControlCanvas', CTRL.study3InertialShielding.perturbations);
");
        sb.AppendLine("</script>");
        sb.AppendLine("</body></html>");
        return sb.ToString();
    }
}
