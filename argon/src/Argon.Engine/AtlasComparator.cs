using System.Globalization;
using System.Text;
using System.Text.Json;

namespace Argon.Engine;

public sealed record CorpusWorkAtlasEntry(
    string WorkId,
    string Title,
    string NarrativeDomain,
    string Language,
    string License,
    double DurationSeconds,
    int WindowCount,
    GlobalNarrativeIndices GlobalIndices,
    double FinalPolarization,
    double ConditionNumberKappa,
    Dictionary<string, double> FinalStateX,
    List<PhaseSpacePoint> Orbit);

public sealed record PhaseSpacePoint(
    int WindowIndex,
    double NormalizedTime,
    double Energy,
    double EntropyBits,
    double NormalizedEntropy,
    double Inertia,
    double Complexity,
    double VisualJsdBits,
    double Polarization);

public sealed record PairwiseWorkDistance(
    string WorkA,
    string WorkB,
    double MetricSpaceEuclideanDistance,
    double StateTrajectoryEndDistance,
    double RegimeContrastScore);

public sealed record CrossDomainAtlasReport(
    string SchemaVersion,
    DateTimeOffset CreatedUtc,
    string Author,
    string Orcid,
    string Doi,
    List<CorpusWorkAtlasEntry> Works,
    List<PairwiseWorkDistance> PairwiseDistances);

public static class AtlasComparator
{
    public static CrossDomainAtlasReport BuildAtlas(
        IReadOnlyList<(string WorkId, string Title, string Domain, string Language, string License, NarrativeDynamicsProfile Profile)> entries)
    {
        var works = entries.Select(e =>
        {
            var p = e.Profile;
            var orbit = p.Timeline.Select((pt, i) => new PhaseSpacePoint(
                pt.WindowIndex,
                Math.Round((double)i / Math.Max(1, p.Timeline.Count - 1), 4),
                pt.NarrativeEnergy,
                pt.CumulativeStanceEntropyBits,
                pt.NormalizedNarrativeEntropy,
                pt.NarrativeInertia,
                pt.NarrativeComplexity,
                pt.Visual.JensenShannonDivergenceFromPrev,
                pt.PolarizationIndex)).ToList();

            return new CorpusWorkAtlasEntry(
                e.WorkId,
                e.Title,
                e.Domain,
                e.Language,
                e.License,
                p.DurationSeconds,
                p.WindowCount,
                p.GlobalIndices,
                p.Timeline.Last().PolarizationIndex,
                p.ControlAnalysis.ConditionNumberKappa,
                p.Timeline.Last().CollectiveStateX,
                orbit);
        }).ToList();

        var distances = new List<PairwiseWorkDistance>();
        for (var i = 0; i < works.Count; i++)
        {
            for (var j = i + 1; j < works.Count; j++)
            {
                var a = works[i];
                var b = works[j];

                // Normalized feature vector in [0, 1]^5: (MeanE/100, PeakE/100, NormEntropy, MeanInertia, FinalPolarization)
                var dE = (a.GlobalIndices.MeanNarrativeEnergy - b.GlobalIndices.MeanNarrativeEnergy) / 100.0;
                var dPeak = (a.GlobalIndices.PeakNarrativeEnergy - b.GlobalIndices.PeakNarrativeEnergy) / 100.0;
                var dH = a.GlobalIndices.NormalizedNarrativeEntropy - b.GlobalIndices.NormalizedNarrativeEntropy;
                var dI = a.GlobalIndices.MeanNarrativeInertia - b.GlobalIndices.MeanNarrativeInertia;
                var dP = a.FinalPolarization - b.FinalPolarization;

                var metricDist = Math.Sqrt(dE * dE + dPeak * dPeak + dH * dH + dI * dI + dP * dP);

                var stateKeys = a.FinalStateX.Keys.Intersect(b.FinalStateX.Keys).ToList();
                var stateDist = Math.Sqrt(stateKeys.Sum(k => Math.Pow(a.FinalStateX[k] - b.FinalStateX[k], 2)));

                var contrast = Math.Round(0.55 * metricDist + 0.45 * (stateDist / 2.0), 4);
                distances.Add(new PairwiseWorkDistance(
                    a.WorkId,
                    b.WorkId,
                    Math.Round(metricDist, 4),
                    Math.Round(stateDist, 4),
                    contrast));
            }
        }

        return new CrossDomainAtlasReport(
            "argon-atlas-0.2",
            DateTimeOffset.UtcNow,
            "Jair Ivan Rojas Ruiz",
            "https://orcid.org/0009-0003-1109-4583",
            "https://doi.org/10.5281/zenodo.23987656",
            works,
            distances);
    }

    public static string RenderAtlasHtml(CrossDomainAtlasReport atlas)
    {
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
        var payloadJson = JsonSerializer.Serialize(atlas, jsonOptions);

        var sb = new StringBuilder();
        sb.AppendLine("<!doctype html>");
        sb.AppendLine("<html lang=\"en\">");
        sb.AppendLine("<head>");
        sb.AppendLine("<meta charset=\"utf-8\">");
        sb.AppendLine("<meta name=\"viewport\" content=\"width=device-width,initial-scale=1\">");
        sb.AppendLine("<title>NSE Phase-Space Atlas — Multi-Work Comparative Telemetry (EXP-002)</title>");
        sb.AppendLine("<style>");
        sb.AppendLine(@"
:root {
  --bg: #070b11;
  --panel: #111927;
  --panel-alt: #172235;
  --border: #253652;
  --text: #e8eef7;
  --muted: #94a9c4;
  --c1: #f59e0b; /* CORPUS-001 Insumision */
  --c2: #ef4444; /* CORPUS-002 Daisy */
  --c3: #38bdf8; /* CORPUS-003 Challenger */
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
  background: linear-gradient(135deg, #0b1325 0%, #1d2b53 100%);
  border-bottom: 1px solid var(--border);
  padding: 28px 38px;
}
header h1 { margin: 6px 0 8px; font-size: 27px; letter-spacing: -0.02em; }
header p { margin: 0; color: var(--muted); font-size: 14px; max-width: 1020px; }
.badge {
  display: inline-block;
  padding: 3px 10px;
  border-radius: 999px;
  font-size: 11px;
  font-weight: 700;
  background: rgba(56, 189, 248, 0.16);
  color: #7dd3fc;
  border: 1px solid rgba(56, 189, 248, 0.35);
  margin-right: 8px;
}
main { max-width: 1440px; margin: 0 auto; padding: 28px 38px 64px; }
.grid-2 { display: grid; grid-template-columns: 1.2fr 1fr; gap: 22px; margin-bottom: 24px; }
@media (max-width: 1060px) { .grid-2 { grid-template-columns: 1fr; } }
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
  background: #0b111b;
  border: 1px solid #1e2e46;
  border-radius: 10px;
  display: block;
}
.legend {
  display: flex;
  flex-wrap: wrap;
  gap: 18px;
  margin-top: 12px;
  font-size: 12px;
  color: var(--muted);
}
.legend span { display: inline-flex; align-items: center; gap: 7px; font-weight: 600; }
.dot { width: 11px; height: 11px; border-radius: 50%; display: inline-block; }
table { width: 100%; border-collapse: collapse; font-size: 13px; }
th, td { text-align: left; padding: 11px 12px; border-bottom: 1px solid var(--border); }
th { color: var(--muted); font-size: 11px; text-transform: uppercase; letter-spacing: 0.05em; }
a { color: #38bdf8; text-decoration: none; }
a:hover { text-decoration: underline; }
");
        sb.AppendLine("</style>");
        sb.AppendLine("</head>");
        sb.AppendLine("<body>");
        sb.AppendLine("<header>");
        sb.AppendLine("  <div><span class=\"badge\">ARGON ATLAS v0.2</span><span class=\"badge\">AUTHOR: JAIR IVAN ROJAS RUIZ (ORCID: 0009-0003-1109-4583)</span><span class=\"badge\">DOI: 10.5281/zenodo.23987656</span></div>");
        sb.AppendLine("  <h1>Narrative Systems Engineering — Cross-Domain Phase-Space Atlas (EXP-001 &amp; EXP-002)</h1>");
        sb.AppendLine("  <p>Comparative multimodal telemetry across three distinct narrative regimes: <strong>Polyphonic Debate</strong> (Spain, 1994), <strong>Acute Fear-Appeal Campaign</strong> (LBJ 'Daisy', 1964), and <strong>Institutional Crisis Consolation</strong> (Reagan 'Challenger', 1986).</p>");
        sb.AppendLine("</header>");
        sb.AppendLine("<main>");

        sb.AppendLine("<div class=\"grid-2\">");
        sb.AppendLine("  <section>");
        sb.AppendLine("    <h2>1. Narrative Phase-Space Orbit: Inertia I(t) vs. Energy E(t) &amp; Entropy H(t)</h2>");
        sb.AppendLine("    <p class=\"desc\">Each work traces a dynamic trajectory in the plane (X = Institutional Inertia I(t) [0-1], Y = Narrative Energy E(t) [0-100]). Circle radius reflects Shannon Stance Entropy H(t).</p>");
        sb.AppendLine("    <canvas id=\"phaseCanvas\" width=\"740\" height=\"410\"></canvas>");
        sb.AppendLine("    <div class=\"legend\">");
        sb.AppendLine("      <span><i class=\"dot\" style=\"background:#f59e0b\"></i> CORPUS-001: Insumisión Debate (ES, 1994)</span>");
        sb.AppendLine("      <span><i class=\"dot\" style=\"background:#ef4444\"></i> CORPUS-002: LBJ 'Daisy' Nuclear Spot (EN, 1964)</span>");
        sb.AppendLine("      <span><i class=\"dot\" style=\"background:#38bdf8\"></i> CORPUS-003: Reagan Challenger Address (EN, 1986)</span>");
        sb.AppendLine("    </div>");
        sb.AppendLine("  </section>");

        sb.AppendLine("  <section>");
        sb.AppendLine("    <h2>2. 6-Axis NSE Regime Fingerprint (Radar Comparison)</h2>");
        sb.AppendLine("    <p class=\"desc\">Normalized comparison across Mean Energy, Peak Energy, Stance Entropy, Institutional Inertia, Complexity, and Social Polarization.</p>");
        sb.AppendLine("    <canvas id=\"radarCanvas\" width=\"560\" height=\"410\"></canvas>");
        sb.AppendLine("  </section>");
        sb.AppendLine("</div>");

        sb.AppendLine("<section>");
        sb.AppendLine("  <h2>3. Normalized Temporal Evolution E(t/T) &amp; Visual Cut Divergence D_JS(t/T)</h2>");
        sb.AppendLine("  <p class=\"desc\">All three works aligned on normalized duration tau = t / T in [0, 1]. Notice the explosive mid-work nuclear spike in CORPUS-002 (tau = 0.50) vs. sustained debate energy in CORPUS-001 and steady consolation in CORPUS-003.</p>");
        sb.AppendLine("  <canvas id=\"timeCanvas\" width=\"1340\" height=\"290\"></canvas>");
        sb.AppendLine("</section>");

        sb.AppendLine("<section>");
        sb.AppendLine("  <h2>4. Cross-Domain Benchmark Summary Table</h2>");
        sb.AppendLine("  <table>");
        sb.AppendLine("    <thead><tr><th>Work ID</th><th>Title &amp; License</th><th>Regime</th><th>Duration</th><th>Mean E</th><th>Peak E</th><th>Entropy H(T)</th><th>Mean Inertia</th><th>Scene Cuts</th><th>Polarization P(T)</th><th>Kappa(W_c)</th></tr></thead>");
        sb.AppendLine("    <tbody>");
        foreach (var w in atlas.Works)
        {
            sb.AppendLine(CultureInfo.InvariantCulture, $"      <tr><td><strong>{w.WorkId}</strong></td><td>{System.Net.WebUtility.HtmlEncode(w.Title)}<br><small style=\"color:var(--muted)\">{System.Net.WebUtility.HtmlEncode(w.License)}</small></td><td><code>{w.NarrativeDomain}</code></td><td>{w.DurationSeconds:F1}s ({w.WindowCount}w)</td><td><strong>{w.GlobalIndices.MeanNarrativeEnergy:F2}</strong></td><td><strong>{w.GlobalIndices.PeakNarrativeEnergy:F2}</strong></td><td>{w.GlobalIndices.FinalNarrativeEntropyBits:F3} bits ({w.GlobalIndices.NormalizedNarrativeEntropy:P1})</td><td>{w.GlobalIndices.MeanNarrativeInertia:F3}</td><td>{w.GlobalIndices.DetectedSceneCutsCount}</td><td>{w.FinalPolarization:F3}</td><td>{w.ConditionNumberKappa:F3}</td></tr>");
        }
        sb.AppendLine("    </tbody>");
        sb.AppendLine("  </table>");
        sb.AppendLine("</section>");

        sb.AppendLine("<section>");
        sb.AppendLine("  <h2>5. Pairwise Structural Distance Matrix in NSE Metric Space</h2>");
        sb.AppendLine("  <table>");
        sb.AppendLine("    <thead><tr><th>Work A</th><th>Work B</th><th>5D Metric Euclidean Distance</th><th>Terminal State ||x_A(T) - x_B(T)||_2</th><th>Composite Regime Contrast Score</th></tr></thead>");
        sb.AppendLine("    <tbody>");
        foreach (var d in atlas.PairwiseDistances)
        {
            sb.AppendLine(CultureInfo.InvariantCulture, $"      <tr><td><strong>{d.WorkA}</strong></td><td><strong>{d.WorkB}</strong></td><td>{d.MetricSpaceEuclideanDistance:F4}</td><td>{d.StateTrajectoryEndDistance:F4}</td><td><strong>{d.RegimeContrastScore:F4}</strong></td></tr>");
        }
        sb.AppendLine("    </tbody>");
        sb.AppendLine("  </table>");
        sb.AppendLine("</section>");

        sb.AppendLine("</main>");
        sb.AppendLine("<script>");
        sb.Append("const ATLAS = ");
        sb.Append(payloadJson);
        sb.AppendLine(";");
        sb.AppendLine(@"
const colors = ['#f59e0b', '#ef4444', '#38bdf8'];

function drawPhaseSpace() {
  const canvas = document.getElementById('phaseCanvas');
  const ctx = canvas.getContext('2d');
  const W = canvas.width, H = canvas.height;
  ctx.clearRect(0, 0, W, H);
  const padL = 54, padR = 24, padT = 24, padB = 42;
  const pW = W - padL - padR, pH = H - padT - padB;

  // Grid
  ctx.strokeStyle = '#1e293b';
  for (let x = 0; x <= 1.0; x += 0.2) {
    const px = padL + x * pW;
    ctx.beginPath(); ctx.moveTo(px, padT); ctx.lineTo(px, H - padB); ctx.stroke();
    ctx.fillStyle = '#64748b'; ctx.font = '11px system-ui';
    ctx.fillText(x.toFixed(1), px - 8, H - 14);
  }
  for (let y = 0; y <= 100; y += 20) {
    const py = padT + pH * (1 - y / 100);
    ctx.beginPath(); ctx.moveTo(padL, py); ctx.lineTo(W - padR, py); ctx.stroke();
    ctx.fillStyle = '#64748b'; ctx.font = '11px system-ui';
    ctx.fillText(y.toString(), 18, py + 4);
  }
  ctx.fillStyle = '#94a3b8'; ctx.font = 'bold 12px system-ui';
  ctx.fillText('Narrative Inertia I(t) [0 - 1]', W / 2 - 70, H - 4);

  ATLAS.works.forEach((w, wi) => {
    const col = colors[wi % colors.length];
    ctx.strokeStyle = col;
    ctx.lineWidth = 2.2;
    ctx.beginPath();
    w.orbit.forEach((pt, i) => {
      const px = padL + pt.inertia * pW;
      const py = padT + pH * (1 - pt.energy / 100);
      if (i === 0) ctx.moveTo(px, py); else ctx.lineTo(px, py);
    });
    ctx.stroke();

    w.orbit.forEach((pt, i) => {
      const px = padL + pt.inertia * pW;
      const py = padT + pH * (1 - pt.energy / 100);
      ctx.fillStyle = col;
      ctx.globalAlpha = i === w.orbit.length - 1 ? 1.0 : 0.65;
      ctx.beginPath();
      ctx.arc(px, py, i === w.orbit.length - 1 ? 7 : Math.max(3, pt.normalizedEntropy * 5.5), 0, Math.PI * 2);
      ctx.fill();
    });
    ctx.globalAlpha = 1.0;
  });
}

function drawRadar() {
  const canvas = document.getElementById('radarCanvas');
  const ctx = canvas.getContext('2d');
  const W = canvas.width, H = canvas.height;
  ctx.clearRect(0, 0, W, H);
  const cx = W / 2, cy = H / 2 + 6, R = Math.min(W, H) * 0.36;
  const labels = ['Mean Energy', 'Peak Energy', 'Stance Entropy', 'Mean Inertia', 'Complexity', 'Polarization'];

  // Webs
  for (let ring = 0.25; ring <= 1.0; ring += 0.25) {
    ctx.strokeStyle = '#1e293b';
    ctx.beginPath();
    labels.forEach((_, i) => {
      const ang = -Math.PI / 2 + (i * 2 * Math.PI) / labels.length;
      const x = cx + Math.cos(ang) * R * ring;
      const y = cy + Math.sin(ang) * R * ring;
      if (i === 0) ctx.moveTo(x, y); else ctx.lineTo(x, y);
    });
    ctx.closePath(); ctx.stroke();
  }

  labels.forEach((lab, i) => {
    const ang = -Math.PI / 2 + (i * 2 * Math.PI) / labels.length;
    const x = cx + Math.cos(ang) * R;
    const y = cy + Math.sin(ang) * R;
    ctx.strokeStyle = '#334155';
    ctx.beginPath(); ctx.moveTo(cx, cy); ctx.lineTo(x, y); ctx.stroke();
    const lx = cx + Math.cos(ang) * (R + 28);
    const ly = cy + Math.sin(ang) * (R + 18);
    ctx.fillStyle = '#cbd5e1'; ctx.font = '11px system-ui';
    ctx.textAlign = 'center';
    ctx.fillText(lab, lx, ly);
  });

  ATLAS.works.forEach((w, wi) => {
    const vals = [
      w.globalIndices.meanNarrativeEnergy / 100,
      w.globalIndices.peakNarrativeEnergy / 100,
      w.globalIndices.normalizedNarrativeEntropy,
      w.globalIndices.meanNarrativeInertia,
      w.globalIndices.meanNarrativeComplexity / 100,
      w.finalPolarization
    ];
    const col = colors[wi % colors.length];
    ctx.strokeStyle = col;
    ctx.fillStyle = col;
    ctx.lineWidth = 2.4;
    ctx.beginPath();
    vals.forEach((v, i) => {
      const ang = -Math.PI / 2 + (i * 2 * Math.PI) / labels.length;
      const x = cx + Math.cos(ang) * R * Math.max(0.05, Math.min(1, v));
      const y = cy + Math.sin(ang) * R * Math.max(0.05, Math.min(1, v));
      if (i === 0) ctx.moveTo(x, y); else ctx.lineTo(x, y);
    });
    ctx.closePath();
    ctx.globalAlpha = 0.16; ctx.fill();
    ctx.globalAlpha = 1.0; ctx.stroke();
  });
}

function drawNormalizedTimeline() {
  const canvas = document.getElementById('timeCanvas');
  const ctx = canvas.getContext('2d');
  const W = canvas.width, H = canvas.height;
  ctx.clearRect(0, 0, W, H);
  const padL = 48, padR = 20, padT = 20, padB = 34;
  const pW = W - padL - padR, pH = H - padT - padB;

  for (let v = 0; v <= 100; v += 25) {
    const y = padT + pH * (1 - v / 100);
    ctx.strokeStyle = '#1e293b';
    ctx.beginPath(); ctx.moveTo(padL, y); ctx.lineTo(W - padR, y); ctx.stroke();
    ctx.fillStyle = '#64748b'; ctx.font = '11px system-ui';
    ctx.fillText(v.toString(), 14, y + 4);
  }

  ATLAS.works.forEach((w, wi) => {
    const col = colors[wi % colors.length];
    ctx.strokeStyle = col;
    ctx.lineWidth = 2.6;
    ctx.beginPath();
    w.orbit.forEach((pt, i) => {
      const x = padL + pt.normalizedTime * pW;
      const y = padT + pH * (1 - pt.energy / 100);
      if (i === 0) ctx.moveTo(x, y); else ctx.lineTo(x, y);
    });
    ctx.stroke();
  });
}

drawPhaseSpace();
drawRadar();
drawNormalizedTimeline();
");
        sb.AppendLine("</script>");
        sb.AppendLine("</body></html>");
        return sb.ToString();
    }
}
