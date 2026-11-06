# EXP-001 Findings: Multimodal Narrative Dynamics & Control Analysis of `06-FCE-V-C110-007-C.mp4`

**Principal Investigator & Creator:** Jair Ivan Rojas Ruiz ([ORCID: 0009-0003-1109-4583](https://orcid.org/0009-0003-1109-4583))
**Experiment ID:** `EXP-001-insumision-dynamics`
**Date:** November 5, 2026
**Engine:** `Argon.Engine` v0.1 (C# / .NET 10)
**Source Media:** `video/06-FCE-V-C110-007-C.mp4` (194.84 seconds, SHA-256: `cbe05bdfc6131d85...`)

---

> **Status (v0.4.1, November 6, 2026):** Acoustic, visual, and speech-rate values in this report are measured from the media. Stance vectors and institutional anchoring were coded by a single coder with no agreement statistic. The belief state $x(t)$, the Controllability Gramian, and Scenario B are outputs of an exploratory model; no audience was measured. The "4 cuts" count includes Window #0, which the software marks by convention; three transitions were flagged between sampled frames, and no annotated shot-boundary reference exists.

## 1. Executive Summary: Bridging the Gap

Before this experiment, the project had two disconnected halves:
1. A high-level theoretical ambition (Foundational Docs 1–3) proposing metrics like *Narrative DNA*, *Narrative Energy*, *Narrative Entropy*, *Narrative Inertia*, and *Control Theory* ($x(t+1) = f(x(t), u(t))$), which risked remaining uncalibrated LLM roleplay.
2. A low-level `.NET` pipeline (`src/Nse.Pipeline`) that extracted 39 silent frames and 5-bin RGB hex histograms, with `audio.wav` left untranscribed and unanalyzed.

In **EXP-001**, we built and executed `Argon.Engine` to close that gap on the actual 3-minute-15-second Spanish university documentary clip (`06-FCE-V-C110-007-C.mp4`), which captures the 1994–1995 Spanish debate over compulsory military service (*la mili*), conscientious objection (*objeción de conciencia*), and total non-compliance (*insumisión*).

---

## 2. Empirical Discoveries from the Multimodal Signal $u(t)$

### 2.1 Deterministic Scene Cut Detection via Jensen-Shannon Divergence ($D_{\text{JS}}$)
By computing the **Jensen-Shannon Divergence** (in bits, bounded in $[0,1]$) between consecutive 512-bin RGB24 probability distributions in [`VisualDynamicsAnalyzer.cs`](../../src/Argon.Engine/VisualDynamicsAnalyzer.cs), the engine automatically detected the **4 major visual environment cuts** (`JSD >= 0.30 bits`) without any neural model:
- **Window #0 ($0.0\text{–}5.0\text{s}$):** Opening close-up of the activist poster (*"LOS QUEREMOS AL NATURAL / LIBERTAD OBJETORES PRESOS"*).
- **Window #1 ($5.0\text{–}10.0\text{s}$, $D_{\text{JS}} = 0.4078\text{ bits}$):** Hard cut from the bright poster (`Lum = 0.468`) to the dark indoor night lobby (`Lum = 0.270`) for Speaker 1 (`SPK_01`, Metallica t-shirt) and Speaker 2 (`SPK_02`, white/pink collared shirt, where a softer camera pan occurs at Window #9 with $D_{\text{JS}} = 0.1302\text{ bits}$).
- **Window #18 ($90.0\text{–}95.0\text{s}$, $D_{\text{JS}} = 0.4895\text{ bits}$):** Hard cut to the bright wall with the world physical map (`Lum = 0.573`) for Speaker 3 (`SPK_03`) and Speaker 4 (`SPK_04`, camera shift at Window #22 with $D_{\text{JS}} = 0.1681\text{ bits}$).
- **Window #26 ($130.0\text{–}135.0\text{s}$, $D_{\text{JS}} = 0.4803\text{ bits}$):** Hard cut to the busy university corridor (`Lum = 0.361`) for Speaker 5 (`SPK_05`, plaid shirt).

### 2.2 Acoustic Prosody & Cross-Modal Resonance ($\Phi_{\text{sync}}$)
Direct analysis of the 3,117,397 16-bit PCM samples in [`AcousticAnalyzer.cs`](../../src/Argon.Engine/AcousticAnalyzer.cs) revealed distinct vocal profiles across the 5 speakers:

| Speaker / Segment | Time Span | Mean Level (dBFS) | Speech Rate (WPM) | Mean Energy $E$ | Complexity $C$ | Inertia $\mathcal{I}$ | Primary Normative Frame |
|---|---|---|---|---|---|---|---|
| `ENV_POSTER` (Cartel) | $0\text{–}5\text{s}$ | $-26.36\text{ dB}$ | $312.0$ (text) | $52.40$ | $65.00$ | $0.088$ | Abolitionist / Anti-Penal ($+0.90$) |
| `SPK_01` (Estudiante 1) | $5\text{–}45\text{s}$ | **$-16.96\text{ dB}$** | **$203.7\text{ WPM}$** | **$72.90$** | $75.67$ | $0.151$ | Anti-Militarist Abolitionism ($+0.93$) |
| `SPK_02` (Estudiante 2) | $45\text{–}90\text{s}$ | $-19.83\text{ dB}$ | $141.8\text{ WPM}$ | $63.86$ | **$87.90$** | $0.539$ | Pragmatic Social Service ($+0.91$) |
| `SPK_03` (Estudiante 3) | $90\text{–}110\text{s}$ | $-21.34\text{ dB}$ | $201.0\text{ WPM}$ | $66.45$ | $76.15$ | **$0.668$** | Legal/Tax Compliance ($+0.95$) |
| `SPK_04` (Estudiante 4) | $110\text{–}130\text{s}$ | $-18.64\text{ dB}$ | $192.0\text{ WPM}$ | $65.08$ | $59.47$ | $0.536$ | Individual Autonomy + Law ($+0.85$) |
| `SPK_05` (Estudiante 5) | $130\text{–}195\text{s}$ | $-22.21\text{ dB}$ | $185.1\text{ WPM}$ | $57.18$ | $68.39$ | $0.663$ | Anti-Free-Rider Social Utility ($+0.89$) |

**Key Finding 1 (Peak Narrative Energy):**
The highest Narrative Energy in the entire work occurs during `SPK_01` at **Window #7 ($35\text{–}40\text{s}$, $E = 76.54/100$)**, where vocal amplitude peaks at $-15.67\text{ dBFS}$ (`VAD = 0.92`) simultaneously with high-speed ideological delivery ($234.8\text{ WPM}$) denouncing war, military conscription, and US interventionism (*"eliminar el ejército... Estados Unidos el que maneja el cotarro"*).

**Key Finding 2 (Energy vs. Institutional Inertia Asymmetric Trade-off):**
Although `SPK_01` generates the highest **instantaneous Narrative Energy ($72.90$)**, his discourse has the lowest **Institutional Inertia ($0.151$)** because it demands abolishing an entrenched state institution. Conversely, `SPK_03` (*"es por ley y hay cosas que la ley impone... a nadie le gusta pagar impuestos y sin embargo hay que pagarlos"*) speaks 4.4 dB quieter ($-21.34\text{ dBFS}$), yet activates the highest **Narrative Inertia ($0.668$)** by anchoring his claim in statutory obligation and fiscal reciprocity.

---

## 3. Testing Control Theory ($x(t+1) = f(x(t), u(t))$) & Counterfactuals

Using the discrete-time state-space control model in [`NarrativeMetricsEngine.cs`](../../src/Argon.Engine/NarrativeMetricsEngine.cs):
1. **Trajectory Inversion ($t = 45\text{s}$ to $105\text{s}$):**
   During the first 45 seconds (`ENV_POSTER` + `SPK_01`), the collective state $x_1$ (*Anti-Militarist Abolitionism*) climbs steeply from $0.000$ to **$+0.6248$**, while $x_2$ (*Legal Compliance*) drops to **$-0.4558$**.
   However, the documentary's editorial structure places **four consecutive speakers (`SPK_02` through `SPK_05`, spanning 150 seconds)** who challenge total *insumisión* in favor of legal compliance and alternative social service (*prestación social sustitutoria*). By $t = 95\text{s}$ (Window #19), $x_1$ crosses zero into negative territory ($-0.0070$), and by the end of the video ($t = 194.8\text{s}$), the final state settles at:
   - $x_3$ (*Pragmatic Social Service*): **$+0.6446$**
   - $x_2$ (*Legal Compliance*): **$+0.5524$**
   - $x_4$ (*Individual Autonomy*): **$+0.0391$**
   - $x_1$ (*Anti-Militarist Abolitionism*): **$-0.1445$**

2. **Controllability Gramian Spectrum ($W_c$):**
   Computing the diagonal Controllability Gramian over the 39 windows yields a condition number $\kappa(W_c) = 2.738$:
   - **Most Controllable State:** `B3_PRAGMATIC_SERVICE` ($W_{c,33} = 0.02587$) — the pragmatic middle ground ("professional soldiers + useful social service instead of military barracks") is the dimension with the highest control sensitivity in the model.
   - **Least Controllable State (in the model):** `B1_ANTIMILITARISM` ($W_{c,11} = 0.00945$) — early gains are overwritten by subsequent high-inertia legal/civic framing.

3. **Counterfactual Simulation (Scenario B vs. Scenario A):**
   When we reverse the documentary's editing order in **Scenario B** (placing the 4 legal/pragmatic speakers first and ending with `SPK_01` and the activist poster), the final state shifts by $\|\Delta x\|_2 = 1.2249$, flipping $x_1$ from $-0.1445$ to **$+0.4976$**. Within the model, therefore, **editorial sequencing (montage order)** acts as a strong input. This is a property of the model's recency-weighted update, not a measured effect on viewers; testing it requires an audience study.
