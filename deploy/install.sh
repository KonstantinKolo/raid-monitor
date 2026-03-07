#!/usr/bin/env bash
set -euo pipefail

INSTALL_DIR="/opt/raidmonitor"
SERVICE_NAME="raidmonitor"
SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"

# --- Root check ---
if [[ $EUID -ne 0 ]]; then
    echo "Error: This script must be run as root (sudo bash install.sh)"
    exit 1
fi

# --- Install dependencies ---
install_packages() {
    local packages=("$@")
    if command -v apt-get &>/dev/null; then
        apt-get update -qq && apt-get install -y -qq "${packages[@]}"
    elif command -v dnf &>/dev/null; then
        dnf install -y -q "${packages[@]}"
    elif command -v yum &>/dev/null; then
        yum install -y -q "${packages[@]}"
    elif command -v pacman &>/dev/null; then
        pacman -S --noconfirm --needed "${packages[@]}"
    elif command -v zypper &>/dev/null; then
        zypper install -y "${packages[@]}"
    else
        echo "Error: No supported package manager found (apt, dnf, yum, pacman, zypper)."
        echo "Please install manually: ${packages[*]}"
        exit 1
    fi
}

for cmd_pkg in "mdadm:mdadm" "smartctl:smartmontools"; do
    cmd="${cmd_pkg%%:*}"
    pkg="${cmd_pkg##*:}"
    if ! command -v "$cmd" &>/dev/null; then
        echo "Installing $pkg..."
        install_packages "$pkg"
    fi
done

# --- Detect architecture ---
ARCH="$(uname -m)"
case "$ARCH" in
    aarch64|arm64) RID="linux-arm64" ;;
    x86_64)        RID="linux-x64" ;;
    *)
        echo "Error: Unsupported architecture: $ARCH"
        exit 1
        ;;
esac

TARBALL="$SCRIPT_DIR/raidmonitor-$RID.tar.gz"
if [[ ! -f "$TARBALL" ]]; then
    # Check in dist/ (if running from repo root)
    TARBALL="$SCRIPT_DIR/../dist/raidmonitor-$RID.tar.gz"
fi

if [[ ! -f "$TARBALL" ]]; then
    echo "Error: Tarball not found for $RID."
    echo "Expected: raidmonitor-$RID.tar.gz in the same directory as this script,"
    echo "or in ../dist/"
    echo ""
    echo "Download the correct tarball from the GitHub releases page,"
    echo "or build from source with: bash deploy/build.sh"
    exit 1
fi

# --- Stop existing service ---
if systemctl is-active --quiet "$SERVICE_NAME" 2>/dev/null; then
    echo "Stopping existing $SERVICE_NAME service..."
    systemctl stop "$SERVICE_NAME"
fi

# --- Install application ---
echo "Installing to $INSTALL_DIR..."
mkdir -p "$INSTALL_DIR"
tar -xzf "$TARBALL" -C "$INSTALL_DIR"
chmod +x "$INSTALL_DIR/RaidMonitor.Api"

# --- Install systemd service ---
echo "Installing systemd service..."
cp "$SCRIPT_DIR/raidmonitor.service" /etc/systemd/system/
systemctl daemon-reload
systemctl enable --now "$SERVICE_NAME"

echo ""
echo "RAID Monitor installed and running."
echo "Open http://localhost:5291 in your browser."
echo ""
echo "Useful commands:"
echo "  systemctl status $SERVICE_NAME    — check status"
echo "  journalctl -u $SERVICE_NAME -f    — view logs"
echo "  sudo bash $(basename "$0")        — re-run to update"
