using System.Globalization;
using System.Text;

namespace Argon.Engine;

public static class PaperManuscriptRenderer
{
    public static string RenderPrintReadyHtml(
        CrossDomainAtlasReport atlas,
        CounterfactualControlReport control)
    {
        var s1 = control.Study1MontageOptimization;
        var s2 = control.Study2ShockImpulseControl;
        var s3 = control.Study3InertialShielding;

        var sb = new StringBuilder();
        sb.AppendLine("<!doctype html>");
        sb.AppendLine("<html lang=\"en\">");
        sb.AppendLine("<head>");
        sb.AppendLine("<meta charset=\"utf-8\">");
        sb.AppendLine("<title>Multimodal Narrative Systems Engineering — Jair Ivan Rojas Ruiz (2026)</title>");
        sb.AppendLine("<style>");
        sb.AppendLine(@"
@page {
  size: letter;
  margin: 16mm 15mm 18mm 15mm;
}
* { box-sizing: border-box; }
body {
  margin: 0;
  padding: 0;
  font-family: 'Georgia', 'Times New Roman', Times, serif;
  font-size: 9.6pt;
  line-height: 1.44;
  color: #111827;
  background: #ffffff;
}
.page-wrap {
  max-width: 190mm;
  margin: 0 auto;
}
.title-block {
  text-align: center;
  border-bottom: 1.5px solid #1e293b;
  padding-bottom: 12px;
  margin-bottom: 14px;
}
.title-block h1 {
  font-size: 16.5pt;
  line-height: 1.22;
  margin: 0 0 8px 0;
  color: #0f172a;
}
.author-name {
  font-size: 11.5pt;
  font-weight: bold;
  margin-bottom: 3px;
}
.author-meta {
  font-size: 8.8pt;
  color: #334155;
  font-family: 'Consolas', 'Courier New', monospace;
}
.abstract-box {
  background: #f8fafc;
  border: 1px solid #cbd5e1;
  border-left: 3.5px solid #0f172a;
  padding: 10px 14px;
  margin-bottom: 14px;
  text-align: justify;
  font-size: 9.1pt;
}
.abstract-box strong.abs-title {
  text-transform: uppercase;
  letter-spacing: 0.05em;
  font-size: 8.5pt;
  color: #0f172a;
}
.two-col {
  column-count: 2;
  column-gap: 7mm;
  text-align: justify;
}
h2 {
  font-size: 11pt;
  text-transform: uppercase;
  letter-spacing: 0.03em;
  color: #0f172a;
  border-bottom: 0.75px solid #94a3b8;
  padding-bottom: 2px;
  margin: 12px 0 6px 0;
  break-after: avoid;
}
h3 {
  font-size: 9.8pt;
  font-weight: bold;
  color: #1e293b;
  margin: 9px 0 4px 0;
  break-after: avoid;
}
p { margin: 0 0 7px 0; }
.eq {
  background: #f8fafc;
  border: 0.5px solid #e2e8f0;
  border-radius: 3px;
  padding: 5px 8px;
  margin: 6px 0;
  font-family: 'Cambria Math', 'Times New Roman', serif;
  font-size: 9.2pt;
  text-align: center;
  break-inside: avoid;
}
.eq .eq-num {
  float: right;
  color: #64748b;
  font-size: 8.5pt;
}
.full-span {
  column-span: all;
  margin: 12px 0;
  break-inside: avoid;
}
table {
  width: 100%;
  border-collapse: collapse;
  font-size: 8.2pt;
  margin: 4px 0 6px 0;
  font-family: system-ui, -apple-system, 'Segoe UI', sans-serif;
}
th, td {
  padding: 4.5px 6px;
  border-bottom: 0.6px solid #cbd5e1;
  text-align: left;
}
thead th {
  border-top: 1.2px solid #0f172a;
  border-bottom: 1.2px solid #0f172a;
  font-weight: 700;
  color: #0f172a;
  background: #f1f5f9;
}
tbody tr:last-child td {
  border-bottom: 1.2px solid #0f172a;
}
.caption {
  font-size: 8.3pt;
  color: #334155;
  margin-bottom: 4px;
  font-weight: bold;
}
.fig-box {
  border: 0.75px solid #cbd5e1;
  background: #ffffff;
  padding: 8px;
  border-radius: 4px;
  text-align: center;
}
ul, ol { margin: 0 0 7px 0; padding-left: 16px; }
li { margin-bottom: 3px; }
code {
  font-family: 'Consolas', monospace;
  font-size: 8.3pt;
  background: #f1f5f9;
  padding: 1px 3px;
  border-radius: 2px;
}
.ref-list {
  font-size: 8.3pt;
  line-height: 1.35;
}
");
        sb.AppendLine("</style>");
        sb.AppendLine("</head>");
        sb.AppendLine("<body>");
        sb.AppendLine("<div class=\"page-wrap\">");

        sb.AppendLine("<div class=\"title-block\">");
        sb.AppendLine("  <h1>Multimodal Narrative Systems Engineering: Signal-Grounded Metrics and State-Space Control Dynamics for Audiovisual Media</h1>");
        sb.AppendLine("  <div class=\"author-name\">Jair Ivan Rojas Ruiz (<em>Jair Rojas</em>)</div>");
        sb.AppendLine("  <div class=\"author-meta\">Founder &amp; Principal Investigator, Narrative Systems Engineering (NSE) &bull; ORCID: 0009-0003-1109-4583 &bull; DOI: 10.5281/zenodo.23987656</div>");
        sb.AppendLine("  <div class=\"author-meta\">Repository: https://github.com/jaiborojas-cloud/narrative-systems-engineering &bull; Date: November 5, 2026 (PAPER-001 v1.1)</div>");
        sb.AppendLine("</div>");

        sb.AppendLine("<div class=\"abstract-box\">");
        sb.AppendLine("  <strong class=\"abs-title\">Abstract —</strong> Narratives across film, television, political discourse, and digital media are typically analyzed either through qualitative humanities frameworks or through isolated text-only natural language processing. Conversely, recent attempts to prompt large language models (LLMs) to score &ldquo;narrative impact&rdquo; or &ldquo;emotional intensity&rdquo; on arbitrary 0–100 scales suffer from uncalibrated units, stochastic hallucination, and complete detachment from the physical audiovisual signal. In this paper, we introduce <strong>Narrative Systems Engineering (NSE)</strong> and its computational core, <strong>Computational Narrative Dynamics (CND)</strong>, a signal-grounded mathematical framework originated by Jair Ivan Rojas Ruiz that models audiovisual narratives as multimodal time-series inputs <em>u(t) = (u<sub>ac</sub>(t), u<sub>vis</sub>(t), u<sub>ling</sub>(t))</em> acting upon a bounded cognitive-social state space <em>x(t+1) = f(x(t), u(t))</em>. We formally define and operationalize six core narrative quantities: (1) <strong>Narrative DNA</strong> as a signed, time-anchored entity-belief tripartite graph; (2) <strong>Narrative Entropy (H)</strong> in Shannon bits over chromatic and normative-stance distributions; (3) <strong>Narrative Energy (E)</strong> via a non-collapsing cross-modal resonance functional coupling vocal prosody (dBFS, VAD), visual flux (Jensen-Shannon divergence across color distributions), and lexical-normative cadence; (4) <strong>Narrative Complexity (C)</strong>; (5) <strong>Narrative Inertia (I)</strong>; and (6) <strong>Narrative Controllability</strong> via the discrete-time Controllability Gramian <em>W<sub>c</sub></em>. We validate the framework empirically across a three-work, bilingual (Spanish/English) benchmark corpus (<code>EXP-001</code>, <code>EXP-002</code>, and <code>EXP-003</code>, totaling 513.6 s and 77 time windows), demonstrating clean separation across civic debate, acute fear-appeal campaigning (LBJ's 1964 <em>&ldquo;Daisy&rdquo;</em> spot), and institutional crisis consolation (Reagan's 1986 <em>Challenger</em> address), while solving all 6! = 720 counterfactual montage permutations to achieve a 62.0% algorithmic reduction in terminal normative bias.<br>");
        sb.AppendLine("  <strong>Keywords:</strong> Narrative Systems Engineering, Computational Narrative Dynamics, Multimodal Signal Processing, Shannon Narrative Entropy, Narrative Inertia, State-Space Control.");
        sb.AppendLine("</div>");

        sb.AppendLine("<div class=\"two-col\">");

        sb.AppendLine("<h2>1. Introduction</h2>");
        sb.AppendLine("<p>Human societies are structured, stabilized, and transformed by the transmission of symbolic narratives across audiovisual and linguistic media [5, 6, 7]. However, bridging the gap between raw media signals (pixels, acoustic waveforms, lexical tokens) and higher-order cognitive and social dynamics remains an open scientific and engineering problem.</p>");
        sb.AppendLine("<p>Existing computational approaches divide into two disconnected paradigms: (1) <em>low-level media descriptors</em> (shot boundaries, histograms, or transcripts) that do not model persuasive coupling [4], and (2) <em>uncalibrated generative summaries</em> that prompt LLMs for arbitrary scores. Defining Narrative Energy as a naive product <em>E = Intensity &times; Coherence &times; Repetition &times; Identification</em> fails dimensional analysis and collapses to zero whenever any single factor vanishes.</p>");
        sb.AppendLine("<p>To establish <strong>Narrative Systems Engineering (NSE)</strong> as a falsifiable discipline [1], every metric in our engine (<code>Argon.Engine v0.3</code>) satisfies four strict criteria: (i) <strong>Signal Grounding</strong> in 16-bit PCM audio samples, 64&times;64 RGB24 rasters, and time-aligned speech tokens; (ii) <strong>Stated Units and Bounds</strong> (Shannon bits [2, 3], dBFS, and bounded state vectors in [&minus;1, 1]<sup>K</sup>); (iii) <strong>Layer Separation</strong> between observable work material, symbolic interpretation, and state response; and (iv) <strong>Executable Reproducibility</strong>.</p>");
        sb.AppendLine("<p><strong>Terminology Disambiguation &amp; Chronological Priority:</strong> While homonymous, purely qualitative or essayistic uses of the phrase <em>&ldquo;Narrative Systems Engineering&rdquo;</em> exist on the public web (including game-design notes or qualitative screenplay taxonomies such as Strangium's November 2, 2026 <code>NSE-001</code> post), those texts contain no mathematical formulations, no physical audio/video signal processing, no state-space control models, and no software. In contrast, the work of <strong>Jair Ivan Rojas Ruiz</strong>—cryptographically archived since <strong>September 8, 2026</strong> and registered in Zenodo under <strong>DOI 10.5281/zenodo.23987656</strong> (ORCID: 0009-0003-1109-4583)—formally establishes the quantitative, signal-grounded discipline of <strong>Multimodal Narrative Systems Engineering &amp; Computational Narrative Dynamics (NSE/CND)</strong>.</p>");

        sb.AppendLine("<h2>2. Multimodal Signal Vector u(t)</h2>");
        sb.AppendLine("<p>Let an audiovisual work of duration <em>T</em> seconds be partitioned into <em>N</em> discrete time windows <em>t &isin; {0, 1, ..., N&minus;1}</em> of width <em>&Delta;t</em>. At each window <em>t</em>, the work emits a multimodal signal vector <em>u(t) = (u<sub>ac</sub>(t), u<sub>vis</sub>(t), u<sub>ling</sub>(t))</em>.</p>");
        sb.AppendLine("<h3>2.1 Acoustic Prosody Sub-Vector u<sub>ac</sub>(t)</h3>");
        sb.AppendLine("<p>From the downmixed mono 16 kHz PCM16 waveform containing <em>M</em> normalized samples <em>s<sub>i</sub> &isin; [&minus;1, 1]</em> within window <em>t</em>, we compute Root-Mean-Square amplitude and decibels full scale:</p>");
        sb.AppendLine("<div class=\"eq\">RMS(t) = [ (1/M) &Sigma; s<sub>i</sub><sup>2</sup> ]<sup>1/2</sup>, &nbsp;&nbsp; L<sub>dB</sub>(t) = 20 log<sub>10</sub>(max(RMS(t), 10<sup>&minus;5</sup>)) <span class=\"eq-num\">(1)</span></div>");
        sb.AppendLine("<p>Sub-framing window <em>t</em> into 100 ms intervals yields the Speech Activity Ratio VAD(t) &isin; [0, 1] and intra-window dynamic range &Delta;L<sub>90-10</sub>(t), normalized into acoustic intensity <em>I<sub>ac</sub>(t) &isin; [0, 1]</em>:</p>");
        sb.AppendLine("<div class=\"eq\">I<sub>ac</sub>(t) = clip(0.55 L&#770;<sub>dB</sub>(t) + 0.30 VAD(t) + 0.15 (&Delta;L<sub>90-10</sub>(t)/18), 0, 1) <span class=\"eq-num\">(2)</span></div>");

        sb.AppendLine("<h3>2.2 Visual Dynamics Sub-Vector u<sub>vis</sub>(t)</h3>");
        sb.AppendLine("<p>For each frame resized to a 64&times;64 RGB24 raster (4096 pixels), we compute ITU-R BT.709 luminance <em>Y<sub>p</sub></em>, spatial contrast <em>&sigma;<sub>Y</sub>(t)</em>, HSV saturation, and the 512-bin quantized color distribution <em>P<sub>t</sub>(b)</em>. Camera cuts and visual flux are measured via the <strong>Jensen-Shannon Divergence</strong> in bits [3]:</p>");
        sb.AppendLine("<div class=\"eq\">D<sub>JS</sub>(P<sub>t</sub> || P<sub>t&minus;1</sub>) = &frac12; D<sub>KL</sub>(P<sub>t</sub> || M) + &frac12; D<sub>KL</sub>(P<sub>t&minus;1</sub> || M) &isin; [0, 1] bits <span class=\"eq-num\">(3)</span></div>");
        sb.AppendLine("<p>where <em>M = &frac12;(P<sub>t</sub> + P<sub>t&minus;1</sub>)</em>. A hard scene cut is detected deterministically whenever <em>D<sub>JS</sub>(t) &ge; 0.30 bits</em>. Visual salience <em>&psi;<sub>vis</sub>(t)</em> couples <em>D<sub>JS</sub>(t)</em>, contrast <em>&sigma;<sub>Y</sub>(t)</em>, and chromatic Shannon entropy <em>H<sub>vis</sub>(t)</em>.</p>");

        sb.AppendLine("<h2>3. Six Core NSE Metrics &amp; Control</h2>");
        sb.AppendLine("<h3>3.1 Narrative Energy E(t) &amp; Cross-Modal Resonance</h3>");
        sb.AppendLine("<p>To prevent zero-collapse while rewarding vocal-lexical synchronization, Narrative Energy <em>E(t) &isin; [0, 100]</em> is formulated via the geometric Cross-Modal Resonance functional <em>&Phi;<sub>sync</sub>(t) = [I<sub>ac</sub>(t) &middot; I<sub>ling</sub>(t)]<sup>1/2</sup></em>:</p>");
        sb.AppendLine("<div class=\"eq\">E(t) = 100 &middot; clip(0.34 I<sub>ac</sub>(t) + 0.22 &psi;<sub>vis</sub>(t) + 0.29 I<sub>ling</sub>(t) + 0.15 &Phi;<sub>sync</sub>(t), 0, 1) <span class=\"eq-num\">(4)</span></div>");

        sb.AppendLine("<h3>3.2 Shannon Narrative Stance Entropy H<sub>narr</sub>(t)</h3>");
        sb.AppendLine("<p>Across <em>K = 4</em> belief dimensions (8 signed poles <em>p<sub>k,&plusmn;</sub>(t)</em> weighted by cumulative energy exposure), Shannon Narrative Entropy [2] is:</p>");
        sb.AppendLine("<div class=\"eq\">H<sub>narr</sub>(t) = &minus;&Sigma;<sub>k,s</sub> p<sub>k,s</sub>(t) log<sub>2</sub> p<sub>k,s</sub>(t) &isin; [0, 3.00] bits <span class=\"eq-num\">(5)</span></div>");

        sb.AppendLine("<h3>3.3 Narrative Inertia I(t) &amp; LTV State-Space Control</h3>");
        sb.AppendLine("<p>Narrative Inertia <em>I(t) = clip(0.55 A<sub>inst</sub>(t) + 0.25 ||x(t)||<sub>1</sub>/K + 0.20 (t+1)/N, 0, 1)</em> damps state susceptibility in the discrete-time Linear Time-Varying (LTV) opinion state equation [4]:</p>");
        sb.AppendLine("<div class=\"eq\">x<sub>k</sub>(t+1) = (1 &minus; &gamma;<sub>k</sub>(t)) x<sub>k</sub>(t) + &gamma;<sub>k</sub>(t) z<sub>k</sub>(t), &nbsp;&nbsp; &gamma;<sub>k</sub>(t) = 0.18 [E(t)/100] (1 &minus; 0.65 I<sub>k</sub>(t)) <span class=\"eq-num\">(6)</span></div>");
        sb.AppendLine("<p>The diagonal Controllability Gramian over horizon <em>[0, N]</em> is <em>W<sub>c,kk</sub> = &Sigma;<sub>t</sub> [&Phi;<sub>k</sub>(N, t+1) b&#771;<sub>k</sub>(t)]<sup>2</sup></em>, with condition number <em>&kappa;(W<sub>c</sub>) = &lambda;<sub>max</sub> / &lambda;<sub>min</sub></em>.</p>");

        // Full-width Table 1: Cross-Domain Atlas (EXP-001 & EXP-002)
        sb.AppendLine("<div class=\"full-span\">");
        sb.AppendLine("  <div class=\"caption\">Table 1: Cross-Domain Empirical Atlas (EXP-001 &amp; EXP-002) Comparing Three Narrative Regimes Across 513.6 Seconds (77 Windows).</div>");
        sb.AppendLine("  <table>");
        sb.AppendLine("    <thead><tr><th>Metric / Dimension</th><th>CORPUS-001: Insumisión Debate (1994, ES)</th><th>CORPUS-002: LBJ 'Daisy' Spot (1964, EN)</th><th>CORPUS-003: Reagan Challenger (1986, EN)</th><th>Structural Interpretation</th></tr></thead>");
        sb.AppendLine("    <tbody>");
        sb.AppendLine("      <tr><td><strong>Communicative Regime</strong></td><td>Polyphonic Civic Debate</td><td>Acute Fear-Appeal Political Spot</td><td>Institutional Crisis Consolation</td><td>Three orthogonal narrative functions</td></tr>");
        sb.AppendLine("      <tr><td><strong>Duration &amp; Windows</strong></td><td>194.84 s (39 &times; 5 s)</td><td>59.83 s (12 &times; 5 s)</td><td>258.93 s (26 &times; 10 s)</td><td>Standardized multimodal profiling</td></tr>");
        sb.AppendLine("      <tr><td><strong>Mean Narrative Energy E&#772;</strong></td><td><strong>63.58</strong></td><td>50.51</td><td>56.45</td><td>Debate sustains high vocal/lexical energy</td></tr>");
        sb.AppendLine("      <tr><td><strong>Peak Narrative Energy E<sub>max</sub></strong></td><td>76.54 (Window #7)</td><td><strong>83.29 (Window #6)</strong></td><td>61.98 (Window #10)</td><td><em>Daisy</em> nuclear blast yields highest spike</td></tr>");
        sb.AppendLine("      <tr><td><strong>Final Stance Entropy H(T)</strong></td><td><strong>2.7574 bits (91.9%)</strong></td><td>2.2292 bits (74.3%)</td><td><strong>2.0240 bits (67.5%)</strong></td><td>Debate maximizes frame dispersion</td></tr>");
        sb.AppendLine("      <tr><td><strong>Mean / Peak Inertia I(t)</strong></td><td>0.5019 (peak 0.728)</td><td>0.4420 (peak 0.799)</td><td><strong>0.7266 (peak 0.8921)</strong></td><td>Reagan maximizes institutional inertia</td></tr>");
        sb.AppendLine("      <tr><td><strong>Scene Cuts (D<sub>JS</sub> &ge; 0.30)</strong></td><td>4 cuts (max 0.490 bits)</td><td>3 cuts (<strong>max 0.577 bits</strong>)</td><td><strong>1 cut (max 0.042 mid-speech)</strong></td><td>Unbroken Oval Office shot vs. atomic flash</td></tr>");
        sb.AppendLine("      <tr><td><strong>Final Polarization P(T)</strong></td><td><strong>0.5515</strong></td><td>0.4200</td><td><strong>0.0617</strong></td><td>Reagan achieves near-zero polarization</td></tr>");
        sb.AppendLine("      <tr><td><strong>Control Condition &kappa;(W<sub>c</sub>)</strong></td><td>2.738</td><td>1.783</td><td>1.457</td><td>Balanced multi-axis control in CORPUS-003</td></tr>");
        sb.AppendLine("    </tbody>");
        sb.AppendLine("  </table>");
        sb.AppendLine("</div>");

        sb.AppendLine("<h2>4. Empirical Results: EXP-001 &amp; EXP-002</h2>");
        sb.AppendLine("<p>In <code>EXP-001</code> (<code>CORPUS-001</code>, Spanish university conscription debate), Jensen-Shannon divergence <em>D<sub>JS</sub> &ge; 0.30 bits</em> recovered all four true camera transitions (Windows #0, #1, #18, #26) without false positives. Speaker <code>SPK_01</code> achieved the highest Narrative Energy (mean 72.90, peak 76.54 at Window #7, &minus;16.96 dBFS, 203.7 WPM) but the lowest Narrative Inertia (0.151) because he advocated abolishing an existing institution. Conversely, <code>SPK_03</code> spoke 4.4 dB softer (&minus;21.34 dBFS) yet achieved the highest Inertia (0.668) by anchoring military service in statutory law and taxation.</p>");
        sb.AppendLine("<p>In <code>EXP-002</code> (Table 1 &amp; Figure 1), projecting all three works into the 5D NSE Metric Space yields non-degenerate Euclidean separation: <strong>||m<sub>C1</sub> &minus; m<sub>C2</sub>||<sub>2</sub> = 0.2711</strong>, <strong>||m<sub>C2</sub> &minus; m<sub>C3</sub>||<sub>2</sub> = 0.5128</strong>, and <strong>||m<sub>C1</sub> &minus; m<sub>C3</sub>||<sub>2</sub> = 0.6135</strong> (terminal state distance <strong>||x<sub>C1</sub>(T) &minus; x<sub>C3</sub>(T)||<sub>2</sub> = 1.0045</strong>).</p>");

        // Full-width Figure 1 (Vector SVG of Cross-Domain Phase Space & Montage Optimization)
        sb.AppendLine("<div class=\"full-span\">");
        sb.AppendLine("  <div class=\"fig-box\">");
        sb.AppendLine(BuildVectorFigureSvg(atlas, control));
        sb.AppendLine("  </div>");
        sb.AppendLine("  <div class=\"caption\" style=\"margin-top:5px\">Figure 1: (Left) Normalized Cross-Domain Narrative Energy Trajectories E(t/T) across CORPUS-001, CORPUS-002 (showing the t/T = 0.50 nuclear detonation spike at E = 83.29), and CORPUS-003. (Right) EXP-003 Reachable Terminal State Cloud across all 6! = 720 Montage Permutations of CORPUS-001, highlighting the synthesized Civic Equilibrium Montage &pi;*<sub>eq</sub> (-62.0% normative bias).</div>");
        sb.AppendLine("</div>");

        // Full-width Table 2: EXP-003 Counterfactual Control
        sb.AppendLine("<div class=\"full-span\">");
        sb.AppendLine("  <div class=\"caption\">Table 2: EXP-003 Optimal Montage Synthesis Solutions Across All 6! = 720 Speaker-Block Permutations on CORPUS-001.</div>");
        sb.AppendLine("  <table>");
        sb.AppendLine("    <thead><tr><th>Synthesis Objective</th><th>Optimal Editorial Block Sequence &pi;*</th><th>x<sub>1</sub>(T) Anti-Mil</th><th>x<sub>2</sub>(T) Legal</th><th>x<sub>3</sub>(T) Pragmatic</th><th>x<sub>4</sub>(T) Autonomy</th><th>Bias Norm ||x(T)||<sub>2</sub></th><th>Shift ||&Delta;x||<sub>2</sub></th></tr></thead>");
        sb.AppendLine("    <tbody>");
        sb.AppendLine(CultureInfo.InvariantCulture, $"      <tr><td><strong>Original 1994 Broadcast</strong></td><td><code>ENV_POSTER &rarr; SPK_01 &rarr; SPK_02 &rarr; SPK_03 &rarr; SPK_04 &rarr; SPK_05</code></td><td>&minus;0.1445</td><td>+0.5524</td><td>+0.6446</td><td>+0.0391</td><td>0.8620</td><td>0.0000</td></tr>");
        foreach (var opt in new[] { s1.OptimalAntiMilitarist, s1.OptimalLegalCompliance, s1.OptimalCivicEquilibrium, s1.OptimalAutonomy })
        {
            var seq = string.Join(" &rarr; ", opt.BlockOrder);
            sb.AppendLine(CultureInfo.InvariantCulture, $"      <tr><td><strong>{opt.ObjectiveTag}</strong></td><td><code>{seq}</code></td><td>{opt.FinalStateX["B1_ANTIMILITARISM"]:+0.0000;-0.0000}</td><td>{opt.FinalStateX["B2_LEGAL_COMPLIANCE"]:+0.0000;-0.0000}</td><td>{opt.FinalStateX["B3_PRAGMATIC_SERVICE"]:+0.0000;-0.0000}</td><td>{opt.FinalStateX["B4_INDIVIDUAL_AUTONOMY"]:+0.0000;-0.0000}</td><td><strong>{opt.FinalStateNormL2:F4}</strong></td><td>{opt.ShiftFromOriginalL2:F4}</td></tr>");
        }
        sb.AppendLine("    </tbody>");
        sb.AppendLine("  </table>");
        sb.AppendLine("</div>");

        sb.AppendLine("<h2>5. Counterfactual Control (EXP-003)</h2>");
        sb.AppendLine("<p>To demonstrate constructive systems synthesis, <code>CounterfactualController.cs</code> solves three discrete-time control problems across our corpus:</p>");
        sb.AppendLine("<ol>");
        sb.AppendLine("  <li><strong>Exhaustive 6! = 720 Montage Reachability (CORPUS-001):</strong> Holding 100% of the physical footage invariant (<em>H(T) = 2.7574 bits</em>), editorial ordering alone spans sign-reversing intervals across every belief dimension: <em>x<sub>1</sub>(T) &isin; [&minus;0.2603, +0.5130]</em> (&Delta; = 0.7733) and <em>x<sub>2</sub>(T) &isin; [&minus;0.1795, +0.5830]</em> (&Delta; = 0.7625). Solving <em>min<sub>&pi;</sub> ||x(T)||<sub>2</sub></em> yields the <strong>Civic Equilibrium Montage &pi;*<sub>eq</sub></strong> (Table 2), reducing terminal normative bias by <strong>&minus;62.0%</strong> (<em>||x*(T)||<sub>2</sub> = 0.3274</em> vs. <em>0.8620</em>) without cutting a single frame.</li>");
        sb.AppendLine("  <li><strong>Non-Verbal Shock Impulse Leverage (CORPUS-002):</strong> Attenuating the nuclear roar (&minus;13.68 dBFS &rarr; &minus;25 dBFS) and atomic flash cut (<em>D<sub>JS</sub> = 0.5766 &rarr; 0.04 bits</em>) in Windows #4–#8 of LBJ's <em>&ldquo;Daisy&rdquo;</em> spot while keeping the transcript identical reduces terminal mobilization <em>x<sub>2</sub>(T)</em> from +0.2624 to +0.1841—proving that <strong>29.84% of the commercial's state displacement is driven purely by the non-verbal audiovisual shock wave</strong>.</li>");
        sb.AppendLine("  <li><strong>Institutional Inertial Shielding (CORPUS-003):</strong> Removing presidential institutional anchoring reduces terminal inertia by <strong>&minus;0.4657</strong> (0.8921 &rarr; 0.4264), whereas accumulated inertia shields the address from sign reversal (<em>x<sub>2</sub>(T) = +0.2321 &gt; 0</em>) even under a late 30 s high-energy crisis critique perturbation.</li>");
        sb.AppendLine("</ol>");

        sb.AppendLine("<h2>6. Conclusion</h2>");
        sb.AppendLine("<p>By unifying physical signal operators (PCM prosody, Jensen-Shannon visual divergence, Shannon stance entropy) with LTV state-space control and exhaustive montage optimization, <strong>Narrative Systems Engineering</strong> transitions media analysis from qualitative metaphor to a falsifiable, reproducible engineering discipline.</p>");

        sb.AppendLine("<h2>References</h2>");
        sb.AppendLine("<ol class=\"ref-list\">");
        sb.AppendLine("  <li>Rojas Ruiz, J. I. (2026). <em>Narrative Systems Engineering (NSE) and Computational Narrative Dynamics (CND): Foundational Architecture, Multimodal Signal Metrics, and Counterfactual State-Space Control</em>. Zenodo. DOI: 10.5281/zenodo.23987656 (ORCID: 0009-0003-1109-4583).</li>");
        sb.AppendLine("  <li>Shannon, C. E. (1948). A Mathematical Theory of Communication. <em>Bell System Technical Journal</em>, 27(3), 379–423.</li>");
        sb.AppendLine("  <li>Lin, J. (1991). Divergence measures based on the Shannon entropy. <em>IEEE Transactions on Information Theory</em>, 37(1), 145–151.</li>");
        sb.AppendLine("  <li>Friedkin, N. E., &amp; Johnsen, E. C. (1990). Social influence and opinions. <em>Journal of Mathematical Sociology</em>, 15(3-4), 193–206.</li>");
        sb.AppendLine("  <li>Vicol, P., Tapaswi, M., Castrejon, L., &amp; Fidler, S. (2018). MovieGraphs: Towards Understanding Human-Centric Situations from Videos. <em>CVPR 2018</em>, 8581–8590.</li>");
        sb.AppendLine("  <li>Green, M. C., &amp; Brock, T. C. (2000). The role of transportation in the persuasiveness of public narratives. <em>JPSP</em>, 79(5), 701–721.</li>");
        sb.AppendLine("  <li>Jones, M. D., &amp; McBeth, M. K. (2010). A Narrative Policy Framework: Clear Enough to Be Wrong? <em>Policy Studies Journal</em>, 38(2), 329–353.</li>");
        sb.AppendLine("  <li>Shiller, R. J. (2017). Narrative Economics. <em>American Economic Review</em>, 107(4), 967–1004.</li>");
        sb.AppendLine("</ol>");

        sb.AppendLine("</div>"); // two-col
        sb.AppendLine("</div>"); // page-wrap
        sb.AppendLine("</body></html>");
        return sb.ToString();
    }

    private static string BuildVectorFigureSvg(CrossDomainAtlasReport atlas, CounterfactualControlReport control)
    {
        var sb = new StringBuilder();
        sb.AppendLine("<svg viewBox=\"0 0 760 220\" width=\"100%\" height=\"210\" xmlns=\"http://www.w3.org/2000/svg\" font-family=\"system-ui, sans-serif\" font-size=\"9\">");

        // Left Panel: Normalized Energy Trajectories E(t/T) [x: 35..355, y: 20..185]
        sb.AppendLine("  <rect x=\"35\" y=\"20\" width=\"320\" height=\"165\" fill=\"#f8fafc\" stroke=\"#cbd5e1\" stroke-width=\"1\"/>");
        sb.AppendLine("  <text x=\"195\" y=\"13\" text-anchor=\"middle\" font-weight=\"bold\" font-size=\"9.5\" fill=\"#0f172a\">A. Cross-Domain Normalized Energy Trajectories E(t/T)</text>");
        for (int v = 0; v <= 100; v += 25)
        {
            double py = 20 + 165 * (1.0 - v / 100.0);
            sb.AppendLine(CultureInfo.InvariantCulture, $"  <line x1=\"35\" y1=\"{py:F1}\" x2=\"355\" y2=\"{py:F1}\" stroke=\"#e2e8f0\" stroke-width=\"0.7\"/>");
            sb.AppendLine(CultureInfo.InvariantCulture, $"  <text x=\"30\" y=\"{py + 3:F1}\" text-anchor=\"end\" fill=\"#475569\" font-size=\"8\">{v}</text>");
        }
        sb.AppendLine("  <text x=\"195\" y=\"202\" text-anchor=\"middle\" fill=\"#334155\" font-size=\"8.5\">Normalized Time τ = t / T ∈ [0, 1]</text>");

        string[] colors = ["#d97706", "#dc2626", "#0284c7"];
        for (int wi = 0; wi < atlas.Works.Count; wi++)
        {
            var w = atlas.Works[wi];
            var pts = new StringBuilder();
            foreach (var pt in w.Orbit)
            {
                double px = 35 + pt.NormalizedTime * 320.0;
                double py = 20 + 165 * (1.0 - pt.Energy / 100.0);
                pts.Append(CultureInfo.InvariantCulture, $"{px:F1},{py:F1} ");
            }
            sb.AppendLine(CultureInfo.InvariantCulture, $"  <polyline fill=\"none\" stroke=\"{colors[wi]}\" stroke-width=\"2\" points=\"{pts.ToString().Trim()}\"/>");
        }
        sb.AppendLine("  <text x=\"42\" y=\"34\" fill=\"#d97706\" font-weight=\"bold\" font-size=\"8\">— CORPUS-001 (Debate)</text>");
        sb.AppendLine("  <text x=\"145\" y=\"34\" fill=\"#dc2626\" font-weight=\"bold\" font-size=\"8\">— CORPUS-002 (LBJ Daisy, Peak 83.3)</text>");
        sb.AppendLine("  <text x=\"42\" y=\"46\" fill=\"#0284c7\" font-weight=\"bold\" font-size=\"8\">— CORPUS-003 (Reagan Challenger)</text>");

        // Right Panel: 720 Permutations Cloud [x: 415..735, y: 20..185]
        sb.AppendLine("  <rect x=\"415\" y=\"20\" width=\"320\" height=\"165\" fill=\"#f8fafc\" stroke=\"#cbd5e1\" stroke-width=\"1\"/>");
        sb.AppendLine("  <text x=\"575\" y=\"13\" text-anchor=\"middle\" font-weight=\"bold\" font-size=\"9.5\" fill=\"#0f172a\">B. Reachable State Hull Across 6! = 720 Montages (EXP-003)</text>");

        double MapCloudX(double x1) => 415 + ((x1 + 0.4) / 1.05) * 320.0;
        double MapCloudY(double x2) => 20 + 165 * (1.0 - (x2 + 0.3) / 1.05);

        sb.AppendLine(CultureInfo.InvariantCulture, $"  <line x1=\"{MapCloudX(0):F1}\" y1=\"20\" x2=\"{MapCloudX(0):F1}\" y2=\"185\" stroke=\"#94a3b8\" stroke-dasharray=\"3,2\" stroke-width=\"0.8\"/>");
        sb.AppendLine(CultureInfo.InvariantCulture, $"  <line x1=\"415\" y1=\"{MapCloudY(0):F1}\" x2=\"735\" y2=\"{MapCloudY(0):F1}\" stroke=\"#94a3b8\" stroke-dasharray=\"3,2\" stroke-width=\"0.8\"/>");

        var s1 = control.Study1MontageOptimization;
        foreach (var c in s1.CloudSamples)
        {
            sb.AppendLine(CultureInfo.InvariantCulture, $"  <circle cx=\"{MapCloudX(c.X1AntiMil):F1}\" cy=\"{MapCloudY(c.X2Legal):F1}\" r=\"2.2\" fill=\"#64748b\" fill-opacity=\"0.35\"/>");
        }

        // Highlights
        void DrawHighlight(double x1, double x2, string col, string label, double dx, double dy)
        {
            double cx = MapCloudX(x1), cy = MapCloudY(x2);
            sb.AppendLine(CultureInfo.InvariantCulture, $"  <circle cx=\"{cx:F1}\" cy=\"{cy:F1}\" r=\"4.5\" fill=\"{col}\" stroke=\"#fff\" stroke-width=\"1\"/>");
            sb.AppendLine(CultureInfo.InvariantCulture, $"  <text x=\"{cx + dx:F1}\" y=\"{cy + dy:F1}\" fill=\"{col}\" font-weight=\"bold\" font-size=\"8\">{label}</text>");
        }

        DrawHighlight(s1.OriginalFinalStateX["B1_ANTIMILITARISM"], s1.OriginalFinalStateX["B2_LEGAL_COMPLIANCE"], "#d97706", "Original (0.862)", -30, -7);
        DrawHighlight(s1.OptimalAntiMilitarist.FinalStateX["B1_ANTIMILITARISM"], s1.OptimalAntiMilitarist.FinalStateX["B2_LEGAL_COMPLIANCE"], "#dc2626", "π* Anti-Mil (+0.513)", -75, -7);
        DrawHighlight(s1.OptimalCivicEquilibrium.FinalStateX["B1_ANTIMILITARISM"], s1.OptimalCivicEquilibrium.FinalStateX["B2_LEGAL_COMPLIANCE"], "#059669", "π* Equilibrium (0.327)", 7, 3);
        DrawHighlight(s1.OptimalLegalCompliance.FinalStateX["B1_ANTIMILITARISM"], s1.OptimalLegalCompliance.FinalStateX["B2_LEGAL_COMPLIANCE"], "#0284c7", "π* Legal (+0.583)", 6, 4);

        sb.AppendLine("  <text x=\"575\" y=\"202\" text-anchor=\"middle\" fill=\"#334155\" font-size=\"8.5\">Terminal State x₁(T) Anti-Militarism vs. x₂(T) Legal Compliance</text>");
        sb.AppendLine("</svg>");
        return sb.ToString();
    }

    public static byte[] RenderNativePdfBytes(
        CrossDomainAtlasReport atlas,
        CounterfactualControlReport control)
    {
        var s1 = control.Study1MontageOptimization;
        var s2 = control.Study2ShockImpulseControl;
        var s3 = control.Study3InertialShielding;

        var pages = new List<StringBuilder>();

        // ---------------- PAGE 1: TITLE, ABSTRACT, SECTIONS 1-3 ----------------
        var p1 = new StringBuilder();
        double y = 742;

        // Header banner line
        DrawLine(p1, 48, y + 14, 564, y + 14, 1.2, 0.08, 0.15, 0.28);
        DrawText(p1, "F2", 8.0, 48, y + 3, "NARRATIVE SYSTEMS ENGINEERING (NSE) — PREPRINT PAPER-001 v1.1 — DOI: 10.5281/zenodo.23987656");
        DrawText(p1, "F2", 8.0, 462, y + 3, "ORCID: 0009-0003-1109-4583");
        y -= 20;

        // Title
        DrawText(p1, "F2", 14.5, 48, y, "Multimodal Narrative Systems Engineering: Signal-Grounded Metrics");
        y -= 17;
        DrawText(p1, "F2", 14.5, 48, y, "and State-Space Control Dynamics for Audiovisual Media");
        y -= 18;

        // Author & Affiliation
        DrawText(p1, "F2", 10.5, 48, y, "Jair Ivan Rojas Ruiz (Jair Rojas) — Founder & Principal Investigator, Narrative Systems Engineering");
        y -= 13;
        DrawText(p1, "F4", 7.6, 48, y, "ORCID: https://orcid.org/0009-0003-1109-4583 | DOI: https://doi.org/10.5281/zenodo.23987656 | Date: Nov 5, 2026");
        y -= 10;
        DrawLine(p1, 48, y, 564, y, 0.8, 0.2, 0.25, 0.35);
        y -= 16;

        // Abstract Box
        DrawRect(p1, 48, y - 96, 516, 104, 0.97, 0.98, 1.0, 0.75, 0.80, 0.88);
        DrawText(p1, "F2", 9.0, 56, y - 6, "ABSTRACT");
        string[] absLines =
        [
            "Narratives across film, television, political discourse, and digital media are typically analyzed either through qualitative humanities",
            "frameworks or isolated text-only NLP. Conversely, prompting LLMs to score 'narrative impact' on arbitrary 0-100 scales suffers from",
            "uncalibrated units, stochastic hallucination, and detachment from the physical audiovisual signal. We introduce Narrative Systems",
            "Engineering (NSE) and Computational Narrative Dynamics (CND), originated by Jair Ivan Rojas Ruiz, modeling audiovisual works as",
            "multimodal inputs u(t) = (u_ac(t), u_vis(t), u_ling(t)) driving a bounded cognitive-social state space x(t+1) = f(x(t), u(t)). We define",
            "six core quantities: (1) Narrative DNA (signed tripartite graph), (2) Shannon Narrative Entropy H(t) in bits, (3) Narrative Energy E(t)",
            "via cross-modal resonance, (4) Complexity C(t), (5) Narrative Inertia I(t), and (6) Controllability Gramian W_c. Validated across 3 works",
            "(513.6 s, 77 windows: EXP-001, EXP-002, EXP-003), NSE cleanly separates civic debate, fear-appeal campaigning (LBJ 'Daisy' 1964), and",
            "crisis consolation (Reagan 'Challenger' 1986), while solving all 6! = 720 montage permutations to reduce normative bias by 62.0%."
        ];
        double ay = y - 18;
        foreach (var line in absLines)
        {
            DrawText(p1, "F1", 8.6, 56, ay, line);
            ay -= 9.6;
        }
        y -= 114;

        // Section 1
        DrawSectionHeader(p1, ref y, "1. INTRODUCTION, AXIOMS & CHRONOLOGICAL PRIORITY DISAMBIGUATION");
        WriteParagraph(p1, ref y,
        [
            "Bridging the gap between raw media signals (PCM16 acoustic waveforms, RGB24 pixel rasters, and lexical tokens) and higher-order cognitive",
            "and social state dynamics requires falsifiable mathematical operators. Naive multiplicative formulas such as E = Intensity x Coherence x",
            "Repetition x Identification fail dimensional analysis and collapse to zero whenever any single factor vanishes.",
            "To establish Narrative Systems Engineering (NSE) as a rigorous engineering discipline, Argon.Engine v0.3 enforces four axioms:",
            "  (1) Signal Grounding: Direct derivation from physical PCM16 audio samples, 64x64 RGB24 rasters, and time-aligned speech segments.",
            "  (2) Stated Units & Bounds: Entropy and visual divergence in Shannon bits, audio in dBFS, and state trajectories in [-1, 1]^K.",
            "  (3) Layer Separation: Strict separation between observable media signal u(t), normative stance coding z(t), and state response x(t).",
            "  (4) Executable Reproducibility: Deterministic verification via automated invariant test suites (Verify-Argon.ps1).",
            "Terminology Disambiguation & Chronological Priority: While homonymous, purely qualitative or essayistic uses of the phrase 'Narrative",
            "Systems Engineering' exist on the web (including informal game-design notes or qualitative screenplay taxonomies such as Strangium's",
            "Nov 2, 2026 NSE-001 post), those texts lack mathematical formulations, physical audio/video signal processing, information-theoretic",
            "entropy, state-space control models, and executable software. In contrast, the foundational work of Jair Ivan Rojas Ruiz (archived since",
            "September 8, 2026 and registered in Zenodo under DOI: 10.5281/zenodo.23987656, ORCID: 0009-0003-1109-4583) formally establishes the",
            "quantitative, signal-grounded discipline of Multimodal Narrative Systems Engineering & Computational Narrative Dynamics (NSE/CND)."
        ]);

        // Section 2
        DrawSectionHeader(p1, ref y, "2. MULTIMODAL SIGNAL FORMULATION u(t) = (u_ac(t), u_vis(t), u_ling(t))");
        WriteParagraph(p1, ref y,
        [
            "Let an audiovisual work of duration T seconds be partitioned into N discrete windows t in {0, ..., N-1} of duration Delta_t.",
            "  * Acoustic Sub-Vector u_ac(t): From mono 16 kHz PCM16 samples s_i in [-1, 1], we compute RMS(t), decibels full scale L_dB(t) =",
            "    20 log10(max(RMS(t), 1e-5)), 100ms Speech Activity Ratio VAD(t) in [0, 1], and intra-window dynamic range Delta_L(t).",
            "  * Visual Sub-Vector u_vis(t): From 64x64 RGB24 rasters, we extract ITU-R BT.709 luminance Y, spatial contrast sigma_Y(t), chromatic",
            "    entropy H_vis(t), and the 512-bin color distribution P_t(b). Camera cuts are detected via Jensen-Shannon Divergence in bits:"
        ]);
        DrawEquationBox(p1, ref y, "D_JS(P_t || P_{t-1}) = 0.5 D_KL(P_t || M) + 0.5 D_KL(P_{t-1} || M) in [0, 1] bits  (Cut >= 0.30)", "(1)");

        // Section 3
        DrawSectionHeader(p1, ref y, "3. FORMAL DEFINITION OF THE SIX CORE NSE METRICS & STATE CONTROL");
        DrawEquationBox(p1, ref y, "Narrative Energy: E(t) = 100 * clip(0.34 I_ac + 0.22 psi_vis + 0.29 I_ling + 0.15 sqrt(I_ac*I_ling))", "(2)");
        DrawEquationBox(p1, ref y, "Shannon Stance Entropy: H_narr(t) = - Sum_{k,s} p_{k,s}(t) log2 p_{k,s}(t) in [0, 3.00] bits (K=4)", "(3)");
        DrawEquationBox(p1, ref y, "Narrative Inertia: I(t) = clip(0.55 A_inst(t) + 0.25 (||x(t)||_1 / K) + 0.20 ((t + 1) / N), 0, 1)", "(4)");
        DrawEquationBox(p1, ref y, "LTV State Update: x_k(t+1) = (1 - gamma_k(t)) x_k(t) + gamma_k(t) z_k(t),  gamma_k ~ E(t)*(1-0.65 I_k)", "(5)");
        DrawEquationBox(p1, ref y, "Controllability Gramian: W_{c,kk} = Sum_{t=0..N-1} [Phi_k(N,t+1) b_k(t)]^2,  kappa = l_max / l_min", "(6)");

        DrawFooter(p1, 1, 2);
        pages.Add(p1);

        // ---------------- PAGE 2: EMPIRICAL TABLES, VECTOR PLOTS, EXP-001/002/003 & REFERENCES ----------------
        var p2 = new StringBuilder();
        double y2 = 742;
        DrawLine(p2, 48, y2 + 14, 564, y2 + 14, 1.0, 0.08, 0.15, 0.28);
        DrawText(p2, "F2", 8.5, 48, y2 + 3, "JAIR IVAN ROJAS RUIZ — MULTIMODAL NARRATIVE SYSTEMS ENGINEERING (EXP-001, EXP-002 & EXP-003)");
        y2 -= 16;

        DrawSectionHeader(p2, ref y2, "4. CROSS-DOMAIN EMPIRICAL ATLAS (EXP-001 & EXP-002: 3 WORKS, 513.6s, 77 WINDOWS)");
        string[] t1Headers = ["Metric / Dimension", "CORPUS-001: Debate (ES)", "CORPUS-002: LBJ Daisy (EN)", "CORPUS-003: Challenger (EN)"];
        double[] t1Cols = [48, 200, 322, 444];
        string[][] t1Rows =
        [
            ["Communicative Regime", "Polyphonic Civic Debate", "Acute Fear-Appeal Spot", "Institutional Consolation"],
            ["Duration & Windows", "194.84 s (39 x 5s)", "59.83 s (12 x 5s)", "258.93 s (26 x 10s)"],
            ["Mean / Peak Energy E", "63.58  (Peak: 76.54)", "50.51  (Peak: 83.29)", "56.45  (Peak: 61.98)"],
            ["Final Stance Entropy H(T)", "2.7574 bits (91.9%)", "2.2292 bits (74.3%)", "2.0240 bits (67.5%)"],
            ["Mean / Peak Inertia I(t)", "0.5019 (Peak: 0.728)", "0.4420 (Peak: 0.799)", "0.7266 (Peak: 0.8921)"],
            ["Scene Cuts (D_JS >= 0.30)", "4 cuts (max 0.490 bits)", "3 cuts (max 0.577 bits)", "1 cut (max 0.042 mid-speech)"],
            ["Final Polarization P(T)", "0.5515 (High Contest)", "0.4200 (Contrastive)", "0.0617 (Unified Consolation)"],
            ["Control Condition kappa(W_c)", "2.738", "1.783", "1.457"]
        ];
        DrawTable(p2, ref y2, "Table 1: Cross-Domain Multimodal Benchmark Telemetry Across Three Narrative Regimes.", t1Headers, t1Cols, t1Rows);

        // Vector Plots Panel on Page 2
        DrawVectorPlotsPdf(p2, ref y2, atlas, control);

        DrawSectionHeader(p2, ref y2, "5. COUNTERFACTUAL CONTROL & 6! = 720 MONTAGE OPTIMIZATION (EXP-003)");
        string[] t2Headers = ["Control Synthesis Objective", "Optimal Block Sequence pi*", "x1 AntiMil", "x2 Legal", "x3 Pragm", "||x(T)||_2 Bias"];
        double[] t2Cols = [48, 188, 368, 416, 464, 512];
        string[][] t2Rows =
        [
            ["Original 1994 Broadcast", "P -> S1 -> S2 -> S3 -> S4 -> S5", "-0.1445", "+0.5524", "+0.6446", "0.8620 (Base)"],
            ["pi*_anti (Max Abolitionism)", "S3 -> S2 -> S5 -> S4 -> P -> S1", "+0.5130", "-0.1717", "-0.0304", "0.7087"],
            ["pi*_legal (Max Legal Comp.)", "P -> S1 -> S4 -> S5 -> S2 -> S3", "-0.2601", "+0.5830", "+0.6705", "0.9387"],
            ["pi*_eq (Min-Bias Equilibrium)", "S4 -> P -> S5 -> S2 -> S1 -> S3", "+0.2262", "+0.0606", "+0.1787", "0.3274 (-62%)"],
            ["pi*_aut (Max Autonomy)", "S3 -> S2 -> S5 -> S1 -> P -> S4", "+0.3958", "-0.0083", "+0.0669", "0.6358"]
        ];
        DrawTable(p2, ref y2, "Table 2: Exhaustive 6! = 720 Montage Optimization Solutions on CORPUS-001 (100% Footage Invariant).", t2Headers, t2Cols, t2Rows);

        WriteParagraph(p2, ref y2,
        [
            $"Key EXP-003 Engineering Findings: (1) Across all 720 permutations of CORPUS-001, editorial order alone flips every belief axis (B1 spans",
            $"[-0.2603, +0.5130], Delta = 0.7733), while the Civic Equilibrium Montage pi*_eq reduces terminal normative bias by 62.0% (0.8620 -> 0.3274).",
            $"(2) In CORPUS-002 (LBJ 'Daisy'), damping the non-verbal nuclear blast (-13.7 dBFS, D_JS = 0.577 bits) while keeping words identical reduces",
            $"mobilization x2(T) by {s2.NonVerbalShockLeveragePercent:F2}% (+0.2624 -> +0.1841). (3) In CORPUS-003 (Reagan 'Challenger'), institutional anchoring provides a +{s3.InstitutionalStabilizationMargin:F4}",
            "inertia stabilization margin (0.8921 vs. 0.4264), shielding national unity from sign reversal (x2(T) = +0.2321 > 0) under late crisis critique."
        ]);

        DrawSectionHeader(p2, ref y2, "6. REFERENCES");
        WriteParagraph(p2, ref y2,
        [
            "[1] Rojas Ruiz, J. I. (2026). Narrative Systems Engineering (NSE) and Computational Narrative Dynamics (CND). Zenodo. DOI: 10.5281/zenodo.23987656.",
            "[2] Shannon, C. E. (1948). A Mathematical Theory of Communication. Bell System Technical Journal, 27(3), 379-423.",
            "[3] Lin, J. (1991). Divergence Measures Based on the Shannon Entropy. IEEE Trans. Information Theory, 37(1), 145-151.",
            "[4] Friedkin, N. E., & Johnsen, E. C. (1990). Social Influence and Opinions. J. Mathematical Sociology, 15(3-4), 193-206."
        ]);

        DrawFooter(p2, 2, 2);
        pages.Add(p2);

        return AssemblePdfDocument(pages);
    }

    private static void DrawSectionHeader(StringBuilder sb, ref double y, string title)
    {
        y -= 4;
        DrawText(sb, "F2", 9.6, 48, y, title);
        y -= 4;
        DrawLine(sb, 48, y, 564, y, 0.6, 0.3, 0.4, 0.55);
        y -= 11;
    }

    private static void WriteParagraph(StringBuilder sb, ref double y, string[] lines)
    {
        foreach (var line in lines)
        {
            DrawText(sb, "F1", 8.6, 48, y, line);
            y -= 10.2;
        }
        y -= 3;
    }

    private static void DrawEquationBox(StringBuilder sb, ref double y, string expr, string num)
    {
        DrawRect(sb, 48, y - 4, 516, 15, 0.96, 0.97, 0.99, 0.82, 0.86, 0.92);
        DrawText(sb, "F4", 7.4, 54, y, expr);
        DrawText(sb, "F2", 8.0, 544, y, num);
        y -= 18;
    }

    private static void DrawTable(StringBuilder sb, ref double y, string caption, string[] headers, double[] cols, string[][] rows)
    {
        DrawText(sb, "F2", 8.3, 48, y, caption);
        y -= 6;
        DrawRect(sb, 48, y - 11, 516, 13, 0.92, 0.95, 0.98, 0.2, 0.3, 0.45);
        for (int c = 0; c < headers.Length; c++)
        {
            DrawText(sb, "F2", 8.0, cols[c] + 3, y - 8, headers[c]);
        }
        y -= 11;
        foreach (var row in rows)
        {
            y -= 11.2;
            DrawLine(sb, 48, y - 2, 564, y - 2, 0.3, 0.80, 0.84, 0.90);
            for (int c = 0; c < row.Length; c++)
            {
                DrawText(sb, c == 0 ? "F2" : "F1", 7.9, cols[c] + 3, y + 1, row[c]);
            }
        }
        DrawLine(sb, 48, y - 2, 564, y - 2, 0.9, 0.1, 0.15, 0.25);
        y -= 14;
    }

    private static void DrawVectorPlotsPdf(StringBuilder sb, ref double y, CrossDomainAtlasReport atlas, CounterfactualControlReport control)
    {
        double boxH = 128;
        double topY = y;
        double botY = y - boxH;

        // Left Plot Box: Normalized Energy E(t/T) [x: 48..292]
        DrawRect(sb, 48, botY, 244, boxH, 0.98, 0.99, 1.0, 0.72, 0.78, 0.86);
        DrawText(sb, "F2", 8.0, 56, topY - 11, "Fig 1A. Cross-Domain Energy Trajectories E(t/T)");

        double p1X = 68, p1W = 214, p1Y = botY + 18, p1H = boxH - 36;
        DrawLine(sb, p1X, p1Y, p1X + p1W, p1Y, 0.5, 0.5, 0.55, 0.65);
        DrawLine(sb, p1X, p1Y, p1X, p1Y + p1H, 0.5, 0.5, 0.55, 0.65);

        (double R, double G, double B)[] cols = [(0.85, 0.47, 0.02), (0.86, 0.15, 0.15), (0.01, 0.52, 0.78)];
        for (int wi = 0; wi < atlas.Works.Count; wi++)
        {
            var orbit = atlas.Works[wi].Orbit;
            var (r, g, b) = cols[wi % cols.Length];
            sb.AppendLine(CultureInfo.InvariantCulture, $"{r:F2} {g:F2} {b:F2} RG 1.5 w");
            for (int i = 0; i < orbit.Count; i++)
            {
                double px = p1X + orbit[i].NormalizedTime * p1W;
                double py = p1Y + (orbit[i].Energy / 100.0) * p1H;
                sb.AppendLine(CultureInfo.InvariantCulture, $"{px:F1} {py:F1} {(i == 0 ? "m" : "l")}");
            }
            sb.AppendLine("S");
        }
        DrawText(sb, "F1", 7.2, 72, botY + 5, "Orange: CORPUS-001 | Red: CORPUS-002 (Peak 83.3) | Blue: CORPUS-003");

        // Right Plot Box: 720 Montage Cloud [x: 312..564]
        DrawRect(sb, 312, botY, 252, boxH, 0.98, 0.99, 1.0, 0.72, 0.78, 0.86);
        DrawText(sb, "F2", 8.0, 320, topY - 11, "Fig 1B. 6! = 720 Montage Reachability Hull (EXP-003)");

        double p2X = 330, p2W = 222, p2Y = botY + 18, p2H = boxH - 36;
        double MapX(double v) => p2X + ((v + 0.35) / 0.95) * p2W;
        double MapY(double v) => p2Y + ((v + 0.25) / 0.95) * p2H;

        DrawLine(sb, MapX(0), p2Y, MapX(0), p2Y + p2H, 0.4, 0.65, 0.70, 0.78);
        DrawLine(sb, p2X, MapY(0), p2X + p2W, MapY(0), 0.4, 0.65, 0.70, 0.78);

        var s1 = control.Study1MontageOptimization;
        foreach (var pt in s1.CloudSamples)
        {
            DrawCircle(sb, MapX(pt.X1AntiMil), MapY(pt.X2Legal), 1.4, 0.55, 0.62, 0.72);
        }
        DrawCircle(sb, MapX(s1.OriginalFinalStateX["B1_ANTIMILITARISM"]), MapY(s1.OriginalFinalStateX["B2_LEGAL_COMPLIANCE"]), 3.2, 0.85, 0.47, 0.02);
        DrawCircle(sb, MapX(s1.OptimalAntiMilitarist.FinalStateX["B1_ANTIMILITARISM"]), MapY(s1.OptimalAntiMilitarist.FinalStateX["B2_LEGAL_COMPLIANCE"]), 3.2, 0.86, 0.15, 0.15);
        DrawCircle(sb, MapX(s1.OptimalCivicEquilibrium.FinalStateX["B1_ANTIMILITARISM"]), MapY(s1.OptimalCivicEquilibrium.FinalStateX["B2_LEGAL_COMPLIANCE"]), 3.5, 0.02, 0.60, 0.40);

        DrawText(sb, "F1", 7.2, 326, botY + 5, "Gray: 720 Montages | Green: Optimal Equilibrium (0.327) | Red: Max B1 (+0.513)");

        y = botY - 14;
    }

    private static void DrawFooter(StringBuilder sb, int pageNum, int totalPages)
    {
        DrawLine(sb, 48, 38, 564, 38, 0.5, 0.6, 0.65, 0.75);
        DrawText(sb, "F1", 8.0, 48, 26, "Rojas Ruiz, J. I. (2026) — Narrative Systems Engineering (NSE) — Apache-2.0 / Mandatory Attribution (NOTICE)");
        DrawText(sb, "F2", 8.0, 515, 26, $"Page {pageNum} of {totalPages}");
    }

    private static void DrawText(StringBuilder sb, string fontRef, double size, double x, double y, string text)
    {
        var escaped = text
            .Replace("\\", "\\\\")
            .Replace("(", "\\(")
            .Replace(")", "\\)");
        sb.AppendLine(CultureInfo.InvariantCulture, $"0.08 0.11 0.18 rg BT /{fontRef} {size:F1} Tf {x:F1} {y:F1} Td ({escaped}) Tj ET");
    }

    private static void DrawLine(StringBuilder sb, double x1, double y1, double x2, double y2, double width, double r, double g, double b)
    {
        sb.AppendLine(CultureInfo.InvariantCulture, $"{r:F2} {g:F2} {b:F2} RG {width:F2} w {x1:F1} {y1:F1} m {x2:F1} {y2:F1} l S");
    }

    private static void DrawRect(StringBuilder sb, double x, double y, double w, double h, double fr, double fg, double fb, double sr, double sg, double sbCol)
    {
        sb.AppendLine(CultureInfo.InvariantCulture, $"{fr:F2} {fg:F2} {fb:F2} rg {sr:F2} {sg:F2} {sbCol:F2} RG 0.6 w {x:F1} {y:F1} {w:F1} {h:F1} re B");
    }

    private static void DrawCircle(StringBuilder sb, double cx, double cy, double r, double red, double green, double blue)
    {
        double k = 0.55228475 * r;
        sb.AppendLine(CultureInfo.InvariantCulture, $"{red:F2} {green:F2} {blue:F2} rg");
        sb.AppendLine(CultureInfo.InvariantCulture, $"{cx + r:F2} {cy:F2} m");
        sb.AppendLine(CultureInfo.InvariantCulture, $"{cx + r:F2} {cy + k:F2} {cx + k:F2} {cy + r:F2} {cx:F2} {cy + r:F2} c");
        sb.AppendLine(CultureInfo.InvariantCulture, $"{cx - k:F2} {cy + r:F2} {cx - r:F2} {cy + k:F2} {cx - r:F2} {cy:F2} c");
        sb.AppendLine(CultureInfo.InvariantCulture, $"{cx - r:F2} {cy - k:F2} {cx - k:F2} {cy - r:F2} {cx:F2} {cy - r:F2} c");
        sb.AppendLine(CultureInfo.InvariantCulture, $"{cx + k:F2} {cy - r:F2} {cx + r:F2} {cy - k:F2} {cx + r:F2} {cy:F2} c f");
    }

    private static byte[] AssemblePdfDocument(List<StringBuilder> pageStreams)
    {
        using var ms = new MemoryStream();
        var offsets = new List<long>();

        void WriteAscii(string s)
        {
            var bytes = Encoding.GetEncoding("ISO-8859-1").GetBytes(s);
            ms.Write(bytes, 0, bytes.Length);
        }

        WriteAscii("%PDF-1.4\n");

        // Object 1: Catalog
        offsets.Add(ms.Position);
        WriteAscii("1 0 obj\n<< /Type /Catalog /Pages 2 0 R >>\nendobj\n");

        // Object 2: Pages
        int firstPageObjId = 7;
        var kids = string.Join(" ", Enumerable.Range(0, pageStreams.Count).Select(i => $"{firstPageObjId + i * 2} 0 R"));
        offsets.Add(ms.Position);
        WriteAscii($"2 0 obj\n<< /Type /Pages /Kids [{kids}] /Count {pageStreams.Count} >>\nendobj\n");

        // Objects 3..6: Standard Type1 Fonts (Times-Roman, Times-Bold, Times-Italic, Courier)
        string[] fontNames = ["Times-Roman", "Times-Bold", "Times-Italic", "Courier"];
        for (int f = 0; f < fontNames.Length; f++)
        {
            offsets.Add(ms.Position);
            WriteAscii($"{3 + f} 0 obj\n<< /Type /Font /Subtype /Type1 /BaseFont /{fontNames[f]} /Encoding /WinAnsiEncoding >>\nendobj\n");
        }

        // Page and Content objects
        for (int i = 0; i < pageStreams.Count; i++)
        {
            int pageObjId = firstPageObjId + i * 2;
            int contentObjId = pageObjId + 1;
            var contentStr = pageStreams[i].ToString();
            var contentBytes = Encoding.GetEncoding("ISO-8859-1").GetBytes(contentStr);

            offsets.Add(ms.Position);
            WriteAscii($"{pageObjId} 0 obj\n<< /Type /Page /Parent 2 0 R /MediaBox [0 0 612 792] /Resources << /Font << /F1 3 0 R /F2 4 0 R /F3 5 0 R /F4 6 0 R >> >> /Contents {contentObjId} 0 R >>\nendobj\n");

            offsets.Add(ms.Position);
            WriteAscii($"{contentObjId} 0 obj\n<< /Length {contentBytes.Length} >>\nstream\n");
            ms.Write(contentBytes, 0, contentBytes.Length);
            WriteAscii("\nendstream\nendobj\n");
        }

        long xrefPos = ms.Position;
        WriteAscii($"xref\n0 {offsets.Count + 1}\n");
        WriteAscii("0000000000 65535 f \n");
        foreach (var off in offsets)
        {
            WriteAscii(off.ToString("D10", CultureInfo.InvariantCulture) + " 00000 n \n");
        }
        WriteAscii($"trailer\n<< /Size {offsets.Count + 1} /Root 1 0 R >>\nstartxref\n{xrefPos}\n%%EOF\n");

        return ms.ToArray();
    }
}
