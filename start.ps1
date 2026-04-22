$ErrorActionPreference = "Stop"
$Root     = $PSScriptRoot
$Backend  = Join-Path $Root "backend"
$Frontend = Join-Path $Root "frontend"

Get-Command dotnet -ErrorAction Stop | Out-Null
Get-Command npm    -ErrorAction Stop | Out-Null

if (-not (Test-Path (Join-Path $Frontend "node_modules"))) {
    Push-Location $Frontend; npm install; Pop-Location
}

Start-Process powershell "-NoExit -Command Set-Location '$Backend';  dotnet run"
Start-Sleep 3
Start-Process powershell "-NoExit -Command Set-Location '$Frontend'; npm run dev"

Write-Host ""
Write-Host "  Front-end -> http://localhost:5173" -ForegroundColor Green
Write-Host "  API       -> http://localhost:5000" -ForegroundColor Cyan
Write-Host "  Swagger   -> http://localhost:5000/swagger" -ForegroundColor Cyan
Write-Host ""
Read-Host "Pressione Enter para fechar"
