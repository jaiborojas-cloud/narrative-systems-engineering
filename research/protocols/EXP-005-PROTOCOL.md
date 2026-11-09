# EXP-005 Protocol: Signal–Perception Gap in Lip-Sync and Playback Detection

**Status:** Preregistration draft and tooling specification; **not yet executed with human participants**.  
**Version:** `0.1` (November 9, 2026)  
**Principal Investigator:** Jair Ivan Rojas Ruiz ([ORCID: 0009-0003-1109-4583](https://orcid.org/0009-0003-1109-4583))  
**Implementation Agent:** Argon (`Argon.Engine v0.4.1`)

---

## 1. Origin and Epistemic Status

This protocol originates from an informal, single-observer observation by the Principal Investigator: while watching a musical performance that used pre-recorded playback, the singer's lip movements appeared visually synchronized with the vocal track, yet the performance felt perceptually anomalous ("something did not fit").

In accordance with the repository's working conventions ([`README.md`](../../README.md#what-is-and-is-not-established), `D020`), **a single observer's impression is treated strictly as a hypothesis-generating observation, not as empirical evidence**. No claim is made prior to data collection that such a gap exists, that human observers reliably experience it, or that a computational detector outperforms human observers.

---

## 2. Research Questions

- **RQ1 (Machine Lag Calibration):** With what error (Mean Absolute Error and Root-Mean-Square Error, in milliseconds) can a signal-level detector—using only raw acoustic waveforms and video frame rasters—recover known audio-to-video temporal offsets $\Delta t_{\text{true}} \in \{-120, -60, -30, 0, +30, +60, +120, +200\}\text{ ms}$ on ground-truth recordings?
- **RQ2 (Temporal Synchrony Gap, $\text{SPG}_{\tau}$):** What is the difference in milliseconds between the human perceptual synchrony window $W_{\text{human}}\text{ [ms]}$ (the range of audio-to-video offsets over which observers still rate lip-audio synchrony at or above the midpoint threshold) and the machine detector's minimum detectable lag $\text{MDL}_{\text{machine}}\text{ [ms]}$?
- **RQ3 (Zero-Lag Playback Gap, $\text{SPG}_{\text{playback}}$):** When temporal offset is within the human synchrony window ($|\Delta t_{\text{true}}| \le 30\text{ ms}$), do human observers report an "uncanny / something feels off" anomaly or identify playback above chance ($50\%$), and how does human classification accuracy compare against a signal-level repetition and micro-timing detector?

---

## 3. Falsifiable Hypotheses and Dismissal Criteria

### Hypotheses Under Test

- **$H_1$ (Sub-Perceptual Temporal Lag Detection):** On ground-truth shifted audiovisual recordings at $30\text{–}60\text{ fps}$, the signal-level lag estimator achieves a Mean Absolute Error $\text{MAE}_{\hat{\tau}} \le 20\text{ ms}$, resulting in a machine Minimum Detectable Lag ($\text{MDL}_{\text{machine}} = 2\,\text{RMSE}_{\hat{\tau}}$) strictly narrower than the human 50% synchrony tolerance window $W_{\text{human}}$ ($\text{SPG}_{\tau} = W_{\text{human}} - \text{MDL}_{\text{machine}} > 15\text{ ms}$).
  - **Null Hypothesis $H_{0,1}$:** $\text{SPG}_{\tau} \le 0\text{ ms}$ (the machine detector is no more precise than human synchrony judgments, or $\text{MAE}_{\hat{\tau}} > 25\text{ ms}$).
- **$H_2$ (Zero-Lag Playback Classification Gap):** On zero-offset playback stimuli ($\Delta t_{\text{true}} = 0\text{ ms}$) where the performer mimes to their own prior studio take, human binary classification accuracy $\text{Acc}_{\text{human}}(0\text{ ms})$ is lower than the signal detector's repetition/micro-timing classification accuracy $\text{Acc}_{\text{machine}}(0\text{ ms})$ by at least $\text{SPG}_{\text{playback}}(0\text{ ms}) \ge 0.20$ ($20$ percentage points).
  - **Null Hypothesis $H_{0,2}$:** $\text{Acc}_{\text{machine}}(0\text{ ms}) - \text{Acc}_{\text{human}}(0\text{ ms}) \le 0$ (humans detect zero-lag playback as accurately as or better than the signal detector).
- **$H_3$ (Synchrony–Authenticity Dissociation in Human Observers):** Among zero-lag and near-zero-lag playback trials ($|\Delta t_{\text{true}}| \le 30\text{ ms}$) that human observers rate as visually synchronized ($\text{synchronyRating} \ge 4$ on the $1\text{–}5$ scale), the proportion of trials in which observers still flag `anomalyFelt = true` ("something felt off") exceeds the false-alarm rate on genuine live takes by at least $\Delta p_{\text{anomaly}} \ge 0.15$.
  - **Null Hypothesis $H_{0,3}$:** $\Delta p_{\text{anomaly}} \le 0$ (when lips appear synchronized, observers do not report an anomaly on playback takes more often than on genuine live takes).

### Explicit Criteria to Discard / Falsify the Hypotheses

1. **Instrument Failure Criterion (Discard Detector Before Human Study):** If the machine detector fails to achieve $\text{MAE}_{\hat{\tau}} \le 25\text{ ms}$ or Pearson $r(\Delta t_{\text{true}}, \hat{\tau}) \ge 0.90$ across the 8 ground-truth offset conditions ($\{-120, -60, -30, 0, +30, +60, +120, +200\}\text{ ms}$), the current signal estimator is declared insufficient and **must be revised or discarded before comparing against human participants**.
2. **Hypothesis Dismissal Criterion for $H_1$:** If the lower bound of the $95\%$ paired bootstrap confidence interval for $\text{SPG}_{\tau} = W_{\text{human}} - \text{MDL}_{\text{machine}}$ is $\le 0\text{ ms}$, discard $H_1$.
3. **Hypothesis Dismissal Criterion for $H_2$:** If McNemar's exact test between machine and human binary predictions on $|\Delta t_{\text{true}}| \le 30\text{ ms}$ trials yields $p \ge 0.05$ or $\text{SPG}_{\text{playback}} < 0.10$, discard $H_2$.
4. **Hypothesis Dismissal Criterion for $H_3$:** If the paired difference in `anomalyFelt` rate between synchronized playback trials ($\text{synchronyRating} \ge 4$) and unshifted live trials has a $95\%$ confidence interval overlapping $0$, discard $H_3$ and conclude that the founder's initial subjective impression does not generalize across observers in this stimulus set.

---

## 4. Stimulus Design and Ground-Truth Recording Protocol

To avoid copyright restrictions and ensure exact physical ground truth, all experimental stimuli consist of self-recorded performances by the Principal Investigator (Jair Ivan Rojas Ruiz, vocals + acoustic guitar) plus synthetic calibration clips for unit-testing the signal pipeline.

### 4.1 Ground-Truth Recording Procedure (For Principal Investigator Execution)

1. **Hardware Setup:**
   - Camera mounted on a fixed tripod framing head, mouth, torso, and guitar strumming hand at $1920 \times 1080$, progressive scan, fixed frame rate **$60\text{ fps}$** (preferred, $16.67\text{ ms/frame}$) or **$30\text{ fps}$** ($33.33\text{ ms/frame}$), manual exposure with shutter speed $\ge 1/250\text{ s}$ to prevent motion blur on the lips.
   - Audio recorded simultaneously at $48\text{ kHz}$ or $16\text{ kHz}$ mono PCM16 WAV.
2. **Clapboard Impulse at $t = 1.0\text{ s}$:**
   - Every take begins with a sharp visible hand-clap in front of the face followed by $1.0\text{ s}$ of silence before singing starts. This provides an objective hardware audio-video alignment reference independent of vocal articulation.
3. **Phonetic Content:**
   - A $15\text{–}20\text{ s}$ Spanish lyrical excerpt rich in bilabial plosives and nasals (`/p/`, `/b/`, `/m/`, e.g., *"Tiempo de partir, paso a paso sin mirar atrás..."*) accompanied by rhythmic acoustic guitar chords.
4. **Three Master Recording Takes:**
   - **`TAKE_STUDIO_MASTER` (`master.wav`):** Clean reference audio recording of the vocal + guitar performance.
   - **`TAKE_LIVE_GENUINE` (`live_000ms.mp4`):** Genuine live camera+microphone performance of the same excerpt (sung live on camera without playback).
   - **`TAKE_PLAYBACK_MIME` (`playback_000ms.mp4`):** Performer mimes/lip-syncs and finger-syncs on camera while `TAKE_STUDIO_MASTER` plays back as the audio reference; the video is muxed with the studio audio aligned at the initial clapboard impulse ($\Delta t_{\text{true}} = 0\text{ ms}$).
5. **Controlled Lag Sweep Variants ($8$ Offsets):**
   - From `live_000ms.mp4` (and optionally `playback_000ms.mp4`), generate $8$ controlled audio-to-video offset files using FFmpeg:
     $$\Delta t_{\text{true}} \in \{-120, -60, -30, 0, +30, +60, +120, +200\}\text{ ms}$$
     where **negative $\Delta t$** means audio leads video (audio arrives early) and **positive $\Delta t$** means audio lags behind video (audio arrives late).

### 4.2 Exact FFmpeg Commands to Generate Ground-Truth Lag Variants

Once `video/exp005/live_raw.mp4` is recorded, the Principal Investigator can generate the 8 ground-truth shifted stimuli deterministically using the following PowerShell loop:

```powershell
$offsetsMs = @(-120, -60, -30, 0, 30, 60, 120, 200)
New-Item -ItemType Directory -Force -Path "video/exp005" | Out-Null

foreach ($ms in $offsetsMs) {
    $sec = [Math]::Round($ms / 1000.0, 3)
    $tag = if ($ms -lt 0) { "neg$([Math]::Abs($ms).ToString('000'))ms" } else { "pos$($ms.ToString('000'))ms" }
    $outFile = "video/exp005/stim_live_$tag.mp4"
    if ($ms -ge 0) {
        # Positive offset: delay audio by +$sec seconds relative to video
        .\.tools\ffmpeg\bin\ffmpeg.exe -y -i "video/exp005/live_raw.mp4" -itsoffset $sec -i "video/exp005/live_raw.mp4" -map 0:v:0 -map 1:a:0 -c:v copy -c:a aac -b:a 192k $outFile
    } else {
        # Negative offset: delay video by |$sec| seconds so audio leads video
        $absSec = [Math]::Abs($sec)
        .\.tools\ffmpeg\bin\ffmpeg.exe -y -itsoffset $absSec -i "video/exp005/live_raw.mp4" -i "video/exp005/live_raw.mp4" -map 0:v:0 -map 1:a:0 -c:v copy -c:a aac -b:a 192k $outFile
    }
}
```

---

## 5. Variables and Operational Definitions

| Variable | Symbol | Type / Role | Unit & Range | Operational Procedure |
| :--- | :---: | :--- | :---: | :--- |
| **True Audio–Video Lag** | $\Delta t_{\text{true}}$ | Independent (Manipulated) | $\text{ms} \in [-120, +200]$ | Controlled shift applied between audio and video streams. $\Delta t_{\text{true}} > 0$: audio lags video. |
| **Performance Mode** | $M_{\text{true}}$ | Independent (Manipulated) | Categorical $\{\text{live}, \text{playback}\}$ | Whether the vocal/guitar audio was produced live during filming or re-used from the studio master take. |
| **Estimated Envelope Lag** | $\hat{\tau}_{\text{xcorr}}$ | Machine Measurement | $\text{ms} \in [-300, +300]$ | Lag maximizing normalized cross-correlation between positive visual mouth-opening rate $\dot{A}_+(t)$ and acoustic RMS onset envelope $\dot{E}_+(t)$ on a $5\text{ ms}$ grid with 3-point parabolic interpolation. |
| **Estimated Bilabial Onset Lag** | $\hat{\tau}_{\text{onset}}$ | Machine Measurement | $\text{ms} \in [-250, +250]$ | Median time difference $t^{\text{ac}}_k - t^{\text{vis}}_k$ between local mouth-closure release events and nearest acoustic plosive bursts. |
| **Consensus Machine Lag** | $\hat{\tau}_{\text{lag}}$ | Machine Measurement | $\text{ms} \in [-300, +300]$ | Weighted consensus $0.65\,\hat{\tau}_{\text{xcorr}} + 0.35\,\hat{\tau}_{\text{onset}}$; error against ground truth is $e_\tau = \hat{\tau}_{\text{lag}} - \Delta t_{\text{true}}\text{ [ms]}$. |
| **Master Repetition Similarity** | $r_{\text{rep}}$ | Machine Measurement | Dimensionless $\in [-1, +1]$ | Lag-aligned correlation of $10\text{ ms}$ log-energy envelopes between the stimulus audio and the studio master recording. |
| **Inter-Onset Timing Drift** | $\sigma_{\text{IOI}}$ | Machine Measurement | $\text{ms} \ge 0$ | Standard deviation of onset timing residuals between the stimulus audio and the studio master recording. |
| **Human Authenticity Judgment** | $J_{\text{human}}$ | Human Dependent | Binary $\{\text{live}, \text{playback}\}$ | Observer classification recorded **before** any feedback or cue explanation. |
| **Human Synchrony Rating** | $S_{\text{human}}$ | Human Dependent | Ordinal $1\text{–}5$ | $1 = \text{Clearly out of sync}$, $3 = \text{Borderline}$, $5 = \text{Completely in sync}$. |
| **Human Anomaly Flag** | $A_{\text{human}}$ | Human Dependent | Binary $\{\text{true}, \text{false}\}$ | Whether the observer felt "something was off/unnatural" regardless of apparent lip synchrony. |
| **Human Unprompted Reason** | $R_{\text{text}}$ | Human Qualitative | Free text | Recorded **before** advancing to the next trial; coded post-hoc into cue categories (lip lag, breathing/chest effort, acoustic room mismatch, strumming hand, intuition). |

---

## 6. Blinded Human Observer Procedure (Strict Anti-Contamination Rule)

1. **Zero Pre-Briefing on Signal Cues:** Observers are told only that they will watch short clips of a musician and judge whether each clip is a live recording or playback. **They must never be told about millisecond offsets, bilabial consonants (`/p/`, `/b/`, `/m/`), optical flow, or repetition metrics prior to or during the task.**
2. **Trial Locking Before Next Stimulus:** In [`research/protocols/exp005-observer-form.html`](exp005-observer-form.html), once an observer submits `(judgment, synchronyRating, anomalyFelt, confidence, reasonText)` for a clip, the trial record is locked with an immutable UTC timestamp (`lockedAtUtc`).
3. **No Trial-by-Trial Feedback:** Neither ground-truth labels nor machine measurements are stored in or displayed by the observer form.

---

## 7. Pre-Specified Statistical Analysis Plan

All analyses are fixed prior to collecting human observer responses:

1. **Step 1 — Machine Detector Calibration Audit (No Human Data):**
   - Evaluate `LipSyncSignalDetector` across the 8 ground-truth offset clips ($\{-120, -60, -30, 0, +30, +60, +120, +200\}\text{ ms}$).
   - Compute Mean Bias $\frac{1}{K}\sum (\hat{\tau}_k - \Delta t_k)\text{ [ms]}$, Mean Absolute Error $\text{MAE}_{\hat{\tau}}\text{ [ms]}$, Root-Mean-Square Error $\text{RMSE}_{\hat{\tau}}\text{ [ms]}$, and define the machine **Minimum Detectable Lag**:
     $$\text{MDL}_{\text{machine}} = 2.0 \times \text{RMSE}_{\hat{\tau}} \quad [\text{ms}]$$
2. **Step 2 — Human Psychometric Synchrony Curve:**
   - For each offset $\Delta t \in \{-120, \dots, +200\}\text{ ms}$, compute the proportion of human observers rating the clip as synchronous ($S_{\text{human}} \ge 4$).
   - Fit a symmetric or asymmetric Gaussian/logistic psychometric curve $P_{\text{sync}}(\Delta t)$ to estimate the lower and upper $50\%$ Points of Subjective Equality ($\text{PSE}_{\text{lead}} < 0$ and $\text{PSE}_{\text{lag}} > 0$) and the total human synchrony integration window width:
     $$W_{\text{human}} = \text{PSE}_{\text{lag}} - \text{PSE}_{\text{lead}} \quad [\text{ms}]$$
3. **Step 3 — Computing the Signal–Perception Gap Metrics:**
   - **Temporal Synchrony Gap ($\text{SPG}_{\tau}$, in milliseconds):**
     $$\text{SPG}_{\tau} = W_{\text{human}} - 2\,\text{MDL}_{\text{machine}} \quad [\text{ms}]$$
     *(Positive values indicate the span of temporal offsets in milliseconds that fall inside the human synchrony tolerance window but outside the machine detector's uncertainty band).*
   - **Playback Detection Gap at Near-Zero Lag ($\text{SPG}_{\text{playback}}$, in accuracy difference $\in [-1, +1]$):**
     $$\text{SPG}_{\text{playback}} = \text{Acc}_{\text{machine}}\bigl(|\Delta t| \le 30\text{ ms}\bigr) - \text{Acc}_{\text{human}}\bigl(|\Delta t| \le 30\text{ ms}\bigr)$$
   - Report $95\%$ percentile bootstrap confidence intervals ($B = 2,000$ resamples over observers) for both gap metrics.
