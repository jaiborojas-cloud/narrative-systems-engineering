# Session log

Append a dated entry after each substantive session. Record what actually happened, distinguish proposed next steps, and preserve earlier entries. Add corrections explicitly instead of silently rewriting history.

## Session 001 - September 8, 2026

**Objective:** review three foundational PDFs in order, assess project feasibility, and initialize project documentation and directories. The founder requested that all newly authored content be in English.

**Materials reviewed:** fundationla-doc-1.pdf (19 pages), fundationla-doc-2.pdf (20 pages), fundationla-doc-3.pdf (24 pages), plus the preliminary external sources recorded in references/SOURCES.md.

**Work performed:**

- Inspected workspace and read the PDF skill.
- Attempted text extraction; no substantive text was extractable.
- Rendered and visually inspected every PDF page in the requested order.
- Traced the progression from HNA to NDE to NSE and distinguished embedded instructions from the current request.
- Checked adjacent research and existing uses of the proposed name.
- Wrote the README, roadmap, source synthesis, feasibility assessment, pilot proposal, glossary, decision register, source register, and this log.
- Created directories for sources, theory, protocols, experiments, data, software, tests, and publications.
- Archived unmodified copies of the source PDFs with SHA-256 hashes and page counts.
- Checked expected deliverables, archive integrity, and relative Markdown file links.

**Main findings:** a focused research program is feasible; the material does not yet establish a distinct discipline or validated theory. Prior research overlaps substantially. The exact proposed name has other public uses. Measurement, independent outcomes, and early validation are the main priorities.

**Implemented decisions:** English authored content, preserved source material, explicit source/request separation, and a self-contained project under outputs/narrative-systems-engineering. Research scope, name, and pilot thresholds remain proposed.

**Limitations:** no systematic literature review, full translation, empirical experiment, working analysis engine, participant recruitment, publication, or registration completed. No founder identity or resource assumptions were treated as confirmed.

**Next session:** review the feasibility judgment, set resources and naming direction, adopt or revise PILOT-001, and develop the first codebook with two examples.

## Session 002 - September 8, 2026

**Objective:** incorporate the founder's direct account of the original idea into the project foundation.

**Input:** the founder's Spanish-language statement describing integrated analysis of films and videos across language, semantics, voice, landscapes, color, clothing, and recurring objects.

**Work performed:** recorded an English rendering and a distinct operational interpretation in docs/ORIGINAL_IDEA.md; drafted a candidate measurement inventory; updated the README mission, roadmap, feasibility scope note, foundational review, pilot scope note, and decision register.

**Clarification:** the original project begins with discovering what can be measured in audiovisual works and how measurements can support interpretation. Audience-response prediction and social simulation are later extensions. The earlier event/goal/relationship pilot is narrower than the original idea and remains optional.

**Proposed next artifact:** one time-aligned audiovisual measurement profile using a small subset of linguistic and visual/material features. This would be method development, not validation on its own.

**Terminology:** proposed integrated multimodal analysis or cross-modal synthesis for combining analytical layers; reserve meta-analysis for statistical synthesis across studies.

**Checks:** reviewed the updated scope across documents and checked relative Markdown links. No experiment, new literature search, full source translation, or media analysis was performed in this session.

**Next actions:** choose a short work, specify a small measurement subset, and develop the profile before selecting a validation protocol. Final naming and resources remain unresolved.

## Session 003 - September 8, 2026

**Objective:** place the project files directly in the repository directory, as requested by the founder.

**Work performed:** copied all project documentation, source PDFs, and project directories from outputs/narrative-systems-engineering into the workspace root. Replaced the root placeholder README with the project README and updated the active-location decision.

**Location:** the repository root is now the active project directory. The earlier outputs copy remains a historical snapshot. The attempted move was blocked by automatic approval policy; a non-destructive copy succeeded.

**Checks:** confirmed root files and project directories exist and checked relative Markdown file links. No commit or remote publication was requested or performed.

## Session 004 - September 8, 2026

**Objective:** begin coding the audiovisual profile pipeline using .NET, with Azure Speech and Foundry as future integrations.

**Input:** the founder requested implementation and supplied `video/268230_medium.mp4`; no Azure models are deployed.

**Work performed:** added a .NET 10 console application; downloaded a project-local FFmpeg build after network escalation approval; implemented media probing, source SHA-256, optional PCM audio extraction, budgeted midpoint frame sampling, RGB color-bin measurements, JSON/CSV export, and an offline HTML report. Added usage documentation, synthetic-media integration checks, and ignore rules for media, tools, builds, secrets, and generated runs.

**Findings:** the supplied file is 12.08 seconds, 2560x1440, and has no audio stream. The first run produced three frame samples at requested positions 2.50, 7.50, and 11.04 seconds. Visually inspected the first extracted frame. No speech analysis is possible on this input alone.

**Checks:** .NET build succeeded without warnings or errors. Synthetic red-video and audio-tone integration checks passed for color bins, timestamps under a frame budget, source hash, no-audio handling, mono 16 kHz PCM16 extraction, overwrite protection, and invalid interval rejection. The real-video run completed successfully; generated artifacts are under `data/processed/runs/` and excluded from Git.

**Limits:** requested seek positions are not verified presentation timestamps; sampled colors describe individual resized frames. No Azure calls, model annotations, acoustic measurements beyond media metadata, transcription, object recurrence detection, or research validation occurred. The pipeline's empty annotation and interpretation fields are placeholders.

**Next actions:** review the visual profile and define a bounded visual annotation codebook; add Azure Speech configuration and integration when a speech-containing example and Azure resource are available. Preserve the original media and review model annotations independently when introduced.

## Session 005 - September 8, 2026

**Objective:** implement the annotation schema and manual review workflow approved by the founder.

**Work performed:** added the `visual-0.1` codebook and JSON schema; implemented `review` and `import-review` CLI commands; created an offline frame review page with evidence labels, uncertainty, reviewer provenance, JSON download/load, and partial review support. Imports bind to the exact original profile and source hashes and create a derived profile plus a standalone read-only annotation report. Original measurement outputs are preserved.

**Deliverable:** `data/processed/runs/visual-review/review.html` contains the three frames from the first video profile, initially unreviewed. No human annotations or independent reference labels were fabricated. Azure integration remains pending.

**Validation:** synthetic-media integration checks cover review round trips, preserved measurements, partial/complete review coverage, portable report evidence, and invalid annotation rejection. A separate Node check validates page script syntax and export validation cases. Browser rendering could not be checked: the browser URL policy rejected access to the local HTML file, and no alternate browser route was attempted.

**Next actions:** the founder reviews the three frames, downloads annotations, and imports them into a new derived profile. Use those reviewed examples to refine the codebook before connecting an Azure vision model or treating labels as evaluation references.

## Session 006 - September 8, 2026

**Objective:** process the additional video identified by the founder, `video/06-FCE-V-C110-007-C.mp4`.

**Work performed and findings:** ran the existing local pipeline successfully. The video is 194.84 seconds long and has an audio stream. Extracted mono 16 kHz PCM16 WAV audio, sampled 39 frames at the default five-second interval, and generated color measurements, provenance, JSON/CSV, and an HTML report. Created a separate offline review workspace with all categories initially unreviewed.

**Outputs:** `data/processed/runs/06-FCE-V-C110-007-C-profile/` and `data/processed/runs/06-FCE-V-C110-007-C-review/`. Both completed successfully. Original videos and previous runs were preserved.

**Limits and next actions:** audio presence alone does not establish speech content. No listening, transcription, Azure calls, or human annotation was performed. Review the frames and configure transcription for this extracted audio when the service is available.

## Session 007 - November 6, 2026

**Objective:** revise claims across the preprint, experiment findings, and release metadata, and change the license, at the founder's request.

**Work performed:** reviewed `argon/` and PAPER-001 against the code. Confirmed that EXP-004 generates its target trajectories with the model itself (`SystemIdentifier.cs`, `TrueBeta0`), so its parameter recovery and leave-one-corpus-out fits verify the estimator, not the model; that EXP-003 results are simulations of the model; that stance and anchoring labels are single-coder; and that `VisualDynamicsAnalyzer.cs` marks Window #0 as a cut by convention, so the reported cut counts (4, 3, 1) correspond to 3, 2, and 0 flagged transitions. Rewrote PAPER-001 as v1.1 (Markdown and LaTeX; PDF rebuilt from LaTeX in one-column layout to fix overflowing equations), added status notes to EXP-001 to EXP-004, `argon/README.md`, and `FORMALIZATION.md`, added a "What is and is not established" table to the root README, and corrected the EXP-003 Challenger perturbation statement (later injections produce larger simulated drops). Changed the license to all rights reserved (D019, D020) and bumped the version to 0.4.1. Numerical outputs are unchanged.

**Not changed:** `publications/latex/PAPER-001-NSE-FOUNDATION-PRINT.html` and the PDF text hard-coded in `PaperManuscriptRenderer.cs` still carry v1.0 wording; the LaTeX source is now the reference for the PDF. The Zenodo record for v0.4.0 keeps its Apache 2.0 license and description.

**Next actions:** second coder for stance labels on CORPUS-001; annotated shot boundaries; decide the license setting for the next Zenodo release.

## Session 008 - November 9, 2026

**Objective:** design EXP-005 ("Signal–Perception Gap: lip-sync and playback detection") and implement its preregistration protocol, synthetic ground-truth calibration stimuli, C#/.NET 10 signal detector, and blinded human observer response form, without running a human participant study.

**Inputs reviewed:** `README.md` ("What is and is not established"), `SESSION_LOG.md` (Session 007), `docs/DECISIONS.md`, `argon/README.md`, and `data/raw/CORPUS_MANIFEST.json`.

**Work performed:**

- Authored `research/protocols/EXP-005-PROTOCOL.md` specifying the research questions, falsifiable hypotheses ($H_1, H_2, H_3$) and null hypotheses, explicit dismissal criteria, self-recorded stimulus acquisition protocol (`CORPUS-005-JAIBO-LIVE-VS-PLAYBACK`, vocals + guitar with clapboard sync impulse and FFmpeg `-itsoffset` commands for $-120, -60, -30, 0, +30, +60, +120, +200\text{ ms}$), and pre-specified statistical analysis plan defining the Signal–Perception Gap metrics ($\text{SPG}_{\tau}$ in ms and $\text{SPG}_{\text{playback}}$ in accuracy difference).
- Implemented `argon/src/Argon.Engine/LipSyncSignalDetector.cs` and wired CLI subcommands (`exp005`, `exp005-video`, `exp005-analyze`) in `argon/src/Argon.Engine/Program.cs`. The detector estimates audio–video lag from raw `60 fps` `32x32` RGB24 mouth-ROI frame sequences and `16 kHz` PCM16 WAV waveforms using two independent signal methods (5 ms positive envelope cross-correlation with parabolic interpolation and bilabial closure-to-burst onset matching) and evaluates playback repetition against a studio reference via aligned envelope correlation and inter-onset timing drift ($\sigma_{\text{IOI}}$).
- Generated the 9-stimulus synthetic calibration suite (`argon/experiments/EXP-005-signal-perception-gap/stimuli/`) and recorded all SHA-256 hashes and the pending real-video recording specification in `data/raw/CORPUS_MANIFEST.json`.
- Built the standalone blinded observer evaluation form (`research/protocols/exp005-observer-form.html`) that locks judgments (`live` vs. `playback`), synchrony ratings ($1\text{–}5$), anomaly flags, confidence, and free-text reasons with UTC timestamps prior to any explanation or measurement disclosure.
- Proposed decision `D021` in `docs/DECISIONS.md`.

**Findings:** on the 9 synthetic ground-truth stimuli (8 playback offsets from $-120\text{ ms}$ to $+200\text{ ms}$ plus 1 simulated live take with $24\text{ ms}$ natural micro-timing drift), `LipSyncSignalDetector` achieved Mean Bias $= -5.21\text{ ms}$, $\text{MAE} = 5.21\text{ ms}$ (max absolute error $8.76\text{ ms}$), $\text{RMSE} = 5.43\text{ ms}$, $\text{MDL}_{\text{machine}} = 10.86\text{ ms}$, and Pearson $r = 0.99990$, while separating the 8 playback tracks ($\sigma_{\text{IOI}} = 0.00\text{ ms}$, $r_{\text{rep}} = 0.995$) from the simulated live take ($\sigma_{\text{IOI}} = 18.95\text{ ms}$, $r_{\text{rep}} = 0.881$). These numbers calibrate the signal estimator on synthetic inputs only; no real camera recordings or human observer data have been collected.

**Decisions adopted:** none (`D021` is recorded as `Proposed` for founder decision).

**Next actions:** the Principal Investigator records `studio_master.wav`, `live_raw.mp4`, and `playback_raw.mp4` under `video/exp005/`, generates the 8 FFmpeg lag variants, runs `exp005-video` to verify detector error on real camera footage before freezing the protocol, and decides whether to adopt `D021`.

## Template for future sessions

```text
Session ID and date:
Objective:
Inputs reviewed:
Work performed:
Findings:
Files created or changed:
Checks and results:
Decisions adopted:
Proposals and unresolved questions:
Next actions:
```
