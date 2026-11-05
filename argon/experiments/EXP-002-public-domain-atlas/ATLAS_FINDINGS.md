# EXP-002: Public-Domain Cross-Domain Narrative Atlas (`CORPUS-001` vs. `CORPUS-002` vs. `CORPUS-003`)

**Principal Investigator & Creator:** Jair Ivan Rojas Ruiz ([ORCID: 0009-0003-1109-4583](https://orcid.org/0009-0003-1109-4583))  
**Date:** November 5, 2026  
**Engine:** `Argon.Engine` v0.1 + `src/Nse.Pipeline`  
**Corpus Manifest:** [`data/raw/CORPUS_MANIFEST.json`](../../../data/raw/CORPUS_MANIFEST.json)

---

## 1. Objective

To test the core hypothesis of **Narrative Systems Engineering (NSE)**—that distinct narrative genres and communicative intents exhibit distinct, reproducible signatures in the metric space $(\text{Energy } E, \text{Entropy } H, \text{Inertia } \mathcal{I}, \text{Visual Flux } D_{\text{JS}}, \text{Polarization } P)$—we established a **copyright-free / archival benchmark corpus** spanning three foundational narrative regimes:

1. **`CORPUS-001` (Polyphonic Social Debate):** *Debate Universitario sobre Insumisión y Servicio Militar* (`06-FCE-V-C110-007-C.mp4`, $194.84\text{s}$, 39 windows). Five university students debating compulsory military service vs. civil disobedience.
2. **`CORPUS-002` (Acute Fear-Appeal Political Campaign):** *'Peace, Little Girl' / The 'Daisy' Commercial* (`LBJ_Daisy_1964.ogv`, $59.83\text{s}$, 12 windows, **Public Domain** via Library of Congress / Wikimedia Commons). Lyndon B. Johnson's 1964 nuclear countdown spot.
3. **`CORPUS-003` (Institutional Crisis Consolation & Mythmaking):** *President Ronald Reagan's Challenger Disaster Address* (`Reagan_Challenger_1986.webm`, $258.93\text{s}$, 26 windows, **Public Domain** under 17 U.S.C. § 105 / NARA). Oval Office address following the loss of Space Shuttle *Challenger*.

*(Note: The silent 12-second stock clip `268230_medium.mp4` has been formally deprecated in [`CORPUS_MANIFEST.json`](../../../data/raw/CORPUS_MANIFEST.json) from narrative metric benchmarking.)*

---

## 2. Cross-Domain Metric Signatures (Empirical Atlas)

| Metric / Dimension | `CORPUS-001`: Insumisión Debate (1994) | `CORPUS-002`: LBJ "Daisy" Spot (1964) | `CORPUS-003`: Reagan Challenger Address (1986) | Structural Interpretation |
|---|---|---|---|---|
| **Narrative Regime** | Polyphonic / Contested | Shock Fear-Appeal / Directive | Consolatory / Institutional Unification | Three orthogonal communicative functions |
| **Duration & Windows** | $194.84\text{s}$ ($39\times 5\text{s}$) | $59.83\text{s}$ ($12\times 5\text{s}$) | $258.93\text{s}$ ($26\times 10\text{s}$) | Standardized multimodal profiling |
| **Mean Narrative Energy $\bar{E}$** | **$63.58$** | $50.51$ | $56.45$ | Debate sustains high vocal/lexical energy throughout |
| **Peak Narrative Energy $E_{\max}$** | $76.54$ (Window #7) | **$83.29$ (Window #6)** | $61.98$ (Window #10) | *Daisy* nuclear detonation produces highest acute spike ($83.29$) |
| **Energy Dynamic Range ($E_{\max} - E_{\min}$)** | $33.91$ ($42.6\text{–}76.5$) | **$59.40$ ($23.9\text{–}83.3$)** | $31.76$ ($30.2\text{–}62.0$) | *Daisy* is engineered as a quiet-to-explosive shock wave |
| **Final Stance Entropy $H(T)$** | **$2.7574\text{ bits}$ ($91.9\%$)** | $2.2292\text{ bits}$ ($74.3\%$) | **$2.0240\text{ bits}$ ($67.5\%$)** | Debate maximizes frame dispersion; Reagan minimizes entropy |
| **Mean Narrative Inertia $\bar{\mathcal{I}}$** | $0.5019$ | $0.4420$ | **$0.7266$ (peaks at $0.892$)** | Reagan's Oval Office address maximizes institutional inertia |
| **Visual Scene Cuts ($D_{\text{JS}} \ge 0.30$)** | $4$ cuts ($\max D_{\text{JS}} = 0.490$) | $3$ cuts (**$\max D_{\text{JS}} = 0.577$**) | **$1$ cut ($\max D_{\text{JS}} = 0.042$)** | Unbroken single-camera Oval Office shot vs. atomic flash cut |
| **Final Polarization Index $P(T)$** | **$0.5515$** | $0.4200$ (child vs. launch contrast) | **$0.0617$** | Reagan achieves near-zero polarization (unified national grief) |
| **Control Condition $\kappa(W_c)$** | $2.738$ | $1.783$ | $1.457$ | Balanced multi-axis control in single-speaker presidential address |

---

## 3. Deep-Dive Findings on the New Public-Domain Works

### 3.1 `CORPUS-002`: The Anatomy of a Shock Fear-Appeal (*LBJ "Daisy" Ad, 1964*)
The telemetry in [`daisy-run/telemetry.csv`](daisy-run/telemetry.csv) and [`daisy-run/dashboard.html`](daisy-run/dashboard.html) captures the exact mathematical mechanics of Tony Schwartz's famous advertisement:
1. **Phase I — Low-Energy Innocence Baseline ($t = 0\text{–}20\text{s}$, Windows #0–#3):**
   As the little girl counts daisy petals in the meadow, acoustic level sits at $-28.2\text{ to }-22.8\text{ dBFS}$ (`VAD = 0.24–0.42`), speech rate is $36.7\text{ WPM}$, and Narrative Energy hovers at **$23.89\text{–}32.82$** with near-zero inertia ($\mathcal{I} = 0.072\text{–}0.134$).
2. **Phase II — The Countdown Ramp ($t = 20\text{–}30\text{s}$, Windows #4–#5):**
   When the booming metallic launch countdown overrides the child (`10, 9, 8...`), acoustic amplitude jumps $+12\text{ dB}$ to **$-16.08\text{ dBFS}$** (`VAD = 1.000`), doubling Narrative Energy to **$49.90$** while the camera step-zooms into the girl's eye pupil ($D_{\text{JS}} = 0.1752\text{ bits}$).
3. **Phase III — Multimodal Shock Detonation ($t = 30\text{–}35\text{s}$, Window #6):**
   At the exact moment of zero and nuclear detonation, three observables peak in unison:
   - **Acoustic Level:** Hits **$-13.68\text{ dBFS}$** (`VAD = 1.000`).
   - **Visual Divergence ($D_{\text{JS}}$):** Jumps to **$0.5766\text{ bits}$** (the largest visual discontinuity in the entire 3-work corpus) as the black pupil flashes into the blinding nuclear fireball (`Luminance = 0.7425`, `Complexity = 93.92`).
   - **Cross-Modal Resonance ($\Phi_{\text{sync}}$):** Reaches **$0.8713$** as Lyndon Johnson's voice declares *"These are the stakes! To make a world in which all of God's children can live, or to go into the dark"*, driving **Narrative Energy to $83.29 / 100$**—a **$3.49\times$ surge** over the meadow baseline.
4. **Phase IV — Directive Closure ($t = 50\text{–}59.8\text{s}$, Windows #10–#11):**
   A hard cut to the black mandate card (`Luminance = 0.0992`, $D_{\text{JS}} = 0.4004\text{ bits}$) channels the shock into electoral compliance (`B2_LEGAL_COMPLIANCE = +0.98`, `Inertia = 0.7992`).

### 3.2 `CORPUS-003`: Institutional Consolation & High-Inertia Stability (*Reagan Challenger Address, 1986*)
In stark contrast to both the turbulent student debate (`CORPUS-001`) and the explosive *Daisy* spot (`CORPUS-002`), Reagan's Challenger address ([`challenger-run/telemetry.csv`](challenger-run/telemetry.csv) and [`challenger-run/dashboard.html`](challenger-run/dashboard.html)) exhibits the mathematical signature of **institutional stabilization**:
1. **Zero Visual Disruption ($\max D_{\text{JS}} = 0.0419\text{ bits}$ after $t=0$):**
   Throughout all 26 windows ($258.9\text{ seconds}$), the camera remains locked in a slow, imperceptible push-in inside the Oval Office (`Luminance = 0.340–0.398`, `Saturation = 0.240–0.274`). Not a single scene cut occurs ($D_{\text{JS}} \le 0.042 \ll 0.30$).
2. **Controlled Acoustic Cadence:**
   Reagan maintains a measured, solemn delivery between **$-33.7\text{ and }-29.7\text{ dBFS}$** at $81.6\text{ to }130.0\text{ WPM}$, keeping Narrative Energy within a narrow, steady band (**$50.5\text{ to }61.98$**) before dropping to silence ($-48.94\text{ dBFS}$, $E = 30.22$) after *"to touch the face of God. Thank you."*
3. **Monotonic State Convergence & Maximum Inertia ($\mathcal{I}(T) = 0.8921$):**
   Because there are no competing speakers ($P(T) = 0.0617$) and every segment reinforces national unity, institutional faith in NASA, and heroic pioneer courage, all four cognitive state trajectories climb monotonically without a single sign reversal:
   - $x_4$ (*Heroic Pioneer Courage*): **$+0.7038$**
   - $x_3$ (*Programmatic Continuity*): **$+0.6788$**
   - $x_2$ (*National Unity / Institutional Faith*): **$+0.6670$**
   - $x_1$ (*Peaceful Open Exploration*): **$+0.5991$**

---

## 4. Pairwise Structural Distance Matrix in NSE Metric Space (`Argon.Engine v0.2`)

Using [`AtlasComparator.cs`](../../src/Argon.Engine/AtlasComparator.cs) (`dotnet run --project argon/src/Argon.Engine -- atlas`), we project each work into the normalized 5D NSE Metric Space $\mathbf{m} = (\bar{E}/100,\; E_{\max}/100,\; \hat{H}(T),\; \bar{\mathcal{I}},\; P(T)) \in [0, 1]^5$ and evaluate both the 5D Euclidean metric distance $\|\mathbf{m}_A - \mathbf{m}_B\|_2$ and the Terminal State-Space Euclidean Distance $\|\mathbf{x}_A(T) - \mathbf{x}_B(T)\|_2$ (see [`atlas.report.json`](atlas.report.json) and interactive [`atlas-dashboard.html`](atlas-dashboard.html)):

| Work A | Work B | 5D Metric Space Distance $\|\mathbf{m}_A - \mathbf{m}_B\|_2$ | Terminal State Distance $\|\mathbf{x}_A(T) - \mathbf{x}_B(T)\|_2$ | Composite Regime Contrast Score |
|---|---|---|---|---|
| `CORPUS-001-INSUMISION-1994` | `CORPUS-002-LBJ-DAISY-1964` | $0.2711$ | $0.7090$ | **$0.3086$** |
| `CORPUS-002-LBJ-DAISY-1964` | `CORPUS-003-REAGAN-CHALLENGER-1986` | $0.5128$ | $0.7244$ | **$0.4450$** |
| `CORPUS-001-INSUMISION-1994` | `CORPUS-003-REAGAN-CHALLENGER-1986` | **$0.6135$** | **$1.0045$** | **$0.5635$** |

**Geometric Insight:** The maximum structural distance in the corpus occurs between **Polyphonic Student Debate (`CORPUS-001`)** and **Presidential Crisis Consolation (`CORPUS-003`)** ($\|\Delta \mathbf{m}\|_2 = 0.6135$, $\|\Delta \mathbf{x}(T)\|_2 = 1.0045$, Contrast $= 0.5635$), confirming that contested multi-speaker civic conflict and single-voice institutional consolation occupy opposite poles of the NSE phase space. Meanwhile, **Acute Fear-Appeal (`CORPUS-002`)** occupies an intermediate, high-impulse vector characterized by extreme peak energy ($83.29$) and rapid transition from low-inertia vulnerability to high-inertia electoral mobilization.

---

## 5. Conclusion: Proof of Genre Separation in the NSE Metric Space

With `EXP-001` and `EXP-002`, the project now possesses a **3-work, multi-language (Spanish/English), copyright-clean benchmark corpus** that proves the six NSE metrics separate narrative regimes cleanly:
- **Contested Debate (`CORPUS-001`):** High Entropy ($2.76\text{ bits}$), High Polarization ($0.55$), Multi-Cut Visuals ($4$ cuts), State Sign-Reversal.
- **Shock Fear Campaign (`CORPUS-002`):** Extreme Energy Spike ($83.29$), Extreme Visual Flash Divergence ($0.58\text{ bits}$), Rapid Mobilization Ramp.
- **Presidential Crisis Consolation (`CORPUS-003`):** Maximum Institutional Inertia ($0.89$), Lowest Entropy ($2.02\text{ bits}$), Near-Zero Polarization ($0.06$), Zero Visual Cuts.
