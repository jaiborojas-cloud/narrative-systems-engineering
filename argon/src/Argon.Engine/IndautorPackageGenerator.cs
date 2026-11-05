using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Argon.Engine;

public static class IndautorPackageGenerator
{
    public static void GenerateIndautorPdfs(string repoRoot)
    {
        var outDir = Path.Combine(repoRoot, "legal", "indautor");
        Directory.CreateDirectory(outDir);

        var sourceFiles = new[]
        {
            "argon/src/Argon.Engine/Program.cs",
            "argon/src/Argon.Engine/AcousticAnalyzer.cs",
            "argon/src/Argon.Engine/VisualDynamicsAnalyzer.cs",
            "argon/src/Argon.Engine/LinguisticAnalyzer.cs",
            "argon/src/Argon.Engine/NarrativeMetricsEngine.cs",
            "argon/src/Argon.Engine/AtlasComparator.cs",
            "argon/src/Argon.Engine/CounterfactualController.cs",
            "argon/src/Argon.Engine/DashboardRenderer.cs",
            "argon/src/Argon.Engine/PaperManuscriptRenderer.cs",
            "argon/src/Argon.Engine/IndautorPackageGenerator.cs",
            "src/Nse.Pipeline/Program.cs",
            "src/Nse.Pipeline/VideoProfiler.cs",
            "src/Nse.Pipeline/ReviewWorkflow.cs"
        };

        var fileEntries = new List<(string RelPath, string Sha256, string[] Lines)>();
        foreach (var rel in sourceFiles)
        {
            var full = Path.Combine(repoRoot, rel.Replace('/', Path.DirectorySeparatorChar));
            if (File.Exists(full))
            {
                var bytes = File.ReadAllBytes(full);
                var sha = Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
                var lines = File.ReadAllLines(full);
                fileEntries.Add((rel, sha, lines));
            }
        }

        var memoriaPath = Path.Combine(outDir, "MEMORIA_DESCRIPTIVA_INDAUTOR_ES.pdf");
        File.WriteAllBytes(memoriaPath, BuildMemoriaDescriptivaPdf(fileEntries));

        var codePath = Path.Combine(outDir, "CODIGO_FUENTE_ARGON_NSE_INDAUTOR.pdf");
        File.WriteAllBytes(codePath, BuildSourceCodeListingPdf(fileEntries));
    }

    private static byte[] BuildMemoriaDescriptivaPdf(List<(string RelPath, string Sha256, string[] Lines)> files)
    {
        var pages = new List<StringBuilder>();

        // Page 1: Ficha Registral, Declaración de Autoría, Arquitectura y Módulos
        var p1 = new StringBuilder();
        double y = 742;

        DrawLine(p1, 48, y + 14, 564, y + 14, 1.4, 0.08, 0.15, 0.28);
        DrawText(p1, "F2", 8.0, 48, y + 3, "REGISTRO PUBLICO DEL DERECHO DE AUTOR (INDAUTOR - MEXICO) — MEMORIA DESCRIPTIVA");
        DrawText(p1, "F2", 8.0, 462, y + 3, "ORCID: 0009-0003-1109-4583");
        y -= 20;

        DrawText(p1, "F2", 13.0, 48, y, "MEMORIA DESCRIPTIVA Y ESPECIFICACION FUNCIONAL DEL SISTEMA:");
        y -= 15;
        DrawText(p1, "F2", 11.5, 48, y, "ARGON.ENGINE & NSE.PIPELINE — INGENIERIA DE SISTEMAS NARRATIVOS (NSE/CND)");
        y -= 16;

        // Metadata Box
        DrawRect(p1, 48, y - 82, 516, 90, 0.96, 0.98, 1.0, 0.70, 0.78, 0.88);
        string[] metaLines =
        [
            "AUTOR Y TITULAR UNICO (100% DERECHOS MORALES Y PATRIMONIALES): Jair Ivan Rojas Ruiz (Jair Rojas)",
            "ORCID iD: https://orcid.org/0009-0003-1109-4583   |   DOI Oficial (Zenodo / CERN): https://doi.org/10.5281/zenodo.23987656",
            "RAMA DE REGISTRO (LFDA ART. 13): Programa de Computo (INDAUTOR-01-002) y Obra Cientifica/Tecnica (INDAUTOR-01-001)",
            "FECHA DE CREACION Y ARCHIVO CRIPTOGRAFICO PRIMIGENIO: 8 de septiembre de 2026 (Caracter de la obra: Primigenia / Original)",
            "LENGUAJE Y PLATAFORMA: C# 13 sobre .NET 10.0 (Ejecucion determinista multiplataforma Windows / Linux)",
            "REPOSITORIO OFICIAL: https://github.com/jaiborojas-cloud/narrative-systems-engineering (Release v0.3.0)"
        ];
        double my = y - 10;
        foreach (var ml in metaLines)
        {
            DrawText(p1, "F2", 8.1, 56, my, ml);
            my -= 12.5;
        }
        y -= 98;

        DrawSection(p1, ref y, "1. DECLARACION DE ORIGINALIDAD, PRIORIDAD CRONOLOGICA Y DESAMBIGUACION");
        WritePara(p1, ref y,
        [
            "La presente obra es una creacion intelectual 100% original y primigenia de Jair Ivan Rojas Ruiz. Los tres documentos fundacionales",
            "en espanol que originan su arquitectura (Doc 1: Atlas de la Narrativa Humana [HNA], Doc 2: Motor de Dinamica Narrativa [NDE] y",
            "Doc 3: Ingenieria de Sistemas Narrativos [NSE], 63 paginas en total) fueron archivados con huella criptografica SHA-256 el 8 de",
            "septiembre de 2026. El sistema establece formalmente la disciplina cuantitativa y computacional de Ingenieria de Sistemas Narrativos",
            "Multimodal (NSE) y Dinamica Narrativa Computacional (CND), fundamentada en procesamiento fisico de senales de audio/video, entropia",
            "de Shannon y control en espacio de estados, diferenciandose de cualquier ensayo cualitativo homonimo carente de matematica o software."
        ]);

        DrawSection(p1, ref y, "2. ARQUITECTURA FUNCIONAL Y FORMULACION MATEMATICA DEL MOTOR");
        WritePara(p1, ref y,
        [
            "El sistema Argon.Engine y Nse.Pipeline transforma senales audiovisuales crudas (.mp4, .ogv, .webm) y transcripciones alineadas en un",
            "vector multimodal u(t) = (u_ac(t), u_vis(t), u_ling(t)) que alimenta un sistema dinamico discreto en espacio de estados x(t+1):",
            "  * Subsistema Acustico (AcousticAnalyzer.cs): Procesa ondas PCM16 a 16 kHz para extraer amplitud RMS, nivel en dBFS, razon de",
            "    actividad de voz (VAD en subtramas de 100 ms) y rango dinamico intra-ventana.",
            "  * Subsistema Visual (VisualDynamicsAnalyzer.cs): Procesa matrices RGB24 de 64x64 pixeles para calcular luminancia ITU-R BT.709,",
            "    saturacion HSV, entropia cromatica y Divergencia de Jensen-Shannon D_JS(P_t || P_{t-1}) en bits para detectar cortes de escena.",
            "  * Subsistema Linguistico (LinguisticAnalyzer.cs): Extrae cadencia (WPM), diversidad lexica (TTR), conectores dialecticos, anclaje",
            "    institucional A_inst(t) y vectores de postura normativa z(t) en [-1, 1]^K.",
            "  * Motor de Seis Metricas NSE y Control Optimo (NarrativeMetricsEngine.cs, AtlasComparator.cs, CounterfactualController.cs):",
            "    Calcula (1) ADN Narrativo G_DNA, (2) Energia Narrativa E(t) con resonancia intermodal Phi_sync, (3) Entropia de Postura de",
            "    Shannon H_narr(t) en bits, (4) Complejidad C(t), (5) Inercia Narrativa I(t), (6) Gramiano de Controlabilidad W_c y resuelve las",
            "    6! = 720 permutaciones de montaje para minimizar el sesgo normativo terminal ||x*(T)||_2 (-62.0% en EXP-003)."
        ]);

        DrawSection(p1, ref y, "3. INVENTARIO CRIPTOGRAFICO DE MODULOS FUENTE C# (SHA-256)");
        DrawRect(p1, 48, y - 11, 516, 13, 0.92, 0.95, 0.98, 0.2, 0.3, 0.45);
        DrawText(p1, "F2", 7.8, 52, y - 8, "Modulo / Archivo Fuente C#");
        DrawText(p1, "F2", 7.8, 225, y - 8, "Lineas");
        DrawText(p1, "F2", 7.8, 268, y - 8, "Huella Criptografica SHA-256 Verificable");
        y -= 11;

        foreach (var f in files)
        {
            y -= 11.2;
            DrawLine(p1, 48, y - 2, 564, y - 2, 0.3, 0.80, 0.84, 0.90);
            DrawText(p1, "F2", 7.5, 52, y + 1, f.RelPath);
            DrawText(p1, "F1", 7.5, 228, y + 1, f.Lines.Length.ToString(CultureInfo.InvariantCulture));
            DrawText(p1, "F4", 6.8, 268, y + 1, f.Sha256);
        }
        DrawLine(p1, 48, y - 2, 564, y - 2, 0.9, 0.1, 0.15, 0.25);

        DrawFooter(p1, 1, 1);
        pages.Add(p1);

        return AssemblePdf(pages);
    }

    private static byte[] BuildSourceCodeListingPdf(List<(string RelPath, string Sha256, string[] Lines)> files)
    {
        var pages = new List<StringBuilder>();
        var current = new StringBuilder();
        double y = StartNewCodePage(current);

        foreach (var f in files)
        {
            if (y < 110)
            {
                pages.Add(current);
                current = new StringBuilder();
                y = StartNewCodePage(current);
            }

            // File banner
            DrawRect(current, 42, y - 12, 528, 16, 0.91, 0.95, 0.99, 0.25, 0.38, 0.58);
            DrawText(current, "F2", 8.0, 48, y - 8, $"ARCHIVO FUENTE: {f.RelPath}   ({f.Lines.Length} lineas)   |   SHA-256: {f.Sha256[..32]}...");
            y -= 20;

            for (int i = 0; i < f.Lines.Length; i++)
            {
                if (y < 52)
                {
                    pages.Add(current);
                    current = new StringBuilder();
                    y = StartNewCodePage(current);
                }

                var rawLine = SanitizeLatin1(f.Lines[i]);
                if (rawLine.Length > 106) rawLine = rawLine[..106] + "...";
                var formatted = $"{i + 1,4}: {rawLine}";
                DrawText(current, "F4", 6.6, 46, y, formatted);
                y -= 7.6;
            }

            y -= 8;
        }

        pages.Add(current);

        for (int p = 0; p < pages.Count; p++)
        {
            DrawFooter(pages[p], p + 1, pages.Count);
        }

        return AssemblePdf(pages);
    }

    private static double StartNewCodePage(StringBuilder sb)
    {
        double y = 748;
        DrawLine(sb, 42, y + 12, 570, y + 12, 1.0, 0.08, 0.15, 0.28);
        DrawText(sb, "F2", 7.8, 42, y + 2, "EJEMPLAR DE CODIGO FUENTE (INDAUTOR) — AUTOR Y TITULAR UNICO (100%): JAIR IVAN ROJAS RUIZ (ORCID: 0009-0003-1109-4583)");
        return y - 12;
    }

    private static string SanitizeLatin1(string input)
    {
        var sb = new StringBuilder(input.Length);
        foreach (var ch in input)
        {
            if (ch == '\t') sb.Append("    ");
            else if (ch >= 32 && ch <= 126) sb.Append(ch);
            else if (ch is '→' or '➔') sb.Append("->");
            else if (ch is '—' or '–') sb.Append('-');
            else if (ch == '±') sb.Append("+/-");
            else if (ch is '“' or '”') sb.Append('"');
            else if (ch is '‘' or '’') sb.Append('\'');
            else if (ch >= 160 && ch <= 255) sb.Append(ch);
            else sb.Append('?');
        }
        return sb.ToString();
    }

    private static void DrawSection(StringBuilder sb, ref double y, string title)
    {
        DrawText(sb, "F2", 9.2, 48, y, title);
        y -= 4;
        DrawLine(sb, 48, y, 564, y, 0.6, 0.25, 0.35, 0.50);
        y -= 11;
    }

    private static void WritePara(StringBuilder sb, ref double y, string[] lines)
    {
        foreach (var line in lines)
        {
            DrawText(sb, "F1", 8.4, 48, y, line);
            y -= 10.2;
        }
        y -= 4;
    }

    private static void DrawFooter(StringBuilder sb, int pageNum, int totalPages)
    {
        DrawLine(sb, 42, 36, 570, 36, 0.5, 0.6, 0.65, 0.75);
        DrawText(sb, "F1", 7.6, 42, 25, "Copyright (c) 2026 Jair Ivan Rojas Ruiz (ORCID: 0009-0003-1109-4583 | DOI: 10.5281/zenodo.23987656) — Registro INDAUTOR");
        DrawText(sb, "F2", 7.8, 505, 25, $"Pagina {pageNum} de {totalPages}");
    }

    private static void DrawText(StringBuilder sb, string fontRef, double size, double x, double y, string text)
    {
        var escaped = SanitizeLatin1(text)
            .Replace("\\", "\\\\")
            .Replace("(", "\\(")
            .Replace(")", "\\)");
        sb.AppendLine(CultureInfo.InvariantCulture, $"0.08 0.11 0.18 rg BT /{fontRef} {size:F1} Tf {x:F1} {y:F1} Td ({escaped}) Tj ET");
    }

    private static void DrawLine(StringBuilder sb, double x1, double y1, double x2, double y2, double width, double r, double g, double b)
    {
        sb.AppendLine(CultureInfo.InvariantCulture, $"{r:F2} {g:F2} {b:F2} RG {width:F2} w {x1:F1} {y1:F1} m {x2:F1} {y2:F1} l S");
    }

    private static void DrawRect(StringBuilder sb, double x, double y, double w, double h, double fr, double fg, double fb, double sr, double sg, double sbCol)
    {
        sb.AppendLine(CultureInfo.InvariantCulture, $"{fr:F2} {fg:F2} {fb:F2} rg {sr:F2} {sg:F2} {sbCol:F2} RG 0.6 w {x:F1} {y:F1} {w:F1} {h:F1} re B");
    }

    private static byte[] AssemblePdf(List<StringBuilder> pageStreams)
    {
        using var ms = new MemoryStream();
        var offsets = new List<long>();
        var latin1 = Encoding.GetEncoding("ISO-8859-1");

        void WriteAscii(string s)
        {
            var bytes = latin1.GetBytes(s);
            ms.Write(bytes, 0, bytes.Length);
        }

        WriteAscii("%PDF-1.4\n");

        offsets.Add(ms.Position);
        WriteAscii("1 0 obj\n<< /Type /Catalog /Pages 2 0 R >>\nendobj\n");

        int firstPageObjId = 7;
        var kids = string.Join(" ", Enumerable.Range(0, pageStreams.Count).Select(i => $"{firstPageObjId + i * 2} 0 R"));
        offsets.Add(ms.Position);
        WriteAscii($"2 0 obj\n<< /Type /Pages /Kids [{kids}] /Count {pageStreams.Count} >>\nendobj\n");

        string[] fontNames = ["Times-Roman", "Times-Bold", "Times-Italic", "Courier"];
        for (int f = 0; f < fontNames.Length; f++)
        {
            offsets.Add(ms.Position);
            WriteAscii($"{3 + f} 0 obj\n<< /Type /Font /Subtype /Type1 /BaseFont /{fontNames[f]} /Encoding /WinAnsiEncoding >>\nendobj\n");
        }

        for (int i = 0; i < pageStreams.Count; i++)
        {
            int pageObjId = firstPageObjId + i * 2;
            int contentObjId = pageObjId + 1;
            var contentBytes = latin1.GetBytes(pageStreams[i].ToString());

            offsets.Add(ms.Position);
            WriteAscii($"{pageObjId} 0 obj\n<< /Type /Page /Parent 2 0 R /MediaBox [0 0 612 792] /Resources << /Font << /F1 3 0 R /F2 4 0 R /F3 5 0 R /F4 6 0 R >> >> /Contents {contentObjId} 0 R >>\nendobj\n");

            offsets.Add(ms.Position);
            WriteAscii($"{contentObjId} 0 obj\n<< /Length {contentBytes.Length} >>\nstream\n");
            ms.Write(contentBytes, 0, contentBytes.Length);
            WriteAscii("\nendstream\nendobj\n");
        }

        long xrefPos = ms.Position;
        WriteAscii($"xref\n0 {offsets.Count + 1}\n");
        WriteAscii("0000000000 65535 f \n");
        foreach (var off in offsets)
        {
            WriteAscii(off.ToString("D10", CultureInfo.InvariantCulture) + " 00000 n \n");
        }
        WriteAscii($"trailer\n<< /Size {offsets.Count + 1} /Root 1 0 R >>\nstartxref\n{xrefPos}\n%%EOF\n");

        return ms.ToArray();
    }
}
