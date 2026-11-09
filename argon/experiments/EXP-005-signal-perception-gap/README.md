# EXP-005: Signal–Perception Gap (Lip-Sync and Playback Detection)

**Status:** Design, preregistration protocol, machine detector calibration on synthetic stimuli, and blinded human observer form completed. **No human participant study has been executed.**  
**Protocol Specification:** [`research/protocols/EXP-005-PROTOCOL.md`](../../../research/protocols/EXP-005-PROTOCOL.md)  
**Blinded Observer Form:** [`research/protocols/exp005-observer-form.html`](../../../research/protocols/exp005-observer-form.html)  
**Engine Implementation:** [`argon/src/Argon.Engine/LipSyncSignalDetector.cs`](../../src/Argon.Engine/LipSyncSignalDetector.cs)

---

## 1. Synthetic Ground-Truth Calibration Results (`calibration.report.json`)

Before testing any human hypotheses or recording live takes, `LipSyncSignalDetector.cs` was evaluated blindly on 9 synthetic ground-truth stimulus pairs (`60 fps` `32x32` RGB24 mouth-ROI frame sequences with pixel sensor noise and $3.5\text{ ms}$ articulatory jitter + `16 kHz` mono PCM16 WAV files with room reflection and ambient noise floor):

| Stimulus ID | Condition | True Lag $\Delta t_{\text{true}}$ (ms) | XCorr Lag $\hat{\tau}_{\text{xcorr}}$ (ms) | Bilabial Onset Lag $\hat{\tau}_{\text{onset}}$ (ms) | Consensus Lag $\hat{\tau}_{\text{lag}}$ (ms) | Error $e_\tau$ (ms) | Master Corr $r_{\text{rep}}$ | IOI Drift $\sigma_{\text{IOI}}$ (ms) | Predicted Mode |
| :--- | :--- | :---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: |
| `STIM-SYNTH-PLAYBACK-NEG120MS` | Playback | `-120.0` | `-124.5` | `-125.0` | `-124.70` | `-4.70` | `0.9951` | `0.00` | `playback` |
| `STIM-SYNTH-PLAYBACK-NEG060MS` | Playback | `-60.0` | `-64.6` | `-65.0` | `-64.72` | `-4.72` | `0.9950` | `0.00` | `playback` |
| `STIM-SYNTH-PLAYBACK-NEG030MS` | Playback | `-30.0` | `-35.0` | `-37.5` | `-35.85` | `-5.85` | `0.9951` | `0.00` | `playback` |
| `STIM-SYNTH-PLAYBACK-POS000MS` | Playback | `0.0` | `-4.3` | `-5.0` | `-4.53` | `-4.53` | `0.9951` | `0.00` | `playback` |
| `STIM-SYNTH-PLAYBACK-POS030MS` | Playback | `+30.0` | `+25.8` | `+25.0` | `+25.51` | `-4.49` | `0.9952` | `0.00` | `playback` |
| `STIM-SYNTH-PLAYBACK-POS060MS` | Playback | `+60.0` | `+54.1` | `+52.5` | `+53.56` | `-6.44` | `0.9951` | `0.00` | `playback` |
| `STIM-SYNTH-PLAYBACK-POS120MS` | Playback | `+120.0` | `+116.5` | `+115.0` | `+115.98` | `-4.02` | `0.9952` | `0.00` | `playback` |
| `STIM-SYNTH-PLAYBACK-POS200MS` | Playback | `+200.0` | `+191.9` | `+190.0` | `+191.24` | `-8.76` | `0.9952` | `0.00` | `playback` |
| `STIM-SYNTH-LIVE-POS000MS` | Live Take | `0.0` | `-5.2` | `0.0` | `-3.36` | `-3.36` | `0.8809` | `18.95` | `live` |

### Summary Calibration Metrics (Synthetic Suite)
- **Mean Bias Error:** `-5.21 ms` (attributable to discrete `60 fps` = `16.67 ms` camera frame sampling and finite-difference derivative alignment).
- **Mean Absolute Error (MAE):** `5.21 ms` (max absolute error `8.76 ms` at `+200 ms`).
- **Root-Mean-Square Error (RMSE):** `5.43 ms`.
- **Machine Minimum Detectable Lag ($\text{MDL}_{\text{machine}} = 2\,\text{RMSE}$):** `10.86 ms`.
- **Pearson $r(\Delta t_{\text{true}}, \hat{\tau}_{\text{lag}})$:** `0.99990`.

> **Epistemic Boundary:** These numbers reflect synthetic-signal instrument calibration only. They do not establish accuracy on real human faces under variable lighting or camera angles, nor do they test any hypothesis about human perception.

---

## 2. Exact Commands for Principal Investigator Execution

### A. Re-run Synthetic Calibration Suite
```powershell
dotnet run --project argon/src/Argon.Engine -- exp005
```

### B. Measure a Real Recorded `.mp4` Video Against a Studio Master `.wav`
Once `video/exp005/live_raw.mp4`, `video/exp005/playback_raw.mp4`, and `video/exp005/studio_master.wav` are recorded:
```powershell
dotnet run --project argon/src/Argon.Engine -- exp005-video video/exp005/live_raw.mp4 video/exp005/studio_master.wav 0 false
dotnet run --project argon/src/Argon.Engine -- exp005-video video/exp005/playback_raw.mp4 video/exp005/studio_master.wav 0 true
```

### C. Compute Signal–Perception Gap Metrics After Observer JSON Collection
Place exported `observer-*.json` files from [`research/protocols/exp005-observer-form.html`](../../../research/protocols/exp005-observer-form.html) into `argon/experiments/EXP-005-signal-perception-gap/responses/` and run:
```powershell
dotnet run --project argon/src/Argon.Engine -- exp005-analyze
```
