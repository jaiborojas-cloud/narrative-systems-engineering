using System.Diagnostics;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

return await Pipeline.Run(args);

static class Pipeline
{
    static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public static async Task<int> Run(string[] args)
    {
        if (args.Length == 0 || args.Contains("--help"))
        {
            Console.WriteLine("NSE local audiovisual profile\nUsage: dotnet run --project src/Nse.Pipeline -- <video> [--output <new-directory>] [--interval <seconds>] [--max-frames <count>] [--tools <ffmpeg-bin-directory>]\nDefaults: interval 5 seconds, max-frames 120. No Azure calls or credentials required.");
            return args.Length == 0 ? 2 : 0;
        }
        try
        {
            var input = Path.GetFullPath(args[0]);
            if (!File.Exists(input)) throw new ArgumentException($"Video does not exist: {input}");
            var options = new Dictionary<string, string>();
            for (var i = 1; i < args.Length; i += 2)
            {
                if (i + 1 == args.Length || !new[] { "--output", "--interval", "--max-frames", "--tools" }.Contains(args[i]))
                    throw new ArgumentException($"Unknown option or missing value: {args[i]}");
                if (!options.TryAdd(args[i], args[i + 1])) throw new ArgumentException($"Duplicate option: {args[i]}");
            }
            var interval = double.Parse(options.GetValueOrDefault("--interval", "5"), CultureInfo.InvariantCulture);
            var maxFrames = int.Parse(options.GetValueOrDefault("--max-frames", "120"), CultureInfo.InvariantCulture);
            if (!double.IsFinite(interval) || interval < 0.1) throw new ArgumentException("Interval must be finite and at least 0.1 seconds.");
            if (maxFrames is < 1 or > 10000) throw new ArgumentException("max-frames must be between 1 and 10000.");
            var output = Path.GetFullPath(options.GetValueOrDefault("--output", Path.Combine("data", "processed", "runs", $"{DateTime.UtcNow:yyyyMMdd-HHmmss}-{Guid.NewGuid().ToString("N")[..8]}")));
            if (Directory.Exists(output) || File.Exists(output)) throw new ArgumentException("Output must be a new directory; existing runs are never overwritten.");
            var ffmpeg = FindTool("ffmpeg", options.GetValueOrDefault("--tools"));
            var ffprobe = FindTool("ffprobe", options.GetValueOrDefault("--tools"));
            var ffmpegVersion = (await Execute(ffmpeg, "-version")).Split('\n')[0].Trim();
            var ffprobeVersion = (await Execute(ffprobe, "-version")).Split('\n')[0].Trim();
            var probeText = await Execute(ffprobe, "-v", "error", "-show_format", "-show_streams", "-of", "json", input);
            using var probe = JsonDocument.Parse(probeText);
            var streams = probe.RootElement.GetProperty("streams").EnumerateArray().ToArray();
            var video = streams.FirstOrDefault(s => s.GetProperty("codec_type").GetString() == "video" &&
                (!s.TryGetProperty("disposition", out var d) || !d.TryGetProperty("attached_pic", out var a) || a.GetInt32() == 0));
            if (video.ValueKind == JsonValueKind.Undefined) throw new ArgumentException("Input has no video stream.");
            var duration = double.Parse(probe.RootElement.GetProperty("format").GetProperty("duration").GetString()!, CultureInfo.InvariantCulture);
            if (!double.IsFinite(duration) || duration <= 0) throw new ArgumentException("A finite, positive duration is required.");
            var audio = streams.FirstOrDefault(s => s.GetProperty("codec_type").GetString() == "audio");
            var hasAudio = audio.ValueKind != JsonValueKind.Undefined;
            var videoMap = $"0:{video.GetProperty("index").GetInt32()}";
            using var source = File.OpenRead(input);
            var hash = Convert.ToHexString(await SHA256.HashDataAsync(source)).ToLowerInvariant();
            Directory.CreateDirectory(Path.Combine(output, "frames"));
            await File.WriteAllTextAsync(Path.Combine(output, "probe.json"), probeText);
            await File.WriteAllTextAsync(Path.Combine(output, "status.json"), "{\"status\":\"running\"}");
            Console.WriteLine($"Video: {duration:F2}s; audio track: {hasAudio}; output: {output}");
            if (hasAudio)
            {
                await Execute(ffmpeg, "-nostdin", "-v", "error", "-n", "-i", input, "-map", $"0:{audio.GetProperty("index").GetInt32()}", "-vn", "-ac", "1", "-ar", "16000", "-c:a", "pcm_s16le", Path.Combine(output, "audio.wav"));
            }
            // Increase the interval for long inputs to cover the entire work within the frame budget.
            var effectiveInterval = Math.Max(interval, duration / maxFrames);
            var frameCount = Math.Min(maxFrames, (int)Math.Min(maxFrames, Math.Ceiling(duration / effectiveInterval)));
            var samples = new List<FrameSample>();
            for (var i = 0; i < frameCount; i++)
            {
                // Midpoints avoid sampling beyond the last decodable frame at the media boundary.
                var start = i * effectiveInterval;
                var end = Math.Min(duration, (i + 1) * effectiveInterval);
                var timestamp = (start + end) / 2;
                var stem = $"frames/frame-{i:D4}";
                var seek = timestamp.ToString("F6", CultureInfo.InvariantCulture);
                await Execute(ffmpeg, "-nostdin", "-v", "error", "-n", "-ss", seek, "-i", input, "-map", videoMap, "-frames:v", "1", "-vf", "scale=640:-2", "-q:v", "2", Path.Combine(output, stem + ".jpg"));
                // Small RGB raster is kept as reproducible evidence for the color histogram.
                await Execute(ffmpeg, "-nostdin", "-v", "error", "-n", "-i", Path.Combine(output, stem + ".jpg"), "-frames:v", "1", "-vf", "scale=64:64:flags=area", "-pix_fmt", "rgb24", "-f", "rawvideo", Path.Combine(output, stem + ".rgb"));
                var rgb = await File.ReadAllBytesAsync(Path.Combine(output, stem + ".rgb"));
                if (rgb.Length != 64 * 64 * 3) throw new InvalidDataException("Unexpected RGB raster length.");
                var bins = new Dictionary<int, int>();
                for (var j = 0; j < rgb.Length; j += 3)
                {
                    var bin = (rgb[j] / 32 << 6) | (rgb[j + 1] / 32 << 3) | rgb[j + 2] / 32;
                    bins[bin] = bins.GetValueOrDefault(bin) + 1;
                }
                var colors = bins.OrderByDescending(b => b.Value).ThenBy(b => b.Key).Take(5)
                    .Select(b => new ColorBin($"#{((b.Key >> 6) * 32 + 16):X2}{(((b.Key >> 3) & 7) * 32 + 16):X2}{((b.Key & 7) * 32 + 16):X2}", b.Value / 4096.0)).ToArray();
                samples.Add(new(timestamp, start, end, stem + ".jpg", stem + ".rgb", colors));
                Console.WriteLine($"Frame {i + 1}/{frameCount}: {timestamp:F2}s");
            }
            var profile = new
            {
                schemaVersion = "0.1", createdUtc = DateTimeOffset.UtcNow,
                source = new { fileName = Path.GetFileName(input), sha256 = hash, bytes = source.Length, durationSeconds = duration, width = video.GetProperty("width").GetInt32(), height = video.GetProperty("height").GetInt32(), hasAudio },
                methods = new { ffmpegVersion, ffprobeVersion, requestedIntervalSeconds = interval, effectiveIntervalSeconds = effectiveInterval, maxFrames,
                    sampling = "One midpoint frame per interval; intervals are sampling windows, not detected scenes. Timestamps are requested seek positions, not verified presentation timestamps.",
                    color = "FFmpeg decoded JPEG resized to 64x64 with area interpolation, RGB24; 8 equal-width bins per channel; top five bins with center RGB and fraction of 4096 pixels. Descriptive, uncalibrated; not a perceptual palette or whole-interval measurement.",
                    audio = "First audio stream, downmixed mono, 16 kHz PCM16 WAV for future transcription. Original remains unchanged; use original audio for future acoustic measurement." },
                audio = new { status = hasAudio ? "extracted_untranscribed" : "no_audio_stream", path = hasAudio ? "audio.wav" : null },
                transcription = new { status = hasAudio ? "not_run" : "not_applicable", reason = hasAudio ? "Azure Speech is not configured." : "Source contains no audio stream." },
                frames = samples,
                annotations = Array.Empty<object>(), interpretations = Array.Empty<object>(),
                limitations = new[] { "No speech, object, scene, emotion, or semantic analysis has been performed.", "A sampled frame cannot establish object recurrence or conditions throughout its interval.", "This single-work design example does not validate a research metric." }
            };
            await File.WriteAllTextAsync(Path.Combine(output, "profile.json"), JsonSerializer.Serialize(profile, JsonOptions));
            var csv = new StringBuilder("requested_timestamp_seconds,window_start_seconds,window_end_seconds,frame,top_color_hex,top_color_pixel_fraction\n");
            foreach (var f in samples) csv.AppendLine(FormattableString.Invariant($"{f.TimestampSeconds:F6},{f.WindowStartSeconds:F6},{f.WindowEndSeconds:F6},{f.Image},{f.Colors[0].Hex},{f.Colors[0].PixelFraction:F6}"));
            await File.WriteAllTextAsync(Path.Combine(output, "timeline.csv"), csv.ToString());
            await File.WriteAllTextAsync(Path.Combine(output, "report.html"), RenderReport(Path.GetFileName(input), duration, hasAudio, samples));
            await File.WriteAllTextAsync(Path.Combine(output, "status.json"), "{\"status\":\"complete\"}");
            Console.WriteLine($"Complete: {Path.Combine(output, "report.html")}");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error: {ex.Message}\nIf a run directory was created, it is incomplete unless status.json says complete. Retry with a new output directory.");
            return 1;
        }
    }

    static string FindTool(string name, string? explicitDirectory)
    {
        var executable = OperatingSystem.IsWindows() ? name + ".exe" : name;
        if (explicitDirectory != null)
        {
            var path = Path.GetFullPath(Path.Combine(explicitDirectory, executable));
            if (!File.Exists(path)) throw new FileNotFoundException($"Missing tool: {path}");
            return path;
        }
        var localTools = Path.Combine(Environment.CurrentDirectory, ".tools");
        if (Directory.Exists(localTools))
        {
            var match = Directory.EnumerateFiles(localTools, executable, SearchOption.AllDirectories).Order().FirstOrDefault();
            if (match != null) return match;
        }
        foreach (var directory in (Environment.GetEnvironmentVariable("PATH") ?? "").Split(Path.PathSeparator))
        {
            if (string.IsNullOrWhiteSpace(directory)) continue;
            var path = Path.Combine(directory.Trim('"'), executable);
            if (File.Exists(path)) return path;
        }
        throw new FileNotFoundException($"{name} was not found. Install FFmpeg (including ffprobe), unpack it under .tools, or pass --tools <bin-directory>.");
    }

    static async Task<string> Execute(string executable, params string[] arguments)
    {
        var info = new ProcessStartInfo(executable) { RedirectStandardOutput = true, RedirectStandardError = true, UseShellExecute = false, CreateNoWindow = true };
        foreach (var argument in arguments) info.ArgumentList.Add(argument);
        using var process = Process.Start(info) ?? throw new InvalidOperationException($"Unable to start {executable}");
        var stdout = process.StandardOutput.ReadToEndAsync();
        var stderr = process.StandardError.ReadToEndAsync();
        using var timeout = new CancellationTokenSource(TimeSpan.FromMinutes(15));
        try { await process.WaitForExitAsync(timeout.Token); }
        catch (OperationCanceledException) { process.Kill(entireProcessTree: true); await process.WaitForExitAsync(); throw new TimeoutException($"{Path.GetFileName(executable)} exceeded 15 minutes."); }
        var result = await stdout;
        var error = await stderr;
        if (process.ExitCode != 0) throw new InvalidOperationException($"{Path.GetFileName(executable)} exited {process.ExitCode}: {error}");
        return result;
    }

    static string RenderReport(string name, double duration, bool audio, List<FrameSample> frames)
    {
        var html = new StringBuilder("<!doctype html><html lang=\"en\"><meta charset=\"utf-8\"><meta name=\"viewport\" content=\"width=device-width,initial-scale=1\"><title>NSE audiovisual profile</title><style>body{font:16px system-ui;background:#101822;color:#eef3f7;max-width:1100px;margin:40px auto;padding:0 24px}h1{font-size:36px}p{line-height:1.6;color:#bcccd8}.grid{display:grid;grid-template-columns:repeat(auto-fit,minmax(290px,1fr));gap:20px}article{background:#1b2938;border-radius:12px;overflow:hidden}img{width:100%;display:block}.info{padding:16px}.swatches{display:flex;height:28px;margin:12px 0}.swatches span{flex:1}a{color:#81d8ee}small{color:#bcccd8}</style><h1>Audiovisual measurement profile</h1>");
        html.Append($"<p>{System.Net.WebUtility.HtmlEncode(name)} · {duration.ToString("F2", CultureInfo.InvariantCulture)} seconds · {(audio ? "Audio extracted; transcription pending" : "No audio stream")}</p><p>Local design example. Each card shows one sampled frame and its five most frequent quantized RGB bins. Colors describe that frame only. No model annotations or interpretations have been generated.</p><p><a href=\"profile.json\">Profile JSON</a> · <a href=\"timeline.csv\">Timeline CSV</a></p><div class=\"grid\">");
        foreach (var f in frames)
        {
            html.Append(CultureInfo.InvariantCulture, $"<article><img src=\"{f.Image}\" alt=\"Frame requested at {f.TimestampSeconds:F2} seconds\"><div class=\"info\"><strong>{f.TimestampSeconds:F2}s</strong><br><small>Sampling window {f.WindowStartSeconds:F2}–{f.WindowEndSeconds:F2}s</small><div class=\"swatches\">");
            foreach (var c in f.Colors) html.Append(CultureInfo.InvariantCulture, $"<span style=\"background:{c.Hex}\" title=\"{c.Hex}: {c.PixelFraction:P1} of pixels\"></span>");
            html.Append("</div><small>RGB bin centers; equal-width swatches. Hover for pixel fractions.</small></div></article>");
        }
        return html.Append("</div></html>").ToString();
    }
}

record ColorBin(string Hex, double PixelFraction);
record FrameSample(double TimestampSeconds, double WindowStartSeconds, double WindowEndSeconds, string Image, string ColorRaster, ColorBin[] Colors);
