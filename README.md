# Narrative Systems Engineering

An exploratory research program beginning with integrated measurement and interpretation of audiovisual works.

**Status:** foundation stage, version 0.1, September 8, 2026. The name is provisional and already has other public uses. This project does not yet claim to have established a new discipline, validated a theory, or demonstrated scientific priority.

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
- Publication, repository visibility, authorship details, and licensing remain future decisions. Nothing has been published by this setup.

## First proposed research question

Which linguistic, acoustic, visual, and material features can be measured reproducibly, and how can their integration support evidence-linked interpretation of an audiovisual work?

A proposed first design artifact is an audiovisual measurement profile for one short work. The existing event, goal, and relationship pilot addresses one possible subset; it does not define the entire scope. Audience-response prediction and societal simulation are optional later extensions requiring separate data and validation.

## Current deliverables and limits

Completed: review of all 63 supplied PDF pages, preliminary external landscape check, feasibility assessment, project structure, roadmap, pilot proposal, session record, and a local .NET pipeline with a first video profile. See [run instructions](src/README.md) and [integration checks](tests/README.md).

Not yet completed: systematic literature review, final project name, evaluation dataset, Azure integration, speech transcription, model annotations, participant study, validated metrics, or publication. The pilot scope and resource estimates are proposals for discussion.
