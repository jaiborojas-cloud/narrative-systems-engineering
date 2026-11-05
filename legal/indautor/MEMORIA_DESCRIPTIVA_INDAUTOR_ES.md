# Memoria Descriptiva y Especificación Funcional para Registro Público del Derecho de Autor (INDAUTOR)

**Título de la Obra:** `ARGON.ENGINE & NSE.PIPELINE: SISTEMA COMPUTACIONAL PARA INGENIERÍA DE SISTEMAS NARRATIVOS (NARRATIVE SYSTEMS ENGINEERING - NSE) Y DINÁMICA NARRATIVA COMPUTACIONAL (CND)`  
**Autor y Titular Único (100%):** Jair Ivan Rojas Ruiz (*Jair Rojas*)  
**ORCID iD:** [https://orcid.org/0009-0003-1109-4583](https://orcid.org/0009-0003-1109-4583)  
**Identificador Científico Internacional (DOI Zenodo / CERN):** [https://doi.org/10.5281/zenodo.23987656](https://doi.org/10.5281/zenodo.23987656) (Concept DOI: `10.5281/zenodo.23987655`)  
**Rama de Registro (Ley Federal del Derecho de Autor, Art. 13):** Programa de Cómputo (`INDAUTOR-01-002`) y Obra Científica/Técnica (`INDAUTOR-01-001`)  
**Fecha de Creación y Archivo Criptográfico Primigenio:** 8 de septiembre de 2026  
**Versión del Sistema:** `v0.4.0` (5 de noviembre de 2026)

---

## 1. Declaración de Autoría, Originalidad y Prioridad Cronológica

La presente obra es una creación intelectual **100% original y primigenia** de **Jair Ivan Rojas Ruiz**, quien ostenta la totalidad (100%) de los derechos morales y patrimoniales.

1. **Archivo Criptográfico Fundacional (8 de septiembre de 2026):** Los tres tratados fundacionales en español que dan origen a la arquitectura conceptual del sistema (*Documento 1: Atlas de la Narrativa Humana [HNA]*, *Documento 2: Motor de Dinámica Narrativa [NDE]* y *Documento 3: Ingeniería de Sistemas Narrativos [NSE]*, con un total de 63 páginas) fueron archivados criptográficamente el **8 de septiembre de 2026** mediante funciones picadillo `SHA-256`.
2. **Desambiguación Técnica:** El sistema `Argon.Engine` y el marco matemático de **Ingeniería de Sistemas Narrativos Multimodal (NSE)** y **Dinámica Narrativa Computacional (CND)** constituyen una disciplina cuantitativa fundamentada en procesamiento físico de señales de audio y video, teoría de la información de Shannon y teoría de control óptimo en espacio de estados, diferenciándose de forma absoluta de cualquier ensayo literario o taxonomía cualitativa homónima carente de formulación matemática o software ejecutable.

---

## 2. Objeto y Propósito Funcional del Programa de Cómputo

El sistema compuesto por los motores **`Nse.Pipeline`** y **`Argon.Engine`** (desarrollados en lenguaje **C# 13 sobre la plataforma .NET 10**) resuelve el problema técnico de transformar archivos audiovisuales crudos (contenedores `.mp4`, `.ogv`, `.webm`) y transcripciones alineadas en el tiempo en un **modelo dinámico en espacio de estados verificable y reproducible**, permitiendo:
1. **Extracción Física de Señales Multimodales $u(t) = (u_{\text{ac}}(t), u_{\text{vis}}(t), u_{\text{ling}}(t))$:**
   - **Subsistema Acústico (`AcousticAnalyzer.cs`):** Decodifica flujos de audio PCM de 16 bits a $16\text{ kHz}$ mono y calcula por ventana temporal la amplitud cuadrática media ($\text{RMS}$), el nivel en decibeles a escala completa ($L_{\text{dB}} \in \text{dBFS}$), la razón de actividad vocal ($\text{VAD} \in [0,1]$ en subtramas de $100\text{ ms}$) y el rango dinámico intra-ventana $\Delta L_{90-10}$.
   - **Subsistema Visual (`VisualDynamicsAnalyzer.cs`):** Decodifica matrices de píxeles RGB24 de $64\times 64$ ($4,096$ píxeles por fotograma) y calcula la luminancia perceptual ITU-R BT.709, el contraste espacial $\sigma_Y$, la saturación cromática HSV, la entropía cromática de Shannon $H_{\text{vis}}$ y la **Divergencia de Jensen-Shannon** $D_{\text{JS}}(P_t \parallel P_{t-1}) \in [0,1]\text{ bits}$ sobre histogramas de color de 512 celdas para la detección determinista de cortes de cámara ($D_{\text{JS}} \ge 0.30\text{ bits}$).
   - **Subsistema Lingüístico y Normativo (`LinguisticAnalyzer.cs`):** Procesa segmentos de habla alineados en el tiempo (en español e inglés) para cuantificar cadencia léxica ($\text{WPM}$), diversidad léxica ($\text{TTR}$), densidad de marcadores normativos, densidad de conectores dialécticos, anclaje institucional $A_{\text{inst}}(t)$ y vectores de postura $z(t) \in [-1,1]^K$.
2. **Cálculo de las Seis Métricas Fundamentales de NSE (`NarrativeMetricsEngine.cs`):**
   - **ADN Narrativo ($\mathcal{G}_{\text{DNA}}$):** Grafo tripartito con signo que vincula hablantes, arquetipos simbólicos y dimensiones de creencia con evidencia textual fechada.
   - **Energía Narrativa $E(t) \in [0,100]$:** Funcional de acoplamiento lineal-geométrico con resonancia intermodal $\Phi_{\text{sync}}(t) = \sqrt{I_{\text{ac}}(t)\cdot I_{\text{ling}}(t)}$.
   - **Entropía Narrativa de Postura de Shannon $H_{\text{narr}}(t) \in [0, 3.00]\text{ bits}$:** Dispersión informacional acumulada sobre los $2K = 8$ polos de creencia.
   - **Complejidad Narrativa $C(t) \in [0,100]$:** Combinación de diversidad léxica, subordinación dialéctica y tensión multi-valor.
   - **Inercia Narrativa $\mathcal{I}(t) \in [0,1]$:** Resistencia al desplazamiento cognitivo basada en anclaje institucional, convicción acumulada y exposición temporal.
   - **Controlabilidad en Espacio de Estados y Gramiano $W_c$:** Evolución discreta $x_k(t+1) = (1-\gamma_k(t))x_k(t) + \gamma_k(t)z_k(t)$ y cálculo espectral del Gramiano de Controlabilidad $W_c(0,N)$ y su número de condición $\kappa(W_c)$.
3. **Atlas Comparativo Multi-Obra (`AtlasComparator.cs`):**
   - Proyecta múltiples obras audiovisuales en el espacio métrico 5D de NSE y calcula distancias euclidianas estructurales y órbitas en el espacio de fases $(\mathcal{I}(t), E(t), H(t))$.
4. **Control Contrafactual y Optimización Exhaustiva de Montaje (`CounterfactualController.cs`):**
   - Evalúa las $6! = 720$ permutaciones de bloques editoriales para determinar la envolvente alcanzable del estado terminal $x(T)$ y sintetizar secuencias óptimas (como el montaje de equilibrio cívico de mínimo sesgo $\min_{\pi} \|x(T)\|_2$).
5. **Identificación Inversa de Sistemas y Calibración Empírica (`SystemIdentifier.cs`, `EXP-004`):**
   - Resuelve el problema inverso mediante Regresión Regularizada de Tikhonov (Ridge System Identification) $\hat{\Theta}(\lambda) = (\Psi^\top \Psi + \lambda I)^{-1}(\Psi^\top \Delta\tilde{X} + \lambda \Theta_0)$ sobre $N = 308$ transiciones de estado para recuperar empíricamente los pesos multimodales $(\alpha_{\text{ac}}, \alpha_{\text{vis}}, \alpha_{\text{ling}}, \alpha_{\text{sync}}, \beta_0)$ bajo ruido gaussiano de telemetría ($10\text{ dB}$ a $40\text{ dB SNR}$) y validación cruzada Leave-One-Corpus-Out ($R^2_{\text{OOS}} = 0.9999$).
6. **Generación de Dashboards Interactivos y Compilación Nativa de Manuscritos PDF (`DashboardRenderer.cs` & `PaperManuscriptRenderer.cs`):**
   - Genera paneles HTML5/Canvas autónomos y compila directamente documentos vectoriales en formato **PDF 1.4** sin dependencias externas.

---

## 3. Arquitectura de Módulos del Código Fuente

| Módulo / Archivo Fuente | Ruta en el Repositorio | Descripción Técnica |
|---|---|---|
| `Program.cs` (Argon.Engine) | `argon/src/Argon.Engine/Program.cs` | Orquestador CLI con modos de perfilado base, `atlas`, `control`, `sysid`, `paper` e `indautor`. |
| `AcousticAnalyzer.cs` | `argon/src/Argon.Engine/AcousticAnalyzer.cs` | Analizador de formas de onda WAV PCM16 ($16\text{ kHz}$), $\text{RMS}$, $\text{dBFS}$ y $\text{VAD}$. |
| `VisualDynamicsAnalyzer.cs` | `argon/src/Argon.Engine/VisualDynamicsAnalyzer.cs` | Analizador de matrices RGB24 ($64\times 64$), luminancia BT.709 y divergencia $D_{\text{JS}}$. |
| `LinguisticAnalyzer.cs` | `argon/src/Argon.Engine/LinguisticAnalyzer.cs` | Analizador léxico-normativo bilingüe (ES/EN), cadencia $\text{WPM}$ y postura $z(t)$. |
| `NarrativeMetricsEngine.cs` | `argon/src/Argon.Engine/NarrativeMetricsEngine.cs` | Motor matemático de las 6 métricas NSE, ecuación de estado LTV y Gramiano $W_c$. |
| `DashboardRenderer.cs` | `argon/src/Argon.Engine/DashboardRenderer.cs` | Generador de tablero interactivo HTML5 para telemetría de obra individual (`EXP-001`). |
| `AtlasComparator.cs` | `argon/src/Argon.Engine/AtlasComparator.cs` | Motor de comparación en espacio de fases multi-obra y matriz de distancias (`EXP-002`). |
| `CounterfactualController.cs` | `argon/src/Argon.Engine/CounterfactualController.cs` | Optimizador combinatorio ($6! = 720$) y simulador de control contrafactual (`EXP-003`). |
| `SystemIdentifier.cs` | `argon/src/Argon.Engine/SystemIdentifier.cs` | Motor de identificación inversa de sistemas (Tikhonov Ridge) y validación LOCO (`EXP-004`). |
| `PaperManuscriptRenderer.cs` | `argon/src/Argon.Engine/PaperManuscriptRenderer.cs` | Compilador determinista de manuscritos científicos en HTML y PDF 1.4 vectorial. |
| `IndautorPackageGenerator.cs` | `argon/src/Argon.Engine/IndautorPackageGenerator.cs` | Generador de ejemplares foliados en PDF 1.4 para registro ante INDAUTOR. |
| `Program.cs` (Nse.Pipeline) | `src/Nse.Pipeline/Program.cs` | Pipeline de ingesta multimedia, validación de manifiestos y extracción FFmpeg/FFprobe. |
| `VideoProfiler.cs` | `src/Nse.Pipeline/VideoProfiler.cs` | Extractor determinista de fotogramas, rasters RGB24 y audio WAV con hash `SHA-256`. |
| `ReviewWorkflow.cs` | `src/Nse.Pipeline/ReviewWorkflow.cs` | Gestor de libro de códigos visual versionado e importación de anotaciones auditadas. |
