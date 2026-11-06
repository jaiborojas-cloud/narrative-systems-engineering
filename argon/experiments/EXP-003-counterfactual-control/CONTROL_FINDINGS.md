# EXP-003: Counterfactual Narrative Control & Montage Optimization Lab

**Principal Investigator & Creator:** Jair Ivan Rojas Ruiz ([ORCID: 0009-0003-1109-4583](https://orcid.org/0009-0003-1109-4583))  
**DOI:** [https://doi.org/10.5281/zenodo.23987656](https://doi.org/10.5281/zenodo.23987656)  
**Date:** November 5, 2026  
**Engine:** `Argon.Engine` v0.3 (`CounterfactualController.cs`)  
**Interactive Control Dashboard:** [`control-dashboard.html`](control-dashboard.html)  
**Structured Telemetry Report:** [`control.report.json`](control.report.json)

---

> **Status (v0.4.1, November 6, 2026):** Every result in this report is a simulation of the exploratory state-space model under modified inputs. No viewers were involved, so the results describe the model's behavior (for example its sensitivity to block order), not effects of montage on audiences. "Bias" here means the norm of the simulated state, a modeling choice. "Key Engineering Discovery" headings below should be read as model observations.

## 1. Objective: From Narrative Observation to Narrative Control Synthesis

Building on the single-work profile of `EXP-001` and the three-work comparison of `EXP-002`, **`EXP-003` tests the core engineering premise of Narrative Systems Engineering (NSE)**: *Can we solve discrete-time optimal control and counterfactual synthesis problems over physical audiovisual signals to steer or balance collective cognitive state trajectories $x(t)$?*

We formulate and solve three counterfactual control studies across our three benchmark works:
1. **Study 1 (`CORPUS-001` — Exhaustive $6! = 720$ Montage Permutation Reachability & Optimal Editorial Synthesis):** Evaluating every possible editorial block ordering of the 6 speaker turns ($39$ windows) to map the exact reachable terminal state hull and synthesize optimal montages.
2. **Study 2 (`CORPUS-002` — Multimodal Shock Impulse Timing & Non-Verbal Attenuation in LBJ's 1964 *"Daisy"* Spot):** Simulating the share of state displacement attributed, within the model, to the non-verbal nuclear detonation impulse ($E = 83.29$, $D_{\text{JS}} = 0.5766\text{ bits}$) versus its verbal transcript.
3. **Study 3 (`CORPUS-003` — Institutional Inertial Shielding & Crisis Perturbation Timing in Reagan's 1986 *Challenger* Address):** Quantifying the stabilization margin provided by institutional anchoring and testing state resilience against early, mid, and late crisis counter-frames.

---

## 2. Study 1 Results: Exhaustive $6! = 720$ Montage Optimization (`CORPUS-001`)

`CORPUS-001` consists of $6$ contiguous editorial blocks (`ENV_POSTER`, `SPK_01`, `SPK_02`, `SPK_03`, `SPK_04`, `SPK_05`) spanning $39$ five-second windows ($194.84\text{s}$). Holding 100% of the physical audio, video frames, and words strictly invariant—so that cumulative Shannon Stance Entropy $H(T) = 2.7574\text{ bits}$ and total energy exposure are identical—we evaluated all $6! = 720$ permutations $\pi \in S_6$ through the LTV state-space equation:
$$x_k(t+1) = \bigl(1 - \gamma_k(t)\bigr)x_k(t) + \gamma_k(t)z_k(t), \qquad \gamma_k(t) = 0.18\,\frac{E(t)}{100}\bigl(1 - 0.65\,\mathcal{I}_k(t)\bigr)$$

### 2.1 Reachable Terminal State Span Purely from Editorial Ordering

| Belief Dimension $k$ | Reachable Minimum $\min_{\pi} x_k(T)$ | Reachable Maximum $\max_{\pi} x_k(T)$ | Editorial Control Span $\Delta x_k(T)$ | Sign Reversal Achievable? |
|---|---|---|---|---|
| $B_1$ (**Anti-Militarist Abolitionism**) | $-0.2603$ | **$+0.5130$** | **$0.7733$** | **Yes** (flips from $-0.26$ to $+0.51$) |
| $B_2$ (**Civic & Legal Compliance**) | $-0.1795$ | **$+0.5830$** | **$0.7625$** | **Yes** (flips from $-0.18$ to $+0.58$) |
| $B_3$ (**Pragmatic Social Service**) | $-0.0403$ | **$+0.7046$** | **$0.7449$** | **Yes** (flips from $-0.04$ to $+0.70$) |
| $B_4$ (**Individual Conscience Autonomy**) | $-0.1618$ | **$+0.4930$** | **$0.6548$** | **Yes** (flips from $-0.16$ to $+0.49$) |

### 2.2 Optimal Montage Synthesis Solutions

| Synthesis Objective | Optimal Block Sequence $\pi^*$ | $x_1(T)$ (Anti-Mil) | $x_2(T)$ (Legal) | $x_3(T)$ (Pragmatic) | $x_4(T)$ (Autonomy) | Bias Norm $\|x(T)\|_2$ | Shift from Original $\|\Delta x\|_2$ |
|---|---|---|---|---|---|---|---|
| **Original Broadcast** | `ENV_POSTER → SPK_01 → SPK_02 → SPK_03 → SPK_04 → SPK_05` | $-0.1445$ | $+0.5524$ | $+0.6446$ | $+0.0391$ | $0.8620$ | $0.0000$ |
| **$\pi^*_{\text{anti}}$ (Max Abolitionism)** | `SPK_03 → SPK_02 → SPK_05 → SPK_04 → ENV_POSTER → SPK_01` | **$+0.5130$** | $-0.1717$ | $-0.0304$ | $+0.4568$ | $0.7087$ | **$1.2597$** |
| **$\pi^*_{\text{legal}}$ (Max Legal Compliance)** | `ENV_POSTER → SPK_01 → SPK_04 → SPK_05 → SPK_02 → SPK_03` | $-0.2601$ | **$+0.5830$** | $+0.6705$ | $-0.1552$ | $0.9387$ | $0.2296$ |
| **$\pi^*_{\text{eq}}$ (Min-Bias Civic Equilibrium)** | `SPK_04 → ENV_POSTER → SPK_05 → SPK_02 → SPK_01 → SPK_03` | $+0.2262$ | $+0.0606$ | $+0.1787$ | $+0.1430$ | **$0.3274$** | $0.7792$ |
| **$\pi^*_{\text{aut}}$ (Max Individual Autonomy)** | `SPK_03 → SPK_02 → SPK_05 → SPK_01 → ENV_POSTER → SPK_04` | $+0.3958$ | $-0.0083$ | $+0.0669$ | **$+0.4930$** | $0.6358$ | $1.0705$ |
| **$\pi^*_{\text{ext}}$ (Max Conviction Extremum)** | `SPK_01 → ENV_POSTER → SPK_04 → SPK_05 → SPK_03 → SPK_02` | $-0.2544$ | $+0.5717$ | $+0.6894$ | $-0.1305$ | **$0.9401$** | $0.2079$ |

**Key Engineering Discovery (Algorithmic Debiasing via Montage):**
In the original 1994 broadcast (`norm = 0.8620`), placing all four pro-service/legal speakers after `SPK_01` creates a strong recency-and-inertia bias toward $B_2$ ($+0.5524$) and $B_3$ ($+0.6446$). By solving $\min_{\pi \in S_6} \|x(T)\|_2$, `Argon.Engine` synthesizes the **Civic Equilibrium Montage** (`SPK_04 → ENV_POSTER → SPK_05 → SPK_02 → SPK_01 → SPK_03`), which reduces terminal normative bias by **$-62.0\%$** ($\|x^*(T)\|_2 = 0.3274$) by interleaving the high-energy abolitionist turn (`SPK_01`, $E=72.9$) immediately before the brief, high-inertia legal anchor (`SPK_03`, $20\text{s}$, $\mathcal{I}=0.7614$).

---

## 3. Study 2 Results: Multimodal Shock Impulse Control (`CORPUS-002`, LBJ *"Daisy"*)

| Intervention Scenario | Peak Energy $E_{\max}$ | Mean Energy $\bar{E}$ | Terminal Inertia $\mathcal{I}(T)$ | $x_1(T)$ (Deterrence) | $x_2(T)$ (Electoral Mobilization) | Shift $\|\Delta x(T)\|_2$ |
|---|---|---|---|---|---|---|
| **Baseline (Actual 1964 Spot, Shock at $t=30\text{s}$)** | **$83.29$** | $50.51$ | $0.7992$ | $+0.4283$ | **$+0.2624$** | $0.0000$ |
| **2A: Cold-Open Shock (Blast at $t=0\text{s}$, Meadow Last)** | $83.29$ | $50.51$ | $0.1341$ | $+0.4009$ | $+0.2447$ | $0.0531$ |
| **2B: Terminal Shock (Meadow $\to$ Pitch $\to$ Blast Last)** | $83.29$ | $50.51$ | $0.6671$ | $+0.4273$ | $+0.2539$ | $0.0171$ |
| **2C: Non-Verbal Shock Attenuation ($-60\%$ Audio/Visual Impulse)** | $72.98$ | $34.74$ | $0.7992$ | $+0.2936$ | **$+0.1841$** | **$0.1912$** |

**Key Engineering Discovery:**
1. **Optimal Mid-Spot Placement:** The actual 1964 structure (Innocence $\to$ Nuclear Shock at $t=30\text{s}$ $\to$ Presidential Mandate) achieves higher terminal electoral mobilization ($x_2(T) = +0.2624$) and $6\times$ higher terminal inertia ($\mathcal{I}(T) = 0.7992$) than a Cold-Open Shock ($\mathcal{I}(T) = 0.1341$, $x_2(T) = +0.2447$).
2. **Quantifying Non-Verbal Shock Leverage ($29.84\%$):** When the transcript and normative stances $z(t)$ are kept $100\%$ identical, but the acoustic roar ($-13.68\text{ dBFS}$) and nuclear flash cut ($D_{\text{JS}} = 0.5766\text{ bits}$) in Windows #4–#8 are attenuated to normal speech levels (`Counterfactual 2C`), terminal electoral mobilization drops from $+0.2624$ to $+0.1841$. Within the model, **$29.84\%$ of the simulated displacement** is attributable to those acoustic and visual inputs; the share depends on the uncalibrated channel weights of $E(t)$ and is not an estimate of the spot's effect on viewers.

---

## 4. Study 3 Results: Institutional Inertial Shielding (`CORPUS-003`, Reagan *Challenger*)

| Perturbation Scenario | Peak Energy $E_{\max}$ | Final Inertia $\mathcal{I}(T)$ | $x_2(T)$ (National Unity) | $x_4(T)$ (Heroic Courage) | State Disruption $\|\Delta x(T)\|_2$ |
|---|---|---|---|---|---|
| **Baseline (Unbroken Oval Office Address)** | $61.98$ | **$0.8921$** | **$+0.6670$** | $+0.7038$ | $0.0000$ |
| **3A: De-Institutionalized Framing ($A_{\text{inst}} \to 0.05$)** | $61.98$ | **$0.4264$** | $+0.8352$ | $+0.8332$ | $0.2780$ |
| **3B: Early Crisis Critique Injection ($t = 30\text{–}60\text{s}$)** | $78.50$ | $0.8575$ | $+0.5013$ | $+0.5832$ | $0.2725$ |
| **3C: Mid-Speech Critique Injection ($t = 120\text{–}150\text{s}$)** | $78.50$ | $0.8262$ | $+0.3836$ | $+0.4568$ | $0.5151$ |
| **3D: Late Speech Critique Injection ($t = 210\text{–}240\text{s}$)** | $78.50$ | $0.7850$ | **$+0.2321$** | $+0.3032$ | **$0.8342$** |

**Key Engineering Discovery:**
1. **Institutional Stabilization Margin ($+0.4657$):** Removing presidential/institutional anchoring (`Perturbation 3A`) collapses terminal Narrative Inertia by **$-0.4657$** (from $0.8921$ down to $0.4264$), leaving the narrative trajectory highly volatile.
2. **Recovery vs. Recency under High-Energy Perturbations:** When a $30\text{-second}$ high-energy adversarial critique ($E = 78.50$, $z_2 = -0.90$) is injected early (`3B`, $t=30\text{–}60\text{s}$), the state briefly dips to $x_2(60\text{s}) = -0.1985$, but the subsequent $200\text{ seconds}$ of steady high-inertia consolation repair the trajectory back to **$+0.5013$** ($75.2\%$ recovery). When the exact same perturbation strikes at the end of the address (`3D`, $t=210\text{–}240\text{s}$), the simulated $x_2(T)$ falls furthest (to $+0.2321$, the largest disruption of the three injection times) but stays positive. Later injections produce larger simulated effects, reflecting the recency of the update rule.
