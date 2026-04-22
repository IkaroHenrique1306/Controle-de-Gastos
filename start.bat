@echo off
setlocal
set "ROOT=%~dp0"

where dotnet >nul 2>&1 || (echo [ERRO] dotnet nao encontrado. & pause & exit /b 1)
where npm    >nul 2>&1 || (echo [ERRO] npm nao encontrado.    & pause & exit /b 1)

if not exist "%ROOT%frontend\node_modules" (
    echo Instalando dependencias do front-end...
    pushd "%ROOT%frontend" & call npm install & popd
)

echo.
echo   Front-end ^> http://localhost:5173
echo   API       ^> http://localhost:5000
echo   Swagger   ^> http://localhost:5000/swagger
echo   Ctrl+C para encerrar
echo.

:: Inicia o backend em segundo plano
start /b cmd /c "cd /d "%ROOT%backend" && dotnet run 2>&1"

:: Aguarda o backend subir
timeout /t 4 /nobreak >nul

:: Inicia o frontend com --open: o Vite abre o navegador sozinho quando estiver pronto
cd /d "%ROOT%frontend"
npm run dev -- --open
