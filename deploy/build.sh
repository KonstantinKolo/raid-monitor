#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
ROOT_DIR="$(cd "$SCRIPT_DIR/.." && pwd)"
DIST_DIR="$ROOT_DIR/dist"

if ! command -v dotnet &>/dev/null; then
    echo "Error: .NET SDK not found. Install from https://dot.net/download"
    exit 1
fi

echo "Building RAID Monitor release binaries..."

rm -rf "$DIST_DIR"
mkdir -p "$DIST_DIR"

for rid in linux-arm64 linux-x64; do
    echo "  Publishing $rid..."
    dotnet publish "$ROOT_DIR/RaidMonitor.Api" \
        -c Release \
        --self-contained \
        -r "$rid" \
        -o "$DIST_DIR/$rid" \
        -p:PublishSingleFile=false \
        --nologo -v quiet

    echo "  Packing $rid tarball..."
    tar -czf "$DIST_DIR/raidmonitor-$rid.tar.gz" -C "$DIST_DIR/$rid" .
    rm -rf "$DIST_DIR/$rid"
done

echo ""
echo "Done. Tarballs:"
ls -lh "$DIST_DIR"/raidmonitor-*.tar.gz
echo ""
echo "Upload these to a GitHub release, then users install with:"
echo "  sudo bash install.sh"
