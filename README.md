# RAID Monitor

A lightweight, visually modern RAID health checker for Linux, built for the homelab community.

## The Problem
Linux homelabbers running software RAID have no purpose-built tool to monitor array health — just raw `mdadm` output or custom bash scripts. RAID Monitor fixes that.

## Features (V1)
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

## Getting Started
_Installation instructions coming soon._

## Status
Active development — V1 in progress.