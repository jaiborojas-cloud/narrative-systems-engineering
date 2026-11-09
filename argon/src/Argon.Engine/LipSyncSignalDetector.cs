using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Argon.Engine;

public sealed record LipSyncStimulusManifestEntry(
    string StimulusId,
    string ConditionKind,
    double TrueAudioLagMs,
    bool IsPlaybackAudio,
    double Fps,
    double DurationSeconds,
    string AudioWavPath,
    string AudioSha256,
    string VideoFramesPath,
    string VideoSha256);

public sealed record LipSyncMachineMeasurement(
    string StimulusId,
    string ConditionKind,
    double TrueAudioLagMs,
    bool IsPlaybackGroundTruth,
    double EstimatedXCorrLagMs,
    double EstimatedBilabialOnsetLagMs,
    double ConsensusLagMs,
    double LagEstimationErrorMs,
    double AbsoluteLagErrorMs,
    double PeakEnvelopeCrossCorrelation,
    int MatchedBilabialOnsetCount,
    double MasterRepetitionCorrelation,
    double InterOnsetDriftStdMs,
    string MachinePredictedMode,
    double LipAudioOnsetDispersionMs);

public sealed record Exp005CalibrationReport(
    string SchemaVersion,
    DateTimeOffset CreatedUtc,
    string ExperimentId,
    string StatusNote,
    string StudioMasterWavPath,
    string StudioMasterSha256,
    int OffsetStimuliCount,
    double MeanBiasErrorMs,
    double MeanAbsoluteErrorMs,
    double RootMeanSquareErrorMs,
    double MaxAbsoluteErrorMs,
    double MachineMinimumDetectableLagMs,
    double PearsonLagCorrelation,
    bool PassesInstrumentCalibrationCriterion,
    List<LipSyncStimulusManifestEntry> StimuliManifest,
    List<LipSyncMachineMeasurement> Measurements);

public sealed record HumanObserverTrialRecord(
    string StimulusId,
    string Judgment,
    int SynchronyRating,
    bool AnomalyFelt,
    int Confidence,
    string ReasonText,
    string LockedAtUtc);

public sealed record HumanObserverSubmission(
    string SchemaVersion,
    string ObserverId,
    string CompletedAtUtc,
    List<HumanObserverTrialRecord> Trials);

public sealed record SignalPerceptionGapAnalysisReport(
    string SchemaVersion,
    DateTimeOffset CreatedUtc,
    int ObserverCount,
    int TotalTrialJudgments,
    double MachineMdlMs,
    double HumanSynchronyWindowWidthMs,
    double SignalPerceptionTemporalGapMs,
    double MachineReferenceAccuracy,
    double HumanUnshiftedAccuracy,
    double SignalPerceptionReferenceGap,
    double? MachineNoReferenceAccuracy,
    double? SignalPerceptionNoReferenceGap,
    string MachineMeasurementSource,
    double SynchronizedPlaybackAnomalyRate,
    double GenuineLiveAnomalyRate,
    double AnomalyDissociationDelta);

public static class LipSyncSignalDetector
{
    private const int SampleRate = 16000;
    private const double GridStepMs = 5.0;
    private const int GridSamples = 80; // 5 ms at 16 kHz

    // Irregular syllable onset schedule (in seconds) for synthetic calibration stimuli
    private static readonly double[] BaseSyllableOnsetsSec =
    [
        0.62, 1.28, 1.94, 2.68, 3.37, 4.12, 4.86, 5.61, 6.38, 7.14
    ];

    public static Exp005CalibrationReport RunSyntheticCalibrationSuite(string repoRoot)
    {
        var expDir = Path.Combine(repoRoot, "argon", "experiments", "EXP-005-signal-perception-gap");
        var stimDir = Path.Combine(expDir, "stimuli");
        Directory.CreateDirectory(stimDir);

        const double durationSec = 8.0;
        const double fps = 60.0;

        // 1. Render Studio Master reference vocal/guitar waveform (16 kHz mono PCM16)
        var masterWavPath = Path.Combine(stimDir, "studio_master.wav");
        var masterSamples = SynthesizeVocalTrack(
            durationSec,
            BaseSyllableOnsetsSec,
            audioShiftMs: 0.0,
            timingJitterStdMs: 0.0,
            noiseFloorAmplitude: 0.004,
            roomEchoGain: 0.0,
            seed: 5000);
        WritePcm16Wav(masterWavPath, masterSamples, SampleRate);
        var masterSha = ComputeFileSha256(masterWavPath);

        // 2. Render 8 controlled audio-lag stimuli: -120, -60, -30, 0, +30, +60, +120, +200 ms
        // Plus 1 genuine live take at 0 ms (with natural human micro-timing drift +/- 24 ms)
        double[] offsetsMs = [-120.0, -60.0, -30.0, 0.0, 30.0, 60.0, 120.0, 200.0];
        var manifest = new List<LipSyncStimulusManifestEntry>();

        for (int i = 0; i < offsetsMs.Length; i++)
        {
            double lagMs = offsetsMs[i];
            string signTag = lagMs < 0
                ? $"neg{Math.Abs((int)lagMs):D3}ms"
                : $"pos{(int)lagMs:D3}ms";
            string stimId = $"STIM-SYNTH-PLAYBACK-{signTag.ToUpperInvariant()}";

            var wavPath = Path.Combine(stimDir, $"{stimId}.wav");
            var rgbPath = Path.Combine(stimDir, $"{stimId}.rgb24seq");

            // Playback audio: uses the exact studio master onset schedule shifted by lagMs,
            // plus room acoustic reflection (12 ms echo) and ambient mic noise (0.012)
            var audioSamples = SynthesizeVocalTrack(
                durationSec,
                BaseSyllableOnsetsSec,
                audioShiftMs: lagMs,
                timingJitterStdMs: 0.0,
                noiseFloorAmplitude: 0.012,
                roomEchoGain: 0.18,
                seed: 6000 + i);
            WritePcm16Wav(wavPath, audioSamples, SampleRate);

            // Visual lip movement frames at 60 fps (unshifted visual reference, with realistic camera pixel noise)
            WriteMouthRoiFrames(rgbPath, durationSec, fps, BaseSyllableOnsetsSec, visualJitterStdMs: 3.5, seed: 7000 + i);

            manifest.Add(new LipSyncStimulusManifestEntry(
                stimId,
                "controlled_lag_playback",
                lagMs,
                IsPlaybackAudio: true,
                fps,
                durationSec,
                Path.GetRelativePath(repoRoot, wavPath).Replace('\\', '/'),
                ComputeFileSha256(wavPath),
                Path.GetRelativePath(repoRoot, rgbPath).Replace('\\', '/'),
                ComputeFileSha256(rgbPath)));
        }

        // Add genuine live performance at 0 ms lag (independent vocal take with natural 24 ms phrase micro-timing variation)
        {
            string liveId = "STIM-SYNTH-LIVE-POS000MS";
            var wavPath = Path.Combine(stimDir, $"{liveId}.wav");
            var rgbPath = Path.Combine(stimDir, $"{liveId}.rgb24seq");

            var rng = new Random(9001);
            var liveOnsets = BaseSyllableOnsetsSec
                .Select(t => t + NextGaussian(rng) * 0.024) // 24 ms natural human performance drift vs studio master
                .ToArray();

            var liveAudio = SynthesizeVocalTrack(
                durationSec,
                liveOnsets,
                audioShiftMs: 0.0,
                timingJitterStdMs: 0.0,
                noiseFloorAmplitude: 0.014,
                roomEchoGain: 0.15,
                seed: 9002);
            WritePcm16Wav(wavPath, liveAudio, SampleRate);
            WriteMouthRoiFrames(rgbPath, durationSec, fps, liveOnsets, visualJitterStdMs: 3.5, seed: 9003);

            manifest.Add(new LipSyncStimulusManifestEntry(
                liveId,
                "genuine_live_take",
                0.0,
                IsPlaybackAudio: false,
                fps,
                durationSec,
                Path.GetRelativePath(repoRoot, wavPath).Replace('\\', '/'),
                ComputeFileSha256(wavPath),
                Path.GetRelativePath(repoRoot, rgbPath).Replace('\\', '/'),
                ComputeFileSha256(rgbPath)));
        }

        // 3. Run the blind machine detector on every stimulus file from disk (without passing true lag)
        var measurements = new List<LipSyncMachineMeasurement>();
        foreach (var item in manifest)
        {
            var fullWav = Path.Combine(repoRoot, item.AudioWavPath.Replace('/', Path.DirectorySeparatorChar));
            var fullRgb = Path.Combine(repoRoot, item.VideoFramesPath.Replace('/', Path.DirectorySeparatorChar));

            var m = MeasureStimulusFromDisk(
                item.StimulusId,
                item.ConditionKind,
                item.TrueAudioLagMs,
                item.IsPlaybackAudio,
                fullWav,
                fullRgb,
                masterWavPath);
            measurements.Add(m);
        }

        var errors = measurements.Select(m => m.LagEstimationErrorMs).ToList();
        var absErrors = measurements.Select(m => m.AbsoluteLagErrorMs).ToList();

        double meanBias = Math.Round(errors.Average(), 2);
        double mae = Math.Round(absErrors.Average(), 2);
        double rmse = Math.Round(Math.Sqrt(errors.Select(e => e * e).Average()), 2);
        double maxAbs = Math.Round(absErrors.Max(), 2);
        double mdl = Math.Round(2.0 * rmse, 2);

        double pearsonR = ComputePearsonCorrelation(
            measurements.Select(m => m.TrueAudioLagMs).ToArray(),
            measurements.Select(m => m.ConsensusLagMs).ToArray());

        bool passes = mae <= 25.0 && pearsonR >= 0.90;

        return new Exp005CalibrationReport(
            "argon-exp005-0.2",
            DateTimeOffset.UtcNow,
            "EXP-005-SIGNAL-PERCEPTION-GAP",
            "Software self-test on synthetic stimuli generated alongside the detector; live/playback separation holds by construction. Not evidence of performance on real recordings. No human observer data have been collected.",
            Path.GetRelativePath(repoRoot, masterWavPath).Replace('\\', '/'),
            masterSha,
            measurements.Count,
            meanBias,
            mae,
            rmse,
            maxAbs,
            mdl,
            Math.Round(pearsonR, 5),
            passes,
            manifest,
            measurements);
    }

    public static LipSyncMachineMeasurement MeasureStimulusFromDisk(
        string stimulusId,
        string conditionKind,
        double trueAudioLagMs,
        bool isPlaybackGroundTruth,
        string wavPath,
        string rgbSeqPath,
        string masterWavPath)
    {
        var audioSamples = ReadPcm16WavMono(wavPath);
        var masterSamples = ReadPcm16WavMono(masterWavPath);
        var (fps, frames) = ReadMouthRoiFrames(rgbSeqPath);

        // 1. Extract visual mouth-aperture trajectory A_mouth(f) from raw RGB24 pixels
        var mouthAperture = new double[frames.Count];
        for (int f = 0; f < frames.Count; f++)
        {
            mouthAperture[f] = ComputeFrameMouthApertureIndex(frames[f], width: 32, height: 32);
        }

        // Compute positive visual opening velocity dA_+/dt at frame timestamps
        var visOpeningRateFrames = new double[frames.Count];
        for (int f = 1; f < frames.Count; f++)
        {
            double diff = mouthAperture[f] - mouthAperture[f - 1];
            visOpeningRateFrames[f] = Math.Max(0.0, diff);
        }

        // Resample visual opening rate onto the 5 ms (200 Hz) analysis grid.
        // Timing convention (v0.2 fix): both difference signals are stamped at the midpoint of the
        // interval they span. The acoustic flux acOnsetGrid[g] = rms[g] - rms[g-1] compares two
        // 5 ms blocks centred at (g-0.5)*5 ms and (g+0.5)*5 ms, so it belongs to t = g*5 ms.
        // The visual difference A[f] - A[f-1] spans frames at (f-1)/fps and f/fps, so it belongs to
        // t = (f-0.5)/fps, i.e. fractional frame index t*fps + 0.5. The v0.1 code stamped the audio
        // flux 2.5 ms late and the visual difference half a frame (8.3 ms at 60 fps) late, which
        // produced a constant bias of about -5 ms in every estimate.
        int gridCount = audioSamples.Length / GridSamples;
        var visGrid = new double[gridCount];
        for (int g = 0; g < gridCount; g++)
        {
            double tSec = g * (GridStepMs / 1000.0);
            double fExactFrame = tSec * fps + 0.5;
            int f0 = Math.Clamp((int)Math.Floor(fExactFrame), 0, frames.Count - 1);
            int f1 = Math.Clamp(f0 + 1, 0, frames.Count - 1);
            double frac = fExactFrame - f0;
            visGrid[g] = (1.0 - frac) * visOpeningRateFrames[f0] + frac * visOpeningRateFrames[f1];
        }

        // 2. Extract acoustic RMS envelope E_ac(g) and positive acoustic onset flux dE_+/dt on 5 ms grid
        var acRmsGrid = new double[gridCount];
        for (int g = 0; g < gridCount; g++)
        {
            int start = g * GridSamples;
            double sumSq = 0.0;
            for (int s = 0; s < GridSamples; s++)
            {
                double v = audioSamples[start + s];
                sumSq += v * v;
            }
            acRmsGrid[g] = Math.Sqrt(sumSq / GridSamples);
        }

        var acOnsetGrid = new double[gridCount];
        for (int g = 1; g < gridCount; g++)
        {
            acOnsetGrid[g] = Math.Max(0.0, acRmsGrid[g] - acRmsGrid[g - 1]);
        }

        // 3. Method A: Envelope Cross-Correlation over [-300 ms, +300 ms] with parabolic sub-grid refinement
        int maxLagSteps = (int)(300.0 / GridStepMs); // 60 steps = 300 ms
        var corrAtLag = new double[2 * maxLagSteps + 1];
        int bestIdx = maxLagSteps;
        double bestCorr = double.NegativeInfinity;

        for (int lagStep = -maxLagSteps; lagStep <= maxLagSteps; lagStep++)
        {
            double corr = ComputeShiftedCorrelation(visGrid, acOnsetGrid, lagStep);
            int arrIdx = lagStep + maxLagSteps;
            corrAtLag[arrIdx] = corr;
            if (corr > bestCorr)
            {
                bestCorr = corr;
                bestIdx = arrIdx;
            }
        }

        double subStepOffset = 0.0;
        if (bestIdx > 0 && bestIdx < corrAtLag.Length - 1)
        {
            double yLeft = corrAtLag[bestIdx - 1];
            double yMid = corrAtLag[bestIdx];
            double yRight = corrAtLag[bestIdx + 1];
            double denom = 2.0 * (2.0 * yMid - yLeft - yRight);
            if (Math.Abs(denom) > 1e-9)
            {
                subStepOffset = Math.Clamp((yRight - yLeft) / denom, -0.5, 0.5);
            }
        }

        double xcorrLagMs = ((bestIdx - maxLagSteps) + subStepOffset) * GridStepMs;

        // 4. Method B: Bilabial Closure-to-Burst Onset Matching
        var visPeaksMs = DetectLocalPeaksMs(visGrid, GridStepMs, minSeparationMs: 350.0, thresholdFraction: 0.30);
        var acPeaksMs = DetectLocalPeaksMs(acOnsetGrid, GridStepMs, minSeparationMs: 350.0, thresholdFraction: 0.30);

        var matchedDiffs = new List<double>();
        foreach (var vMs in visPeaksMs)
        {
            double bestDiff = double.PositiveInfinity;
            foreach (var aMs in acPeaksMs)
            {
                double diff = aMs - vMs;
                if (Math.Abs(diff) < Math.Abs(bestDiff))
                {
                    bestDiff = diff;
                }
            }
            if (Math.Abs(bestDiff) <= 260.0)
            {
                matchedDiffs.Add(bestDiff);
            }
        }

        double onsetLagMs = matchedDiffs.Count > 0 ? ComputeMedian(matchedDiffs) : xcorrLagMs;

        // Reference-free cue for H2b: dispersion of lip-to-audio onset differences within the stimulus.
        double lipAudioDispersionMs = 0.0;
        if (matchedDiffs.Count > 1)
        {
            double meanDiff = matchedDiffs.Average();
            lipAudioDispersionMs = Math.Sqrt(matchedDiffs.Select(d => (d - meanDiff) * (d - meanDiff)).Average());
        }
        double consensusLagMs = Math.Round(0.65 * xcorrLagMs + 0.35 * onsetLagMs, 2);
        double errMs = Math.Round(consensusLagMs - trueAudioLagMs, 2);

        // 5. Master Repetition & Inter-Onset Micro-Timing Drift Test against studio_master.wav
        var masterRmsGrid = new double[Math.Min(gridCount, masterSamples.Length / GridSamples)];
        var masterOnsetGrid = new double[masterRmsGrid.Length];
        for (int g = 0; g < masterRmsGrid.Length; g++)
        {
            int start = g * GridSamples;
            double sumSq = 0.0;
            for (int s = 0; s < GridSamples; s++)
            {
                double v = masterSamples[start + s];
                sumSq += v * v;
            }
            masterRmsGrid[g] = Math.Sqrt(sumSq / GridSamples);
            if (g > 0) masterOnsetGrid[g] = Math.Max(0.0, masterRmsGrid[g] - masterRmsGrid[g - 1]);
        }

        // Find best audio-to-master alignment lag
        int bestAudioLagStep = 0;
        double bestMasterCorr = double.NegativeInfinity;
        for (int lagStep = -maxLagSteps; lagStep <= maxLagSteps; lagStep++)
        {
            double c = ComputeShiftedCorrelation(masterRmsGrid, acRmsGrid, lagStep);
            if (c > bestMasterCorr)
            {
                bestMasterCorr = c;
                bestAudioLagStep = lagStep;
            }
        }

        // Compute inter-onset timing drift residuals between studio master peaks and aligned candidate audio peaks
        var masterPeaksMs = DetectLocalPeaksMs(masterOnsetGrid, GridStepMs, minSeparationMs: 350.0, thresholdFraction: 0.30);
        double alignedShiftMs = bestAudioLagStep * GridStepMs;
        var timingResiduals = new List<double>();
        foreach (var mPeak in masterPeaksMs)
        {
            double expectedAcMs = mPeak + alignedShiftMs;
            double nearestResidual = acPeaksMs.Select(a => a - expectedAcMs).OrderBy(Math.Abs).FirstOrDefault();
            if (Math.Abs(nearestResidual) < 120.0)
            {
                timingResiduals.Add(nearestResidual);
            }
        }

        double ioiDriftStdMs = 0.0;
        if (timingResiduals.Count > 1)
        {
            double meanRes = timingResiduals.Average();
            ioiDriftStdMs = Math.Sqrt(timingResiduals.Select(r => (r - meanRes) * (r - meanRes)).Average());
        }

        // Reference-based classification rule (H2a). Thresholds were chosen for the synthetic suite, where the
        // separation holds by construction; re-fix them on pilot recordings before the main study.
        string predictedMode = (bestMasterCorr >= 0.94 && ioiDriftStdMs < 8.0) ? "playback" : "live";

        return new LipSyncMachineMeasurement(
            stimulusId,
            conditionKind,
            trueAudioLagMs,
            isPlaybackGroundTruth,
            Math.Round(xcorrLagMs, 2),
            Math.Round(onsetLagMs, 2),
            consensusLagMs,
            errMs,
            Math.Round(Math.Abs(errMs), 2),
            Math.Round(bestCorr, 4),
            matchedDiffs.Count,
            Math.Round(bestMasterCorr, 4),
            Math.Round(ioiDriftStdMs, 2),
            predictedMode,
            Math.Round(lipAudioDispersionMs, 2));
    }

    public static SignalPerceptionGapAnalysisReport AnalyzeHumanResponses(
        List<LipSyncMachineMeasurement> machineMeasurements,
        double machineMdlMs,
        string machineMeasurementSource,
        List<HumanObserverSubmission> submissions,
        double? noReferenceDispersionThresholdMs = null)
    {
        var allTrials = submissions.SelectMany(s => s.Trials).ToList();
        var machineByStim = machineMeasurements.ToDictionary(m => m.StimulusId, StringComparer.OrdinalIgnoreCase);
        var matchedTrials = allTrials.Where(t => machineByStim.ContainsKey(t.StimulusId)).ToList();

        // W_human: proportion rated synchronous (S >= 4) per tested offset, then linear interpolation of the
        // 0.5 crossing on each side of zero. Provisional: the protocol requires a logistic fit before main-study analysis.
        var syncRateByLag = matchedTrials
            .GroupBy(t => machineByStim[t.StimulusId].TrueAudioLagMs)
            .OrderBy(g => g.Key)
            .Select(g => (Lag: g.Key, Rate: g.Count(t => t.SynchronyRating >= 4) / (double)g.Count()))
            .ToList();
        double leadEdge = FindHalfCrossing(syncRateByLag.Where(p => p.Lag <= 0).OrderByDescending(p => p.Lag).ToList());
        double lagEdge = FindHalfCrossing(syncRateByLag.Where(p => p.Lag >= 0).OrderBy(p => p.Lag).ToList());
        double humanWindowWidthMs = Math.Round(lagEdge - leadEdge, 2);
        double spgTemporalMs = Math.Round(humanWindowWidthMs - 2.0 * machineMdlMs, 2);

        // H2 and H3 use unshifted takes only (protocol 0.2, Section 4.4).
        static bool IsUnshifted(LipSyncMachineMeasurement m) => Math.Abs(m.TrueAudioLagMs) < 0.5;
        static string Truth(LipSyncMachineMeasurement m) => m.IsPlaybackGroundTruth ? "playback" : "live";

        var unshiftedTrials = matchedTrials.Where(t => IsUnshifted(machineByStim[t.StimulusId])).ToList();
        double humanAcc = unshiftedTrials.Count > 0
            ? Math.Round(unshiftedTrials.Count(t => string.Equals(t.Judgment, Truth(machineByStim[t.StimulusId]), StringComparison.OrdinalIgnoreCase)) / (double)unshiftedTrials.Count, 4)
            : 0.0;

        var unshiftedMachine = machineMeasurements.Where(IsUnshifted).ToList();
        double machineRefAcc = unshiftedMachine.Count > 0
            ? Math.Round(unshiftedMachine.Count(m => Truth(m) == m.MachinePredictedMode) / (double)unshiftedMachine.Count, 4)
            : 0.0;

        double? machineNoRefAcc = null;
        double? noRefGap = null;
        if (noReferenceDispersionThresholdMs is double thr && unshiftedMachine.Count > 0)
        {
            // Hypothesis direction: miming produces larger lip-to-audio onset dispersion than live singing.
            machineNoRefAcc = Math.Round(unshiftedMachine.Count(m => (m.LipAudioOnsetDispersionMs > thr ? "playback" : "live") == Truth(m)) / (double)unshiftedMachine.Count, 4);
            noRefGap = Math.Round(machineNoRefAcc.Value - humanAcc, 4);
        }

        var syncPlaybackTrials = unshiftedTrials
            .Where(t => machineByStim[t.StimulusId].IsPlaybackGroundTruth && t.SynchronyRating >= 4)
            .ToList();
        var liveTrials = unshiftedTrials
            .Where(t => !machineByStim[t.StimulusId].IsPlaybackGroundTruth)
            .ToList();

        double playbackAnomalyRate = syncPlaybackTrials.Count > 0
            ? Math.Round(syncPlaybackTrials.Count(t => t.AnomalyFelt) / (double)syncPlaybackTrials.Count, 4)
            : 0.0;
        double liveAnomalyRate = liveTrials.Count > 0
            ? Math.Round(liveTrials.Count(t => t.AnomalyFelt) / (double)liveTrials.Count, 4)
            : 0.0;

        return new SignalPerceptionGapAnalysisReport(
            "argon-exp005-analysis-0.2",
            DateTimeOffset.UtcNow,
            submissions.Count,
            allTrials.Count,
            machineMdlMs,
            humanWindowWidthMs,
            spgTemporalMs,
            machineRefAcc,
            humanAcc,
            Math.Round(machineRefAcc - humanAcc, 4),
            machineNoRefAcc,
            noRefGap,
            machineMeasurementSource,
            playbackAnomalyRate,
            liveAnomalyRate,
            Math.Round(playbackAnomalyRate - liveAnomalyRate, 4));
    }

    // Walks outward from zero lag and returns the interpolated lag where the synchronous proportion falls below 0.5.
    // Returns the outermost tested lag if it never falls below 0.5, or 0 if the 0 ms condition is already below 0.5.
    private static double FindHalfCrossing(List<(double Lag, double Rate)> outward)
    {
        if (outward.Count == 0) return 0.0;
        if (outward[0].Rate < 0.5) return outward[0].Lag;
        for (int i = 1; i < outward.Count; i++)
        {
            var (l0, r0) = outward[i - 1];
            var (l1, r1) = outward[i];
            if (r1 < 0.5)
            {
                return l0 + (r0 - 0.5) / (r0 - r1) * (l1 - l0);
            }
        }
        return outward[^1].Lag;
    }

    public static LipSyncMachineMeasurement MeasureRealVideoWithFfmpeg(
        string repoRoot,
        string videoPath,
        string masterWavPath,
        double trueAudioLagMs = 0.0,
        bool isPlaybackGroundTruth = false,
        double fps = 60.0,
        string cropFilter = "crop=iw*0.25:ih*0.20:iw*0.375:ih*0.35,scale=32:32")
    {
        var ffmpegCandidate = Path.Combine(repoRoot, ".tools", "ffmpeg", "bin", "ffmpeg.exe");
        var ffmpegExe = File.Exists(ffmpegCandidate) ? ffmpegCandidate : "ffmpeg";

        var tempDir = Path.Combine(repoRoot, "argon", "experiments", "EXP-005-signal-perception-gap", "temp_extract");
        Directory.CreateDirectory(tempDir);

        var baseName = Path.GetFileNameWithoutExtension(videoPath);
        var extractedWav = Path.Combine(tempDir, $"{baseName}.16k.wav");
        var rawRgbFile = Path.Combine(tempDir, $"{baseName}.raw.rgb24");
        var rgbSeqFile = Path.Combine(tempDir, $"{baseName}.rgb24seq");

        RunProcess(ffmpegExe, $"-y -v error -i \"{videoPath}\" -ac 1 -ar {SampleRate} -f wav \"{extractedWav}\"");
        RunProcess(ffmpegExe, $"-y -v error -i \"{videoPath}\" -vf \"fps={fps.ToString(CultureInfo.InvariantCulture)},{cropFilter}\" -f rawvideo -pix_fmt rgb24 \"{rawRgbFile}\"");

        var rawBytes = File.ReadAllBytes(rawRgbFile);
        const int w = 32, h = 32;
        int frameBytes = w * h * 3;
        int frameCount = rawBytes.Length / frameBytes;

        using (var fs = new FileStream(rgbSeqFile, FileMode.Create, FileAccess.Write))
        using (var bw = new BinaryWriter(fs))
        {
            bw.Write(fps);
            bw.Write(frameCount);
            bw.Write(w);
            bw.Write(h);
            bw.Write(rawBytes, 0, frameCount * frameBytes);
        }

        return MeasureStimulusFromDisk(
            baseName,
            "real_recorded_video",
            trueAudioLagMs,
            isPlaybackGroundTruth,
            extractedWav,
            rgbSeqFile,
            masterWavPath);
    }

    private static void RunProcess(string fileName, string arguments)
    {
        var psi = new System.Diagnostics.ProcessStartInfo
        {
            FileName = fileName,
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        using var proc = System.Diagnostics.Process.Start(psi)
            ?? throw new InvalidOperationException($"Failed to start process: {fileName}");
        proc.WaitForExit();
        if (proc.ExitCode != 0)
        {
            var err = proc.StandardError.ReadToEnd();
            throw new InvalidOperationException($"Process {fileName} exited with code {proc.ExitCode}: {err}");
        }
    }

    private static double[] SynthesizeVocalTrack(
        double durationSec,
        double[] syllableOnsetsSec,
        double audioShiftMs,
        double timingJitterStdMs,
        double noiseFloorAmplitude,
        double roomEchoGain,
        int seed)
    {
        int totalSamples = (int)(durationSec * SampleRate);
        var samples = new double[totalSamples];
        var rng = new Random(seed);
        double shiftSec = audioShiftMs / 1000.0;

        foreach (var baseOnset in syllableOnsetsSec)
        {
            double jitterSec = timingJitterStdMs > 0 ? NextGaussian(rng) * (timingJitterStdMs / 1000.0) : 0.0;
            double onsetSec = baseOnset + shiftSec + jitterSec;
            double sylDur = 0.34; // 340 ms syllable

            int startSample = Math.Max(0, (int)(onsetSec * SampleRate));
            int endSample = Math.Min(totalSamples - 1, (int)((onsetSec + sylDur) * SampleRate));

            for (int i = startSample; i <= endSample; i++)
            {
                double localT = (i - startSample) / (double)SampleRate;
                // Sharp bilabial plosive release attack (12 ms rise) + exponential vocal decay
                double env = localT < 0.012
                    ? (localT / 0.012)
                    : Math.Exp(-6.5 * (localT - 0.012));

                // Initial 15 ms bilabial burst + harmonic vowel formants (F0 = 145 Hz, 290 Hz, 720 Hz)
                double plosive = localT < 0.015 ? (rng.NextDouble() * 2.0 - 1.0) * 0.45 : 0.0;
                double f0 = 145.0 + 4.0 * Math.Sin(2.0 * Math.PI * 5.5 * localT);
                double voiced =
                    0.55 * Math.Sin(2.0 * Math.PI * f0 * localT) +
                    0.30 * Math.Sin(2.0 * Math.PI * 2.0 * f0 * localT) +
                    0.15 * Math.Sin(2.0 * Math.PI * 5.0 * f0 * localT);

                samples[i] += 0.72 * env * (plosive + voiced);
            }
        }

        // Optional simulated room reflection (12 ms acoustic delay) and ambient microphone noise floor
        var output = new double[totalSamples];
        int echoDelaySamples = (int)(0.012 * SampleRate);
        for (int i = 0; i < totalSamples; i++)
        {
            double v = samples[i];
            if (roomEchoGain > 0 && i >= echoDelaySamples)
            {
                v += roomEchoGain * samples[i - echoDelaySamples];
            }
            v += (rng.NextDouble() * 2.0 - 1.0) * noiseFloorAmplitude;
            output[i] = Math.Clamp(v, -0.98, 0.98);
        }

        return output;
    }

    private static void WriteMouthRoiFrames(
        string rgbPath,
        double durationSec,
        double fps,
        double[] syllableOnsetsSec,
        double visualJitterStdMs,
        int seed)
    {
        int frameCount = (int)(durationSec * fps);
        const int w = 32, h = 32;
        var rng = new Random(seed);

        var jitteredOnsets = syllableOnsetsSec
            .Select(t => t + (visualJitterStdMs > 0 ? NextGaussian(rng) * (visualJitterStdMs / 1000.0) : 0.0))
            .ToArray();

        using var fs = new FileStream(rgbPath, FileMode.Create, FileAccess.Write);
        using var bw = new BinaryWriter(fs);
        bw.Write(fps);
        bw.Write(frameCount);
        bw.Write(w);
        bw.Write(h);

        var frameBuf = new byte[w * h * 3];
        for (int f = 0; f < frameCount; f++)
        {
            double tSec = f / fps;
            double aperture = 0.04; // closed lips baseline

            foreach (var onset in jitteredOnsets)
            {
                double dt = tSec - onset;
                if (dt >= -0.025 && dt < 0.0)
                {
                    // Bilabial compression before release
                    aperture = 0.01;
                }
                else if (dt >= 0.0 && dt <= 0.32)
                {
                    double openEnv = dt < 0.015
                        ? (dt / 0.015)
                        : Math.Exp(-6.5 * (dt - 0.015));
                    aperture = Math.Max(aperture, 0.04 + 0.78 * openEnv);
                }
            }

            double halfGapY = aperture * 9.5; // vertical mouth opening in pixels
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    double dx = (x - 15.5) / 10.0;
                    double dy = y - 15.5;
                    bool insideMouth = (dx * dx) + (halfGapY > 0.2 ? (dy * dy) / (halfGapY * halfGapY) : 999.0) <= 1.0;

                    int noise = rng.Next(-4, 5);
                    byte r = (byte)Math.Clamp((insideMouth ? 24 : 185) + noise, 0, 255);
                    byte g = (byte)Math.Clamp((insideMouth ? 16 : 142) + noise, 0, 255);
                    byte b = (byte)Math.Clamp((insideMouth ? 18 : 130) + noise, 0, 255);

                    int idx = (y * w + x) * 3;
                    frameBuf[idx] = r;
                    frameBuf[idx + 1] = g;
                    frameBuf[idx + 2] = b;
                }
            }

            bw.Write(frameBuf);
        }
    }

    private static (double Fps, List<byte[]> Frames) ReadMouthRoiFrames(string rgbSeqPath)
    {
        using var fs = new FileStream(rgbSeqPath, FileMode.Open, FileAccess.Read);
        using var br = new BinaryReader(fs);
        double fps = br.ReadDouble();
        int frameCount = br.ReadInt32();
        int w = br.ReadInt32();
        int h = br.ReadInt32();
        int frameBytes = w * h * 3;

        var list = new List<byte[]>(frameCount);
        for (int i = 0; i < frameCount; i++)
        {
            list.Add(br.ReadBytes(frameBytes));
        }
        return (fps, list);
    }

    private static double ComputeFrameMouthApertureIndex(byte[] rgb, int width, int height)
    {
        // Measure fraction of dark oral-cavity pixels in central lip region (x: 6..25, y: 6..25)
        int darkCount = 0, total = 0;
        double luminanceSum = 0.0;
        for (int y = 6; y < height - 6; y++)
        {
            for (int x = 6; x < width - 6; x++)
            {
                int idx = (y * width + x) * 3;
                double lum = 0.2126 * rgb[idx] + 0.7152 * rgb[idx + 1] + 0.0722 * rgb[idx + 2];
                luminanceSum += lum;
                if (lum < 65.0) darkCount++;
                total++;
            }
        }
        double darkRatio = (double)darkCount / Math.Max(1, total);
        double invLum = 1.0 - (luminanceSum / (total * 255.0));
        return 0.70 * darkRatio + 0.30 * invLum;
    }

    private static double ComputeShiftedCorrelation(double[] refSeries, double[] targetSeries, int lagSteps)
    {
        int n = Math.Min(refSeries.Length, targetSeries.Length);
        double sumX = 0.0, sumY = 0.0;
        int count = 0;

        for (int i = 0; i < n; i++)
        {
            int j = i + lagSteps;
            if (j < 0 || j >= n) continue;
            sumX += refSeries[i];
            sumY += targetSeries[j];
            count++;
        }
        if (count < 10) return 0.0;

        double meanX = sumX / count;
        double meanY = sumY / count;
        double num = 0.0, denX = 0.0, denY = 0.0;

        for (int i = 0; i < n; i++)
        {
            int j = i + lagSteps;
            if (j < 0 || j >= n) continue;
            double dx = refSeries[i] - meanX;
            double dy = targetSeries[j] - meanY;
            num += dx * dy;
            denX += dx * dx;
            denY += dy * dy;
        }

        double denom = Math.Sqrt(denX * denY);
        return denom > 1e-12 ? num / denom : 0.0;
    }

    private static List<double> DetectLocalPeaksMs(double[] grid, double stepMs, double minSeparationMs, double thresholdFraction)
    {
        var peaks = new List<double>();
        if (grid.Length == 0) return peaks;
        double maxVal = grid.Max();
        double thresh = maxVal * thresholdFraction;
        int minSepSteps = Math.Max(1, (int)(minSeparationMs / stepMs));
        int lastPeakIdx = -minSepSteps * 2;

        for (int i = 1; i < grid.Length - 1; i++)
        {
            if (grid[i] >= thresh && grid[i] >= grid[i - 1] && grid[i] >= grid[i + 1])
            {
                if (i - lastPeakIdx >= minSepSteps)
                {
                    peaks.Add(i * stepMs);
                    lastPeakIdx = i;
                }
                else if (peaks.Count > 0 && grid[i] > grid[lastPeakIdx])
                {
                    peaks[^1] = i * stepMs;
                    lastPeakIdx = i;
                }
            }
        }
        return peaks;
    }

    private static double ComputeMedian(List<double> values)
    {
        var sorted = values.OrderBy(v => v).ToList();
        int mid = sorted.Count / 2;
        return sorted.Count % 2 == 1
            ? sorted[mid]
            : 0.5 * (sorted[mid - 1] + sorted[mid]);
    }

    private static double ComputePearsonCorrelation(double[] x, double[] y)
    {
        double mx = x.Average(), my = y.Average();
        double num = 0.0, dx2 = 0.0, dy2 = 0.0;
        for (int i = 0; i < x.Length; i++)
        {
            double dx = x[i] - mx;
            double dy = y[i] - my;
            num += dx * dy;
            dx2 += dx * dx;
            dy2 += dy * dy;
        }
        return Math.Sqrt(dx2 * dy2) > 1e-12 ? num / Math.Sqrt(dx2 * dy2) : 0.0;
    }

    private static void WritePcm16Wav(string path, double[] samples, int sampleRate)
    {
        using var fs = new FileStream(path, FileMode.Create, FileAccess.Write);
        using var bw = new BinaryWriter(fs);
        int byteRate = sampleRate * 2;
        int dataSize = samples.Length * 2;

        bw.Write(Encoding.ASCII.GetBytes("RIFF"));
        bw.Write(36 + dataSize);
        bw.Write(Encoding.ASCII.GetBytes("WAVE"));
        bw.Write(Encoding.ASCII.GetBytes("fmt "));
        bw.Write(16);
        bw.Write((short)1); // PCM
        bw.Write((short)1); // Mono
        bw.Write(sampleRate);
        bw.Write(byteRate);
        bw.Write((short)2); // BlockAlign
        bw.Write((short)16); // BitsPerSample
        bw.Write(Encoding.ASCII.GetBytes("data"));
        bw.Write(dataSize);

        foreach (var s in samples)
        {
            short pcm = (short)Math.Clamp(Math.Round(s * 32767.0), short.MinValue, short.MaxValue);
            bw.Write(pcm);
        }
    }

    private static double[] ReadPcm16WavMono(string path)
    {
        var bytes = File.ReadAllBytes(path);
        int offset = 12;
        while (offset + 8 <= bytes.Length)
        {
            string chunkId = Encoding.ASCII.GetString(bytes, offset, 4);
            int chunkSize = BitConverter.ToInt32(bytes, offset + 4);
            if (chunkId == "data")
            {
                int sampleCount = Math.Min(chunkSize, bytes.Length - (offset + 8)) / 2;
                var samples = new double[sampleCount];
                int pos = offset + 8;
                for (int i = 0; i < sampleCount; i++)
                {
                    short val = BitConverter.ToInt16(bytes, pos + i * 2);
                    samples[i] = val / 32768.0;
                }
                return samples;
            }
            offset += 8 + chunkSize;
        }
        throw new InvalidDataException($"WAV file has no data chunk: {path}");
    }

    private static string ComputeFileSha256(string path)
    {
        var bytes = File.ReadAllBytes(path);
        return Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
    }

    private static double NextGaussian(Random rng)
    {
        double u1 = 1.0 - rng.NextDouble();
        double u2 = 1.0 - rng.NextDouble();
        return Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2.0 * Math.PI * u2);
    }
}
