namespace Argon.Engine;

public sealed record VisualWindowMetrics(
    int WindowIndex,
    double TimestampSeconds,
    string FramePath,
    double MeanLuminance,
    double LuminanceContrastStd,
    double MeanSaturation,
    double WarmthIndex,
    double ChromaticEntropyBits,
    double JensenShannonDivergenceFromPrev,
    bool IsSceneCutDetected,
    double VisualSalienceScore);

public static class VisualDynamicsAnalyzer
{
    public static List<VisualWindowMetrics> AnalyzeRasters(
        string profileDirectory,
        IReadOnlyList<(int Index, double Timestamp, string FrameImage, string RasterPath)> frames)
    {
        var results = new List<VisualWindowMetrics>();
        double[]? prevDistribution = null;

        foreach (var f in frames)
        {
            var fullRasterPath = Path.Combine(profileDirectory, f.RasterPath);
            var rgb = File.ReadAllBytes(fullRasterPath);
            if (rgb.Length != 64 * 64 * 3)
                throw new InvalidDataException($"Expected 12288 bytes in {fullRasterPath}, got {rgb.Length}");

            const int pixelCount = 4096;
            var distribution = new double[512];
            double sumLum = 0;
            double sumLumSq = 0;
            double sumSat = 0;
            double sumWarmth = 0;

            for (var i = 0; i < rgb.Length; i += 3)
            {
                var r = rgb[i] / 255.0;
                var g = rgb[i + 1] / 255.0;
                var b = rgb[i + 2] / 255.0;

                // ITU-R BT.709 perceptual luminance
                var y = 0.2126 * r + 0.7152 * g + 0.0722 * b;
                sumLum += y;
                sumLumSq += y * y;

                var max = Math.Max(r, Math.Max(g, b));
                var min = Math.Min(r, Math.Min(g, b));
                var sat = max > 1e-5 ? (max - min) / max : 0.0;
                sumSat += sat;

                // Red-Blue warmth balance in [-1, 1]
                sumWarmth += (r - b);

                var bin = ((rgb[i] / 32) << 6) | ((rgb[i + 1] / 32) << 3) | (rgb[i + 2] / 32);
                distribution[bin] += 1.0 / pixelCount;
            }

            var meanLum = sumLum / pixelCount;
            var varLum = Math.Max(0.0, (sumLumSq / pixelCount) - (meanLum * meanLum));
            var stdLum = Math.Sqrt(varLum);
            var meanSat = sumSat / pixelCount;
            var meanWarmth = sumWarmth / pixelCount;

            // Shannon chromatic entropy in bits (max = log2(512) = 9 bits)
            double entropyBits = 0.0;
            for (var b = 0; b < 512; b++)
            {
                var p = distribution[b];
                if (p > 1e-12)
                {
                    entropyBits -= p * Math.Log2(p);
                }
            }

            // Jensen-Shannon Divergence in bits [0, 1] between consecutive frames
            double jsd = 0.0;
            if (prevDistribution != null)
            {
                jsd = ComputeJensenShannonDivergence(prevDistribution, distribution);
            }
            prevDistribution = distribution;

            // Scene cut or major camera shift threshold: JSD >= 0.30 bits
            var isCut = f.Index == 0 || jsd >= 0.30;

            // Visual salience combines chromatic richness, luminance contrast, and visual flux
            var normEntropy = Math.Clamp(entropyBits / 6.5, 0.0, 1.0);
            var normContrast = Math.Clamp(stdLum / 0.32, 0.0, 1.0);
            var salience = Math.Clamp(0.40 * jsd + 0.35 * normContrast + 0.25 * normEntropy, 0.0, 1.0);

            results.Add(new VisualWindowMetrics(
                f.Index,
                Math.Round(f.Timestamp, 3),
                f.FrameImage,
                Math.Round(meanLum, 4),
                Math.Round(stdLum, 4),
                Math.Round(meanSat, 4),
                Math.Round(meanWarmth, 4),
                Math.Round(entropyBits, 4),
                Math.Round(jsd, 4),
                isCut,
                Math.Round(salience, 4)));
        }

        return results;
    }

    public static double ComputeJensenShannonDivergence(double[] p, double[] q)
    {
        double klPm = 0.0;
        double klQm = 0.0;
        for (var i = 0; i < p.Length; i++)
        {
            var m = 0.5 * (p[i] + q[i]);
            if (m <= 1e-15) continue;
            if (p[i] > 1e-15) klPm += p[i] * Math.Log2(p[i] / m);
            if (q[i] > 1e-15) klQm += q[i] * Math.Log2(q[i] / m);
        }
        return Math.Clamp(0.5 * (klPm + klQm), 0.0, 1.0);
    }
}
