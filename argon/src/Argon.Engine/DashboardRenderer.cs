using System.Globalization;
using System.Text;
using System.Text.Json;

namespace Argon.Engine;

public static class DashboardRenderer
{
    public static string RenderHtml(
        NarrativeDynamicsProfile profile,
        TranscriptDocument transcript,
        string profileDirectory)
    {
        // Embed representative keyframe images (at scene cuts / speaker transitions) as base64 for portable viewing
        var keyFrames = new Dictionary<int, string>();
        var representativeIndices = new[] { 0, 1, 5, 9, 18, 22, 26, 34, 38 };
        foreach (var idx in representativeIndices)
        {
            if (idx >= 0 && idx < profile.Timeline.Count)
            {
                var relPath = profile.Timeline[idx].Visual.FramePath;
                var fullPath = Path.Combine(profileDirectory, relPath);
                if (File.Exists(fullPath))
                {
                    keyFrames[idx] = "data:image/jpeg;base64," + Convert.ToBase64String(File.ReadAllBytes(fullPath));
                }
            }
        }

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        var payloadJson = JsonSerializer.Serialize(new
        {
            profile,
            transcript,
            keyFrames
        }, jsonOptions);

        var sb = new StringBuilder();
        sb.AppendLine("<!doctype html>");
        sb.AppendLine("<html lang=\"en\">");
        sb.AppendLine("<head>");
        sb.AppendLine("<meta charset=\"utf-8\">");
        sb.AppendLine("<meta name=\"viewport\" content=\"width=device-width,initial-scale=1\">");
        sb.AppendLine("<title>Argon Narrative Dynamics & Control Telemetry — EXP-001</title>");
        sb.AppendLine("<style>");
        sb.AppendLine(@"
:root {
  --bg: #0b1017;
  --panel: #131c28;
  --panel-alt: #182434;
  --border: #263850;
  --text: #e6edf5;
  --muted: #93a8c1;
  --accent: #38bdf8;
  --energy: #f59e0b;
  --entropy: #a855f7;
  --inertia: #10b981;
  --danger: #ef4444;
}
* { box-sizing: border-box; }
body {
  margin: 0;
  font-family: system-ui, -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
  background: var(--bg);
  color: var(--text);
  line-height: 1.5;
}
header {
  background: linear-gradient(135deg, #0f172a 0%, #172554 100%);
  border-bottom: 1px solid var(--border);
  padding: 28px 36px;
}
header h1 { margin: 0 0 8px 0; font-size: 26px; letter-spacing: -0.02em; }
header p { margin: 0; color: var(--muted); font-size: 14px; max-width: 960px; }
.badge {
  display: inline-block;
  padding: 3px 10px;
  border-radius: 999px;
  font-size: 12px;
  font-weight: 600;
  background: rgba(56, 189, 248, 0.15);
  color: var(--accent);
  border: 1px solid rgba(56, 189, 248, 0.35);
  margin-right: 8px;
}
main { max-width: 1420px; margin: 0 auto; padding: 28px 36px 64px; }
.kpi-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 16px;
  margin-bottom: 28px;
}
.kpi {
  background: var(--panel);
  border: 1px solid var(--border);
  border-radius: 12px;
  padding: 16px 18px;
}
.kpi .label { font-size: 12px; text-transform: uppercase; letter-spacing: 0.06em; color: var(--muted); }
.kpi .value { font-size: 28px; font-weight: 700; margin: 6px 0 4px; }
.kpi .sub { font-size: 12px; color: var(--muted); }
section {
  background: var(--panel);
  border: 1px solid var(--border);
  border-radius: 14px;
  padding: 22px 24px;
  margin-bottom: 24px;
}
section h2 { margin: 0 0 6px 0; font-size: 18px; color: #fff; }
section p.desc { margin: 0 0 16px 0; font-size: 13px; color: var(--muted); }
.two-col {
  display: grid;
  grid-template-columns: 1.35fr 1fr;
  gap: 24px;
}
@media (max-width: 1024px) { .two-col { grid-template-columns: 1fr; } }
canvas {
  width: 100%;
  background: #0d1520;
  border: 1px solid #1e2d42;
  border-radius: 10px;
  display: block;
}
.legend {
  display: flex;
  flex-wrap: wrap;
  gap: 16px;
  margin-top: 10px;
  font-size: 12px;
  color: var(--muted);
}
.legend span { display: inline-flex; align-items: center; gap: 6px; }
.dot { width: 10px; height: 10px; border-radius: 50%; display: inline-block; }
table {
  width: 100%;
  border-collapse: collapse;
  font-size: 13px;
}
th, td {
  text-align: left;
  padding: 10px 12px;
  border-bottom: 1px solid var(--border);
}
th { color: var(--muted); font-weight: 600; font-size: 12px; text-transform: uppercase; }
.pill {
  padding: 2px 8px;
  border-radius: 6px;
  font-weight: 600;
  font-size: 12px;
}
.frames-strip {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
  gap: 14px;
}
.frame-card {
  background: var(--panel-alt);
  border: 1px solid var(--border);
  border-radius: 10px;
  overflow: hidden;
}
.frame-card img { width: 100%; height: 145px; object-fit: cover; display: block; }
.frame-card .meta { padding: 12px; font-size: 12px; }
.frame-card .meta strong { color: #fff; display: block; margin-bottom: 4px; }
.sim-controls {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(210px, 1fr));
  gap: 14px;
  background: var(--panel-alt);
  padding: 16px;
  border-radius: 10px;
  margin-bottom: 16px;
}
.sim-controls label { display: block; font-size: 12px; color: var(--muted); margin-bottom: 4px; }
.sim-controls input[type=range] { width: 100%; }
");
        sb.AppendLine("</style>");
        sb.AppendLine("</head>");
        sb.AppendLine("<body>");
        sb.AppendLine("<header>");
        sb.AppendLine("  <div><span class=\"badge\">ARGON LAB EXP-001</span><span class=\"badge\">AUTHOR: JAIR IVAN ROJAS RUIZ (ORCID: 0009-0003-1109-4583)</span><span class=\"badge\">REAL SIGNAL GROUNDED</span></div>");
        sb.AppendLine("  <h1>Narrative Systems Engineering — Multimodal Signal u(t) &amp; Cognitive State Trajectory x(t)</h1>");
        sb.AppendLine($"  <p>Originated &amp; Directed by <strong>Jair Ivan Rojas Ruiz</strong> (<a href=\"https://orcid.org/0009-0003-1109-4583\" style=\"color:#38bdf8\">0009-0003-1109-4583</a>) · Source: <code>{profile.SourceVideo}</code> ({profile.DurationSeconds:F2}s, SHA-256: <code>{profile.SourceSha256[..16]}...</code>) · 39 windows (5.0s).</p>");
        sb.AppendLine("</header>");
        sb.AppendLine("<main>");

        // KPI Row
        sb.AppendLine("<div class=\"kpi-grid\">");
        sb.AppendLine($"  <div class=\"kpi\"><div class=\"label\">Mean / Peak Narrative Energy</div><div class=\"value\" style=\"color:var(--energy)\">{profile.GlobalIndices.MeanNarrativeEnergy:F1} <span style=\"font-size:16px;color:var(--muted)\">/ {profile.GlobalIndices.PeakNarrativeEnergy:F1}</span></div><div class=\"sub\">Peak at window #{profile.GlobalIndices.PeakEnergyWindowIndex} ({profile.Timeline[profile.GlobalIndices.PeakEnergyWindowIndex].MidpointSeconds:F1}s)</div></div>");
        sb.AppendLine($"  <div class=\"kpi\"><div class=\"label\">Narrative Stance Entropy H(T)</div><div class=\"value\" style=\"color:var(--entropy)\">{profile.GlobalIndices.FinalNarrativeEntropyBits:F3} bits</div><div class=\"sub\">Normalized: {profile.GlobalIndices.NormalizedNarrativeEntropy:P1} of max 3.00 bits</div></div>");
        sb.AppendLine($"  <div class=\"kpi\"><div class=\"label\">Mean Narrative Inertia</div><div class=\"value\" style=\"color:var(--inertia)\">{profile.GlobalIndices.MeanNarrativeInertia:F3}</div><div class=\"sub\">Institutional &amp; legal resistance [0-1]</div></div>");
        sb.AppendLine($"  <div class=\"kpi\"><div class=\"label\">Mean Narrative Complexity</div><div class=\"value\" style=\"color:var(--accent)\">{profile.GlobalIndices.MeanNarrativeComplexity:F1} <span style=\"font-size:16px;color:var(--muted)\">/ 100</span></div><div class=\"sub\">Lexical TTR + dialectical connectors</div></div>");
        sb.AppendLine($"  <div class=\"kpi\"><div class=\"label\">Visual Scene Cuts (JSD &ge; 0.30)</div><div class=\"value\">{profile.GlobalIndices.DetectedSceneCutsCount}</div><div class=\"sub\">Deterministic Jensen-Shannon transitions</div></div>");
        sb.AppendLine($"  <div class=\"kpi\"><div class=\"label\">Control Condition &kappa;(W_c)</div><div class=\"value\">{profile.ControlAnalysis.ConditionNumberKappa:F2}</div><div class=\"sub\">Most controllable: {profile.ControlAnalysis.MostControllableDimension}</div></div>");
        sb.AppendLine("</div>");

        // Section 1: Multimodal Signal Telemetry
        sb.AppendLine("<section>");
        sb.AppendLine("  <h2>1. Multimodal Signal Telemetry u(t) &amp; Coupled Metrics (t = 0s to 194.8s)</h2>");
        sb.AppendLine("  <p class=\"desc\">Top chart: Narrative Energy E(t), Complexity C(t), and Acoustic Intensity I_ac(t). Bottom chart: Visual Shot Transition Divergence D_JS(t), Stance Entropy H(t), and Narrative Inertia I(t).</p>");
        sb.AppendLine("  <canvas id=\"signalCanvas\" width=\"1320\" height=\"320\"></canvas>");
        sb.AppendLine("  <div class=\"legend\">");
        sb.AppendLine("    <span><i class=\"dot\" style=\"background:#f59e0b\"></i> Narrative Energy E(t) [0-100]</span>");
        sb.AppendLine("    <span><i class=\"dot\" style=\"background:#38bdf8\"></i> Narrative Complexity C(t) [0-100]</span>");
        sb.AppendLine("    <span><i class=\"dot\" style=\"background:#ec4899\"></i> Visual Cut Divergence D_JS(t) [&times;100]</span>");
        sb.AppendLine("    <span><i class=\"dot\" style=\"background:#a855f7\"></i> Normalized Stance Entropy H(t) [&times;100]</span>");
        sb.AppendLine("    <span><i class=\"dot\" style=\"background:#10b981\"></i> Narrative Inertia I(t) [&times;100]</span>");
        sb.AppendLine("  </div>");
        sb.AppendLine("</section>");

        // Section 2: Control State Space & Interactive Simulator
        sb.AppendLine("<div class=\"two-col\">");
        sb.AppendLine("  <section>");
        sb.AppendLine("    <h2>2. State-Space Control Trajectory: x(t+1) = (I - &Gamma;(t))x(t) + &Gamma;(t)B u(t)</h2>");
        sb.AppendLine("    <p class=\"desc\">Evolution of the 4 collective belief states across the documentary's 5 interview turns. Adjustspeaker gains below to run live counterfactual control simulations.</p>");
        sb.AppendLine("    <div class=\"sim-controls\">");
        sb.AppendLine("      <div><label>Abolitionist Gain (SPK_01 + Poster): <strong id=\"lblBoost1\">1.0x</strong></label><input type=\"range\" id=\"rngBoost1\" min=\"0\" max=\"3\" step=\"0.1\" value=\"1.0\"></div>");
        sb.AppendLine("      <div><label>Legal/Civic Gain (SPK_02..05): <strong id=\"lblBoost2\">1.0x</strong></label><input type=\"range\" id=\"rngBoost2\" min=\"0\" max=\"3\" step=\"0.1\" value=\"1.0\"></div>");
        sb.AppendLine("      <div><label>Institutional Inertia Damping: <strong id=\"lblInertia\">0.65</strong></label><input type=\"range\" id=\"rngInertia\" min=\"0\" max=\"0.95\" step=\"0.05\" value=\"0.65\"></div>");
        sb.AppendLine("    </div>");
        sb.AppendLine("    <canvas id=\"stateCanvas\" width=\"760\" height=\"300\"></canvas>");
        sb.AppendLine("    <div class=\"legend\">");
        sb.AppendLine("      <span><i class=\"dot\" style=\"background:#ef4444\"></i> x1: Anti-Militarist Abolitionism</span>");
        sb.AppendLine("      <span><i class=\"dot\" style=\"background:#3b82f6\"></i> x2: Civic &amp; Legal Compliance</span>");
        sb.AppendLine("      <span><i class=\"dot\" style=\"background:#10b981\"></i> x3: Pragmatic Social Service</span>");
        sb.AppendLine("      <span><i class=\"dot\" style=\"background:#eab308\"></i> x4: Individual Conscience Autonomy</span>");
        sb.AppendLine("    </div>");
        sb.AppendLine("  </section>");

        sb.AppendLine("  <section>");
        sb.AppendLine("    <h2>3. Narrative DNA — Signed Speaker-Belief Bipartite Graph</h2>");
        sb.AppendLine("    <p class=\"desc\">Green edges denote reinforcement (+); red dashed edges denote challenge/rejection (-). Node size reflects average Narrative Energy.</p>");
        sb.AppendLine("    <canvas id=\"dnaCanvas\" width=\"540\" height=\"395\"></canvas>");
        sb.AppendLine("  </section>");
        sb.AppendLine("</div>");

        // Section 4: Speaker Comparison Table
        sb.AppendLine("<section>");
        sb.AppendLine("  <h2>4. Speaker-Level Multimodal &amp; Cognitive Vector Breakdown (Phases 1–6)</h2>");
        sb.AppendLine("  <p class=\"desc\">Direct comparison across the 6 segments/speakers combining acoustic prosody, lexical cadence, institutional inertia, and stance vectors.</p>");
        sb.AppendLine("  <table>");
        sb.AppendLine("    <thead><tr><th>Speaker / Turn</th><th>Span</th><th>Acoustic dBFS</th><th>Speech WPM</th><th>Energy E</th><th>Complexity</th><th>Inertia</th><th>B1: Anti-Mil</th><th>B2: Legal</th><th>B3: Pragmatic</th><th>B4: Autonomy</th></tr></thead>");
        sb.AppendLine("    <tbody>");
        foreach (var s in profile.SpeakerSummaries)
        {
            sb.AppendLine(CultureInfo.InvariantCulture, $"      <tr><td><strong>{System.Net.WebUtility.HtmlEncode(s.SpeakerLabel)}</strong></td><td>{s.StartSeconds:F0}–{s.EndSeconds:F0}s</td><td>{s.MeanAcousticDbFs:F1} dB</td><td>{s.MeanWordsPerMinute:F0}</td><td><span class=\"pill\" style=\"background:rgba(245,158,11,0.2);color:#fbbf24\">{s.MeanEnergy:F1}</span></td><td>{s.MeanComplexity:F1}</td><td>{s.MeanInertia:F2}</td><td>{FormatStance(s.StanceVector["B1_ANTIMILITARISM"])}</td><td>{FormatStance(s.StanceVector["B2_LEGAL_COMPLIANCE"])}</td><td>{FormatStance(s.StanceVector["B3_PRAGMATIC_SERVICE"])}</td><td>{FormatStance(s.StanceVector["B4_INDIVIDUAL_AUTONOMY"])}</td></tr>");
        }
        sb.AppendLine("    </tbody>");
        sb.AppendLine("  </table>");
        sb.AppendLine("</section>");

        // Section 5: Phase 8 & 9 Controllability & Counterfactuals
        sb.AppendLine("<section>");
        sb.AppendLine("  <h2>5. Phase 8 &amp; 9 — Controllability Gramian &amp; Preregistered Counterfactual Scenarios</h2>");
        sb.AppendLine("  <p class=\"desc\">Comparison of final collective cognitive states under Scenario A (actual documentary order), Scenario B (reversed order testing recency vs. inertia), and Scenario C (equalized airtime).</p>");
        sb.AppendLine("  <table>");
        sb.AppendLine("    <thead><tr><th>Scenario</th><th>Description</th><th>Final x1 (Anti-Mil)</th><th>Final x2 (Legal)</th><th>Final x3 (Pragmatic)</th><th>Final x4 (Autonomy)</th><th>Net Shift ||&Delta;x||</th></tr></thead>");
        sb.AppendLine("    <tbody>");
        foreach (var sc in profile.ControlAnalysis.CounterfactualScenarios)
        {
            sb.AppendLine(CultureInfo.InvariantCulture, $"      <tr><td><strong>{System.Net.WebUtility.HtmlEncode(sc.Name)}</strong></td><td>{System.Net.WebUtility.HtmlEncode(sc.Description)}</td><td>{FormatStance(sc.FinalStateX["B1_ANTIMILITARISM"])}</td><td>{FormatStance(sc.FinalStateX["B2_LEGAL_COMPLIANCE"])}</td><td>{FormatStance(sc.FinalStateX["B3_PRAGMATIC_SERVICE"])}</td><td>{FormatStance(sc.FinalStateX["B4_INDIVIDUAL_AUTONOMY"])}</td><td><strong>{sc.NetShiftFromBaseline:F4}</strong></td></tr>");
        }
        sb.AppendLine("    </tbody>");
        sb.AppendLine("  </table>");
        sb.AppendLine("</section>");

        // Section 6: Visual Evidence Keyframes
        sb.AppendLine("<section>");
        sb.AppendLine("  <h2>6. Time-Aligned Visual &amp; Symbolic Evidence Anchors</h2>");
        sb.AppendLine("  <p class=\"desc\">Representative frames at detected shot transitions and speaker turns, linked to their exact acoustic and visual divergence telemetry.</p>");
        sb.AppendLine("  <div class=\"frames-strip\" id=\"framesStrip\"></div>");
        sb.AppendLine("</section>");

        sb.AppendLine("</main>");
        sb.AppendLine("<script>");
        sb.Append("const DATA = ");
        sb.Append(payloadJson);
        sb.AppendLine(";");
        sb.AppendLine(@"
const pts = DATA.profile.timeline;

function drawSignalCanvas() {
  const canvas = document.getElementById('signalCanvas');
  const ctx = canvas.getContext('2d');
  const W = canvas.width, H = canvas.height;
  ctx.clearRect(0, 0, W, H);

  const padL = 48, padR = 20, padT = 20, padB = 34;
  const plotW = W - padL - padR;
  const plotH = H - padT - padB;

  // Grid lines
  ctx.strokeStyle = '#1e293b';
  ctx.lineWidth = 1;
  for (let v = 0; v <= 100; v += 25) {
    const y = padT + plotH * (1 - v / 100);
    ctx.beginPath(); ctx.moveTo(padL, y); ctx.lineTo(W - padR, y); ctx.stroke();
    ctx.fillStyle = '#64748b'; ctx.font = '11px system-ui';
    ctx.fillText(v.toString(), 14, y + 4);
  }

  // Speaker vertical boundaries
  let prevSpk = pts[0].speakerId;
  pts.forEach((p, i) => {
    const x = padL + (i / (pts.length - 1)) * plotW;
    if (p.speakerId !== prevSpk || i === 0) {
      ctx.strokeStyle = 'rgba(148, 163, 184, 0.28)';
      ctx.setLineDash([4, 4]);
      ctx.beginPath(); ctx.moveTo(x, padT); ctx.lineTo(x, H - padB); ctx.stroke();
      ctx.setLineDash([]);
      ctx.fillStyle = '#94a3b8'; ctx.font = 'bold 11px system-ui';
      ctx.fillText(p.speakerId, x + 4, padT + 12);
      prevSpk = p.speakerId;
    }
    if (i % 4 === 0) {
      ctx.fillStyle = '#64748b'; ctx.font = '11px system-ui';
      ctx.fillText(p.midpointSeconds.toFixed(0) + 's', x - 10, H - 10);
    }
  });

  function plotSeries(fn, color, width = 2.2) {
    ctx.strokeStyle = color;
    ctx.lineWidth = width;
    ctx.beginPath();
    pts.forEach((p, i) => {
      const x = padL + (i / (pts.length - 1)) * plotW;
      const val = Math.max(0, Math.min(100, fn(p)));
      const y = padT + plotH * (1 - val / 100);
      if (i === 0) ctx.moveTo(x, y); else ctx.lineTo(x, y);
    });
    ctx.stroke();
  }

  plotSeries(p => p.visual.jensenShannonDivergenceFromPrev * 100, '#ec4899', 1.8);
  plotSeries(p => p.normalizedNarrativeEntropy * 100, '#a855f7', 2.0);
  plotSeries(p => p.narrativeInertia * 100, '#10b981', 2.0);
  plotSeries(p => p.narrativeComplexity, '#38bdf8', 2.0);
  plotSeries(p => p.narrativeEnergy, '#f59e0b', 2.8);
}

function simulateAndDrawState() {
  const boost1 = parseFloat(document.getElementById('rngBoost1').value);
  const boost2 = parseFloat(document.getElementById('rngBoost2').value);
  const inertiaDamp = parseFloat(document.getElementById('rngInertia').value);

  document.getElementById('lblBoost1').textContent = boost1.toFixed(1) + 'x';
  document.getElementById('lblBoost2').textContent = boost2.toFixed(1) + 'x';
  document.getElementById('lblInertia').textContent = inertiaDamp.toFixed(2);

  const keys = ['B1_ANTIMILITARISM', 'B2_LEGAL_COMPLIANCE', 'B3_PRAGMATIC_SERVICE', 'B4_INDIVIDUAL_AUTONOMY'];
  const colors = {
    B1_ANTIMILITARISM: '#ef4444',
    B2_LEGAL_COMPLIANCE: '#3b82f6',
    B3_PRAGMATIC_SERVICE: '#10b981',
    B4_INDIVIDUAL_AUTONOMY: '#eab308'
  };

  const state = { B1_ANTIMILITARISM: 0, B2_LEGAL_COMPLIANCE: 0, B3_PRAGMATIC_SERVICE: 0, B4_INDIVIDUAL_AUTONOMY: 0 };
  const history = [];

  pts.forEach(p => {
    const spkBoost = (p.speakerId === 'SPK_01' || p.speakerId === 'ENV_POSTER') ? boost1 : boost2;
    keys.forEach(k => {
      const uVal = p.linguistic.activeStanceVector[k] || 0;
      const dimIn = k === 'B2_LEGAL_COMPLIANCE' ? Math.min(0.95, p.narrativeInertia * 1.15) : p.narrativeInertia;
      const gamma = Math.min(0.55, 0.18 * spkBoost * (p.narrativeEnergy / 100.0) * (1.0 - inertiaDamp * dimIn));
      state[k] = Math.max(-1, Math.min(1, (1 - gamma) * state[k] + gamma * uVal));
    });
    history.push({ ...state });
  });

  const canvas = document.getElementById('stateCanvas');
  const ctx = canvas.getContext('2d');
  const W = canvas.width, H = canvas.height;
  ctx.clearRect(0, 0, W, H);

  const padL = 44, padR = 16, padT = 18, padB = 28;
  const plotW = W - padL - padR, plotH = H - padT - padB;

  // Zero axis
  const zeroY = padT + plotH * 0.5;
  ctx.strokeStyle = '#334155'; ctx.lineWidth = 1.2;
  ctx.beginPath(); ctx.moveTo(padL, zeroY); ctx.lineTo(W - padR, zeroY); ctx.stroke();

  [-1, -0.5, 0, 0.5, 1].forEach(v => {
    const y = padT + plotH * (1 - (v + 1) / 2);
    ctx.fillStyle = '#64748b'; ctx.font = '11px system-ui';
    ctx.fillText(v.toFixed(1), 10, y + 4);
  });

  keys.forEach(k => {
    ctx.strokeStyle = colors[k];
    ctx.lineWidth = 2.5;
    ctx.beginPath();
    history.forEach((h, i) => {
      const x = padL + (i / (history.length - 1)) * plotW;
      const y = padT + plotH * (1 - (h[k] + 1) / 2);
      if (i === 0) ctx.moveTo(x, y); else ctx.lineTo(x, y);
    });
    ctx.stroke();
  });
}

function drawDnaGraph() {
  const canvas = document.getElementById('dnaCanvas');
  const ctx = canvas.getContext('2d');
  const W = canvas.width, H = canvas.height;
  ctx.clearRect(0, 0, W, H);

  const spks = DATA.profile.speakerSummaries;
  const beliefs = DATA.transcript.beliefDimensions;

  const leftX = 125, rightX = W - 145;
  const spkPos = {};
  spks.forEach((s, i) => {
    spkPos[s.speakerId] = { x: leftX, y: 45 + i * ((H - 80) / Math.max(1, spks.length - 1)), label: s.speakerId, energy: s.meanEnergy };
  });

  const belPos = {};
  beliefs.forEach((b, i) => {
    belPos[b.id] = { x: rightX, y: 65 + i * ((H - 120) / Math.max(1, beliefs.length - 1)), label: b.id.replace('B1_','').replace('B2_','').replace('B3_','').replace('B4_','') };
  });

  // Edges
  DATA.profile.dnaEdges.forEach(e => {
    if (!spkPos[e.source] || !belPos[e.target]) return;
    const p1 = spkPos[e.source], p2 = belPos[e.target];
    ctx.save();
    ctx.strokeStyle = e.weight >= 0 ? 'rgba(16, 185, 129, 0.75)' : 'rgba(239, 68, 68, 0.75)';
    ctx.lineWidth = Math.max(1.2, Math.abs(e.weight) * 4.2);
    if (e.weight < 0) ctx.setLineDash([5, 4]);
    ctx.beginPath(); ctx.moveTo(p1.x + 14, p1.y); ctx.lineTo(p2.x - 14, p2.y); ctx.stroke();
    ctx.restore();
  });

  // Speaker nodes
  Object.values(spkPos).forEach(p => {
    ctx.fillStyle = '#f59e0b';
    ctx.beginPath(); ctx.arc(p.x, p.y, 11, 0, Math.PI * 2); ctx.fill();
    ctx.fillStyle = '#e2e8f0'; ctx.font = 'bold 11px system-ui';
    ctx.textAlign = 'right';
    ctx.fillText(p.label + ' (E=' + p.energy.toFixed(0) + ')', p.x - 16, p.y + 4);
  });

  // Belief nodes
  Object.values(belPos).forEach(p => {
    ctx.fillStyle = '#38bdf8';
    ctx.beginPath(); ctx.arc(p.x, p.y, 12, 0, Math.PI * 2); ctx.fill();
    ctx.fillStyle = '#e2e8f0'; ctx.font = 'bold 11px system-ui';
    ctx.textAlign = 'left';
    ctx.fillText(p.label, p.x + 18, p.y + 4);
  });
}

function renderKeyframes() {
  const container = document.getElementById('framesStrip');
  Object.entries(DATA.keyFrames).forEach(([idxStr, b64]) => {
    const idx = parseInt(idxStr, 10);
    const pt = pts[idx];
    const card = document.createElement('div');
    card.className = 'frame-card';
    card.innerHTML = `<img src=""${b64}"" alt=""Frame ${idx}""><div class=""meta""><strong>Window #${idx} (${pt.midpointSeconds.toFixed(1)}s) · ${pt.speakerId}</strong>Energy: ${pt.narrativeEnergy.toFixed(1)} · Acoustic: ${pt.acoustic.dbFs.toFixed(1)} dB<br>Visual Cut JSD: ${pt.visual.jensenShannonDivergenceFromPrev.toFixed(3)} bits ${pt.visual.isSceneCutDetected ? '⚡ [CUT]' : ''}</div>`;
    container.appendChild(card);
  });
}

document.getElementById('rngBoost1').addEventListener('input', simulateAndDrawState);
document.getElementById('rngBoost2').addEventListener('input', simulateAndDrawState);
document.getElementById('rngInertia').addEventListener('input', simulateAndDrawState);

drawSignalCanvas();
simulateAndDrawState();
drawDnaGraph();
renderKeyframes();
");
        sb.AppendLine("</script>");
        sb.AppendLine("</body></html>");
        return sb.ToString();
    }

    private static string FormatStance(double v)
    {
        var color = v > 0.15 ? "#34d399" : v < -0.15 ? "#f87171" : "#94a3b8";
        return FormattableString.Invariant($"<span style=\"color:{color};font-weight:600\">{v:+0.00;-0.00;0.00}</span>");
    }
}
