# Visual annotation codebook

Version `visual-0.1` — design-stage protocol, September 8, 2026.

## Unit and scope

Annotate each sampled frame independently. The frame path and requested timestamp identify the evidence; they are not a scene interval or an exact decoded presentation timestamp. Review the visible image, not what might occur elsewhere in the clip. This first codebook describes visible content only; narrative meaning remains a separate interpretation.

| Category | Include | Avoid |
|---|---|---|
| objects | Visible non-garment items, with shape or position when useful | Speculative brands, contents, or functions |
| clothing | Visible garments/accessories; refer to people by frame position | Inferring identity, profession, class, or material without evidence |
| setting | Visible environmental features, such as interior walls and a display | Assuming a filming location or organization |
| actions | Poses and visible interactions, such as holding an item | Inferring motion, speech, intention, or emotion from a still |

Example observation: label `rectangular display`; evidence `A bright rectangular surface with chart-like marks occupies the left edge`; certainty `clear`. This is an illustration of the rubric, not a completed human annotation.

## Review states

- `not_reviewed`: no judgment; observations must be empty.
- `none_visible`: reviewer inspected the frame and found no codable instance in this category; observations must be empty. This is not an assertion about the entire clip.
- `observed`: one or more observations, each with a nonempty label, visible evidence, and `clear` certainty.
- `uncertain`: one or more observations; at least one must have `uncertain` certainty. Explain ambiguity in its evidence field (for example, an item is partly occluded).

Certainty is a qualitative reviewer judgment, not a probability. A partial review is allowed: leave untouched categories `not_reviewed`. Blank observations cannot be exported. A completed review may contain uncertain judgments; completion measures coverage, not agreement or correctness.

## Provenance and format

The [JSON schema](annotation.schema.json) defines the file format. The importer also checks profile/source hashes, unique frame references, exact timestamps, and exactly one entry for each category. Schema validation alone cannot establish those relationships.

Each document includes a reviewer identifier and provenance kind (`human` or `ai_assisted`). Use `ai_assisted` if AI helped draft the annotations, even if a person later reviews them. This is a self-reported provenance field, not identity verification. Do not treat AI-created examples as independent human reference data.

The profile SHA-256 binds annotations to the exact input profile bytes. Preserve that file to resume/import a review. Frame references resolve relative to that measurement profile. The imported `profile.reviewed.json` preserves measured values and original relative evidence paths, and explicitly records `annotationReview.evidenceBaseDirectory` and `measurementProfilePath`. Its standalone HTML report embeds frame images for portability; the JSON profile's original media paths remain local.

## First review workflow

1. Generate a review workspace from an existing profile with the `review` command.
2. Open `review.html` in a browser, enter your identifier and provenance, and inspect each frame.
3. Add labels with visible evidence and set the category review state. Download annotations to save; drafts are not automatically persisted. Downloaded partial reviews can be loaded again.
4. Import the downloaded JSON using `import-review`. The importer writes a new derived profile and standalone read-only annotation report, preserving the original run.
5. For later model evaluation, create an independent model annotation set and compare it with reviewed human references. Three sampled frames support workflow development, not validated accuracy claims or object tracking.

Object identity across frames, bounding boxes, motion analysis, inter-annotator agreement, and Azure model execution are out of scope for this version.
