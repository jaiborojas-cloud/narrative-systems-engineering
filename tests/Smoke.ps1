$ErrorActionPreference = 'Stop'
$repo = Split-Path $PSScriptRoot -Parent
Push-Location $repo
try {
    dotnet build src/Nse.Pipeline --nologo
    if ($LASTEXITCODE -ne 0) { throw 'Build failed' }
    $dll = Join-Path $repo 'src/Nse.Pipeline/bin/Debug/net10.0/Nse.Pipeline.dll'
    $ffmpeg = Get-ChildItem (Join-Path $repo '.tools') -Filter ffmpeg.exe -Recurse | Select-Object -First 1 -ExpandProperty FullName
    if (-not $ffmpeg) { $ffmpeg = (Get-Command ffmpeg).Source }
    $toolDirectory = Split-Path $ffmpeg -Parent
    $run = Join-Path $repo ('data/processed/runs/test-' + [guid]::NewGuid().ToString('N'))
    New-Item -ItemType Directory -Path $run | Out-Null
    $silent = Join-Path $run 'silent.mp4'
    & $ffmpeg -nostdin -v error -n -f lavfi -i 'color=c=red:s=160x120:r=10:d=2' -c:v libx264 -pix_fmt yuv420p $silent
    if ($LASTEXITCODE -ne 0) { throw 'Silent fixture generation failed' }
    $silentOutput = Join-Path $run 'silent-profile'
    dotnet $dll $silent --output $silentOutput --interval 0.5 --max-frames 2 --tools $toolDirectory
    if ($LASTEXITCODE -ne 0) { throw 'Silent pipeline failed' }
    $profile = Get-Content (Join-Path $silentOutput 'profile.json') -Raw | ConvertFrom-Json
    if ($profile.source.hasAudio -or $profile.audio.status -ne 'no_audio_stream') { throw 'Incorrect silent-video handling' }
    if ($profile.frames.Count -ne 2 -or $profile.methods.effectiveIntervalSeconds -ne 1) { throw 'Frame budget did not preserve full-duration coverage' }
    if ($profile.frames[0].timestampSeconds -ne 0.5 -or $profile.frames[1].timestampSeconds -ne 1.5) { throw 'Unexpected midpoint timestamps' }
    if ($profile.frames[0].colors[0].hex -ne '#F01010' -or $profile.frames[0].colors[0].pixelFraction -lt 0.99) { throw 'Known red fixture has incorrect color measurements' }
    if ($profile.source.sha256 -ne (Get-FileHash $silent -Algorithm SHA256).Hash.ToLowerInvariant()) { throw 'Source provenance hash is incorrect' }
    if ((Get-Content (Join-Path $silentOutput 'status.json') -Raw | ConvertFrom-Json).status -ne 'complete') { throw 'Completion not recorded' }
    $voiced = Join-Path $run 'tone.mp4'
    & $ffmpeg -nostdin -v error -n -i $silent -f lavfi -i 'sine=frequency=440:sample_rate=48000:duration=2' -map 0:v:0 -map 1:a:0 -c:v copy -c:a aac -shortest $voiced
    if ($LASTEXITCODE -ne 0) { throw 'Audio fixture generation failed' }
    $audioOutput = Join-Path $run 'audio-profile'
    dotnet $dll $voiced --output $audioOutput --tools $toolDirectory
    if ($LASTEXITCODE -ne 0) { throw 'Audio pipeline failed' }
    $audioProfile = Get-Content (Join-Path $audioOutput 'profile.json') -Raw | ConvertFrom-Json
    if (-not $audioProfile.source.hasAudio -or $audioProfile.audio.status -ne 'extracted_untranscribed') { throw 'Audio extraction not recorded' }
    $ffprobe = Join-Path $toolDirectory 'ffprobe.exe'
    if (-not (Test-Path $ffprobe)) { $ffprobe = Join-Path $toolDirectory 'ffprobe' }
    $audioProbe = & $ffprobe -v error -show_streams -of json (Join-Path $audioOutput 'audio.wav') | ConvertFrom-Json
    if ($audioProbe.streams[0].sample_rate -ne '16000' -or $audioProbe.streams[0].channels -ne 1 -or $audioProbe.streams[0].codec_name -ne 'pcm_s16le') { throw 'Audio format is incorrect' }
    dotnet $dll $silent --output $silentOutput --tools $toolDirectory
    if ($LASTEXITCODE -eq 0) { throw 'Existing output was not protected' }
    dotnet $dll $silent --interval NaN --tools $toolDirectory
    if ($LASTEXITCODE -eq 0) { throw 'Invalid interval accepted' }
    Write-Host "PASS: color measurements, source hash, frame budget, silent media, audio extraction, and input guards. Artifacts: $run"
}
finally { Pop-Location }
