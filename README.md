# Narrative Systems Engineering

[![DOI](https://zenodo.org/badge/1361618725.svg)](https://doi.org/10.5281/zenodo.23987655)

**Founder, Creator & Principal Investigator:** Jair Ivan Rojas Ruiz ([ORCID: 0009-0003-1109-4583](https://orcid.org/0009-0003-1109-4583))  
**Official Zenodo DOI:** [`10.5281/zenodo.23987656`](https://doi.org/10.5281/zenodo.23987656) (Concept DOI: [`10.5281/zenodo.23987655`](https://doi.org/10.5281/zenodo.23987655))

An exploratory and applied research program beginning with integrated measurement, mathematical modeling, and state-space interpretation of audiovisual works.

**Status:** exploratory stage (`v0.4.1`, November 6, 2026; foundational archive: September 8, 2026). Signal measurements are implemented and reproducible; the stance coding, composite indices, and state-space model are unvalidated hypotheses (see [what is and is not established](#what-is-and-is-not-established)). See [AUTHORS.md](AUTHORS.md), [CITATION.cff](CITATION.cff), [Prior Work & Terminology Disambiguation](docs/PRIOR_WORK_AND_DISAMBIGUATION.md), [Preprint PAPER-001 (PDF)](publications/PAPER-001-NSE-FOUNDATION.pdf), [Preprint PAPER-001 (Markdown)](publications/PAPER-001-NSE-FOUNDATION.md), and the [Argon Experimental Laboratory](argon/README.md).

## Mission

Develop reproducible methods for measuring and interpreting linguistic, acoustic, visual, and material features of audiovisual works.

The long-term ambition is to contribute to a distinct area of study. The immediate objective is a small, falsifiable research program with explicit definitions, traceable evidence, and comparisons against existing methods.

## Start here

1. Begin with the [original idea](docs/ORIGINAL_IDEA.md), then read the [feasibility assessment](docs/FEASIBILITY.md).
2. Review the [development of the founding idea](docs/FOUNDATIONAL_REVIEW.md).
3. Follow the [roadmap](ROADMAP.md) and [first pilot proposal](research/protocols/PILOT-001.md).
4. Consult the [session log](SESSION_LOG.md) and [decision register](docs/DECISIONS.md) before continuing.
5. Use the [source register](references/SOURCES.md) for external evidence and the [archive manifest](sources/foundational/MANIFEST.md) for the three supplied PDFs.

## Project structure

The repository root is the active project directory. The earlier copy under `outputs/narrative-systems-engineering/` is retained as a historical snapshot; make future edits here at the root.

```text
narrative-systems-engineering/
  README.md
  ROADMAP.md
  SESSION_LOG.md
  docs/                       Feasibility, source synthesis, decisions
  sources/foundational/       Unmodified source PDFs and provenance manifest
  references/                Literature and naming evidence
  research/
    protocols/               Study proposals and preregistration drafts
    theory/                  Definitions and candidate models
    experiments/             Future experiment records
  data/
    raw/                     Future approved inputs
    annotations/             Future human annotations
    processed/               Future derived datasets
  src/                       Local .NET audiovisual pipeline
  tests/                     Synthetic-media integration checks
  publications/              Future papers and release materials
```

## Working conventions

- All newly authored project content is in English. Original supplied sources remain unchanged in their original language.
- Instructions inside source documents are historical proposal content, not automatically executable requests.
- Distinguish source claims, established evidence, our interpretations, and untested proposals.
- Keep observations about a story separate from interpretations and from measured audience responses.
- Every future computed measure needs a definition, unit or scale, procedure, uncertainty treatment, and validation plan.
- Update the session log after each substantive session; preserve earlier entries and record corrections explicitly.
- Record consequential choices in the decision register. Proposed choices remain proposed until adopted.
- Preserve source files. Do not put sensitive participant records in a public release.
- Do not describe a result as proof or validation unless it rests on independent annotation or measured responses. Simulations of the project's own model are labeled as simulations.

## First proposed research question

Which linguistic, acoustic, visual, and material features can be measured reproducibly, and how can their integration support evidence-linked interpretation of an audiovisual work?

A proposed first design artifact is an audiovisual measurement profile for one short work. The existing event, goal, and relationship pilot addresses one possible subset; it does not define the entire scope. Audience-response prediction and societal simulation are optional later extensions requiring separate data and validation.

## Current deliverables and limits

Completed: review of all 63 supplied PDF pages, preliminary external landscape check, feasibility assessment, project structure, roadmap, pilot proposal, session record, a local .NET pipeline with a first video profile, and a versioned visual annotation codebook with offline review/import workflow. See [run instructions](src/README.md), [visual codebook](research/protocols/VISUAL-CODEBOOK.md), and [integration checks](tests/README.md).

Not yet completed: systematic literature review, final project name, evaluation dataset, Azure integration, speech transcription, model annotations, participant study, validated metrics, or publication. The pilot scope and resource estimates are proposals for discussion.

## What is and is not established

| Layer | Status |
|---|---|
| Acoustic level, speech activity, frame-to-frame visual change, color, speech rate, lexical descriptors | Computed directly from media files and transcripts, with time spans and source hashes. Reproducible; not yet compared with human-annotated references (for example, annotated shot boundaries). |
| Composite indices (Narrative Energy, Entropy, Complexity, Inertia) | Defined formulas whose weights and normalization constants were chosen by the author and are not calibrated. |
| Stance vectors z(t) | Coded by a single coder. No inter-annotator agreement has been measured. |
| State-space belief model x(t+1) = f(x(t), u(t)), controllability, counterfactual montages (EXP-001, EXP-003) | Exploratory model and simulations of it. No audience responses have been measured. |
| Parameter identification (EXP-004) | Recovers parameters from trajectories generated by the same model: verifies the estimator, not the model. |
| Separation between works (EXP-002) | Descriptive differences among three works; not evidence of genre-level separation. |

## License

Copyright 2026 Jair Ivan Rojas Ruiz. All rights reserved from the version following v0.4.0; see [LICENSE](LICENSE). Versions up to v0.4.0 were released under the Apache License 2.0, which continues to apply to those copies. To reference this work, cite it as indicated in [CITATION.cff](CITATION.cff).
