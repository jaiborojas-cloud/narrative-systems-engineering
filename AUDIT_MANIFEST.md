# Cryptographic Provenance & Reproducibility Audit Ledger (`AUDIT_MANIFEST.md`)

**Author, Creator & Principal Investigator:** Jair Ivan Rojas Ruiz (*Jair Rojas*)  
**ORCID iD:** [https://orcid.org/0009-0003-1109-4583](https://orcid.org/0009-0003-1109-4583)  
**Zenodo DOI:** [https://doi.org/10.5281/zenodo.23987656](https://doi.org/10.5281/zenodo.23987656) (Concept DOI: [`10.5281/zenodo.23987655`](https://doi.org/10.5281/zenodo.23987655))  
**Repository:** [https://github.com/jaiborojas-cloud/narrative-systems-engineering](https://github.com/jaiborojas-cloud/narrative-systems-engineering)  
**Release Version:** `v0.1.0` (November 5, 2026)

---

## 1. Purpose of This Ledger

This document provides an immutable, cryptographically verifiable chain of custody linking:
1. **The Original Foundational Conception** (`sources/foundational/`),
2. **The Mathematical Formalization & Preprint** (`publications/PAPER-001-NSE-FOUNDATION.md` & `argon/theory/FORMALIZATION.md`),
3. **The Deterministic Computational Engine** (`src/Nse.Pipeline` & `argon/src/Argon.Engine`),
4. **The Copyright-Clean Benchmark Video Corpus** (`data/raw/CORPUS_MANIFEST.json`), and
5. **The Exact Computed Telemetry Outputs** (`EXP-001` & `EXP-002`).

Any external auditor, peer reviewer, or R&D laboratory can verify file integrity and reproduce every number in `PAPER-001` with a single command:
```powershell
powershell -ExecutionPolicy Bypass -File argon/tests/Verify-Argon.ps1
```

---

## 2. Foundational Source Documents (SHA-256)

| File Path | Pages | SHA-256 Hash |
|---|---|---|
| `sources/foundational/fundationla-doc-1.pdf` | 19 | *(See [`sources/foundational/MANIFEST.md`](sources/foundational/MANIFEST.md))* |
| `sources/foundational/fundationla-doc-2.pdf` | 20 | *(See [`sources/foundational/MANIFEST.md`](sources/foundational/MANIFEST.md))* |
| `sources/foundational/fundationla-doc-3.pdf` | 24 | *(See [`sources/foundational/MANIFEST.md`](sources/foundational/MANIFEST.md))* |

---

## 3. Benchmark Audiovisual Corpus (`data/raw/CORPUS_MANIFEST.json`)

All active benchmark inputs have verified `SHA-256` hashes bound inside both `profile.json` and `dynamics.profile.json`:

| Corpus ID | File Name | Duration | License / Provenance | Source SHA-256 |
|---|---|---|---|---|
| `CORPUS-001` | `video/06-FCE-V-C110-007-C.mp4` | $194.84\text{s}$ | Archival Research Documentary (Spain, c. 1994) | `cbe05bdfc6131d85e9a749884a68fdf64f3aa12948cc89ff9cbe02f3c2a0e293` |
| `CORPUS-002` | `video/LBJ_Daisy_1964.ogv` | $59.83\text{s}$ | Public Domain (Library of Congress / Wikimedia Commons) | `e09dbeb6ec9e5db43cee89feb577a59e0eff1ce5de58102c01e5c0b84240bda8` |
| `CORPUS-003` | `video/Reagan_Challenger_1986.webm` | $258.93\text{s}$ | Public Domain (17 U.S.C. § 105 / NARA / Reagan Library) | `aedfaa61cddcf91537e956a4233008b32e98333ae2d207c322479a0aa7b17bcd` |

---

## 4. Authorship, Theory, Engine & Experiment Artifacts (SHA-256)

| Artifact Path | Role | SHA-256 Hash |
|---|---|---|
| `AUTHORS.md` | Intellectual priority declaration | `4f2998bbb28c44e396008c791de632b140ccdc0339f4b588109611a307ff5058` |
| `CITATION.cff` | GitHub/Scholar citation metadata | `8ba152a90474d7f2b4a2fe07b97789bda3ad255d3401190ecab1c883a8a11204` |
| `.zenodo.json` | Zenodo DOI creator & license metadata | `9cee13b1d902956bcb912c33ff7a6c7c655eb74f2e7aa7dedfb64b27a5f17e93` |
| `LICENSE` | Apache License 2.0 | `3a0986f32e0fb5bda014f3e2eede2a39ac4470b0d1012891941a5b89ba43630d` |
| `NOTICE` | Mandatory Section 4(d) attribution | `fbc74a1cefc9755179b46912ff14b87438ad3b65024f7e720d85a1f992de78d3` |
| `publications/PAPER-001-NSE-FOUNDATION.md` | Foundational Preprint v1.0 | `14cca6e96d9fd5b29e42fa07aaaacc36e3798bb61c1e1ae45afd77cb0ed5fe46` |
| `argon/theory/FORMALIZATION.md` | Mathematical specification of the 6 NSE metrics | `645368dd11854c7a0a65b7a1893d1044706e6f71ebb2db49764e17bd6be13f31` |
| `argon/src/Argon.Engine/Program.cs` | Multimodal engine CLI orchestrator | `f7ad2dc26fff3629951235c6a5030f50cb1ff3e7d13c95ce1808e32c59fbc4d5` |
| `argon/src/Argon.Engine/AcousticAnalyzer.cs` | 16 kHz PCM16 prosodic & VAD analyzer | `d37de9fecb2d6815bd4d765870d37f05cb8503a7b1350c5faecf208134600805` |
| `argon/src/Argon.Engine/VisualDynamicsAnalyzer.cs` | BT.709 luminance, HSV saturation & JSD cuts | `cefe0908143520b18e3d7d96c7b58d2f652ac04d99bcf9222e9ca02fdee567fa` |
| `argon/src/Argon.Engine/LinguisticAnalyzer.cs` | Bilingual lexical/normative cadence analyzer | `ab7cbca0b11f99401a27c5b3efe8132e9e7f802c395fca97d0abaf5d9506cbf3` |
| `argon/src/Argon.Engine/NarrativeMetricsEngine.cs` | Core NSE metrics, LTV state-space & Gramian $W_c$ | `7776b564872ba9587cff8e3e97d0b478c5734a7be22d5554efc66ff561b6f5f1` |
| `argon/experiments/EXP-001-insumision-dynamics/telemetry.csv` | `CORPUS-001` 39-window telemetry | `2642ea042fb9e3cf4586b0e6c16f2e1b5299b76de809ca4f4fc654a37894a7ac` |
| `argon/experiments/EXP-002-public-domain-atlas/daisy-run/telemetry.csv` | `CORPUS-002` 12-window telemetry | `a24dae32ed29817c67e82afdf5a13209c50f3c240ec2cdde85a5ceef2406fcaf` |
| `argon/experiments/EXP-002-public-domain-atlas/challenger-run/telemetry.csv` | `CORPUS-003` 26-window telemetry | `d8f07312a53f6e1f41bba627c5e8e280d1a2b61f9679cc1919451600ef3ef673` |

---

## Addendum: v0.4.1 (November 6, 2026)

The hashes above record the v0.1.0 state. For v0.4.1 the license changed to all rights reserved (decision D019) and claims were revised (D020); numerical outputs are unchanged. Current hashes of the changed files:

| Artifact Path | SHA-256 Hash |
|---|---|
| `LICENSE` | `d67c71bd870fc14027f5b828babc102e3d26bb7a841f917c5768f304c3eb95a4` |
| `NOTICE` | `37e3677079bfcaf9dabd51b7ffa339cb139a70183732d220a56f2ea7ce75e856` |
| `CITATION.cff` | `ca43dffbcecdb7bd28387f92f91543784e93e09b97d528aadf924a6e115f9048` |
| `.zenodo.json` | `fa99fb88252eb9e95af032122e1dbb9025c2206c9d6a6460c35ce8d4812068d4` |
| `README.md` | `205f7ac7677ceb8e971f87d4de5ae7a90446d8553f4915480776603d7b8db9ae` |
| `publications/PAPER-001-NSE-FOUNDATION.md` | `4a19913d5afa564f8aa9af5916e89e472e5df84a67b87acfa1dc98f35b4e5a31` |
| `publications/latex/PAPER-001-NSE-FOUNDATION.tex` | `47d80cd55c752c61dd8996156d9650923a2b4f27f8641fc2666ff14fb7e62d93` |
| `publications/PAPER-001-NSE-FOUNDATION.pdf` | `d2fa509e4edb44c9d7529e91670a03a462f719926171a52cbd2e1137cf8ec2c8` |
| `argon/theory/FORMALIZATION.md` | `19690cb58fa4a971deb935d68f61fa4ac7379ea3a14246917f9f4cfcd19620d9` |
