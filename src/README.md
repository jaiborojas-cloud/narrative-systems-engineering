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

## Azure extension

Next: add an optional Azure Speech transcription stage, preserve its raw response, and align word timestamps to this timeline. Configuration should use environment variables or .NET user secrets. No credentials belong in source control. Add model-generated annotations with source references separately from computed observations; the current empty annotation and interpretation arrays are placeholders, not a frozen schema.

Video files, local tools, and generated runs are excluded from Git. The original video is read-only to the pipeline. No media is uploaded and no Azure calls occur.
