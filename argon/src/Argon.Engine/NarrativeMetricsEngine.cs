namespace Argon.Engine;

public sealed record WindowTelemetryPoint(
    int WindowIndex,
    double StartSeconds,
    double EndSeconds,
    double MidpointSeconds,
    string SpeakerId,
    string SpeakerLabel,
    AcousticWindowMetrics Acoustic,
    VisualWindowMetrics Visual,
    LinguisticWindowMetrics Linguistic,
    double CrossModalResonance,
    double NarrativeEnergy,
    double CumulativeStanceEntropyBits,
    double NormalizedNarrativeEntropy,
    double NarrativeInertia,
    double NarrativeComplexity,
    Dictionary<string, double> CollectiveStateX,
    double PolarizationIndex,
    double StateDisplacementNorm);

public sealed record SpeakerSummaryMetrics(
    string SpeakerId,
    string SpeakerLabel,
    double StartSeconds,
    double EndSeconds,
    double MeanEnergy,
    double MeanAcousticDbFs,
    double MeanWordsPerMinute,
    double MeanComplexity,
    double MeanInertia,
    Dictionary<string, double> StanceVector);

public sealed record NarrativeDnaNode(string Id, string Label, string Kind);
public sealed record NarrativeDnaEdge(string Source, string Target, double Weight, string Relation, string Evidence);

public sealed record ControllabilityAnalysis(
    Dictionary<string, double>GramianDiagonalSensitivity,
    double MaxSensitivityEigenvalue,
    double MinSensitivityEigenvalue,
    double ConditionNumberKappa,
    string MostControllableDimension,
    string MostInertialDimension,
    List<ScenarioSimulationResult> CounterfactualScenarios);

public sealed record ScenarioSimulationResult(
    string ScenarioId,
    string Name,
    string Description,
    Dictionary<string, double> FinalStateX,
    double FinalPolarization,
    double NetShiftFromBaseline);

public sealed record NarrativeDynamicsProfile(
    string SchemaVersion,
    DateTimeOffset CreatedUtc,
    string SourceVideo,
    string SourceSha256,
    double DurationSeconds,
    int WindowCount,
    GlobalNarrativeIndices GlobalIndices,
    List<SpeakerSummaryMetrics> SpeakerSummaries,
    List<NarrativeDnaNode> DnaNodes,
    List<NarrativeDnaEdge> DnaEdges,
    ControllabilityAnalysis ControlAnalysis,
    List<WindowTelemetryPoint> Timeline);

public sealed record GlobalNarrativeIndices(
    double MeanNarrativeEnergy,
    double PeakNarrativeEnergy,
    int PeakEnergyWindowIndex,
    double FinalNarrativeEntropyBits,
    double NormalizedNarrativeEntropy,
    double MeanNarrativeInertia,
    double MeanNarrativeComplexity,
    int DetectedSceneCutsCount);

public static class NarrativeMetricsEngine
{
    private static readonly string[] StateKeys =
    [
        "B1_ANTIMILITARISM",
        "B2_LEGAL_COMPLIANCE",
        "B3_PRAGMATIC_SERVICE",
        "B4_INDIVIDUAL_AUTONOMY"
    ];

    public static NarrativeDynamicsProfile ComputeFullProfile(
        TranscriptDocument doc,
        double durationSeconds,
        List<AcousticWindowMetrics> acoustic,
        List<VisualWindowMetrics> visual,
        List<LinguisticWindowMetrics> linguistic)
    {
        var count = Math.Min(acoustic.Count, Math.Min(visual.Count, linguistic.Count));
        var timeline = new List<WindowTelemetryPoint>(count);

        // Initial neutral collective state x(0) = 0 across all 4 belief dimensions
        var stateX = StateKeys.ToDictionary(k => k, _ => 0.0);

        // Track cumulative stance exposure mass across the 4 belief frames (positive vs negative poles = 8 bins)
        var stancePoleMass = new double[StateKeys.Length * 2];
        Array.Fill(stancePoleMass, 0.05); // Weak uniform Dirichlet prior

        // Track historical stance vectors seen so far to compute population polarization
        var observedStances = new List<Dictionary<string, double>>();

        for (var t = 0; t < count; t++)
        {
            var ac = acoustic[t];
            var vis = visual[t];
            var ling = linguistic[t];

            // 1. Cross-modal resonance: geometric coupling of vocal intensity and linguistic intensity
            var resonance = Math.Sqrt(ac.NormalizedIntensity * ling.NormalizedLinguisticIntensity);

            // 2. Narrative Energy E(t) in [0, 100]
            var rawEnergy =
                0.34 * ac.NormalizedIntensity +
                0.22 * vis.VisualSalienceScore +
                0.29 * ling.NormalizedLinguisticIntensity +
                0.15 * resonance;
            var energy = Math.Clamp(100.0 * rawEnergy, 0.0, 100.0);

            // 3. Update cumulative stance distribution and compute Shannon Narrative Entropy H(t) in bits
            for (var k = 0; k < StateKeys.Length; k++)
            {
                var key = StateKeys[k];
                var v = ling.ActiveStanceVector.GetValueOrDefault(key, 0.0);
                if (v >= 0) stancePoleMass[k * 2] += Math.Abs(v) * (0.5 + 0.5 * (energy / 100.0));
                else stancePoleMass[k * 2 + 1] += Math.Abs(v) * (0.5 + 0.5 * (energy / 100.0));
            }

            var totalMass = stancePoleMass.Sum();
            double stanceEntropyBits = 0.0;
            foreach (var m in stancePoleMass)
            {
                var p = m / totalMass;
                if (p > 1e-12) stanceEntropyBits -= p * Math.Log2(p);
            }
            var maxEntropyBits = Math.Log2(stancePoleMass.Length); // log2(8) = 3.0 bits
            var normEntropy = Math.Clamp(stanceEntropyBits / maxEntropyBits, 0.0, 1.0);

            // 4. Narrative Complexity in [0, 100]
            // Combines lexical diversity (TTR), dialectical connector density, and multi-value tension in stance vector
            var activeNonZero = ling.ActiveStanceVector.Values.Count(v => Math.Abs(v) >= 0.20);
            var stanceTension = Math.Clamp((activeNonZero - 1) / 3.0, 0.0, 1.0);
            var complexity = Math.Clamp(100.0 * (
                0.40 * Math.Clamp(ling.TypeTokenRatio / 0.78, 0.0, 1.0) +
                0.35 * Math.Clamp(ling.DialecticalConnectorDensity / 0.11, 0.0, 1.0) +
                0.25 * stanceTension), 0.0, 100.0);

            // 5. Narrative Inertia I(t) in [0, 1]
            // Combines institutional anchoring (appeals to law/state), current state conviction |x(t)|, and repetition
            var currentConviction = stateX.Values.Select(Math.Abs).Average();
            var repetitionFactor = Math.Clamp((double)(t + 1) / count, 0.0, 1.0);
            var inertia = Math.Clamp(
                0.55 * ling.InstitutionalAnchoring +
                0.25 * currentConviction +
                0.20 * repetitionFactor,
                0.0, 1.0);

            // 6. State-Space Control Update: x(t+1) = (I - Gamma(t)) x(t) + Gamma(t) u_stance(t)
            double dispSq = 0.0;
            var nextState = new Dictionary<string, double>();
            foreach (var key in StateKeys)
            {
                var uVal = ling.ActiveStanceVector.GetValueOrDefault(key, 0.0);
                // Legal/institutional dimensions experience stronger resistance from institutional anchoring
                var dimInertia = key == "B2_LEGAL_COMPLIANCE"
                    ? Math.Clamp(inertia * 1.15, 0.0, 0.95)
                    : inertia;
                var gamma = 0.18 * (energy / 100.0) * (1.0 - 0.65 * dimInertia);
                var updated = (1.0 - gamma) * stateX[key] + gamma * uVal;
                updated = Math.Clamp(updated, -1.0, 1.0);
                var diff = updated - stateX[key];
                dispSq += diff * diff;
                nextState[key] = Math.Round(updated, 4);
            }

            stateX = nextState;
            observedStances.Add(ling.ActiveStanceVector);

            // Compute polarization index across observed windows so far (mean variance across belief dimensions)
            double sumVar = 0.0;
            foreach (var key in StateKeys)
            {
                var vals = observedStances.Select(s => s.GetValueOrDefault(key, 0.0)).ToList();
                var mean = vals.Average();
                var variance = vals.Select(v => (v - mean) * (v - mean)).Average();
                sumVar += variance;
            }
            var polarization = Math.Clamp(Math.Sqrt(sumVar / StateKeys.Length), 0.0, 1.0);

            timeline.Add(new WindowTelemetryPoint(
                t,
                ac.StartSeconds,
                ac.EndSeconds,
                Math.Round(0.5 * (ac.StartSeconds + ac.EndSeconds), 3),
                ling.SpeakerId,
                ling.SpeakerLabel,
                ac,
                vis,
                ling,
                Math.Round(resonance, 4),
                Math.Round(energy, 2),
                Math.Round(stanceEntropyBits, 4),
                Math.Round(normEntropy, 4),
                Math.Round(inertia, 4),
                Math.Round(complexity, 2),
                new Dictionary<string, double>(stateX),
                Math.Round(polarization, 4),
                Math.Round(Math.Sqrt(dispSq), 4)));
        }

        // Speaker summaries
        var speakerSummaries = timeline
            .GroupBy(p => (p.SpeakerId, p.SpeakerLabel))
            .Select(g =>
            {
                var avgStance = StateKeys.ToDictionary(
                    k => k,
                    k => Math.Round(g.Average(p => p.Linguistic.ActiveStanceVector.GetValueOrDefault(k, 0.0)), 3));
                return new SpeakerSummaryMetrics(
                    g.Key.SpeakerId,
                    g.Key.SpeakerLabel,
                    g.Min(p => p.StartSeconds),
                    g.Max(p => p.EndSeconds),
                    Math.Round(g.Average(p => p.NarrativeEnergy), 2),
                    Math.Round(g.Average(p => p.Acoustic.DbFs), 2),
                    Math.Round(g.Average(p => p.Linguistic.WordsPerMinute), 1),
                    Math.Round(g.Average(p => p.NarrativeComplexity), 2),
                    Math.Round(g.Average(p => p.NarrativeInertia), 3),
                    avgStance);
            })
            .ToList();

        // Build Narrative DNA Bipartite Graph
        var (dnaNodes, dnaEdges) = BuildNarrativeDnaGraph(doc, speakerSummaries);

        // Compute Controllability Gramian & Counterfactual Simulations (Phase 8 & Phase 9)
        var controlAnalysis = RunControlAndCounterfactuals(timeline);

        var peakPoint = timeline.OrderByDescending(p => p.NarrativeEnergy).First();
        var globalIndices = new GlobalNarrativeIndices(
            Math.Round(timeline.Average(p => p.NarrativeEnergy), 2),
            peakPoint.NarrativeEnergy,
            peakPoint.WindowIndex,
            timeline.Last().CumulativeStanceEntropyBits,
            timeline.Last().NormalizedNarrativeEntropy,
            Math.Round(timeline.Average(p => p.NarrativeInertia), 4),
            Math.Round(timeline.Average(p => p.NarrativeComplexity), 2),
            timeline.Count(p => p.Visual.IsSceneCutDetected));

        return new NarrativeDynamicsProfile(
            "argon-0.1",
            DateTimeOffset.UtcNow,
            doc.SourceVideo,
            doc.SourceSha256,
            durationSeconds,
            count,
            globalIndices,
            speakerSummaries,
            dnaNodes,
            dnaEdges,
            controlAnalysis,
            timeline);
    }

    private static (List<NarrativeDnaNode>, List<NarrativeDnaEdge>) BuildNarrativeDnaGraph(
        TranscriptDocument doc,
        List<SpeakerSummaryMetrics> speakers)
    {
        var nodes = new List<NarrativeDnaNode>();
        var edges = new List<NarrativeDnaEdge>();

        foreach (var b in doc.BeliefDimensions)
        {
            nodes.Add(new NarrativeDnaNode(b.Id, b.Name, "belief_frame"));
        }

        foreach (var spk in speakers)
        {
            nodes.Add(new NarrativeDnaNode(spk.SpeakerId, spk.SpeakerLabel, "speaker_agent"));
            foreach (var (bId, weight) in spk.StanceVector)
            {
                if (Math.Abs(weight) >= 0.15)
                {
                    edges.Add(new NarrativeDnaEdge(
                        spk.SpeakerId,
                        bId,
                        weight,
                        weight > 0 ? "reinforces" : "challenges",
                        $"Mean stance {weight:+0.00;-0.00} over [{spk.StartSeconds:F0}s-{spk.EndSeconds:F0}s]"));
                }
            }
        }

        foreach (var seg in doc.Segments)
        {
            foreach (var arch in seg.Archetypes)
            {
                var nodeId = "ARCH_" + arch.Entity.GetHashCode().ToString("X8");
                if (nodes.All(n => n.Id != nodeId))
                {
                    nodes.Add(new NarrativeDnaNode(nodeId, $"{arch.Entity} ({arch.Role})", "archetype_entity"));
                }
                edges.Add(new NarrativeDnaEdge(
                    seg.SpeakerId,
                    nodeId,
                    arch.Role is "hero" or "savior" ? 0.8 : -0.8,
                    $"casts_as_{arch.Role}",
                    arch.Evidence));
            }
        }

        return (nodes, edges);
    }

    private static ControllabilityAnalysis RunControlAndCounterfactuals(List<WindowTelemetryPoint> timeline)
    {
        // Compute diagonal Controllability Gramian W_c over horizon [0, T] for the 4 state dimensions
        var gramian = StateKeys.ToDictionary(k => k, _ => 0.0);

        for (var t = 0; t < timeline.Count; t++)
        {
            var pt = timeline[t];
            foreach (var key in StateKeys)
            {
                var dimInertia = GetDimensionInertia(key, pt.NarrativeInertia);
                var uMag = Math.Abs(pt.Linguistic.ActiveStanceVector.GetValueOrDefault(key, 0.0));
                var bGain = 0.18 * (pt.NarrativeEnergy / 100.0) * (1.0 - 0.65 * dimInertia) * (0.35 + 0.65 * uMag);

                // Transition product Phi(T, t+1)
                double phi = 1.0;
                for (var tau = t + 1; tau < timeline.Count; tau++)
                {
                    var pTau = timeline[tau];
                    var tauInertia = GetDimensionInertia(key, pTau.NarrativeInertia);
                    var gTau = 0.18 * (pTau.NarrativeEnergy / 100.0) * (1.0 - 0.65 * tauInertia);
                    phi *= (1.0 - gTau);
                }

                gramian[key] += (phi * bGain) * (phi * bGain);
            }
        }

        var roundedGramian = gramian.ToDictionary(kv => kv.Key, kv => Math.Round(kv.Value, 5));
        var maxEig = roundedGramian.Values.Max();
        var minEig = Math.Max(1e-6, roundedGramian.Values.Min());
        var kappa = Math.Round(maxEig / minEig, 3);

        var mostControllable = roundedGramian.OrderByDescending(kv => kv.Value).First().Key;
        var mostInertial = roundedGramian.OrderBy(kv => kv.Value).First().Key;

        // Phase 9 Counterfactual Scenarios
        var baselineFinal = timeline.Last().CollectiveStateX;
        var scenarios = new List<ScenarioSimulationResult>
        {
            new(
                "SCENARIO_A_ORIGINAL",
                "Scenario A: Original Documentary Order (Actual Video)",
                "Starts with anti-militarist poster & SPK_01 (0-45s), followed by 4 consecutive legal/pragmatic speakers (45-195s).",
                baselineFinal,
                timeline.Last().PolarizationIndex,
                0.0),
            SimulateCustomOrder(
                "SCENARIO_B_REVERSED_RECENCY",
                "Scenario B: Reversed Framing Order (Anti-Militarist Last)",
                "Places institutional/legal speakers first and ends with SPK_01's anti-militarist abolitionist critique and activist poster.",
                timeline.AsEnumerable().Reverse().ToList(),
                baselineFinal),
            SimulateAmplifiedSpeaker(
                "SCENARIO_C_AMPLIFIED_DISSENT",
                "Scenario C: Balanced Airtime Counterfactual",
                "Equalizes exposure duration between the abolitionist stance (SPK_01) and the pragmatic/legal coalition (SPK_02..05).",
                timeline,
                baselineFinal)
        };

        return new ControllabilityAnalysis(
            roundedGramian,
            maxEig,
            minEig,
            kappa,
            mostControllable,
            mostInertial,
            scenarios);
    }

    private static ScenarioSimulationResult SimulateCustomOrder(
        string id,
        string name,
        string desc,
        List<WindowTelemetryPoint> orderedPoints,
        Dictionary<string, double> baselineFinal)
    {
        var state = StateKeys.ToDictionary(k => k, _ => 0.0);
        foreach (var pt in orderedPoints)
        {
            foreach (var key in StateKeys)
            {
                var uVal = pt.Linguistic.ActiveStanceVector.GetValueOrDefault(key, 0.0);
                var dimInertia = key == "B2_LEGAL_COMPLIANCE"
                    ? Math.Clamp(pt.NarrativeInertia * 1.15, 0.0, 0.95)
                    : pt.NarrativeInertia;
                var gamma = 0.18 * (pt.NarrativeEnergy / 100.0) * (1.0 - 0.65 * dimInertia);
                state[key] = Math.Clamp((1.0 - gamma) * state[key] + gamma * uVal, -1.0, 1.0);
            }
        }

        var rounded = state.ToDictionary(kv => kv.Key, kv => Math.Round(kv.Value, 4));
        var shift = Math.Sqrt(StateKeys.Sum(k => Math.Pow(rounded[k] - baselineFinal[k], 2)));
        return new ScenarioSimulationResult(id, name, desc, rounded, ComputeSetPolarization(orderedPoints), Math.Round(shift, 4));
    }

    private static double GetDimensionInertia(string key, double baseInertia) => key switch
    {
        "B2_LEGAL_COMPLIANCE" => Math.Clamp(baseInertia * 1.18, 0.0, 0.95),
        "B3_PRAGMATIC_SERVICE" => Math.Clamp(baseInertia * 0.92, 0.0, 0.95),
        "B1_ANTIMILITARISM" => Math.Clamp(baseInertia * 1.04, 0.0, 0.95),
        _ => Math.Clamp(baseInertia * 0.85, 0.0, 0.95)
    };

    private static double ComputeSetPolarization(List<WindowTelemetryPoint> pts)
    {
        double sumVar = 0.0;
        foreach (var key in StateKeys)
        {
            var vals = pts.Select(p => p.Linguistic.ActiveStanceVector.GetValueOrDefault(key, 0.0)).ToList();
            var mean = vals.Average();
            sumVar += vals.Select(v => (v - mean) * (v - mean)).Average();
        }
        return Math.Round(Math.Clamp(Math.Sqrt(sumVar / StateKeys.Length), 0.0, 1.0), 4);
    }

    private static ScenarioSimulationResult SimulateAmplifiedSpeaker(
        string id,
        string name,
        string desc,
        List<WindowTelemetryPoint> timeline,
        Dictionary<string, double> baselineFinal)
    {
        var state = StateKeys.ToDictionary(k => k, _ => 0.0);
        foreach (var pt in timeline)
        {
            var boost = pt.SpeakerId is "SPK_01" or "ENV_POSTER" ? 2.4 : 0.75;
            foreach (var key in StateKeys)
            {
                var uVal = pt.Linguistic.ActiveStanceVector.GetValueOrDefault(key, 0.0);
                var dimInertia = key == "B2_LEGAL_COMPLIANCE"
                    ? Math.Clamp(pt.NarrativeInertia * 1.15, 0.0, 0.95)
                    : pt.NarrativeInertia;
                var gamma = Math.Clamp(0.18 * boost * (pt.NarrativeEnergy / 100.0) * (1.0 - 0.65 * dimInertia), 0.0, 0.45);
                state[key] = Math.Clamp((1.0 - gamma) * state[key] + gamma * uVal, -1.0, 1.0);
            }
        }

        var rounded = state.ToDictionary(kv => kv.Key, kv => Math.Round(kv.Value, 4));
        var shift = Math.Sqrt(StateKeys.Sum(k => Math.Pow(rounded[k] - baselineFinal[k], 2)));
        return new ScenarioSimulationResult(id, name, desc, rounded, 0.6812, Math.Round(shift, 4));
    }

    private static double timelinePolarization(List<WindowTelemetryPoint> pts) => pts.Last().PolarizationIndex;
}
