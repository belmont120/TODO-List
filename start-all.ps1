<#
.SYNOPSIS
    Starts UserService, TodoService and the Angular app, then opens the Angular app in the browser.

.DESCRIPTION
    Each app runs in its own PowerShell window so its logs stay visible.
    Close a window (or press Ctrl+C in it) to stop that app.

.EXAMPLE
    .\start-all.ps1
#>

$ErrorActionPreference = 'Stop'

$root       = $PSScriptRoot
$angularUrl = 'http://localhost:4200'

$apps = @(
    @{ Name = 'UserService'; Dir = 'TODO-UserService'; Command = 'dotnet run --no-restore --launch-profile http'; Port = 5123 },
    @{ Name = 'TodoService'; Dir = 'TODO-TodoService'; Command = 'dotnet run --no-restore --launch-profile http'; Port = 5066 },
    @{ Name = 'Angular';     Dir = 'TODO-Angular';     Command = 'npm start';                                    Port = 4200 }
)

# Checks both IPv4 and IPv6 loopback: the Angular dev server may listen on ::1 only,
# and a default TcpClient is IPv4-only.
function Test-Port([int]$Port) {
    foreach ($address in [Net.IPAddress]::Loopback, [Net.IPAddress]::IPv6Loopback) {
        $client = New-Object System.Net.Sockets.TcpClient($address.AddressFamily)
        try {
            if ($client.ConnectAsync($address, $Port).Wait(500) -and $client.Connected) {
                return $true
            }
        } catch {
            # Connection refused on this address; try the next one.
        } finally {
            $client.Dispose()
        }
    }

    $false
}

# Restore NuGet packages for the backend services up front so errors surface here.
foreach ($app in $apps | Where-Object { $_.Command -like 'dotnet*' }) {
    Write-Host "Restoring $($app.Name)..." -ForegroundColor Yellow
    dotnet restore (Join-Path $root $app.Dir)

    if ($LASTEXITCODE -ne 0) {
        Write-Error "dotnet restore failed for $($app.Name)."
        exit 1
    }
}

# Install Angular dependencies on first run.
$angularDir = Join-Path $root 'TODO-Angular'
if (-not (Test-Path (Join-Path $angularDir 'node_modules'))) {
    Write-Host 'Installing Angular dependencies...' -ForegroundColor Yellow
    Push-Location $angularDir
    try {
        npm install

        if ($LASTEXITCODE -ne 0) {
            Write-Error 'npm install failed for the Angular app.'
            exit 1
        }
    } finally {
        Pop-Location
    }
}

foreach ($app in $apps) {
    if (Test-Port $app.Port) {
        Write-Host "$($app.Name) already running on port $($app.Port), skipping." -ForegroundColor DarkYellow
        continue
    }

    $dir = Join-Path $root $app.Dir
    $cmd = "`$Host.UI.RawUI.WindowTitle = '$($app.Name)'; Set-Location '$dir'; $($app.Command)"
    Start-Process powershell -ArgumentList '-NoExit', '-Command', $cmd | Out-Null
    Write-Host "Started $($app.Name) (port $($app.Port))" -ForegroundColor Green
}

# Wait for the Angular dev server before opening the browser.
Write-Host "Waiting for Angular at $angularUrl ..." -ForegroundColor Cyan
$timeout = [TimeSpan]::FromMinutes(3)
$sw = [Diagnostics.Stopwatch]::StartNew()

while (-not (Test-Port 4200)) {
    if ($sw.Elapsed -gt $timeout) {
        Write-Warning "Angular did not start within $($timeout.TotalMinutes) minutes. Check the Angular window for errors."
        exit 1
    }

    Start-Sleep -Seconds 1
}

Start-Process $angularUrl
Write-Host "Opened $angularUrl" -ForegroundColor Green