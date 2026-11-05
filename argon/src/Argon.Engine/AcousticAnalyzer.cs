namespace Argon.Engine;

public sealed record AcousticWindowMetrics(
    int WindowIndex,
    double StartSeconds,
    double EndSeconds,
    double RmsAmplitude,
    double DbFs,
    double PeakAmplitude,
    double ZeroCrossingRate,
    double SpeechActivityRatio,
    double DynamicRangeDb,
    double NormalizedIntensity);

public static class AcousticAnalyzer
{
    public static List<AcousticWindowMetrics> AnalyzeWav(
        string wavPath,
        IReadOnlyList<(double StartSeconds, double EndSeconds)> windows)
    {
        var bytes = File.ReadAllBytes(wavPath);
        var dataOffset = FindDataChunkOffset(bytes, out var dataLength, out var sampleRate);
        var totalSamples = dataLength / 2;
        var samples = new float[totalSamples];
        for (var i = 0; i < totalSamples; i++)
        {
            short pcm = BitConverter.ToInt16(bytes, dataOffset + i * 2);
            samples[i] = pcm / 32768f;
        }

        // First pass: compute raw metrics per window
        var raw = new List<(int idx, double start, double end, double rms, double dbfs, double peak, double zcr, double vad, double dynDb)>();
        foreach (var (w, idx) in windows.Select((w, i) => (w, i)))
        {
            var s0 = Math.Clamp((int)Math.Round(w.StartSeconds * sampleRate), 0, totalSamples);
            var s1 = Math.Clamp((int)Math.Round(w.EndSeconds * sampleRate), s0, totalSamples);
            var count = Math.Max(1, s1 - s0);

            double sumSq = 0;
            double peak = 0;
            long zeroCrossings = 0;
            for (var i = s0; i < s1; i++)
            {
                var v = (double)samples[i];
                sumSq += v * v;
                var abs = Math.Abs(v);
                if (abs > peak) peak = abs;
                if (i > s0 && ((samples[i - 1] >= 0 && samples[i] < 0) || (samples[i - 1] < 0 && samples[i] >= 0)))
                {
                    zeroCrossings++;
                }
            }

            var rms = Math.Sqrt(sumSq / count);
            var dbfs = 20.0 * Math.Log10(Math.Max(rms, 1e-5));

            // Sub-window 100ms analysis for Speech Activity Ratio (VAD) and Dynamic Range
            var subLen = Math.Max(1, sampleRate / 10);
            var subDbs = new List<double>();
            for (var p = s0; p < s1; p += subLen)
            {
                var endP = Math.Min(s1, p + subLen);
                double subSq = 0;
                for (var k = p; k < endP; k++) subSq += (double)samples[k] * samples[k];
                var subRms = Math.Sqrt(subSq / Math.Max(1, endP - p));
                subDbs.Add(20.0 * Math.Log10(Math.Max(subRms, 1e-5)));
            }

            subDbs.Sort();
            var p10 = subDbs[(int)Math.Floor(0.10 * (subDbs.Count - 1))];
            var p90 = subDbs[(int)Math.Floor(0.90 * (subDbs.Count - 1))];
            var dynRange = Math.Max(0, p90 - p10);

            // Speech activity threshold: sub-frames above -31 dBFS (above campus background murmur)
            var activeSub = subDbs.Count(d => d > -31.0);
            var vad = (double)activeSub / Math.Max(1, subDbs.Count);
            var zcr = (double)zeroCrossings / count;

            raw.Add((idx, w.StartSeconds, w.EndSeconds, rms, dbfs, peak, zcr, vad, dynRange));
        }

        var minDb = raw.Min(r => r.dbfs);
        var maxDb = raw.Max(r => r.dbfs);
        var span = Math.Max(1.0, maxDb - minDb);

        return raw.Select(r =>
        {
            var normLevel = Math.Clamp((r.dbfs - minDb) / span, 0.0, 1.0);
            var normDyn = Math.Clamp(r.dynDb / 18.0, 0.0, 1.0);
            var intensity = Math.Clamp(0.55 * normLevel + 0.30 * r.vad + 0.15 * normDyn, 0.0, 1.0);
            return new AcousticWindowMetrics(
                r.idx,
                Math.Round(r.start, 3),
                Math.Round(r.end, 3),
                Math.Round(r.rms, 5),
                Math.Round(r.dbfs, 2),
                Math.Round(r.peak, 4),
                Math.Round(r.zcr, 4),
                Math.Round(r.vad, 3),
                Math.Round(r.dynDb, 2),
                Math.Round(intensity, 4));
        }).ToList();
    }

    private static int FindDataChunkOffset(byte[] wav, out int dataLength, out int sampleRate)
    {
        sampleRate = BitConverter.ToInt32(wav, 24);
        var offset = 12;
        while (offset + 8 <= wav.Length)
        {
            var chunkId = System.Text.Encoding.ASCII.GetString(wav, offset, 4);
            var chunkSize = BitConverter.ToInt32(wav, offset + 4);
            if (chunkId == "data")
            {
                dataLength = Math.Min(chunkSize, wav.Length - (offset + 8));
                return offset + 8;
            }
            offset += 8 + chunkSize + (chunkSize % 2);
        }
        throw new InvalidDataException("WAV file has no data chunk.");
    }
}
