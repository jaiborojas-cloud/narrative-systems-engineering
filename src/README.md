# Local audiovisual pipeline

The .NET 10 console application in `Nse.Pipeline` creates the first local measurement profile. It requires FFmpeg and ffprobe, with no NuGet packages, Azure resources, or model deployments.

From the repository root:

```powershell
dotnet run --project src/Nse.Pipeline -- video/268230_medium.mp4
```

FFmpeg executables are discovered under `.tools/`, then on PATH. Alternatively pass `--tools C:/path/to/ffmpeg/bin`. Obtain FFmpeg through https://ffmpeg.org/download.html; Windows builds are available at https://www.gyan.dev/ffmpeg/builds/. The current workspace has a local build under `.tools/ffmpeg/`; this dependency is not committed or automatically downloaded by the application.

Optional flags:

```powershell
dotnet run --project src/Nse.Pipeline -- video/268230_medium.mp4 --interval 5 --max-frames 120 --output data/processed/runs/example
```

Output directories must not exist. The default creates a unique directory under `data/processed/runs/`. Run from the repository root for these relative paths. Each successful run produces:

- `probe.json`: original FFprobe metadata.
- `profile.json`: source SHA-256, methods/tool versions, measurements, and explicit pending analysis states.
- `timeline.csv`: sample timestamps, windows, and most frequent color bin.
- `report.html`: offline visual report with relative frame links.
- `frames/`: sampled JPEGs plus 64x64 RGB24 rasters used for color measurements.
- `audio.wav`: mono 16 kHz PCM16 audio if an audio track exists.
- `status.json`: `complete` only after all outputs have been written. Other states or missing status mean an incomplete run.

Sampling selects the midpoint of each interval. The interval increases when needed to respect the frame budget while covering the work's duration. These are requested seek positions, not verified frame presentation timestamps or scene boundaries. Color output is the top five bins of an 8x8x8 RGB histogram over a resized JPEG; it is not a calibrated perceptual palette. Pixel fractions refer to each sampled frame, never the entire time window.

The application uses the first non-attached-picture video stream and first audio stream. Audio presence does not establish speech presence. No transcription, silence detection, word count, object recurrence, emotion inference, or interpretation is implemented. Keep original audio for later acoustic measurements; the extracted WAV is intended for transcription. Failed runs are retained for inspection and must be retried in a new directory.

## Review visual annotations

The [visual codebook](../research/protocols/VISUAL-CODEBOOK.md) and [JSON schema](../research/protocols/annotation.schema.json) define objects, clothing, setting, and visible actions. Create a review page from a completed measurement profile:

```powershell
dotnet run --project src/Nse.Pipeline -- review data/processed/runs/first-profile-v1/profile.json --output data/processed/runs/my-review
```

Open the new `review.html` in a browser. It embeds the frames and works offline. Enter a reviewer identifier, choose Human or AI-assisted provenance, annotate frames, and download `annotations.json`. Changes are not automatically saved. You can download a partial review with untouched categories marked Not reviewed, then load that JSON later to resume. The initially generated `annotations.json` is a blank template; it needs a reviewer identifier before import.

Import the downloaded file (substitute its actual path):

```powershell
dotnet run --project src/Nse.Pipeline -- import-review data/processed/runs/first-profile-v1/profile.json C:/path/to/downloaded/annotations.json --output data/processed/runs/my-reviewed-profile
```

The importer checks exact profile/source hashes, frame references, timestamps, category uniqueness, required evidence, and uncertainty states before creating output. It writes `profile.reviewed.json`, a standalone read-only `report.html`, a resumable `review.html`, and the imported annotations. Original measurements remain unchanged. Relative media paths in the derived JSON resolve using `annotationReview.evidenceBaseDirectory`; keep the original run. Output directories must be new; omitting `--output` creates a unique run directory. The standalone reports embed JPEG evidence but do not embed audio or raw rasters.

No human reference annotations have been filled in automatically. Review provenance is self-reported, and complete coverage does not imply correctness or independent validation.

## Azure extension

Next: connect a vision model to the versioned annotation format and compare its draft against independently reviewed references. Add an optional Azure Speech transcription stage when a speech-containing input is available. Configuration should use environment variables or .NET user secrets. No credentials belong in source control. Annotations remain separate from computed observations; interpretations are still unimplemented.

Video files, local tools, and generated runs are excluded from Git. The original video is read-only to the pipeline. No media is uploaded and no Azure calls occur.
