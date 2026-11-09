# EXP-005 Protocol: Signal–Perception Gap in Lip-Sync and Playback Detection

**Status:** Preregistration draft and tooling specification; **not yet executed with human participants**. Not frozen: the protocol is frozen only after the real-video instrument check (Section 7, Step 1) and the pilot (Section 6.4).  
**Version:** `0.2` (November 9, 2026). See the [change log](#9-change-log) for corrections to `0.1`.  
**Principal Investigator:** Jair Ivan Rojas Ruiz ([ORCID: 0009-0003-1109-4583](https://orcid.org/0009-0003-1109-4583))  
**Implementation Agent:** Argon (`Argon.Engine`), with review corrections by Claude

---

## 1. Origin and Epistemic Status

This protocol originates from an informal, single-observer observation by the Principal Investigator: while watching a performance that used pre-recorded playback, the singer's lip movements appeared synchronized with the vocal track, yet something about the performance did not fit.

In accordance with the repository's working conventions ([`README.md`](../../README.md#what-is-and-is-not-established), `D020`), **a single observer's impression is treated strictly as a hypothesis-generating observation, not as empirical evidence**. No claim is made prior to data collection that such a gap exists, that human observers reliably experience it, or that a computational detector outperforms human observers.

---

## 2. Research Questions

- **RQ1 (Machine Lag Calibration):** With what error (Mean Absolute Error and Root-Mean-Square Error, in milliseconds) can a signal-level detector, using only audio waveforms and video frames, recover known audio-to-video offsets $\Delta t_{\text{true}} \in \{-120, -60, -30, 0, +30, +60, +120, +200\}\text{ ms}$ on **real** recordings with known ground truth?
- **RQ2 (Temporal Synchrony Gap, $\text{SPG}_{\tau}$):** How much wider, in milliseconds, is the range of offsets that human observers accept as synchronous ($W_{\text{human}}$) than the machine's uncertainty band ($2\,\text{MDL}_{\text{machine}}$)?
- **RQ3 (Playback Detection at Zero Offset):** On unshifted takes ($\Delta t_{\text{true}} = 0$), how accurately do human observers distinguish live singing from playback, and do they report that "something felt off" on playback takes they rate as synchronous?

---

## 3. Falsifiable Hypotheses and Dismissal Criteria

### Hypotheses Under Test

- **$H_1$ (Temporal Synchrony Gap):** The human synchrony window is wider than the machine's uncertainty band:
  $$\text{SPG}_{\tau} = W_{\text{human}} - 2\,\text{MDL}_{\text{machine}} > 15\text{ ms}$$
  Both terms are full widths in milliseconds: $W_{\text{human}}$ spans from the lead-side to the lag-side 50% point, and $2\,\text{MDL}_{\text{machine}}$ spans $\pm\text{MDL}_{\text{machine}}$ around the estimate.
  - **Null $H_{0,1}$:** $\text{SPG}_{\tau} \le 0\text{ ms}$.

- **$H_{2a}$ (Machine with reference access vs. humans, information-asymmetric):** On unshifted takes, a detector that compares the audio against the studio master (repetition test) classifies live vs. playback more accurately than human observers by at least 20 percentage points.
  - **Null $H_{0,2a}$:** $\text{Acc}_{\text{machine,ref}} - \text{Acc}_{\text{human}} \le 0$.
  - **Interpretation limit:** the machine has the studio master and the observers do not. A positive result shows the value of reference access, not superior perception. It is reported under that label.

- **$H_{2b}$ (Machine without reference vs. humans, information-matched):** On unshifted takes, a detector that sees only the stimulus itself (dispersion of lip-to-audio onset differences, $\sigma_{\text{LA}}$; no studio master) classifies live vs. playback more accurately than human observers.
  - **Null $H_{0,2b}$:** $\text{Acc}_{\text{machine,noref}} - \text{Acc}_{\text{human}} \le 0$.
  - The $\sigma_{\text{LA}}$ decision threshold is fixed on pilot recordings (Section 6.4) and frozen before the main study. It is never tuned on main-study data or on synthetic stimuli.

- **$H_3$ (Synchrony–Authenticity Dissociation):** Among unshifted playback trials that observers rate as synchronous ($S_{\text{human}} \ge 4$), the rate of `anomalyFelt = true` exceeds the rate on unshifted live trials by at least 0.15.
  - **Null $H_{0,3}$:** $\Delta p_{\text{anomaly}} \le 0$.

### Explicit Criteria to Discard the Instrument or the Hypotheses

1. **Instrument failure (checked before any human study):** the detector is declared insufficient, and must be revised or discarded before comparison with observers, if on the real-video lag sweep (Section 4.3) **either** $\text{MAE}_{\hat{\tau}} > 25\text{ ms}$ **or** Pearson $r(\Delta t_{\text{true}}, \hat{\tau}) < 0.90$. Passing on synthetic stimuli does not satisfy this criterion.
2. **Discard $H_1$** if the lower bound of the 95% bootstrap confidence interval (resampling observers) for $\text{SPG}_{\tau}$ is $\le 0\text{ ms}$.
3. **Discard $H_{2a}$ / $H_{2b}$** if McNemar's exact test between machine and human predictions on unshifted trials ($\Delta t_{\text{true}} = 0$) yields $p \ge 0.05$, or the accuracy difference is below 0.10.
4. **Discard $H_3$** if the 95% confidence interval for $\Delta p_{\text{anomaly}}$ overlaps 0, and conclude that the founder's impression does not generalize across observers in this stimulus set.

---

## 4. Stimulus Design and Ground-Truth Recording Protocol

All human-study stimuli are self-recorded performances by the Principal Investigator (vocals + acoustic guitar), which keeps them free of third-party rights. Synthetic clips (Section 4.5) only unit-test the software.

### 4.1 Equal Acoustic Chain (prevents sound quality from revealing the condition)

If the studio master is recorded with a good microphone and the live take with the camera microphone, observers could classify by sound quality instead of synchrony. To prevent this:

- Record **every** take in the same room, with the camera in the same position, and use the **camera's own audio track** as the stimulus audio in both conditions.
- The studio master is recorded in that same room with a separate microphone and is used only as the playback source and as the machine's reference.
- In the playback take, the master is played through a loudspeaker placed where the performer's mouth would be (about 30–50 cm in front of the camera), so the camera records it through the same room and microphone chain as a live voice.
- Residual cue: the loudspeaker colors the sound slightly. Record the loudspeaker model and distance; describe this cue in the limitations.

### 4.2 Takes per Excerpt

Use **at least three** different 15–20 s Spanish excerpts rich in bilabials (`/p/`, `/b/`, `/m/`), each with guitar. For each excerpt:

| Take | Content | Ground truth |
|---|---|---|
| `master_<id>.wav` | Studio master (separate microphone) | Reference only, never shown to observers |
| `live_<id>.mp4` | Sung and played live on camera | Live, $\Delta t_{\text{true}} = 0$ |
| `playback_<id>.mp4` | Performer mimes and finger-syncs while the master plays through the loudspeaker; camera audio captures it | Playback, $\Delta t_{\text{true}} = 0$ by construction (same capture device; sound travel at 40 cm adds about 1.2 ms) |

Every take begins with a visible hand clap followed by 1 s of silence. The clap is a check of the camera's own audio-video alignment, not the alignment method for playback: because the playback audio is captured by the camera, no muxing or clap alignment is needed.

Camera: fixed tripod, $1920 \times 1080$ progressive, fixed **60 fps** (or 30 fps), shutter $\ge 1/250$ s, framing that keeps the mouth inside a fixed region for the whole take. Note the mouth region (x, y, width, height) for each take; it is passed to the detector as the crop (Section 8).

### 4.3 Controlled Lag Sweep (for $H_1$ only)

From each `live_<id>.mp4`, generate eight variants with $\Delta t_{\text{true}} \in \{-120, -60, -30, 0, +30, +60, +120, +200\}$ ms. Negative means audio leads video; positive means audio lags video.

Re-encode the audio with an explicit delay or trim rather than relying on stream-copy timestamp offsets, which some players ignore. Then **verify every variant** with the detector on the clap (or by measuring the clap's audio onset against its visual frame) before using it; record the measured offset next to the intended one.

```powershell
$offsetsMs = @(-120, -60, -30, 0, 30, 60, 120, 200)
$src = "video/exp005/live_A.mp4"
foreach ($ms in $offsetsMs) {
    $tag = if ($ms -lt 0) { "neg$([Math]::Abs($ms).ToString('000'))ms" } else { "pos$($ms.ToString('000'))ms" }
    $out = "video/exp005/stim_live_A_$tag.mp4"
    if ($ms -ge 0) {
        # Audio lags: insert $ms of silence at the start of the audio
        .\.tools\ffmpeg\bin\ffmpeg.exe -y -i $src -map 0:v:0 -map 0:a:0 -c:v copy -af "adelay=${ms}:all=1" -c:a aac -b:a 192k -shortest $out
    } else {
        # Audio leads: drop the first |$ms| of audio
        $abs = [Math]::Abs($ms) / 1000.0
        .\.tools\ffmpeg\bin\ffmpeg.exe -y -i $src -map 0:v:0 -map 0:a:0 -c:v copy -af "atrim=start=$abs,asetpts=PTS-STARTPTS" -c:a aac -b:a 192k -shortest $out
    }
}
```

### 4.4 Which Stimuli Serve Which Hypothesis (removes the lag/condition confound)

All lag variants come from live takes, and a live take shifted by 120 ms can look like bad playback. Therefore:

- **$H_1$** uses the synchrony ratings on the eight live lag variants.
- **$H_{2a}$, $H_{2b}$, $H_3$** use only the **unshifted** takes (`live_<id>.mp4` and `playback_<id>.mp4`, $\Delta t_{\text{true}} = 0$).
- Live/playback judgments on shifted variants are still recorded but are exploratory and excluded from $H_2$ accuracy.

### 4.5 Synthetic Calibration Suite (software test only)

`dotnet run --project argon/src/Argon.Engine -- exp005` regenerates nine deterministic synthetic stimuli (fixed seeds) and checks the estimator. These stimuli are generated by code written alongside the detector: playback tracks reuse the master's onset times exactly and the live track receives injected timing jitter, so the live/playback separation they show is true **by construction**. They test that the code runs and that its timing conventions are unbiased; they are not evidence of performance on real recordings.

---

## 5. Variables and Operational Definitions

| Variable | Symbol | Role | Unit & Range | Procedure |
| :--- | :---: | :--- | :---: | :--- |
| True audio–video lag | $\Delta t_{\text{true}}$ | Manipulated | ms, $[-120, +200]$ | Applied as in Section 4.3 and verified per file. $> 0$: audio lags video. |
| Performance mode | $M_{\text{true}}$ | Manipulated | $\{\text{live}, \text{playback}\}$ | Section 4.2. |
| Envelope lag | $\hat{\tau}_{\text{xcorr}}$ | Machine | ms, $[-300, +300]$ | Lag maximizing normalized cross-correlation between positive mouth-opening rate and positive acoustic RMS flux on a 5 ms grid, with parabolic interpolation. Both difference signals are time-stamped at the midpoint of the interval they span. |
| Onset lag | $\hat{\tau}_{\text{onset}}$ | Machine | ms, $[-260, +260]$ | Median of (acoustic onset − nearest visual opening onset). |
| Consensus lag | $\hat{\tau}_{\text{lag}}$ | Machine | ms | $0.65\,\hat{\tau}_{\text{xcorr}} + 0.35\,\hat{\tau}_{\text{onset}}$; error $e_\tau = \hat{\tau}_{\text{lag}} - \Delta t_{\text{true}}$. Weights fixed before real data. |
| Lip–audio onset dispersion | $\sigma_{\text{LA}}$ | Machine (no reference) | ms $\ge 0$ | Standard deviation of the matched lip-to-audio onset differences within the stimulus. Uses no studio master. |
| Master repetition similarity | $r_{\text{rep}}$ | Machine (reference) | $[-1, +1]$ | Lag-aligned correlation of 5 ms RMS envelopes between stimulus audio and studio master. |
| Inter-onset timing drift | $\sigma_{\text{IOI}}$ | Machine (reference) | ms $\ge 0$ | Standard deviation of onset residuals between stimulus audio and studio master. |
| Authenticity judgment | $J_{\text{human}}$ | Human | $\{\text{live}, \text{playback}\}$ | Recorded before any feedback. |
| Synchrony rating | $S_{\text{human}}$ | Human | 1–5 | 1 = clearly out of sync, 5 = completely in sync. |
| Anomaly flag | $A_{\text{human}}$ | Human | binary | "Even if the lips looked synchronized, did anything feel off?" |
| Unprompted reason | $R_{\text{text}}$ | Human, qualitative | free text | Coded after data collection into cue categories (lip timing, breathing/effort, sound/room, guitar hand, intuition). |

---

## 6. Blinded Human Observer Procedure

### 6.1 Anti-contamination rule

Observers are told only that they will watch short clips of a musician and judge whether each is live or playback. They are never told, before or during the task, about millisecond offsets, bilabial consonants, or the detector's measures.

### 6.2 Trials and order

- Per observer: for each of the three or more excerpts, the 8 live lag variants plus the unshifted playback take (the unshifted live take is the 0 ms variant). With three excerpts this is 27 clips.
- Order is fully randomized per observer, with the constraint that two versions of the same excerpt are never consecutive. The randomization seed is stored in the observer file.
- Each judgment is locked with a UTC timestamp before the next clip ([`exp005-observer-form.html`](exp005-observer-form.html)); clips are shown under blind codes, never file names. No feedback is given.

### 6.3 Sample size (planning target, not a formal power analysis)

Target **30 observers** for the main study. Thirty observers × 6 unshifted trials (3 excerpts × live/playback) give 180 binary judgments; if accuracy is near 0.70, the 95% interval half-width is about ±7 percentage points before accounting for clustering, enough to resolve the preregistered 20-point difference. Confidence intervals use a cluster bootstrap over observers ($B = 2{,}000$). Repeated viewings of the same excerpt are a known limitation; excerpt is included as a factor in the analysis.

### 6.4 Pilot (excluded from confirmatory analysis)

Run 3–5 pilot observers and record pilot takes **not reused** in the main study. The pilot is used only to (a) check the form and instructions, (b) check the detector on real video (Section 7, Step 1), and (c) fix the $\sigma_{\text{LA}}$ threshold for $H_{2b}$. After the pilot, freeze this protocol (version, date, and file hash in `SESSION_LOG.md`) before collecting main-study data.

---

## 7. Pre-Specified Analysis Plan

1. **Step 1 — Instrument check on real video (no human data):** run the detector on every verified lag variant (Section 4.3). Report bias, MAE, RMSE, Pearson $r$, and $\text{MDL}_{\text{machine}} = 2\,\text{RMSE}_{\hat{\tau}}$. Apply criterion 1 of Section 3. Synthetic results are reported separately and never substitute for this step.
2. **Step 2 — Human synchrony curve:** for each $\Delta t_{\text{true}}$, compute the proportion of $S_{\text{human}} \ge 4$. Fit a logistic curve to each side (lead and lag) and take the 50% points $\text{PSE}_{\text{lead}} < 0$ and $\text{PSE}_{\text{lag}} > 0$; $W_{\text{human}} = \text{PSE}_{\text{lag}} - \text{PSE}_{\text{lead}}$.
   - *Implementation status:* `exp005-analyze` currently estimates $W_{\text{human}}$ by linear interpolation between the tested offsets where the proportion crosses 0.5. The logistic fit must be implemented and checked on simulated responses before main-study data are analysed.
3. **Step 3 — Gap metrics:**
   $$\text{SPG}_{\tau} = W_{\text{human}} - 2\,\text{MDL}_{\text{machine}} \quad [\text{ms}]$$
   $$\text{SPG}_{\text{ref}} = \text{Acc}_{\text{machine,ref}}(0) - \text{Acc}_{\text{human}}(0), \qquad \text{SPG}_{\text{noref}} = \text{Acc}_{\text{machine,noref}}(0) - \text{Acc}_{\text{human}}(0)$$
   where $(0)$ denotes unshifted trials only. Report 95% cluster-bootstrap intervals for all three.
4. **Step 4 — $H_3$:** compare `anomalyFelt` rates between unshifted playback trials with $S_{\text{human}} \ge 4$ and unshifted live trials.
5. **Step 5 — Qualitative:** code $R_{\text{text}}$ into cue categories with a second coder blind to condition; report agreement.

---

## 8. Commands

```powershell
# Software self-test on synthetic stimuli (regenerates stimuli deterministically)
dotnet run --project argon/src/Argon.Engine -- exp005

# Real video against its studio master; last argument is the FFmpeg crop for the mouth region (w:h:x:y in pixels)
dotnet run --project argon/src/Argon.Engine -- exp005-video video/exp005/stim_live_A_pos060ms.mp4 video/exp005/master_A.wav 60 false "crop=220:160:850:520,scale=32:32"

# Gap metrics after collecting observer JSON files in argon/experiments/EXP-005-signal-perception-gap/responses/
dotnet run --project argon/src/Argon.Engine -- exp005-analyze
```

---

## 9. Change Log

**0.2 (November 9, 2026), review corrections:**

- Unified the definition of $\text{SPG}_{\tau}$ ($W_{\text{human}} - 2\,\text{MDL}_{\text{machine}}$, both full widths); 0.1 stated two different formulas.
- Corrected the instrument-failure criterion: fail if MAE > 25 ms **or** $r$ < 0.90, evaluated on real video. The 0.1 wording inverted the logic.
- Restricted $H_2$ and $H_3$ to unshifted takes; 0.1 mixed $\Delta t = 0$ and $|\Delta t| \le 30$ ms.
- Split $H_2$ into $H_{2a}$ (machine with the studio master, information-asymmetric) and $H_{2b}$ (machine without it, information-matched), since the 0.1 comparison gave the machine the answer key.
- Removed the confound between lag and performance mode by assigning lag variants to $H_1$ only (Section 4.4).
- Required the same room and camera microphone for live and playback stimuli, so sound quality cannot reveal the condition (Section 4.1); playback audio is now captured by the camera, which removes clap-based muxing.
- Replaced stream-copy `-itsoffset` with re-encoded audio delay/trim and per-file verification.
- Added at least three excerpts, trial ordering rules, a sample-size target, and a pilot that fixes thresholds before the main study.
- Stated that the synthetic suite is a software test whose live/playback separation is true by construction.
