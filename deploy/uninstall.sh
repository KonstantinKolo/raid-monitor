#!/usr/bin/env bash
set -euo pipefail

SERVICE_NAME="raidmonitor"
INSTALL_DIR="/opt/raidmonitor"

if [[ $EUID -ne 0 ]]; then
    echo "Error: This script must be run as root (sudo bash uninstall.sh)"
    exit 1
fi

echo "Uninstalling RAID Monitor..."

if systemctl is-active --quiet "$SERVICE_NAME" 2>/dev/null; then
    systemctl stop "$SERVICE_NAME"
fi

if systemctl is-enabled --quiet "$SERVICE_NAME" 2>/dev/null; then
    systemctl disable "$SERVICE_NAME"
fi

rm -f /etc/systemd/system/raidmonitor.service
systemctl daemon-reload

rm -rf "$INSTALL_DIR"

echo "RAID Monitor has been removed."
