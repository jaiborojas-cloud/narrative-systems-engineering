$ErrorActionPreference = 'Stop'
$repo = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
Push-Location $repo
try {
    dotnet build argon/src/Argon.Engine --nologo
    if ($LASTEXITCODE -ne 0) { throw 'Argon.Engine build failed' }

    dotnet run --project argon/src/Argon.Engine --no-build
    if ($LASTEXITCODE -ne 0) { throw 'Argon.Engine execution failed' }

    $expDir = Join-Path $repo 'argon/experiments/EXP-001-insumision-dynamics'
    $profilePath = Join-Path $expDir 'dynamics.profile.json'
    $csvPath = Join-Path $expDir 'telemetry.csv'
    $htmlPath = Join-Path $expDir 'dashboard.html'

    if (-not (Test-Path $profilePath) -or -not (Test-Path $csvPath) -or -not (Test-Path $htmlPath)) {
        throw 'Missing expected experiment output files'
    }

    $profile = Get-Content $profilePath -Raw | ConvertFrom-Json
    if ($profile.windowCount -ne 39) { throw "Expected 39 windows, got $($profile.windowCount)" }
    if ($profile.speakerSummaries.Count -ne 6) { throw "Expected 6 speaker/symbolic turns, got $($profile.speakerSummaries.Count)" }

    foreach ($pt in $profile.timeline) {
        if ($pt.narrativeEnergy -lt 0 -or $pt.narrativeEnergy -gt 100) {
            throw "Narrative Energy out of [0,100] bounds at window $($pt.windowIndex): $($pt.narrativeEnergy)"
        }
        if ($pt.cumulativeStanceEntropyBits -lt 0 -or $pt.cumulativeStanceEntropyBits -gt 3.0001) {
            throw "Shannon Stance Entropy out of [0, 3.0] bits bounds at window $($pt.windowIndex): $($pt.cumulativeStanceEntropyBits)"
        }
        if ($pt.narrativeInertia -lt 0 -or $pt.narrativeInertia -gt 1.0) {
            throw "Narrative Inertia out of [0,1] bounds at window $($pt.windowIndex): $($pt.narrativeInertia)"
        }
        if ($pt.visual.jensenShannonDivergenceFromPrev -lt 0 -or $pt.visual.jensenShannonDivergenceFromPrev -gt 1.0) {
            throw "Jensen-Shannon divergence out of [0,1] bits bounds at window $($pt.windowIndex)"
        }
    }

    # Verify deterministic scene cut detection matches true camera transitions (windows 0, 1, 18, 26)
    $cutIndices = @($profile.timeline | Where-Object { $_.visual.isSceneCutDetected } | Select-Object -ExpandProperty windowIndex)
    if (($cutIndices -join ',') -ne '0,1,18,26') {
        throw "Unexpected scene cut indices: $($cutIndices -join ',')"
    }

    # Verify Controllability Gramian eigenvalues are positive and condition number > 1
    if ($profile.controlAnalysis.minSensitivityEigenvalue -le 0 -or $profile.controlAnalysis.conditionNumberKappa -le 1.0) {
        throw "Invalid Controllability Gramian spectrum: kappa=$($profile.controlAnalysis.conditionNumberKappa)"
    }

    # Verify EXP-002 Public Domain Corpus works (LBJ Daisy 1964 & Reagan Challenger 1986)
    $daisyProfile = Get-Content (Join-Path $repo 'argon/experiments/EXP-002-public-domain-atlas/daisy-run/dynamics.profile.json') -Raw | ConvertFrom-Json
    $chalProfile = Get-Content (Join-Path $repo 'argon/experiments/EXP-002-public-domain-atlas/challenger-run/dynamics.profile.json') -Raw | ConvertFrom-Json

    if ($daisyProfile.windowCount -ne 12 -or $daisyProfile.globalIndices.peakNarrativeEnergy -lt 80.0) {
        throw "Unexpected LBJ Daisy 1964 profile metrics"
    }
    if ($chalProfile.windowCount -ne 26 -or $chalProfile.globalIndices.detectedSceneCutsCount -ne 1) {
        throw "Unexpected Reagan Challenger 1986 profile metrics"
    }

    # Build and verify Cross-Domain Phase-Space Atlas (Argon Atlas v0.2)
    dotnet run --project argon/src/Argon.Engine --no-build -- atlas
    if ($LASTEXITCODE -ne 0) { throw 'Argon.Engine atlas execution failed' }

    $atlasReportPath = Join-Path $repo 'argon/experiments/EXP-002-public-domain-atlas/atlas.report.json'
    $atlasHtmlPath = Join-Path $repo 'argon/experiments/EXP-002-public-domain-atlas/atlas-dashboard.html'
    if (-not (Test-Path $atlasReportPath) -or -not (Test-Path $atlasHtmlPath)) {
        throw 'Missing Cross-Domain Atlas report or dashboard HTML'
    }
    $atlas = Get-Content $atlasReportPath -Raw | ConvertFrom-Json
    if ($atlas.works.Count -ne 3 -or $atlas.pairwiseDistances.Count -ne 3) {
        throw 'Unexpected work or pairwise distance count in atlas.report.json'
    }

    # Build and verify Counterfactual Control & Montage Optimization Lab (EXP-003, Argon v0.3)
    dotnet run --project argon/src/Argon.Engine --no-build -- control
    if ($LASTEXITCODE -ne 0) { throw 'Argon.Engine control execution failed' }

    $ctrlReportPath = Join-Path $repo 'argon/experiments/EXP-003-counterfactual-control/control.report.json'
    $ctrlHtmlPath = Join-Path $repo 'argon/experiments/EXP-003-counterfactual-control/control-dashboard.html'
    if (-not (Test-Path $ctrlReportPath) -or -not (Test-Path $ctrlHtmlPath)) {
        throw 'Missing EXP-003 Counterfactual Control report or dashboard HTML'
    }
    $ctrl = Get-Content $ctrlReportPath -Raw | ConvertFrom-Json
    if ($ctrl.study1MontageOptimization.totalPermutationsEvaluated -ne 720) {
        throw "Expected 720 (6!) montage permutations, got $($ctrl.study1MontageOptimization.totalPermutationsEvaluated)"
    }
    if ($ctrl.study1MontageOptimization.optimalCivicEquilibrium.finalStateNormL2 -ge $ctrl.study1MontageOptimization.originalStateNormL2) {
        throw 'Optimal civic equilibrium norm did not improve over original broadcast norm'
    }

    # Build and verify Inverse System Identification & Parameter Calibration (EXP-004, Argon v0.4)
    dotnet run --project argon/src/Argon.Engine --no-build -- sysid
    if ($LASTEXITCODE -ne 0) { throw 'Argon.Engine sysid execution failed' }

    $sysidReportPath = Join-Path $repo 'argon/experiments/EXP-004-system-identification/sysid.report.json'
    $sysidHtmlPath = Join-Path $repo 'argon/experiments/EXP-004-system-identification/sysid-dashboard.html'
    if (-not (Test-Path $sysidReportPath) -or -not (Test-Path $sysidHtmlPath)) {
        throw 'Missing EXP-004 System Identification report or dashboard HTML'
    }
    $sysid = Get-Content $sysidReportPath -Raw | ConvertFrom-Json
    if ($sysid.totalStateTransitions -ne 308 -or $sysid.empiricalNoisyRecovery25dB.trajectoryR2 -lt 0.99) {
        throw "Unexpected EXP-004 System Identification metrics: R2=$($sysid.empiricalNoisyRecovery25dB.trajectoryR2)"
    }

    # Build and verify Print-Ready Paper Manuscript & LaTeX/BibTeX Package
    dotnet run --project argon/src/Argon.Engine --no-build -- paper
    if ($LASTEXITCODE -ne 0) { throw 'Argon.Engine paper execution failed' }

    foreach ($pubFile in @('publications/latex/PAPER-001-NSE-FOUNDATION.tex', 'publications/latex/references.bib', 'publications/latex/PAPER-001-NSE-FOUNDATION-PRINT.html', 'publications/PAPER-001-NSE-FOUNDATION.pdf')) {
        $pp = Join-Path $repo $pubFile
        if (-not (Test-Path $pp)) { throw "Missing publication package file: $pubFile" }
    }

    # Build and verify INDAUTOR Copyright Registration Package (legal/indautor/)
    dotnet run --project argon/src/Argon.Engine --no-build -- indautor
    if ($LASTEXITCODE -ne 0) { throw 'Argon.Engine indautor execution failed' }

    foreach ($indFile in @('legal/indautor/GUIA_REGISTRO_INDARELIN.md', 'legal/indautor/MEMORIA_DESCRIPTIVA_INDAUTOR_ES.md', 'legal/indautor/MEMORIA_DESCRIPTIVA_INDAUTOR_ES.pdf', 'legal/indautor/CODIGO_FUENTE_ARGON_NSE_INDAUTOR.pdf', 'legal/indautor/OBRA_CIENTIFICA_NSE_CND_INDAUTOR.pdf')) {
        $ip = Join-Path $repo $indFile
        if (-not (Test-Path $ip)) { throw "Missing INDAUTOR registration file: $indFile" }
    }

    # Verify Zenodo, Citation, Authorship, Disambiguation, and Audit Manifest integrity
    foreach ($metaFile in @('.zenodo.json', 'CITATION.cff', 'AUTHORS.md', 'LICENSE', 'NOTICE', 'AUDIT_MANIFEST.md', 'docs/PRIOR_WORK_AND_DISAMBIGUATION.md', 'data/raw/CORPUS_MANIFEST.json')) {
        $p = Join-Path $repo $metaFile
        if (-not (Test-Path $p)) { throw "Missing mandatory audit/provenance file: $metaFile" }
    }
    $zenodo = Get-Content (Join-Path $repo '.zenodo.json') -Raw | ConvertFrom-Json
    if ($zenodo.creators[0].orcid -ne '0009-0003-1109-4583') { throw 'Zenodo ORCID mismatch' }

    Write-Host "PASS [Argon Verification]: All 3 corpus works (CORPUS-001 Insumision, CORPUS-002 LBJ Daisy 1964, CORPUS-003 Reagan Challenger 1986) + Zenodo/ORCID provenance audit verified."
}
finally {
    Pop-Location
}
