[CmdletBinding()]
param(
    [ValidateSet('Auto', 'Server', 'Led')]
    [string]$Role = 'Auto',
    [string]$ProgramDataRoot = "$env:ProgramData\TG Exhibition",
    [switch]$RepairLedAudioCache
)

$ErrorActionPreference = 'Stop'
$failed = $false

function Write-Check([string]$name, [bool]$passed, [string]$detail) {
    $label = if ($passed) { 'PASS' } else { 'FAIL' }
    if (-not $passed) { $script:failed = $true }
    Write-Host "[$label] $name - $detail"
}

$dataRoot = Join-Path $ProgramDataRoot 'Data'
$uiPath = Join-Path $dataRoot 'ui-experience.json'
$cacheRoot = Join-Path $ProgramDataRoot 'Cache\LedPlayer\Content'
$playerLog = Join-Path $ProgramDataRoot 'Logs\LedPlayer\Player.log'
$runServerChecks = $Role -in @('Auto', 'Server') -and (Test-Path -LiteralPath $uiPath -PathType Leaf)
$runLedChecks = $Role -in @('Auto', 'Led') -and ((Test-Path -LiteralPath $cacheRoot -PathType Container) -or
    (Get-Process -Name LedPlayer -ErrorAction SilentlyContinue))

if ($runServerChecks) {
    try {
        $config = Get-Content -LiteralPath $uiPath -Raw | ConvertFrom-Json
        $audio = @($config.touchElements | Where-Object { $_.key -eq 'welcome.audio' }) | Select-Object -Last 1
        Write-Check 'Welcome audio configuration' ($null -ne $audio) 'welcome.audio is present'
        if ($null -ne $audio) {
            $required = -not [string]::IsNullOrWhiteSpace($audio.assetId) -and
                -not [string]::IsNullOrWhiteSpace($audio.assetUrl) -and
                -not [string]::IsNullOrWhiteSpace($audio.assetSha256) -and
                [long]$audio.assetSizeBytes -gt 0 -and
                -not [string]::IsNullOrWhiteSpace($audio.assetMediaType)
            Write-Check 'Welcome audio metadata' $required "url=$($audio.assetUrl), bytes=$($audio.assetSizeBytes), type=$($audio.assetMediaType)"

            $pathPart = ([string]$audio.assetUrl -split '[?#]')[0]
            if ([Uri]::IsWellFormedUriString($pathPart, [UriKind]::Absolute)) { $pathPart = ([Uri]$pathPart).AbsolutePath }
            $storedName = [Uri]::UnescapeDataString(($pathPart -replace '^/media/', ''))
            $mediaPath = Join-Path (Join-Path $dataRoot 'Media') $storedName
            $exists = Test-Path -LiteralPath $mediaPath -PathType Leaf
            Write-Check 'Server welcome audio file' $exists $mediaPath
            if ($exists) {
                $file = Get-Item -LiteralPath $mediaPath
                Write-Check 'Server welcome audio size' ($file.Length -eq [long]$audio.assetSizeBytes) "actual=$($file.Length), expected=$($audio.assetSizeBytes)"
                $actualHash = (Get-FileHash -LiteralPath $mediaPath -Algorithm SHA256).Hash.ToLowerInvariant()
                Write-Check 'Server welcome audio SHA-256' ($actualHash -eq ([string]$audio.assetSha256).ToLowerInvariant()) $actualHash
            }
        }
    } catch {
        Write-Check 'Server welcome audio inspection' $false $_.Exception.Message
    }
} elseif ($Role -eq 'Server') {
    Write-Check 'Server UI configuration' $false "Not found: $uiPath"
}

if ($runLedChecks) {
    $players = @(Get-CimInstance Win32_Process -Filter "Name='LedPlayer.exe'" -ErrorAction SilentlyContinue)
    Write-Check 'LedPlayer process count' ($players.Count -eq 1) "count=$($players.Count); only one process should run"
    if (Test-Path -LiteralPath $cacheRoot -PathType Container) {
        $audioCache = @(Get-ChildItem -LiteralPath $cacheRoot -File | Where-Object { $_.Extension -in '.mp3', '.wav' })
        Write-Host "[INFO] LED audio cache files: $($audioCache.Count) in $cacheRoot"
    }
    if (Test-Path -LiteralPath $playerLog -PathType Leaf) {
        Write-Host '[INFO] Recent welcome-related player logs:'
        Get-Content -LiteralPath $playerLog -Tail 300 |
            Select-String 'Welcome|欢迎音频|欢迎语音|缓存校验|解码失败' |
            Select-Object -Last 40 | ForEach-Object { Write-Host $_.Line }
    } else {
        Write-Host "[INFO] Player log not found: $playerLog"
    }

    if ($RepairLedAudioCache) {
        $resolvedRoot = [IO.Path]::GetFullPath($ProgramDataRoot).TrimEnd('\') + '\'
        $resolvedCache = [IO.Path]::GetFullPath($cacheRoot)
        if (-not $resolvedCache.StartsWith($resolvedRoot, [StringComparison]::OrdinalIgnoreCase)) {
            throw "Refusing to clean cache outside ProgramDataRoot: $resolvedCache"
        }
        Get-Process -Name LedPlayer -ErrorAction SilentlyContinue | Stop-Process -Force
        if (Test-Path -LiteralPath $resolvedCache -PathType Container) {
            Get-ChildItem -LiteralPath $resolvedCache -File |
                Where-Object { $_.Extension -in '.mp3', '.wav' } |
                Remove-Item -Force
        }
        $launcher = Join-Path $env:ProgramFiles 'TG Exhibition Led\Launcher\TG.Control.Launcher.exe'
        if (Test-Path -LiteralPath $launcher -PathType Leaf) {
            Start-Process -FilePath $launcher -WindowStyle Hidden
            Write-Host '[REPAIRED] LED audio cache cleared and launcher restarted.'
        } else {
            Write-Host "[REPAIRED] LED audio cache cleared. Launcher not found: $launcher"
        }
    }
} elseif ($Role -eq 'Led') {
    Write-Check 'LedPlayer installation' $false "No process or cache found under $ProgramDataRoot"
}

if (-not $runServerChecks -and -not $runLedChecks -and $Role -eq 'Auto') {
    Write-Check 'TG Exhibition installation' $false "No Server or LedPlayer data found under $ProgramDataRoot"
}

if ($failed) { exit 1 }
exit 0
