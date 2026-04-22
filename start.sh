#!/usr/bin/env bash
set -e

ROOT_DIR="$(cd "$(dirname "$0")" && pwd)"
BACKEND_DIR="$ROOT_DIR/backend"
FRONTEND_DIR="$ROOT_DIR/frontend"

command -v dotnet &>/dev/null || { echo "dotnet não encontrado."; exit 1; }
command -v npm    &>/dev/null || { echo "npm não encontrado.";    exit 1; }

[ ! -d "$FRONTEND_DIR/node_modules" ] && (cd "$FRONTEND_DIR" && npm install)

cleanup() { kill $(jobs -p) 2>/dev/null || true; }
trap cleanup EXIT INT TERM

echo "  Front-end → http://localhost:5173"
echo "  API       → http://localhost:5000"
echo "  Ctrl+C para encerrar"
echo ""

(cd "$BACKEND_DIR" && dotnet run 2>&1 | sed 's/^/[backend]  /') &

sleep 3

(cd "$FRONTEND_DIR" && npm run dev 2>&1 | sed 's/^/[frontend] /') &

wait
