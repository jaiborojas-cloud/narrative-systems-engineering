# Argon Experimental Laboratory (`argon/`)

**Founder, Creator & Principal Investigator:** Jair Ivan Rojas Ruiz ([ORCID: 0009-0003-1109-4583](https://orcid.org/0009-0003-1109-4583))
**Created:** November 5, 2026
**Purpose:** Experimental engineering laboratory for operationalizing, testing, and falsifying the mathematical formulations of **Narrative Systems Engineering (NSE)** and **Computational Narrative Dynamics (CND)** originated by Jair Ivan Rojas Ruiz on real multimodal signals.

## Design Philosophy

Whereas the root repository (`src/Nse.Pipeline`, `research/protocols/`) establishes the conservative baseline (frame sampling, RGB histograms, manual visual review), `argon/` builds the **bridge** between raw audiovisual observables and the dynamic cognitive/social system formulations of Foundational Documents 1–3:

1. **Zero Hallucinated Metrics:** Every number in `argon/` is either:
   - Directly computed from raw bytes (`PCM16` audio samples, `RGB24` rasters, verified transcript tokens), or
   - Derived from explicit, auditable mathematical formulas with stated units (e.g., bits for Shannon entropy, dBFS for acoustic RMS, JS-divergence for visual cuts, normalized state vectors for belief dynamics).
2. **Traceable Grounding:** All extracted narrative structures (archetypes, beliefs, stances, speech acts) link directly to exact `[startSeconds, endSeconds]` spans and frame indices.
3. **Executable Falsifiability:** Mathematical models of narrative energy, entropy, inertia, and control (`x(t+1) = f(x(t), u(t))`) are implemented as runnable code that produces testable predictions and interactive visual telemetry.

## Directory Structure

```text
argon/
  README.md                         This manifest and architecture overview
  theory/
    FORMALIZATION.md                Rigorous mathematical re-specification of the 6 core NSE metrics
  src/
    Argon.Engine/                   C#/.NET 10 Multimodal Narrative Dynamics & Control Engine
      Argon.Engine.csproj
      Program.cs                    CLI entry point & pipeline orchestrator
      AcousticAnalyzer.cs           PCM16 WAV windowing, RMS energy, dBFS, ZCR, pause/speech cadence
      VisualDynamicsAnalyzer.cs     Perceptual luminance/saturation, inter-frame JSD, scene cut detection
      LinguisticAnalyzer.cs         Time-aligned transcript, lexical density, speech rate, stance & belief graph
      NarrativeMetricsEngine.cs     Formal calculation of E(t), H(t), Inertia, Complexity, and Control x(t+1)=f(x,u)
      DashboardRenderer.cs          Self-contained interactive HTML5/Canvas/SVG multimodal telemetry report
  experiments/
    EXP-001-insumision-dynamics/    Full multimodal + narrative dynamics run on 06-FCE-V-C110-007-C.mp4
      transcript.aligned.json       Time-aligned Spanish speech segments, speakers, and belief stances
      dynamics.profile.json         Complete computed time-series u(t), state trajectories x(t), and indices
      telemetry.csv                 Second-by-second / window-by-window numerical export
      dashboard.html                Interactive visual & mathematical narrative control laboratory
      FINDINGS.md                   Analytical report comparing the 5 real speakers and testing NSE hypotheses
  tests/
    Verify-Argon.ps1                Automated verification of mathematical bounds, invariants, and reproducibility
```
