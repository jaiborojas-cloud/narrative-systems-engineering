# Multimodal Narrative Systems Engineering: Signal-Grounded Metrics and State-Space Control Dynamics for Audiovisual Media

**Author:** Jair Ivan Rojas Ruiz (*Jair Rojas*)  
**ORCID iD:** [https://orcid.org/0009-0003-1109-4583](https://orcid.org/0009-0003-1109-4583)  
**DOI:** [https://doi.org/10.5281/zenodo.23987656](https://doi.org/10.5281/zenodo.23987656) (Concept DOI: [`10.5281/zenodo.23987655`](https://doi.org/10.5281/zenodo.23987655))  
**Date:** November 5, 2026  
**Version:** Preprint v1.0 (`PAPER-001`)  
**Repository & Reproducibility Package:** [https://github.com/jaiborojas-cloud/narrative-systems-engineering](https://github.com/jaiborojas-cloud/narrative-systems-engineering) (`src/Nse.Pipeline` & `argon/src/Argon.Engine`)

---

## Abstract

Narratives across film, television, political discourse, and digital media are typically analyzed either through qualitative humanities frameworks or through isolated text-only natural language processing. Conversely, recent attempts to prompt large language models (LLMs) to score "narrative impact" or "emotional intensity" on arbitrary $0\text{–}100$ scales suffer from uncalibrated units, stochastic hallucination, and complete detachment from the physical audiovisual signal. In this paper, we introduce **Narrative Systems Engineering (NSE)** and its computational core, **Computational Narrative Dynamics (CND)**, a signal-grounded mathematical framework originated by Jair Ivan Rojas Ruiz that models audiovisual narratives as multimodal time-series inputs $u(t) = (u_{\text{ac}}(t), u_{\text{vis}}(t), u_{\text{ling}}(t))$ acting upon a bounded cognitive-social state space $x(t+1) = f(x(t), u(t))$. We formally define and operationalize six core narrative quantities: (1) **Narrative DNA** as a signed, time-anchored entity-belief tripartite graph; (2) **Narrative Entropy ($H$)** in Shannon bits over chromatic and normative-stance distributions; (3) **Narrative Energy ($E$)** via a non-collapsing cross-modal resonance functional coupling vocal prosody (dBFS, VAD), visual flux (Jensen-Shannon divergence across color distributions), and lexical-normative cadence; (4) **Narrative Complexity ($C$)** combining lexical diversity, dialectical connector density, and multi-value tension; (5) **Narrative Inertia ($\mathcal{I}$)** quantifying institutional and prior-conviction resistance to state displacement; and (6) **Narrative Controllability** via the discrete-time Controllability Gramian $W_c$ over finite horizons. We validate the framework empirically across a three-work, bilingual (Spanish/English) benchmark corpus (`EXP-001` and `EXP-002`, totaling $513.6\text{ seconds}$ and $77$ time windows), demonstrating that the six NSE metrics cleanly separate polyphonic social debate (`CORPUS-001`), acute fear-appeal political campaigning (`CORPUS-002`, LBJ's 1964 *"Daisy"* spot), and institutional crisis consolation (`CORPUS-003`, President Reagan's 1986 *Challenger* address).

**Keywords:** Narrative Systems Engineering, Computational Narrative Dynamics, Multimodal Signal Processing, Narrative Energy, Shannon Narrative Entropy, Narrative Inertia, State-Space Control, Audiovisual Forensics.

---

## 1. Introduction

Human societies are structured, stabilized, and transformed by the transmission of symbolic narratives across audiovisual and linguistic media. However, bridging the gap between raw media signals (pixels, acoustic waveforms, lexical tokens) and higher-order cognitive and social dynamics remains an open scientific and engineering problem.

Existing computational approaches divide into two disconnected paradigms:
1. **Low-level media descriptors:** Computer vision and audio processing pipelines extract shot boundaries, color histograms, or automatic speech transcripts without modeling how these observables couple to form persuasive or symbolic structures.
2. **Uncalibrated generative summaries:** Prompting LLMs to output "narrative energy" or "belief shift" without formal measurement operators produces plausible-sounding numerical fiction. For instance, defining Narrative Energy as a naive product $E = \text{Intensity} \times \text{Coherence} \times \text{Repetition} \times \text{Identification}$ fails dimensional analysis and collapses to zero whenever any single factor vanishes (e.g., a high-impact singular twist with zero repetition).

To establish **Narrative Systems Engineering (NSE)** as a falsifiable, reproducible discipline, every proposed metric must satisfy four strict criteria:
1. **Signal Grounding:** Direct derivation from verifiable physical observables (16-bit PCM audio samples, decoded RGB rasters, and time-aligned speech tokens) or explicitly audited stance annotations.
2. **Stated Units and Bounds:** Information-theoretic quantities expressed in bits (Shannon entropy, Jensen-Shannon divergence), acoustic levels in decibels full scale ($\text{dBFS}$), and state vectors in bounded metric spaces $[-1, 1]^D$.
3. **Layer Separation:** Strict separation between *(i) observable material in the work*, *(ii) structured normative/symbolic interpretation*, and *(iii) downstream cognitive/social state response*.
4. **Executable Reproducibility:** Full implementation in deterministic, open-source software verified by automated invariant tests.

---

## 2. Mathematical Formulation of Multimodal Signal $u(t)$

Let an audiovisual work of duration $T$ seconds be partitioned into $N$ discrete time windows $t \in \{0, 1, \dots, N-1\}$ of duration $\Delta t$ (with $\Delta t = 5.0\text{ s}$ in our baseline profile). At each window $t$, the work emits a multimodal signal vector:
$$u(t) = \bigl(u_{\text{ac}}(t),\; u_{\text{vis}}(t),\; u_{\text{ling}}(t)\bigr)$$

### 2.1 Acoustic Prosody Sub-Vector $u_{\text{ac}}(t)$
Given the downmixed mono $16\text{ kHz}$ PCM16 waveform containing $M$ normalized samples $s_1, \dots, s_M \in [-1, 1]$ within window $t$:
$$\text{RMS}(t) = \sqrt{\frac{1}{M}\sum_{i=1}^{M} s_i^2}, \qquad L_{\text{dB}}(t) = 20 \log_{10}\!\bigl(\max(\text{RMS}(t), 10^{-5})\bigr)$$
Partitioning window $t$ into $100\text{ ms}$ sub-frames yields the **Speech Activity Ratio** $\text{VAD}(t) \in [0, 1]$ (fraction of sub-frames exceeding $-31\text{ dBFS}$) and the intra-window **Dynamic Range** $\Delta L_{90-10}(t) = P_{90}(L_{\text{dB}}) - P_{10}(L_{\text{dB}})$. The normalized acoustic intensity $I_{\text{ac}}(t) \in [0, 1]$ is:
$$I_{\text{ac}}(t) = \text{clip}\!\left(0.55\,\frac{L_{\text{dB}}(t) - L_{\min}}{L_{\max} - L_{\min}} + 0.30\,\text{VAD}(t) + 0.15\,\frac{\Delta L_{90-10}(t)}{18.0},\; 0,\; 1\right)$$

### 2.2 Visual Dynamics Sub-Vector $u_{\text{vis}}(t)$
For each sampled frame resized to a $64\times 64$ area-interpolated RGB24 raster ($P = 4096$ pixels), we compute ITU-R BT.709 perceptual luminance $Y_p = 0.2126 R_p + 0.7152 G_p + 0.0722 B_p$, spatial contrast $\sigma_Y(t)$, HSV chromatic saturation $S(t)$, and the 512-bin quantized color probability distribution $P_t(b)$.

To measure visual pacing and camera transitions deterministically, we compute the **Jensen-Shannon Divergence** (in bits, bounded in $[0, 1]$) between consecutive frames:
$$D_{\text{JS}}(P_t \parallel P_{t-1}) = \frac{1}{2} D_{\text{KL}}(P_t \parallel M) + \frac{1}{2} D_{\text{KL}}(P_{t-1} \parallel M), \qquad M = \frac{1}{2}(P_t + P_{t-1})$$
where $D_{\text{KL}}(P \parallel M) = \sum_{b=1}^{512} P(b) \log_2 \frac{P(b)}{M(b)}$. A hard scene cut or major camera shift is detected whenever $D_{\text{JS}}(t) \ge \tau_{\text{cut}} = 0.30\text{ bits}$. Visual salience $\psi_{\text{vis}}(t) \in [0, 1]$ combines shot flux, spatial contrast, and chromatic entropy $H_{\text{vis}}(t) = -\sum_b P_t(b)\log_2 P_t(b)$:
$$\psi_{\text{vis}}(t) = \text{clip}\!\left(0.40\,D_{\text{JS}}(t) + 0.35\,\frac{\sigma_Y(t)}{0.32} + 0.25\,\frac{H_{\text{vis}}(t)}{6.5},\; 0,\; 1\right)$$

### 2.3 Linguistic & Normative Sub-Vector $u_{\text{ling}}(t)$
From the time-aligned speech segments overlapping window $t$, we extract speech cadence $\text{WPM}(t)$ (words per minute), lexical Type-Token Ratio $\text{TTR}(t)$, normative/value marker density $D_{\text{norm}}(t)$, dialectical/adversative connector density $D_{\text{dial}}(t)$ (*pero, sino, aunque, porque, sin embargo*), institutional anchoring $A_{\text{inst}}(t) \in [0, 1]$, and the active stance vector $z(t) \in [-1, 1]^K$ across $K$ belief dimensions.

---

## 3. Formal Definition of the Six Core NSE Metrics

### 3.1 Narrative DNA ($\mathcal{G}_{\text{DNA}}$)
The Narrative DNA of a work is defined as a signed, evidence-linked tripartite graph $\mathcal{G}_{\text{DNA}} = (\mathcal{S} \cup \mathcal{A} \cup \mathcal{B}, \mathcal{E}, W)$ where $\mathcal{S}$ is the set of speaker agents, $\mathcal{A}$ is the set of invoked symbolic archetypes (*hero, villain, victim, savior, threat*), $\mathcal{B} = \{B_1, \dots, B_K\}$ is the set of core belief propositions, and $W: \mathcal{E} \to [-1, +1]$ assigns signed weights backed by verbatim timestamped quotes.

### 3.2 Narrative Energy $E(t) \in [0, 100]$
To eliminate zero-collapse while capturing multimodal synergy, Narrative Energy is formulated as a linear-geometric coupling functional:
$$\Phi_{\text{sync}}(t) = \sqrt{I_{\text{ac}}(t) \cdot I_{\text{ling}}(t)}$$
$$E(t) = 100 \cdot \text{clip}\!\Bigl(0.34\,I_{\text{ac}}(t) + 0.22\,\psi_{\text{vis}}(t) + 0.29\,I_{\text{ling}}(t) + 0.15\,\Phi_{\text{sync}}(t),\; 0,\; 1\Bigr)$$
Here $\Phi_{\text{sync}}(t)$ acts as a **Cross-Modal Resonance** detector that rewards windows where vocal prosodic force and dense normative argumentation co-occur.

### 3.3 Narrative Entropy $H_{\text{narr}}(t)$ (in Shannon Bits)
Across $K$ belief dimensions, each possessing a positive endorsement pole ($+$) and a negative rejection pole ($-$), let $m_{k,\pm}(t)$ denote the cumulative energy-weighted stance exposure mass up to window $t$, and $p_{k,\pm}(t) = m_{k,\pm}(t) / \sum_{j,s} m_{j,s}(t)$. The Shannon Narrative Stance Entropy is:
$$H_{\text{narr}}(t) = -\sum_{k=1}^{K} \sum_{s \in \{+,-\}} p_{k,s}(t) \log_2 p_{k,s}(t) \in [0, \log_2(2K)]\text{ bits}$$
For $K=4$ belief dimensions ($2K = 8$ poles), $H_{\text{max}} = \log_2(8) = 3.00\text{ bits}$. Normalized entropy $\hat{H}_{\text{narr}}(t) = H_{\text{narr}}(t)/3.00 \in [0, 1]$ distinguishes **monolithic/dogmatic narratives** ($\hat{H}_{\text{narr}} \to 0$) from **polyphonic/contested narratives** ($\hat{H}_{\text{narr}} \to 1$).

### 3.4 Narrative Complexity $C(t) \in [0, 100]$
Defined as the weighted combination of lexical diversity, syntactic/dialectical subordination, and multi-value stance tension (number of simultaneously active belief axes $|z_k(t)| \ge 0.20$):
$$C(t) = 100 \cdot \text{clip}\!\left(0.40\,\frac{\text{TTR}(t)}{0.78} + 0.35\,\frac{D_{\text{dial}}(t)}{0.11} + 0.25\,\frac{\max(0, |\{k : |z_k(t)| \ge 0.20\}| - 1)}{3.0},\; 0,\; 1\right)$$

### 3.5 Narrative Inertia $\mathcal{I}(t) \in [0, 1]$
Inertia measures resistance to cognitive state reorientation, combining institutional/statutory anchoring $A_{\text{inst}}(t)$, current collective conviction magnitude $\|x(t)\|_1 / K$, and cumulative temporal exposure $t/N$:
$$\mathcal{I}(t) = \text{clip}\!\left(0.55\,A_{\text{inst}}(t) + 0.25\,\frac{1}{K}\sum_{k=1}^{K} |x_k(t)| + 0.20\,\frac{t+1}{N},\; 0,\; 1\right)$$

### 3.6 State-Space Control Dynamics and Controllability Gramian $W_c$
We model the evolution of the collective cognitive state vector $x(t) \in [-1, 1]^K$ as a discrete-time Linear Time-Varying (LTV) system driven by stance input $z(t)$:
$$x_k(t+1) = \bigl(1 - \gamma_k(t)\bigr) x_k(t) + \gamma_k(t) z_k(t)$$
where the susceptibility gain $\gamma_k(t)$ is proportional to Narrative Energy $E(t)$ and damped by dimension-specific Inertia $\mathcal{I}_k(t)$:
$$\gamma_k(t) = \eta \cdot \frac{E(t)}{100} \cdot \bigl(1 - 0.65\,\mathcal{I}_k(t)\bigr), \qquad \eta = 0.18$$
Over a finite horizon $[0, N]$, let $\Phi_k(N, t+1) = \prod_{\tau=t+1}^{N-1} (1 - \gamma_k(\tau))$ denote the state transition operator and $\tilde{b}_k(t) = \gamma_k(t)\bigl(0.35 + 0.65|z_k(t)|\bigr)$ the effective input coupling. The diagonal **Controllability Gramian** is:
$$W_{c,kk}(0, N) = \sum_{t=0}^{N-1} \Bigl(\Phi_k(N, t+1)\,\tilde{b}_k(t)\Bigr)^2$$
The spectral condition number $\kappa(W_c) = \lambda_{\max}(W_c)/\lambda_{\min}(W_c)$ quantifies control anisotropy across belief dimensions.

---

## 4. Empirical Validation: Experiment `EXP-001`

### 4.1 Stimulus and Multimodal Processing
We evaluated the framework on `06-FCE-V-C110-007-C.mp4` ($194.84\text{ s}$, $720\times 576$, SHA-256 `cbe05bdfc6131d85...`), an archival Spanish university documentary recording campus interviews during the 1994–1995 conflict over compulsory military conscription (*la mili*), conscientious objection (*objeción de conciencia*), and civil disobedience (*insumisión*).

The work was discretized into $N = 39$ five-second windows. `Nse.Pipeline` and `Argon.Engine` extracted $3,117,397$ PCM16 audio samples and $39$ RGB24 rasters ($64\times 64$), evaluating all metrics over $K = 4$ belief dimensions:
- $B_1$: **Anti-Militarist Abolitionism** (abolishing the army and rejecting war/US interventionism).
- $B_2$: **Civic & Legal Obligation** (obeying statutory law and tax-like civic reciprocity).
- $B_3$: **Pragmatic Social Utility** (replacing conscription with a professional army + mandatory social service).
- $B_4$: **Individual Conscience & Autonomy** (acting according to personal conviction).

### 4.2 Empirical Results Across Speakers

| Turn ID | Speaker / Visual Anchor | Span (s) | Acoustic (dBFS) | Cadence (WPM) | Mean Energy $E$ | Complexity $C$ | Inertia $\mathcal{I}$ | Stance $(B_1, B_2, B_3, B_4)$ |
|---|---|---|---|---|---|---|---|---|
| `ENV_POSTER` | Activist Poster Close-up | $0\text{–}5$ | $-26.36$ | $312.0^*$ | $52.40$ | $65.00$ | $0.088$ | $(+0.90, -0.85, -0.20, +0.80)$ |
| `SPK_01` | Student 1 (Metallica shirt) | $5\text{–}45$ | **$-16.96$** | **$203.7$** | **$72.90$** | $75.67$ | $0.151$ | $(+0.93, -0.66, -0.38, +0.67)$ |
| `SPK_02` | Student 2 (White/pink shirt) | $45\text{–}90$ | $-19.83$ | $141.8$ | $63.86$ | **$87.90$** | $0.539$ | $(-0.48, +0.77, +0.91, -0.32)$ |
| `SPK_03` | Student 3 (World map wall) | $90\text{–}110$ | $-21.34$ | $201.0$ | $66.45$ | $76.15$ | **$0.668$** | $(-0.55, +0.95, +0.70, -0.60)$ |
| `SPK_04` | Student 4 (Navy NY shirt) | $110\text{–}130$ | $-18.64$ | $192.0$ | $65.08$ | $59.47$ | $0.536$ | $(+0.15, +0.60, +0.20, +0.85)$ |
| `SPK_05` | Student 5 (Plaid shirt, hall) | $130\text{–}194.8$ | $-22.21$ | $185.1$ | $57.18$ | $68.39$ | $0.663$ | $(-0.25, +0.72, +0.89, -0.04)$ |

*\*Note: `ENV_POSTER` WPM reflects on-screen printed text density during the 5-second visual epigraph.*

### 4.3 Key Scientific Findings

1. **Deterministic Shot Boundary Recovery:**
   Jensen-Shannon Divergence $D_{\text{JS}}(P_t \parallel P_{t-1}) \ge 0.30\text{ bits}$ recovered all four major visual setting transitions without false positives: Window #0 (poster), Window #1 ($D_{\text{JS}} = 0.4078\text{ bits}$, cut to dark night lobby), Window #18 ($D_{\text{JS}} = 0.4895\text{ bits}$, cut to bright world-map wall), and Window #26 ($D_{\text{JS}} = 0.4803\text{ bits}$, cut to university corridor).
2. **Asymmetry Between Narrative Energy and Institutional Inertia:**
   `SPK_01` achieved the highest instantaneous Narrative Energy in the corpus ($E = 76.54$ at Window #7, $35\text{–}40\text{s}$; mean $E = 72.90$), driven by loud vocal projection ($-15.67\text{ dBFS}$) and rapid ideological delivery ($234.8\text{ WPM}$). However, because `SPK_01` advocated abolishing an entrenched state institution (*"eliminar el ejército"*), his turn exhibited the lowest Narrative Inertia ($\mathcal{I} = 0.151$). Conversely, `SPK_03` spoke $4.4\text{ dB}$ softer ($-21.34\text{ dBFS}$) but achieved the highest Narrative Inertia ($\mathcal{I} = 0.668$) by anchoring military service in statutory law and tax compliance (*"hay cosas que la ley impone... como pagar impuestos"*).
3. **Montage Order as a State-Space Control Input:**
   In the actual documentary sequence (**Scenario A**), the early abolitionist surge ($x_1(45\text{s}) = +0.6248$) is progressively overwritten by $150\text{ seconds}$ of consecutive legal/pragmatic speakers (`SPK_02`–`SPK_05`), yielding a final state dominated by Pragmatic Social Service ($x_3(T) = +0.6446$) and Legal Compliance ($x_2(T) = +0.5524$), while Anti-Militarist Abolitionism ends negative ($x_1(T) = -0.1445$).
   When we simulate **Scenario B (Reversed Montage Order)**—placing the four legal/pragmatic speakers first and ending with `SPK_01` and the activist poster—the terminal state shifts by Euclidean norm $\|\Delta x(T)\|_2 = 1.2249$, flipping $x_1(T)$ to $+0.4976$ and $x_2(T)$ to $-0.2120$.
4. **Controllability Gramian Spectrum:**
   The diagonal Controllability Gramian over $[0, 194.84\text{s}]$ yielded eigenvalues $\lambda \in [0.00945, 0.02587]$ with condition number $\kappa(W_c) = 2.738$, identifying $B_3$ (*Pragmatic Social Service*, $W_{c,33} = 0.02587$) as the most controllable consensus attractor and $B_1$ (*Abolitionism*, $W_{c,11} = 0.00945$) as the most damped state under the actual montage.

---

## 5. Cross-Domain Empirical Atlas: Experiment `EXP-002`

To verify that the six NSE metrics generalize across languages and communicative regimes without recalibration, we established a copyright-clean Public Domain / archival benchmark corpus ([`data/raw/CORPUS_MANIFEST.json`](../data/raw/CORPUS_MANIFEST.json)) adding two historic U.S. broadcasts to `CORPUS-001`:
- **`CORPUS-002` (*Acute Fear-Appeal Campaign*):** Lyndon B. Johnson's 1964 presidential campaign advertisement *"Peace, Little Girl" ("Daisy")* (`LBJ_Daisy_1964.ogv`, $59.83\text{s}$, $N=12$ five-second windows, SHA-256 `e09dbeb6ec9e5db4...`, Public Domain via Library of Congress / NARA).
- **`CORPUS-003` (*Institutional Crisis Consolation*):** President Ronald Reagan's Oval Office address following the Space Shuttle *Challenger* disaster on January 28, 1986 (`Reagan_Challenger_1986.webm`, $258.93\text{s}$, $N=26$ ten-second windows, SHA-256 `aedfaa61cddcf915...`, Public Domain under 17 U.S.C. § 105).

### 5.1 Cross-Domain Metric Comparison

| Metric / Dimension | `CORPUS-001`: Insumisión Debate (1994, ES) | `CORPUS-002`: LBJ "Daisy" Spot (1964, EN) | `CORPUS-003`: Reagan Challenger Address (1986, EN) |
|---|---|---|---|
| **Communicative Regime** | Polyphonic / Contested Debate | Shock Fear-Appeal / Directive | Institutional Consolation / Unity |
| **Duration & Windows** | $194.84\text{s}$ ($39\times 5\text{s}$) | $59.83\text{s}$ ($12\times 5\text{s}$) | $258.93\text{s}$ ($26\times 10\text{s}$) |
| **Mean Narrative Energy $\bar{E}$** | **$63.58$** | $50.51$ | $56.45$ |
| **Peak Narrative Energy $E_{\max}$** | $76.54$ (Window #7) | **$83.29$ (Window #6)** | $61.98$ (Window #10) |
| **Energy Dynamic Range ($E_{\max}-E_{\min}$)** | $33.91$ ($42.6\text{–}76.5$) | **$59.40$ ($23.9\text{–}83.3$)** | $31.76$ ($30.2\text{–}62.0$) |
| **Final Stance Entropy $H(T)$** | **$2.7574\text{ bits}$ ($91.9\%$)** | $2.2292\text{ bits}$ ($74.3\%$) | **$2.0240\text{ bits}$ ($67.5\%$)** |
| **Mean Narrative Inertia $\bar{\mathcal{I}}$** | $0.5019$ | $0.4420$ | **$0.7266$ (peaks at $0.8921$)** |
| **Visual Scene Cuts ($D_{\text{JS}} \ge 0.30$)** | $4$ cuts ($\max D_{\text{JS}} = 0.490$) | $3$ cuts (**$\max D_{\text{JS}} = 0.577$**) | **$1$ cut ($\max D_{\text{JS}} = 0.042$)** |
| **Final Polarization Index $P(T)$** | **$0.5515$** | $0.4200$ | **$0.0617$** |
| **Control Condition $\kappa(W_c)$** | $2.738$ | $1.783$ | $1.457$ |

### 5.2 Genre Separation in the NSE Metric Space
1. **Shock Fear-Appeal Mechanics (`CORPUS-002`):** In the *"Daisy"* spot, Windows #0–#3 (child counting petals in a meadow) establish a low-energy baseline ($E \in [23.89, 32.82]$, $-28.2\text{ dBFS}$). At Window #4 ($20\text{–}25\text{s}$), the echoing missile countdown raises acoustic amplitude by $+12\text{ dB}$ ($-16.08\text{ dBFS}$) and doubles Narrative Energy to $49.90$. At Window #6 ($30\text{–}35\text{s}$, nuclear detonation), visual divergence spikes to **$D_{\text{JS}} = 0.5766\text{ bits}$** (transition from dark eye pupil to blinding atomic flash), acoustic level hits **$-13.68\text{ dBFS}$**, cross-modal resonance reaches $\Phi_{\text{sync}} = 0.8713$, and **Narrative Energy surges to $83.29 / 100$** ($3.49\times$ over the baseline) before channeling into high-inertia electoral mobilization ($\mathcal{I} = 0.7992$).
2. **Institutional Consolation Mechanics (`CORPUS-003`):** Reagan's *Challenger* address exhibits zero mid-speech visual cuts ($\max D_{\text{JS}} = 0.0419\text{ bits}$), narrow prosodic variance ($-33.7\text{ to }-29.7\text{ dBFS}$), lowest stance entropy ($2.0240\text{ bits}$), near-zero polarization ($P(T) = 0.0617$), and the **highest Narrative Inertia in the corpus ($\mathcal{I}(T) = 0.8921$)**, driving monotonic state convergence across all four belief dimensions.
3. **Pairwise Metric-Space Separation (`Argon Atlas v0.2`):** Projecting all three works into the normalized 5D NSE Metric Space $\mathbf{m} = (\bar{E}/100, E_{\max}/100, \hat{H}(T), \bar{\mathcal{I}}, P(T)) \in [0,1]^5$ yields non-degenerate pairwise Euclidean distances: $\|\mathbf{m}_{\text{C1}} - \mathbf{m}_{\text{C2}}\|_2 = 0.2711$ (Debate vs. Fear-Appeal), $\|\mathbf{m}_{\text{C2}} - \mathbf{m}_{\text{C3}}\|_2 = 0.5128$ (Fear-Appeal vs. Consolation), and $\|\mathbf{m}_{\text{C1}} - \mathbf{m}_{\text{C3}}\|_2 = 0.6135$ with terminal state separation $\|\mathbf{x}_{\text{C1}}(T) - \mathbf{x}_{\text{C3}}(T)\|_2 = 1.0045$ (Debate vs. Consolation).

---

## 6. Counterfactual Narrative Control & Montage Synthesis: Experiment `EXP-003`

To advance **Narrative Systems Engineering** from passive observation to active systems synthesis, we implemented a discrete-time optimal control and counterfactual intervention module ([`CounterfactualController.cs`](../argon/src/Argon.Engine/CounterfactualController.cs), `Argon.Engine v0.3`) and executed three control studies ([`argon/experiments/EXP-003-counterfactual-control/`](../argon/experiments/EXP-003-counterfactual-control/CONTROL_FINDINGS.md)):

### 6.1 Exhaustive $6! = 720$ Montage Permutation Reachability (`CORPUS-001`)
Holding $100\%$ of the physical audio samples, RGB rasters, and words invariant across the $6$ speaker blocks ($39$ windows) of `CORPUS-001`, we evaluated all $6! = 720$ block permutations $\pi \in S_6$ through the LTV state equation:
- **Reachable Terminal State Hull:** Purely by reordering existing speaker blocks, every belief dimension spans a sign-reversing interval: $x_1(T) \in [-0.2603, +0.5130]$ ($\Delta x_1 = 0.7733$), $x_2(T) \in [-0.1795, +0.5830]$ ($\Delta x_2 = 0.7625$), $x_3(T) \in [-0.0403, +0.7046]$ ($\Delta x_3 = 0.7449$), and $x_4(T) \in [-0.1618, +0.4930]$ ($\Delta x_4 = 0.6548$).
- **Algorithmic Debiasing ($\min_{\pi \in S_6} \|x(T)\|_2$):** While the original 1994 broadcast order yields a strongly biased terminal state norm $\|x_{\text{orig}}(T)\|_2 = 0.8620$, the synthesized **Civic Equilibrium Montage** $\pi^*_{\text{eq}} = (\text{SPK\_04} \to \text{ENV\_POSTER} \to \text{SPK\_05} \to \text{SPK\_02} \to \text{SPK\_01} \to \text{SPK\_03})$ reduces terminal normative bias by **$-62.0\%$** ($\|x^*(T)\|_2 = 0.3274$) without deleting a single frame of footage.

### 6.2 Non-Verbal Shock Impulse Leverage (`CORPUS-002`, LBJ *"Daisy"*)
When the verbal transcript and stance vectors $z(t)$ of *"Daisy"* are held identical, but the non-verbal nuclear detonation impulse in Windows #4–#8 ($E_{\max} = 83.29$, $-13.68\text{ dBFS}$, $D_{\text{JS}} = 0.5766\text{ bits}$) is attenuated to calm speech levels, terminal electoral mobilization $x_2(T)$ drops from $+0.2624$ to $+0.1841$—demonstrating that **$29.84\%$ of the commercial's cognitive state displacement is driven purely by the non-verbal acoustic/visual shock wave**.

### 6.3 Institutional Inertial Shielding (`CORPUS-003`, Reagan *Challenger*)
Removing statutory/presidential institutional anchoring in `CORPUS-003` collapses terminal Narrative Inertia by **$-0.4657$** (from $0.8921$ to $0.4264$). Furthermore, injecting a $30\text{-second}$ high-energy crisis critique ($E = 78.50$, $z_2 = -0.90$) at $t=30\text{s}$ vs. $t=120\text{s}$ vs. $t=210\text{s}$ demonstrates that accumulated institutional inertia prevents terminal sign reversal ($x_2(T) = +0.2321 > 0$) even under late-stage perturbation.

---

## 7. Limitations and Research Roadmap

1. **Corpus Expansion Across Fiction and Long-Form Cinema:** While `EXP-001`, `EXP-002`, and `EXP-003` establish observability, three-regime invariance, and counterfactual control across $513.6\text{ seconds}$ ($77$ windows) of Spanish and English civic/historical broadcasts, future expansions will evaluate full-length feature films and multi-episode serial narratives.
2. **Stance Extraction Provenance:** While acoustic ($u_{\text{ac}}$) and visual ($u_{\text{vis}}$) vectors are computed directly from raw media bytes, the normative stance vectors $z(t)$ in `EXP-001` and `EXP-002` rely on structured human-audited transcript coding. Future work will benchmark automated LLM stance extractors against multi-annotator Krippendorff's $\alpha \ge 0.80$.
3. **Empirical Calibration of Control Gains:** The state-space susceptibility parameters ($\eta = 0.18$, inertia damping $= 0.65$) represent a preregistered theoretical baseline. Phase 4 of the roadmap (`EXP-004`) will fit $(\eta, w_{\text{ac}}, w_{\text{vis}}, w_{\text{ling}})$ against independent human audience response data (second-by-second retention and pre/post belief-shift surveys).

---

## 8. Conclusion

By replacing metaphorical scores with signal-grounded operators—Shannon entropy in bits, Jensen-Shannon visual divergence, PCM acoustic prosody, cross-modal resonance, and LTV state-space control—**Narrative Systems Engineering** transitions from philosophical speculation to an executable engineering discipline.

---

## References

1. Shannon, C. E. (1948). A Mathematical Theory of Communication. *Bell System Technical Journal*, 27(3), 379–423.
2. Lin, J. (1991). Divergence measures based on the Shannon entropy. *IEEE Transactions on Information Theory*, 37(1), 145–151.
3. Friedkin, N. E., & Johnsen, E. C. (1990). Social influence and opinions. *Journal of Mathematical Sociology*, 15(3-4), 193–206.
4. Vicol, P., Tapaswi, M., Castrejon, L., & Fidler, S. (2018). MovieGraphs: Towards Understanding Human-Centric Situations from Videos. *CVPR 2018*, 8581–8590.
5. Green, M. C., & Brock, T. C. (2000). The role of transportation in the persuasiveness of public narratives. *Journal of Personality and Social Psychology*, 79(5), 701–721.
6. Jones, M. D., & McBeth, M. K. (2010). A Narrative Policy Framework: Clear Enough to Be Wrong? *Policy Studies Journal*, 38(2), 329–353.
7. Shiller, R. J. (2017). Narrative Economics. *American Economic Review*, 107(4), 967–1004.
