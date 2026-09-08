# PILOT-001: Reliability of a bounded narrative representation

Status: proposed; not preregistered or executed. Version 0.1, September 8, 2026.

## Scope note after the founder's clarification

This protocol covers only a narrative subset of the broader audiovisual inquiry. It remains proposed. First develop a measurement inventory and a one-work audiovisual profile; then decide whether this protocol is the most useful validation study or should be replaced by a cross-modal measurement protocol. See [the original idea](../../docs/ORIGINAL_IDEA.md).

## Purpose

Test the measurement layer needed by HNA before making claims about cognition or social dynamics. The research unit is a narrative event within a short audiovisual work. A narrative event is provisionally a depicted or stated change involving an entity; the codebook must resolve implied events and repetitions.

## Research questions

RQ1: Can independent annotators identify selected events, goals, and relationships with adequate agreement under a shared codebook?

RQ2: Can a bounded computational method recover those labels and evidence spans on works excluded from development, and does it improve on a simple baseline?

This pilot does not measure real audience emotion, beliefs, persuasion, social diffusion, or controllability.

## Proposed corpus and split

Use 6-10 short audiovisual works, approximately 2-5 minutes each, in one language and a limited genre range. Record source, version, duration, provenance, and permitted uses. Prefer material the project can lawfully analyze and document. No titles have been selected or acquired.

Allocate two works to codebook and system development. Freeze the codebook, prompts, model versions, matching rules, and metrics before annotating or evaluating the remaining 4-8 works. Keep each work entirely in one partition. This small sample supports feasibility assessment, not population-level generalization.

## Annotation protocol

Use two independent human annotators for both development and evaluation; resolve development disagreements collaboratively, but preserve original evaluation annotations before adjudication. Machine labels are produced separately and cannot be presented as an independent human annotator.

For each event, record:

- Work and segment identifiers, start and end seconds.
- Entities involved and a short description of the observable event.
- A small categorical event label defined in the codebook.
- Any explicit goal and its evidence; distinguish inferred goals and unknown goals.
- A typed relationship, with explicit evidence or an interpretation flag.
- Evidence location, annotator identifier, codebook version, and uncertainty flag.

Do not include a global complexity or energy score. Avoid broad psychological classifications. Start with a small label set established through the two development works.

## Evaluation

For time spans, match events one-to-one using a frozen temporal intersection-over-union threshold of 0.5, then report precision, recall, and F1. Also inspect split and merged events. The threshold is a proposed convention, not a validated universal rule.

For categorical agreement, use common analysis units fixed before independent labeling, report raw agreement and Krippendorff's alpha by label family, and include label prevalence. Treat inference flags separately. If alpha is undefined because of no variation, report it as undefined and inspect the label; do not substitute a perfect score.

For machine evaluation, report macro-F1 by label family against an adjudicated reference, but also compare against each original annotator and preserve disagreements. Report evidence-span correctness and unsupported assertions in an error audit. For spans use the matching rule above. Aggregate first by work and also show individual-work results.

Use a majority-label baseline for categorical tasks and a simple transcript-based or rule-based extraction baseline. Compare the candidate system on exactly the same held-out works. Freeze a single primary comparison before evaluation; exploratory analyses must be labeled.

## Provisional decision rules

These are local screening rules to debate and freeze before the evaluation, not discipline-wide standards:

- Categorical alpha at least 0.80: retain the label family for the next pilot, subject to prevalence and error review.
- Alpha from 0.67 to below 0.80: exploratory use only; revise before stronger claims.
- Alpha below 0.67: do not use the construct for predictive claims; revise or remove it.
- All released annotations must have a source reference and required metadata. Missing provenance fails the release check.
- For RQ2, propose an absolute improvement of at least 0.05 in macro-F1 over the stronger frozen baseline as a practical target. Report work-level paired differences and uncertainty; an unstable improvement is inconclusive.

A small set of works may produce wide uncertainty. Do not treat the target as statistically established merely because a point estimate crosses it. A confirmatory follow-up needs sample-size justification based on pilot variability and the smallest useful effect.

## Deliverables

Corpus manifest, versioned codebook, independent annotations, adjudication notes, frozen system configuration, evaluation script, per-work results, failure examples, and effort/cost log.

## Later extension

If measurement is usable, develop a separate protocol asking whether selected features predict event recall or another defined human response. Obtain independent participant data, specify participant and stimulus sampling, and determine the required sample size. That study is not included in this pilot or authorized for recruitment by this document.
