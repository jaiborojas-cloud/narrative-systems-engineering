# Pipeline checks

Run `powershell -File tests/Smoke.ps1` from the repository root (or `pwsh -File tests/Smoke.ps1`). Requires .NET 10 and FFmpeg/ffprobe under `.tools/` or on PATH.

With Node installed, run `node tests/ReviewValidation.cjs` for page JavaScript syntax and pure export-validator checks. These do not replace a browser rendering/interaction check.

The integration checks generate a known red silent clip and a clip with a synthetic audio tone. They verify color bin measurements, full-duration sampling under a frame budget, source hashes, silent-video handling, PCM audio output, and rejection of overwriting/invalid intervals. Review checks cover blank templates, annotation import/export round trips, unchanged measurements, embedded evidence in reports, partial/complete coverage, and rejection of mismatched hashes/timestamps, duplicated frames/categories, missing evidence, invalid uncertainty, absent observations, and empty reviewer names. Test artifacts remain in ignored `data/processed/runs/test-*` directories for inspection. This checks pipeline behavior, not research validity or Azure integration.
