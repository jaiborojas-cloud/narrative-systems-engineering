# Preliminary feasibility assessment

September 8, 2026 | Version 0.1 | Decision support for starting the project

## Scope clarification after the initial assessment

The founder subsequently supplied the [original idea](ORIGINAL_IDEA.md): explore what can be measured across the linguistic, acoustic, visual, and material dimensions of audiovisual works and integrate the results to support interpretation. This is the immediate project center. The assessment below also examines the more ambitious extensions in documents 2 and 3; their unresolved problems do not prevent the original audiovisual inquiry from proceeding.

The recommended first step is now a measurement inventory and a one-work audiovisual profile for method development. The existing narrative-annotation pilot is one possible follow-up subset. Audience-response prediction is optional, not a necessary condition for success of the initial framework. All feasibility ratings remain preliminary, with no new empirical validation conducted.

## Assessment

**Proceed with a focused, falsifiable research program.** The supplied documents provide a coherent ambition and a broad list of questions. They do not yet support a claim that a new discipline has been founded, that universal narrative quantities have been discovered, or that society can be reliably predicted or controlled from narrative inputs.

The strongest potential contribution is a reproducible framework connecting narrative representations, explicit measurement uncertainty, and independently observed outcomes. Whether that contribution is novel and useful remains to be tested.

This assessment combines a complete visual review of the three supplied PDFs with a preliminary web-based check of adjacent research and existing uses of the proposed name. It is not a systematic review, a legal naming clearance, or an empirical performance evaluation. Resource estimates below are planning assumptions, not quotations or commitments.

## 1. What is feasible at each level?

| Objective | Assessment | Evidence needed to advance |
|---|---|---|
| Initialize a documented research project | Feasible now; completed | Maintained records and clear scope |
| Annotate events, characters, goals, and time spans | Feasible pilot | Explicit codebook and independent agreement |
| Build a bounded audiovisual representation | Plausible engineering task | Held-out extraction quality and error audit |
| Quantify meaning or complexity | Conditional | Construct definition, reliability, and validity |
| Predict a defined audience response | Open empirical question | Independent responses, baselines, unseen stimuli |
| Explain causal effects of narrative changes | Conditional research task | Identified intervention and appropriate study design |
| Model diffusion in one setting | Possible later study | Exposure, network, timing, and outcome data |
| Build a faithful social digital twin | Unsupported by this material | Observability, validation, and generalization evidence |
| Establish a distinct field | Possible long-term outcome | Original results, replication, adoption, and community |
| Claim universal laws or general social control | Not justified | No supporting evidence supplied or established here |

These assessments are judgments about the proposed scope, not numeric probabilities of success.

## 2. Novelty: there is a research opportunity, but no empty territory

Computational modeling of narrative already has an organized research history, including the [Computational Models of Narrative workshop](https://narrative.csail.mit.edu/cmn13/). This means that computational representation alone is not a new disciplinary object.

[MovieGraphs](https://openaccess.thecvf.com/content_cvpr_2018/html/Vicol_MovieGraphs_Towards_Understanding_CVPR_2018_paper.html) already represents movie situations through graphs containing characters, attributes, relationships, interactions, and temporal grounding. HNA must be compared against work of this kind before claiming novelty in narrative graphs.

[Green and Brock's transportation study](https://pubmed.ncbi.nlm.nih.gov/11079236/) developed a measure of absorption in a story and investigated its relationship with beliefs. Narrative reception therefore already has empirical constructs that should inform or challenge proposed NSE measures.

[Jones and McBeth's Narrative Policy Framework](https://onlinelibrary.wiley.com/doi/abs/10.1111/j.1541-0072.2010.00364.x) proposes systematic empirical study of narrative structure and content in policy contexts. It is especially relevant to the proposed political and collective dimensions.

[Shiller's Narrative Economics](https://www.nber.org/papers/w23075) studies the spread of economically relevant narratives. An economic branch of this project must identify what it adds to that agenda.

[DARPA's completed Narrative Networks program](https://www.darpa.mil/research/programs/narrative-networks) also proposed quantitative narrative analysis and models of cognitive and social influence. Its stated objectives demonstrate overlap; the program page does not establish that general prediction or control was achieved.

**Naming finding:** the exact phrase Narrative Systems Engineering appears in [As If Productions' discussion of dynamic fiction and game systems](https://www.patreon.com/posts/soft-science-of-135508402) and in [Pinnup's leadership framework](https://pinnup.org/nse-founder). These are first-party evidence of public use, not scientific validation of those frameworks. We cannot assume the label is original or exclusive. This search does not resolve first use or legal availability.

The candidate contribution is therefore an integration that produces demonstrable value: for example, a representation linking narrative events to multiple human interpretations, retaining disagreement and evidence provenance, and improving an independently evaluated task. Integration can be valuable, but its novelty is a hypothesis until a more systematic comparison is completed.

## 3. The main scientific problems

### Separate the work, interpretation, and reception

At least three layers are required:

1. **Observable material:** a line of dialogue, an action, a shot boundary, or a sound interval.
2. **Interpretation:** an annotator judges the line to be threatening, symbolic, or goal-directed.
3. **Reception:** a participant reports fear, recalls an event, or changes a measured response after exposure.

A model assigning fear to a scene has not measured viewers' fear. A character's expressed belief is not the audience's belief. Multiple interpretations can be legitimate; reproducibility should include the ability to record disagreement rather than force a single reading.

### Replace total analysis with bounded coverage

Document 1's complete representation of everything communicated is not testable without a defined universe of features. Define a specific annotation vocabulary and report coverage within it. Retain an unknown or other category and document omissions. A bounded model can still be useful.

### Define narrative without making the theory true by definition

Proposed working definition: a representation of connected events involving entities over time, with temporal and/or causal relations that support an account of change. This definition needs testing against borderline cases: a slogan, an isolated claim, a list, an instruction, and an ambiguous scene.

The source proposition that every narrative changes or intends to change cognition conflates effect and intention. An unread story, an ignored message, or a story with no detectable effect must remain possible. Treat effects as hypotheses, and define intention separately when evidence for it exists.

### Treat the proposed quantities as candidates

| Source term | Problem | More defensible starting point |
|---|---|---|
| Narrative DNA | Biological metaphor without a specified encoding | Versioned feature set or typed event graph |
| Narrative energy | Product of undefined quantities with an unexplained scale | Compare a preregistered composite against its components |
| Narrative entropy | Number of interpretations is not a probability distribution | Entropy over a defined annotation distribution |
| Narrative inertia | Content alone cannot show resistance to change | Longitudinal response persistence under stated conditions |
| Narrative resonance | Could conflate similarity, engagement, and effects | Pick one measured construct and a separate outcome |
| Narrative complexity | Risks mixing unrelated dimensions | Separate structural descriptors with validity tests |
| Controllability | Requires an identified system and admissible inputs | A later, bounded mathematical question |
| Cognitive transformation | Implies an effect without a measurement protocol | Predefined change in a named response measure |

For the source energy formula `E = intensity * coherence * repetition * identification`, four inputs in [0,100] produce a maximum of 100,000,000, not 100. Inputs in [0,1] would give [0,1], but normalizing them and multiplying by 100 would only repair the numerical range. It would not justify multiplication, the implied zero effect when any component is zero, or a universal predictive interpretation.

For interpretive entropy, a candidate is `H = -sum(p_k * log2(p_k))`, where `p_k` is the fraction of independent annotations assigned to category `k` under a fixed codebook. Its unit is bits. This estimates dispersion of those annotations, not the intrinsic ambiguity of all possible interpretations. Results depend on categories, sample size, and the annotation population. A normalized version needs fixed `K > 1`; it does not become a universal scale merely by being mapped to 0-100.

### Add an observation model before discussing control

A more explicit candidate template is:

```text
x[i,t+1] = f_theta(x[i,t], u[i,t], c[i,t], network[t]) + process_error
y[i,t]   = g_phi(x[i,t]) + measurement_error
```

Here `x` denotes a hypothesized state, `u` measured exposure, `c` context, and `y` observed responses. The functions and errors must be specified for a particular study. Without observable outcomes and identifiable parameters, this is notation rather than a scientific result. Predictive success would still not establish that changing `u` causes a desired outcome; that requires a separate causal argument.

## 4. Technical feasibility and architecture

The initial system should retain a simple pipeline: approved media and metadata -> time-aligned segments -> human annotations -> structured representation -> evaluation report. Use ordinary structured files before choosing a graph database or a large application stack.

Each annotation should carry a source identifier, time span, entity references, label, evidence reference, annotator or model version, uncertainty status, and revision history. Human and machine labels should remain distinguishable.

Transcription, segmentation, and extraction should be evaluated separately so that errors can be located. Do not assess the final graph solely through a fluent generated summary. Emotion labels require context and a distinction between perceived expression and internal state; research on naturalistic expressions also illustrates the importance of measuring perceived categories rather than assuming a direct readout of emotion ([Cowen and Keltner](https://pmc.ncbi.nlm.nih.gov/articles/PMC6917997/)).

Start with a few short works and simple baselines. A small representation and evaluation script can precede a complete multimodal engine. No specific model vendor, subscription, or hardware purchase is needed to make the first conceptual decisions.

## 5. First validation pathway

The [pilot proposal](../research/protocols/PILOT-001.md) begins with measurement reliability. It asks whether independent annotators can identify a small set of events, goals, and relationships consistently. A subsequent extraction comparison tests performance on unseen works.

Only after that measurement layer is stable should the project test whether these features add predictive information about a named audience outcome, such as event recall. Response data must come from people independently of the model producing the features. Using a language model to generate both features and supposed human outcomes would validate agreement within a model, not behavior in people.

For later prediction, compare against duration, word count, simple text features, and baseline responses where available. Hold out entire works and account for repeated observations from the same participant. For causal questions, use a defined intervention, comparison condition, and analysis of plausible confounders. Reserve social diffusion for a separate study with exposure and network data.

## 6. Resources and practical constraints

Illustrative planning envelope for the proposed initial pilot:

| Work package | Assumed effort |
|---|---|
| Literature map, scope, and definitions | 20-40 hours |
| Corpus selection and codebook development | 15-30 hours |
| Independent annotation and disagreement review | 25-50 combined person-hours |
| Basic extraction, evaluation, and documentation | 30-60 hours |
| Total | 90-180 person-hours |

At 10-15 combined project hours per week, that envelope corresponds to approximately 6-18 weeks. Parallel annotation and availability can change elapsed time. These estimates assume short materials, a small vocabulary, no custom model training, and access to at least two annotators. They exclude a later participant study and external review.

Budget cannot be responsibly fixed without knowing available skills, hardware, media access, paid annotation rates, and compute choices. Track costs as `annotation hours * rate + compute usage + storage + media access + any participant compensation`. First measure time per annotated minute and cost per processed minute, then extrapolate. A founder can coordinate the pilot; independent annotation and specialist criticism require other people.

The source roadmap postpones validation to years 1-2 and software to years 3-4. We recommend reversing that dependency: early measurement and small scripts should expose weaknesses before a long treatise or institutional expansion.

## 7. Risks that directly affect feasibility

- **Scope expansion:** too many media, domains, and constructs prevent a decisive test. Limit the first corpus and question.
- **False precision:** a 0-100 number may only conceal an impression. Publish definitions and uncertainty before aggregate scores.
- **Circular evaluation:** model-generated outcomes can reward the model's own assumptions. Use independent observations.
- **Interpretive and cultural dependence:** agreement in one language or genre is not universality. Report scope and test transfer later.
- **Data access:** media reuse and audience data may be constrained. Record provenance and permitted uses before corpus selection or release.
- **Manipulation and cognitive surveillance:** these risks appear explicitly in document 3. Begin with descriptive analysis; any future human intervention needs its own transparent protocol and appropriate review.
- **Institutional overclaiming:** a lab name or polished manifesto does not establish a field. Use research-program language until evidence and uptake justify more.

The supplied suggestions about registration and DOI publication are not assessed here as legal strategies. Preserve provenance now; examine release, authorship, and registration questions separately when a concrete publication exists.

## 8. Evidence gates and recommendation

**Gate A: distinct question.** Produce a comparison with the closest approaches and state the additional capability precisely. If it duplicates existing work, revise the contribution or pursue a replication openly.

**Gate B: usable measurement.** Independent annotations must be sufficiently consistent for the intended claim. If not, refine or remove constructs; do not automate unreliable labels and call that validation.

**Gate C: incremental value.** A representation or predictor should improve a predefined task against a credible baseline on unseen material, with uncertainty and errors reported. Failure is a result that can narrow the theory.

**Gate D: independent replication.** Broader disciplinary claims need results that others can reproduce and use. A single pilot cannot establish generality.

The immediate recommendation is to pursue Gates A and B, using Narrative Systems Engineering only as a provisional working title. The most defensible pioneering ambition is to originate a precise, useful contribution with a documented research history. Recognition as a founder is a possible social outcome of sustained work; it cannot be secured by a name or declaration alone.
