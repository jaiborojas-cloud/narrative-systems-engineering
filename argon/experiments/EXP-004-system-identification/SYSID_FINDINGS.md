# EXP-004: Inverse System Identification, Parameter Calibration & Cross-Domain Generalization

**Author & Principal Investigator:** Jair Ivan Rojas Ruiz (*Jair Rojas*)  
**ORCID:** [https://orcid.org/0009-0003-1109-4583](https://orcid.org/0009-0003-1109-4583)  
**DOI (Zenodo / CERN):** [https://doi.org/10.5281/zenodo.23987656](https://doi.org/10.5281/zenodo.23987656)  
**Engine Module:** [`argon/src/Argon.Engine/SystemIdentifier.cs`](../../src/Argon.Engine/SystemIdentifier.cs) (`Argon.Engine v0.4`)

---

## 1. Mathematical Formulation of the Inverse Problem

In **Narrative Systems Engineering (NSE)** and **Computational Narrative Dynamics (CND)**, the forward state-space law updates the $K=4$ collective belief state $x_k(t)$ via:

$$\Delta x_k(t) = x_k(t+1) - x_k(t) = \gamma_k(t)\,\bigl(z_k(t) - x_k(t)\bigr)$$

Expanding the time-varying susceptance gain $\gamma_k(t) = \beta_0\,\frac{E(t;\alpha)}{100}\,(1 - 0.65\,I_k(t))$ into its four multimodal signal channels yields a bilinear system that is **strictly linear in the structural parameter vector** $\Theta = \beta_0\,(\alpha_{\text{ac}}, \alpha_{\text{vis}}, \alpha_{\text{ling}}, \alpha_{\text{sync}})^\top \in \mathbb{R}^4$:

$$\Delta x_k(t) = \sum_{m=1}^{4} \Theta_m\,\psi_{k,m}(t) + \varepsilon_k(t), \quad \psi_{k,m}(t) = \phi_m(t)\,\bigl(1 - 0.65\,I_k(t)\bigr)\,\bigl(z_k(t) - x_k(t)\bigr)$$

where $\phi(t) = \bigl(I_{\text{ac}}(t), \psi_{\text{vis}}(t), I_{\text{ling}}(t), \Phi_{\text{sync}}(t)\bigr)$. Stacking all $N = 77 \times 4 = 308$ state-transition observations across `CORPUS-001`, `CORPUS-002`, and `CORPUS-003` into the design matrix $\Psi \in \mathbb{R}^{308 \times 4}$, the **Tikhonov-Regularized (Ridge) System Identification Estimator** is solved in closed form via:

$$\hat{\Theta}(\lambda) = \left(\Psi^\top \Psi + \lambda I_4\right)^{-1} \left(\Psi^\top \Delta \tilde{X} + \lambda \Theta_0\right)$$

Decomposing $\hat{\Theta}$ under the simplex constraint $\sum_{m=1}^4 \hat{\alpha}_m = 1$ uniquely recovers both the base state susceptance gain $\hat{\beta}_0 = \sum_{m=1}^4 \hat{\Theta}_m$ and the normalized multimodal channel weights $\hat{\alpha}_m = \hat{\Theta}_m / \hat{\beta}_0$.

---

## 2. Empirical Results Summary (`sysid.report.json`)

### Study 1: Structural Parameter Recovery ($N = 308$ Transitions)

| Parameter | Physical Channel Role | Ground Truth $\theta^*$ | Noiseless Inverse $\hat{\theta}$ | Noisy Telemetry (25 dB SNR) |
| :--- | :--- | :---: | :---: | :---: |
| $\alpha_{\text{ac}}$ | Acoustic Prosody ($L_{\text{dB}}$, VAD) | `0.3400` | `0.3400` | `0.3853` |
| $\alpha_{\text{vis}}$ | Visual Flux ($D_{\text{JS}}$, Contrast) | `0.2200` | `0.2200` | `0.2256` |
| $\alpha_{\text{ling}}$ | Linguistic Cadence (WPM, Normative) | `0.2900` | `0.2900` | `0.2924` |
| $\alpha_{\text{sync}}$ | Cross-Modal Resonance $\sqrt{I_{\text{ac}} I_{\text{ling}}}$ | `0.1500` | `0.1500` | `0.0967` |
| $\beta_0$ | Base State Susceptance Gain | `0.1800` | `0.1800` | `0.1794` |
| **Trajectory Fit $R^2$** | Full Horizon Rollout Accuracy | `1.0000` | `1.0000` | **`0.9999` (RMSE `0.0016`)** |

---

### Study 2: Monte Carlo SNR Robustness Curve ($50$ Trials per SNR Level)

| Telemetry SNR (dB) | Noise Std $\sigma_\eta$ | OLS Error (%) | Tikhonov Ridge Error (%) | Rollout Trajectory $R^2$ |
| :---: | :---: | :---: | :---: | :---: |
| **40 dB** | `0.000388` | `2.789%` | **`2.479%`** ($\pm 1.488\%$) | `1.0000` |
| **30 dB** | `0.001226` | `8.017%` | **`7.166%`** ($\pm 4.966\%$) | `1.0000` |
| **25 dB** | `0.002179` | `15.711%` | **`14.068%`** ($\pm 9.206\%$) | `0.9999` |
| **20 dB** | `0.003876` | `27.227%` | **`24.430%`** ($\pm 16.498\%$) | `0.9993` |
| **15 dB** | `0.006892` | `43.005%` | **`38.919%`** ($\pm 22.245\%$) | `0.9887` |
| **10 dB** | `0.012255` | `64.815%` | **`58.631%`** ($\pm 30.270\%$) | `0.9629` |

---

### Study 3: Leave-One-Corpus-Out (LOCO) Cross-Domain Generalization

| Held-Out Test Corpus | Training Corpora | Train / Test Obs | Out-of-Sample $R^2_{\text{OOS}}$ | Out-of-Sample RMSE | Terminal $\|\Delta x(T)\|_2$ |
| :--- | :--- | :---: | :---: | :---: | :---: |
| `CORPUS-001-INSUMISION-1994` (ES Debate) | `CORPUS-002` + `CORPUS-003` (EN) | `152 / 156` | **`0.99999`** | `0.00079` | `0.00132` |
| `CORPUS-002-LBJ-DAISY-1964` (EN Fear Spot) | `CORPUS-001` + `CORPUS-003` | `260 / 48` | **`0.99974`** | `0.00180` | `0.00409` |
| `CORPUS-003-REAGAN-CHALLENGER-1986` (EN Crisis) | `CORPUS-001` + `CORPUS-002` | `204 / 104` | **`0.99996`** | `0.00105` | `0.00222` |
