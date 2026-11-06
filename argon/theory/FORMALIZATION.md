# Mathematical Formalization of Narrative Systems Engineering (Argon v0.1)

**Original Concept & Metrics Author:** Jair Ivan Rojas Ruiz ([ORCID: 0009-0003-1109-4583](https://orcid.org/0009-0003-1109-4583))
**Technical Operationalization:** Argon Experimental Laboratory (under direction of Jair Ivan Rojas Ruiz)
**Date:** November 5, 2026
**Target:** Operationalize the 6 core metrics originated by Jair Ivan Rojas Ruiz in Foundational Documents 1–3 (`ADN Narrativo`, `Energía Narrativa`, `Entropía Narrativa`, `Inercia Narrativa`, `Complejidad Narrativa`, `Controlabilidad Narrativa`) so they are computable from real audiovisual signals without arbitrary LLM hallucination.

---

> **Status (v0.4.1, November 6, 2026):** This document specifies working definitions. The weights and normalization constants are author choices and are not calibrated; the stance inputs are single-coder labels; the state-space model is an exploratory hypothesis that has not been compared with measured audience responses. See the "What is and is not established" table in the root `README.md`.

## 1. Multimodal Signal Vector $u(t)$

Let an audiovisual work of duration $T$ seconds be discretized into uniform time windows $t \in \{1, 2, \dots, N\}$ of width $\Delta t$ (e.g., $\Delta t = 5.0\text{ s}$, matching the frame sampling of `Nse.Pipeline`).

At each window $t$, the observable input signal $u(t)$ is a tuple of three grounded sub-vectors:
$$u(t) = \bigl(u_{\text{ac}}(t),\; u_{\text{vis}}(t),\; u_{\text{ling}}(t)\bigr)$$

### 1.1 Acoustic Vector $u_{\text{ac}}(t)$ (from 16 kHz PCM16 Mono Audio)
For window $t$ containing $M$ raw 16-bit signed audio samples $s_1, \dots, s_M$ normalized to $[-1, 1]$:
- **Root Mean Square Energy:**
  $$\text{RMS}(t) = \sqrt{\frac{1}{M}\sum_{i=1}^{M} s_i^2}$$
- **Acoustic Level (dBFS):**
  $$L_{\text{dB}}(t) = 20 \log_{10}\!\bigl(\max(\text{RMS}(t), 10^{-5})\bigr) \in [-100, 0]$$
- **Normalized Acoustic Intensity:**
  $$I_{\text{ac}}(t) = \text{clip}\!\left(\frac{L_{\text{dB}}(t) - L_{\min}}{L_{\max} - L_{\min}}, 0, 1\right)$$
- **Zero-Crossing Rate (ZCR):** Fraction of adjacent samples changing sign (distinguishesvoiced speech vs. fricatives/ambient noise).
- **Speech Activity Ratio $V(t) \in [0, 1]$:** Fraction of 100 ms sub-frames within window $t$ whose RMS exceeds the noise floor threshold $\theta_{\text{vad}}$.

### 1.2 Visual Dynamics Vector $u_{\text{vis}}(t)$ (from $64\times 64$ RGB24 Rasters)
Instead of uncalibrated RGB hex strings alone, each sampled frame $t$ is mapped to perceptual descriptors and temporal flux:
- **Mean Perceptual Luminance (ITU-R BT.709):**
  $$Y(t) = \frac{1}{P}\sum_{p=1}^{P} \bigl(0.2126 R_p + 0.7152 G_p + 0.0722 B_p\bigr) \in [0, 1]$$
- **Mean Chromatic Saturation (HSV):**
  $$S(t) = \frac{1}{P}\sum_{p=1}^{P} \frac{\max(R_p,G_p,B_p) - \min(R_p,G_p,B_p)}{\max(R_p,G_p,B_p) + \epsilon} \in [0, 1]$$
- **Visual Flux / Shot Transition Divergence $D_{\text{JS}}(t) \in [0, 1]$:**
  Given the normalized 512-bin color probability distribution $P_t$ of frame $t$ and $P_{t-1}$ of frame $t-1$, we compute the **Jensen-Shannon Divergence** (in bits, bounded in $[0, 1]$):
  $$D_{\text{JS}}(P_t \parallel P_{t-1}) = \frac{1}{2} D_{\text{KL}}(P_t \parallel M) + \frac{1}{2} D_{\text{KL}}(P_{t-1} \parallel M), \quad M = \frac{1}{2}(P_t + P_{t-1})$$
  A spike in $D_{\text{JS}}(t) > \tau_{\text{cut}}$ deterministically detects a **scene/shot cut** or major camera movement.

### 1.3 Linguistic & Symbolic Vector $u_{\text{ling}}(t)$ (from Time-Aligned Transcript)
For speech segments overlapping window $t$:
- **Speech Rate $R_w(t)$:** Words per minute (WPM) in window $t$.
- **Lexical Content Density $D_{\text{lex}}(t)$:** Ratio of content words (nouns, verbs, adjectives, arguments) to total tokens.
- **Normative/Evaluative Marker Density $M_{\text{eval}}(t)$:** Density of explicit value/obligation/conflict markers (e.g., *obligación, ley, guerra, rechazar, libertad, impuesto, país, ejército, conciencia*).
- **Stance Vector $z(t) \in \Delta^K$:** Distribution over explicit normative claims active in window $t$.

---

## 2. Re-Specifying the 6 Core NSE Metrics

### Metric 1: Narrative DNA (`ADN Narrativo`)
Instead of a vague metaphor, the **Narrative DNA** of a work or speaker $s$ is a formal **Typed Belief-Entity Bipartite Graph** $\mathcal{G}_s = (\mathcal{E}, \mathcal{B}, \mathcal{W})$:
- $\mathcal{E}$: Set of actors/speakers and institutional entities (*Ejército, Estado/Ley, Objetores, Insumisos, EE.UU., Sociedad*).
- $\mathcal{B}$: Set of core normative propositions/beliefs ($b_1, \dots, b_K$).
- $\mathcal{W} \in [-1, +1]^{|\mathcal{E}| \times |\mathcal{B}|}$: Signed, evidence-weighted adjacency matrix where $W_{e,b} > 0$ denotes endorsement/reinforcement, $W_{e,b} < 0$ denotes rejection/attack, and $W_{e,b} = 0$ denotes absence.

### Metric 2: Narrative Entropy (`Entropía Narrativa`) — Unit: **Bits**
Foundational Document 2 defines Narrative Entropy as the openness or multiplicity of interpretations. We formalize this via **Shannon Entropy** at two levels:
1. **Intra-Window Chromatic/Visual Entropy $H_{\text{vis}}(t)$:**
   $$H_{\text{vis}}(t) = -\sum_{b=1}^{512} P_t(b) \log_2 P_t(b) \in [0, 9]\text{ bits}$$
2. **Normative/Interpretive Stance Entropy $H_{\text{narr}}(t)$:**
   Across the cumulative or windowed distribution of normative stances $p_k(t)$ over $K$ competing belief frames:
   $$H_{\text{narr}}(t) = -\sum_{k=1}^{K} p_k(t) \log_2 p_k(t) \in [0, \log_2 K]\text{ bits}$$
   Normalized to $[0, 1]$ via $\hat{H}_{\text{narr}}(t) = H_{\text{narr}}(t) / \log_2 K$.
   - When $\hat{H}_{\text{narr}} \to 0$: **Monolithic/Dogmatic Narrative** (single uncontested frame).
   - When $\hat{H}_{\text{narr}} \to 1$: **Polyphonic/Contested Narrative** (high competing frame dispersion).

### Metric 3: Narrative Energy (`Energía Narrativa`) — Scale: $[0, 100]$
Foundational Document 2 proposed $E = \text{Intensity} \times \text{Coherence} \times \text{Repetition} \times \text{Identification}$, which suffers from zero-collapse (if any factor is 0, energy is 0) and uncalibrated units.

We replace the raw product with a **Bounded Multimodal Coupling Functional**:
$$E(t) = 100 \cdot \sigma_{\text{norm}}\!\Bigl( w_{\text{ac}} I_{\text{ac}}(t) + w_{\text{vis}} \psi_{\text{vis}}(t) + w_{\text{ling}} I_{\text{ling}}(t) + \lambda_{\text{sync}} \Phi_{\text{sync}}(t) \Bigr)$$
where:
- $I_{\text{ac}}(t) \in [0,1]$: Acoustic intensity (RMS level $\times$ speech activity).
- $\psi_{\text{vis}}(t) \in [0,1]$: Visual salience (combination of chromatic contrast and shot transition flux).
- $I_{\text{ling}}(t) \in [0,1]$: Linguistic intensity (normalized speech rate + normative/value claim density + thematic recurrence).
- $\Phi_{\text{sync}}(t) = \sqrt{I_{\text{ac}}(t) \cdot I_{\text{ling}}(t)}$: **Cross-modal resonance term** measuring when vocal energy and semantic argument density peak simultaneously.

### Metric 4: Narrative Complexity (`Índice de Complejidad Narrativa`) — Scale: $[0, 100]$
Measured as a composite of three orthogonal, observable structural dimensions:
1. **Syntactic/Clause Complexity $C_{\text{syn}}$:** Mean words per clause and subordinating/adversative connector rate (*pero, porque, sino, aunque, si*).
2. **Conceptual Diversity $C_{\text{lex}}$:** Moving-window Type-Token Ratio (MATTR) over content lemmas.
3. **Dialectical Tension $C_{\text{dial}}$:** Co-presence of competing values within the same speaker turn (e.g., acknowledging individual freedom *while* invoking legal obligation).

### Metric 5: Narrative Inertia (`Inercia Narrativa`) — Unit: **Dimensionless Resistance $[0, 1]$**
Inertia is a property of the **belief system state $x(t)$**, enhanced by institutional/traditional anchoring in the signal. For a belief dimension $k$:
$$\mathcal{I}_k(t) = \alpha_{\text{inst}} A_{\text{inst}}(k, t) + \alpha_{\text{rep}} R(k, t) + \alpha_{\text{prior}} |x_k(t)|$$
where $A_{\text{inst}}(k,t)$ measures institutional/legal anchoring (appeals to law, state coercion, taxes, tradition), $R(k,t)$ measures cumulative repetition across speakers, and $|x_k(t)|$ is current conviction magnitude. High inertia means an opposing narrative input $u(t)$ produces minimal state displacement $\|\Delta x(t)\|$.

### Metric 6: State-Space Control Model (`Controlabilidad Narrativa`)
We formalize Phase 8 ($x(t+1) = f(x(t), u(t))$) as a discrete-time **Linear Time-Varying (LTV) Bounded DeGroot-Friedkin Opinion/Cognitive State System** over $D=4$ core collective state variables:
- $x_1(t) \in [-1, 1]$: **Anti-Militarist / Abolitionist Stance** (rejection of war & army vs. defense of military institution).
- $x_2(t) \in [-1, 1]$: **Civic Duty / Legal Compliance** (obligation to obey law/state vs. civil disobedience/insubmission).
- $x_3(t) \in [-1, 1]$: **Pragmatic Reformism** (preference for professional army + mandatory social service vs. status quo).
- $x_4(t) \in [0, 1]$: **Social Polarization Index** (variance across active agent stances in the population).

The state update equation at each window $t$ is:
$$x(t+1) = \bigl(I - \Gamma(t)\bigr) x(t) + \Gamma(t) B u_{\text{stance}}(t)$$
where:
- $\Gamma(t) = \text{diag}\bigl(\gamma_1(t), \dots, \gamma_D(t)\bigr)$ is the **Susceptibility Gain Matrix**, inversely proportional to Narrative Inertia $\mathcal{I}(t)$ and scaled by Narrative Energy $E(t)/100$:
  $$\gamma_d(t) = \eta \cdot \frac{E(t)}{100} \cdot \bigl(1 - 0.7\,\mathcal{I}_d(t)\bigr)$$
- $B u_{\text{stance}}(t)$ is the directed narrative push exerted by the speaker active in window $t$.

**Controllability Gramian over Horizon $[0, T]$:**
For local sensitivity around trajectory $x(t)$ with state transition $\tilde{A}(t) = I - \Gamma(t)$ and input matrix $\tilde{B}(t) = \Gamma(t) B$:
$$W_c(0, T) = \sum_{t=0}^{T-1} \Phi(T, t+1) \tilde{B}(t) \tilde{B}(t)^\top \Phi(T, t+1)^\top$$
The minimum eigenvalue $\lambda_{\min}(W_c)$ and condition number $\kappa(W_c) = \lambda_{\max}/\lambda_{\min}$ quantify **which cognitive dimensions are controllable** by the video's narrative sequence and which are locked by inertia.
