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
            "argon/src/Argon.Engine/SystemIdentifier.cs",
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

        var obraPath = Path.Combine(outDir, "OBRA_CIENTIFICA_NSE_CND_INDAUTOR.pdf");
        File.WriteAllBytes(obraPath, BuildObraCientificaCompendioPdf(fileEntries));
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
            "REPOSITORIO OFICIAL: https://github.com/jaiborojas-cloud/narrative-systems-engineering (Release v0.4.0)"
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
            "de Shannon, control en espacio de estados e identificacion inversa de sistemas, diferenciandose de cualquier ensayo cualitativo."
        ]);

        DrawSection(p1, ref y, "2. ARQUITECTURA DE CUATRO PILARES Y FORMULACION MATEMATICA DEL MOTOR");
        WritePara(p1, ref y,
        [
            "El sistema Argon.Engine v0.4 y Nse.Pipeline implementa los 4 pilares completos de la Ingenieria de Sistemas Narrativos sobre u(t):",
            "  * Pilar 1 — Extraccion Multimodal (AcousticAnalyzer.cs, VisualDynamicsAnalyzer.cs, LinguisticAnalyzer.cs): Procesa audio PCM16",
            "    (RMS, dBFS, VAD), matrices RGB24 de 64x64 (luminancia BT.709, Divergencia de Jensen-Shannon D_JS en bits) y cadencia lexica.",
            "  * Pilar 2 — Simulacion en Espacio de Estados y Atlas de Fase (NarrativeMetricsEngine.cs, AtlasComparator.cs, EXP-001/EXP-002):",
            "    Integra x(t+1) = (I - Gamma(t))x(t) + Gamma(t)z(t) y proyecta obras reales en el espacio metrico 5D (E, H_narr, I, C, P).",
            "  * Pilar 3 — Control Contrafactual y Optimizacion de Montaje (CounterfactualController.cs, EXP-003): Resuelve las 6! = 720",
            "    permutaciones de bloques editoriales para minimizar el sesgo normativo terminal ||x*(T)||_2 (-62.0%) y cuantifica choques.",
            "  * Pilar 4 — Identificacion Inversa de Sistemas y Calibracion Empirica (SystemIdentifier.cs, EXP-004): Estima las matrices de",
            "    acoplamiento Theta = (alpha_ac, alpha_vis, alpha_ling, alpha_sync, beta_0) mediante Regresion Regularizada de Tikhonov (Ridge)",
            "    sobre N = 308 transiciones con validacion cruzada Leave-One-Corpus-Out (R^2 fuera de muestra = 0.9999)."
        ]);

        DrawSection(p1, ref y, "3. INVENTARIO CRIPTOGRAFICO DE LOS 14 MODULOS FUENTE C# (SHA-256)");
        DrawRect(p1, 48, y - 11, 516, 13, 0.92, 0.95, 0.98, 0.2, 0.3, 0.45);
        DrawText(p1, "F2", 7.8, 52, y - 8, "Modulo / Archivo Fuente C#");
        DrawText(p1, "F2", 7.8, 225, y - 8, "Lineas");
        DrawText(p1, "F2", 7.8, 268, y - 8, "Huella Criptografica SHA-256 Verificable");
        y -= 11;

        foreach (var f in files)
        {
            y -= 10.6;
            DrawLine(p1, 48, y - 2, 564, y - 2, 0.3, 0.80, 0.84, 0.90);
            DrawText(p1, "F2", 7.3, 52, y + 1, f.RelPath);
            DrawText(p1, "F1", 7.3, 228, y + 1, f.Lines.Length.ToString(CultureInfo.InvariantCulture));
            DrawText(p1, "F4", 6.6, 268, y + 1, f.Sha256);
        }
        DrawLine(p1, 48, y - 2, 564, y - 2, 0.9, 0.1, 0.15, 0.25);

        DrawFooter(p1, 1, 1);
        pages.Add(p1);

        return AssemblePdf(pages);
    }

    private static byte[] BuildObraCientificaCompendioPdf(List<(string RelPath, string Sha256, string[] Lines)> files)
    {
        var pages = new List<StringBuilder>();

        // Page 1: Compendio Científico y Matemático (INDAUTOR-01-001 Obra Literaria / Científica)
        var p1 = new StringBuilder();
        double y = 742;

        DrawLine(p1, 48, y + 14, 564, y + 14, 1.4, 0.08, 0.15, 0.28);
        DrawText(p1, "F2", 8.0, 48, y + 3, "REGISTRO DE OBRA LITERARIA / CIENTIFICA (INDAUTOR-01-001) — COMPENDIO FUNDACIONAL NSE/CND");
        DrawText(p1, "F2", 8.0, 462, y + 3, "ORCID: 0009-0003-1109-4583");
        y -= 20;

        DrawText(p1, "F2", 12.8, 48, y, "INGENIERIA DE SISTEMAS NARRATIVOS MULTIMODALES Y DINAMICA NARRATIVA");
        y -= 15;
        DrawText(p1, "F2", 11.8, 48, y, "COMPUTACIONAL: ESPECIFICACION MATEMATICA Y CORPUS EXPERIMENTAL (v0.4.0)");
        y -= 16;

        DrawRect(p1, 48, y - 70, 516, 78, 0.96, 0.98, 1.0, 0.70, 0.78, 0.88);
        string[] meta =
        [
            "AUTOR UNICO Y TITULAR ORIGINARIO (100%): Jair Ivan Rojas Ruiz (Jair Rojas) — ORCID: https://orcid.org/0009-0003-1109-4583",
            "MODALIDAD REGISTRAL: Obra Literaria / Cientifica y Tecnica (INDAUTOR-01-001) vinculada a Programa de Computo (INDAUTOR-01-002)",
            "FECHA DE PRIMERA DIVULGACION Y SELLO CRIPTOGRAFICO: 8 de septiembre de 2026 (Mexico) | DOI: 10.5281/zenodo.23987656",
            "ALCANCE INTEGRAL DE LA OBRA: Formulacion Axiomatica + Extraccion de Senales + Simulacion + Control Optimo + Identificacion Inversa",
            "VOLUMENES PRIMIGENIOS INCLUIDOS: Doc 1 (Atlas HNA, 19 pp.), Doc 2 (Motor NDE, 22 pp.), Doc 3 (NSE, 22 pp.) y PAPER-001 (v1.2)"
        ];
        double my = y - 10;
        foreach (var m in meta)
        {
            DrawText(p1, "F2", 8.0, 56, my, m);
            my -= 12.5;
        }
        y -= 86;

        DrawSection(p1, ref y, "1. OBJETO CIENTIFICO Y AXIOMAS DE LA DISCIPLINA (NSE / CND)");
        WritePara(p1, ref y,
        [
            "La Ingenieria de Sistemas Narrativos (Narrative Systems Engineering — NSE) y su nucleo matematico, la Dinamica Narrativa",
            "Computacional (Computational Narrative Dynamics — CND), creadas por Jair Ivan Rojas Ruiz el 8 de septiembre de 2026, constituyen",
            "la primera formulacion fisico-matematica que modela obras audiovisuales como sistemas dinamicos de tiempo discreto excitados por",
            "vectores multimodales u(t) = (u_ac(t), u_vis(t), u_ling(t)). La disciplina se rige por cuatro axiomas estrictos:",
            "  Axioma I (Anclaje Fisico en la Senal): Toda metrica se deriva directamente de muestras acusticas PCM16 a 16 kHz, matrices de",
            "    pixeles RGB24 de 64x64 y segmentos lexicos alineados en el tiempo, prohibiendo puntajes subjetivos o alucinaciones de LLM.",
            "  Axioma II (Unidades Fisicas e Informacionales Acotadas): La entropia cromatica, la divergencia visual entre cuadros y la",
            "    dispersion de posturas se expresan en bits de Shannon; la energia acustica en dBFS; y el estado colectivo en [-1, 1]^K.",
            "  Axioma III (Separacion de Capas): Distincion formal entre la senal fisica observable u(t), la codificacion simbolica de postura",
            "    normativa z(t) y la respuesta dinamica del estado cognitivo-social x(t).",
            "  Axioma IV (Reproducibilidad Computacional Exacta): Verificacion determinista de invariantes matematicos mediante software libre."
        ]);

        DrawSection(p1, ref y, "2. FORMULACION MATEMATICA DE LAS OCHO ECUACIONES CANONICAS NSE/CND");
        WritePara(p1, ref y,
        [
            "  (Ec. 1) Intensidad Acustica: RMS(t) = sqrt((1/M) Sum s_i^2),  L_dB(t) = 20 log10(max(RMS(t), 1e-5)), acoplada con VAD(t).",
            "  (Ec. 2) Flujo Visual de Jensen-Shannon: D_JS(P_t || P_{t-1}) = 0.5 D_KL(P_t || M) + 0.5 D_KL(P_{t-1} || M) en [0, 1] bits.",
            "  (Ec. 3) Resonancia Intermodal: Phi_sync(t) = sqrt(I_ac(t) * I_ling(t)) en [0, 1].",
            "  (Ec. 4) Energia Narrativa: E(t; alpha) = 100 * clip(alpha_ac I_ac + alpha_vis psi_vis + alpha_ling I_ling + alpha_sync Phi_sync).",
            "  (Ec. 5) Entropia Narrativa de Postura de Shannon: H_narr(t) = - Sum_{k,s} p_{k,s}(t) log2 p_{k,s}(t) en [0, 3.00] bits.",
            "  (Ec. 6) Inercia Narrativa Institucional: I(t) = clip(0.55 A_inst(t) + 0.25 (||x(t)||_1 / K) + 0.20 ((t + 1) / N), 0, 1).",
            "  (Ec. 7) Ecuacion de Estado LTV y Control Optimo de Montaje: x_k(t+1) = (1 - gamma_k(t))x_k(t) + gamma_k(t)z_k(t), donde el",
            "          controlador sintetiza la permutacion optima pi* en S_n que minimiza ||x(T; pi)||_2 o maximiza una dimension objetivo.",
            "  (Ec. 8) Identificacion Inversa de Sistemas (Tikhonov Ridge): Theta_hat(lambda) = (Psi^T Psi + lambda I)^{-1} Psi^T Delta_X,",
            "          recuperando empiricamente los pesos multimodales (alpha_ac, alpha_vis, alpha_ling, alpha_sync, beta_0) desde telemetria."
        ]);

        DrawSection(p1, ref y, "3. SINTESIS DE LOS CUATRO EXPERIMENTOS DE VALIDACION EMPIRICA (EXP-001 A EXP-004)");
        WritePara(p1, ref y,
        [
            "  * EXP-001 (CORPUS-001, Debate 'La Mili y la Insumision', Espana, 1994, 194.84 s, 39 ventanas): D_JS >= 0.30 bits detecta el 100%",
            "    de los cortes de camara reales sin falsos positivos; H_narr(T) alcanza 2.7574 bits (91.9% del maximo teorico).",
            "  * EXP-002 (Atlas Trans-Dominio, 3 obras bilingues, 513.6 s, 77 ventanas): Separa geometricamente el debate civico (CORPUS-001),",
            "    el spot nuclear 'Daisy' de LBJ (CORPUS-002, 1964, pico E = 83.29) y el discurso 'Challenger' de Reagan (CORPUS-003, 1986,",
            "    inercia I = 0.8921, polarizacion minima P = 0.0617), con distancia euclidiana en espacio metrico 5D de hasta 0.6135.",
            "  * EXP-003 (Control Contrafactual y Optimizacion 6! = 720): Evalua las 720 permutaciones de bloques de CORPUS-001, hallando el",
            "    Montaje de Equilibrio Civico pi*_eq que reduce el sesgo normativo terminal en -62.0% (0.8620 -> 0.3274) sin alterar un solo",
            "    cuadro, y demuestra que el 29.84% de la movilizacion del spot 'Daisy' proviene exclusivamente del choque acustico-visual.",
            "  * EXP-004 (Identificacion Inversa de Sistemas y Generalizacion Trans-Dominio): Recupera exactamente Theta* en ausencia de ruido",
            "    (error 0.000%), logra R^2 = 0.9999 bajo ruido gaussiano de 25 dB SNR (50 ensayos Monte Carlo por nivel de 10 a 40 dB) y",
            "    alcanza R^2 fuera de muestra de 0.9999 en validacion cruzada Leave-One-Corpus-Out (LOCO) entre idiomas y generos."
        ]);

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
