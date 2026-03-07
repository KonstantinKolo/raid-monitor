# RAID Monitor

A lightweight, visually modern RAID health checker for Linux, built for the homelab community.

## The Problem
Linux homelabbers running software RAID have no purpose-built tool to monitor array health — just raw `mdadm` output or custom bash scripts. RAID Monitor fixes that.

## Features
- Visual per-drive slot status (healthy / degraded / failed / rebuilding)
- SMART data surfacing (health, temperature, reallocated sectors)
- Rebuild/resync progress tracking
- Real-time push updates via SignalR
- Runs as a systemd service, accessed via browser on localhost

## Tech Stack
- **Backend** — ASP.NET Core (C#), systemd service
- **Frontend** — Blazor WebAssembly + MudBlazor
- **Real-time** — SignalR
- **Data sources** — `/proc/mdstat`, `mdadm`, `smartctl`

## Install

Download the latest release tarball for your architecture (`linux-arm64` or `linux-x64`) along with `install.sh` and `raidmonitor.service` from the [releases page](https://github.com/your-user/RaidMonitor/releases), then:

```bash
sudo bash install.sh
```

The script will:
- Install `mdadm` and `smartmontools` if missing
- Extract the binary to `/opt/raidmonitor`
- Set up and start a systemd service

Open **http://localhost:5291** in your browser.

## Uninstall

```bash
sudo bash deploy/uninstall.sh
```

## Building from Source

Requires [.NET 10 SDK](https://dot.net/download).

```bash
# Build release tarballs for arm64 and x64
bash deploy/build.sh
```

Tarballs are output to `dist/`.

## Development

```bash
# Build, then run with root (needed for mdadm/smartctl)
dotnet build
sudo dotnet run --project RaidMonitor.Api --no-build
```

Open **http://localhost:5291**.

## Configuration

The service listens on `http://localhost:5291` by default. To change the port, edit `/etc/systemd/system/raidmonitor.service`:

```ini
Environment=ASPNETCORE_URLS=http://localhost:YOUR_PORT
```

Then reload: `sudo systemctl daemon-reload && sudo systemctl restart raidmonitor`
