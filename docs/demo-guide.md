# PurpleTrace Lab Demo Guide

This guide explains how to run a safe local demo of PurpleTrace Lab.

PurpleTrace Lab is a defensive endpoint telemetry and detection engineering lab. It generates synthetic telemetry, analyzes events with detection rules, exports reports, and visualizes alerts in a local dashboard.

---

## Demo Flow

The recommended demo flow is:

```text
Simulator -> Agent -> Reports -> Dashboard -> Investigation Report
```

This means:

1. Generate safe synthetic endpoint telemetry.
2. Analyze the telemetry with PurpleTrace Agent.
3. Export alerts and reports.
4. Load the JSON alert output in the local dashboard.
5. Review the generated investigation report.

---

## 1. Generate Synthetic Telemetry

Run the simulator:

```powershell
dotnet run --project src\PurpleTrace.Simulator -- --scenario all --format jsonl --out samples\simulated-events.local.jsonl
```

Expected result:

```text
PurpleTrace Simulator
Scenario: all
Format: jsonl
Generated events: 6
```

The simulator writes safe static telemetry only. It does not execute commands.

---

## 2. Analyze Telemetry with PurpleTrace Agent

Run the Agent against the generated telemetry:

```powershell
dotnet run --project src\PurpleTrace.Agent -- --source sample --rules rules --event samples\simulated-events.local.jsonl --out samples\simulator-alerts.local.json --report samples\simulator-report.local.md --csv samples\simulator-alerts.local.csv --html samples\simulator-report.local.html --summary samples\simulator-summary.local.json --investigation samples\simulator-investigation.local.md
```

Expected result:

```text
PurpleTrace Agent - Detection Pipeline
Loaded rules: 8
Loaded events: 6
Exported alerts: ...
```

The exact alert count may change when new rules are added.

---

## 3. Review Exported Files

PurpleTrace Agent can export multiple output formats:

```text
samples\simulator-alerts.local.json
samples\simulator-report.local.md
samples\simulator-alerts.local.csv
samples\simulator-report.local.html
samples\simulator-summary.local.json
samples\simulator-investigation.local.md
```

These files are local outputs and are not meant to be committed.

---

## 4. Open the Local Dashboard

Open the dashboard:

```powershell
start dashboard\index.html
```

Then load this file in the browser:

```text
samples\simulator-alerts.local.json
```

The dashboard displays:

* Total alerts
* High alerts
* MITRE technique count
* Detection rule count
* Severity distribution
* MITRE technique distribution
* Rule distribution
* Searchable alert cards
* Command-line evidence
* Rule tags

The dashboard runs locally in the browser and does not upload data anywhere.

---

## 5. Review the Investigation Report

Open the investigation report:

```powershell
code samples\simulator-investigation.local.md
```

The investigation report includes structured cases for each alert:

* Case summary
* Alert details
* MITRE ATT&CK mapping
* Rule context
* Process evidence
* Matched fields
* Matched values
* Source event review
* Analyst questions
* Recommended next steps
* Final decision section

---

## 6. Optional: Run the Sample Config Demo

PurpleTrace also includes a sample config file:

```powershell
dotnet run --project src\PurpleTrace.Agent -- --config config\purpletrace.sample.json --investigation samples\investigation-report.local.md
```

Expected result:

```text
Loaded rules: 8
Loaded events: 2
Exported alerts: 3
```

---

## Demo Talking Points

When explaining the project, focus on this:

```text
PurpleTrace Lab is a defensive detection engineering project.
It simulates endpoint telemetry, applies detection rules, exports investigation-ready reports, and visualizes alerts in a local dashboard.
```

Key points:

* Built with C# and .NET
* Uses JSON-based detection rules
* Supports MITRE ATT&CK mapping
* Generates JSON, Markdown, CSV, HTML, summary, and investigation outputs
* Includes a safe synthetic telemetry simulator
* Includes a local browser dashboard
* Includes structured analyst investigation workflow
* Defensive-only and safe for portfolio demonstration

---

## Safety Scope

PurpleTrace Lab does not include:

* Malware
* Exploit code
* Credential theft
* Persistence creation
* Evasion implementation
* Shellcode
* Process injection
* Destructive behavior

The project focuses on defensive detection engineering and safe alert review.
