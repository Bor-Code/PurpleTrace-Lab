
> [!CAUTION]
> # ⛔🚨 WARNING 🚨⛔
> ## 🔴 UNDER DEVELOPMENT — PLEASE DO NOT USE! 🔴
> ### ❌ REVIEW ONLY AND SHARE YOUR THOUGHTS &lt;3 ❌ 

# PurpleTrace Lab

PurpleTrace Lab is a defensive Windows endpoint telemetry and detection engineering lab built for safe Purple Team learning.

The project loads endpoint-style events, applies JSON-based detection rules, enriches alerts with MITRE ATT&CK metadata, and exports the results into analyst-friendly reports.

> This project is defensive-only. It does not contain malware, exploit code, evasion logic, shellcode, process injection, credential theft, or offensive payloads.

---

## What This Project Demonstrates

PurpleTrace Lab demonstrates practical skills in:

* Windows endpoint telemetry concepts
* Detection engineering
* JSON-based rule design
* MITRE ATT&CK mapping
* Alert enrichment
* CLI tool development
* Report generation
* Unit testing
* GitHub Actions CI

---

## Features

* Sample endpoint event loading
* Batch JSON event loading
* JSONL / NDJSON event loading
* Sysmon process creation event collection
* JSON detection rule loading
* Rule validation
* Rule catalog listing
* Severity-based alert filtering
* MITRE technique filtering
* Rule ID filtering
* Rule tag filtering
* Alert evidence enrichment
* JSON alert export
* Markdown report export
* CSV alert export
* HTML report export
* Detection run summary export
* Unit tests with xUnit
* GitHub Actions build workflow

---

## Project Structure

```text
PurpleTrace-Lab/
├── config/
│   └── purpletrace.sample.json
├── docs/
├── rules/
│   ├── suspicious_powershell.json
│   ├── command_shell_started_powershell.json
│   └── windows_discovery_commands.json
├── samples/
│   ├── sample-powershell-event.json
│   ├── sample-recon-event.json
│   ├── sample-event-batch.json
│   └── sample-event-batch.jsonl
├── src/
│   └── PurpleTrace.Agent/
├── tests/
│   └── PurpleTrace.Agent.Tests/
└── README.md
```

---

## How It Works

PurpleTrace Agent follows a simple detection pipeline:

```text
Endpoint Events
      ↓
Event Loader / Sysmon Collector
      ↓
Detection Rules
      ↓
Detection Engine
      ↓
Alert Enrichment
      ↓
Filters
      ↓
Reports and Exports
```

The detection engine compares endpoint event fields such as process name, command line, and parent process name against rule conditions.

When a rule matches, the generated alert includes:

* Rule information
* Severity
* MITRE tactic and technique
* Host and process details
* Evidence summary
* Matched fields
* Matched values
* Rule metadata
* Source event reference

---

## Requirements

* Windows
* .NET SDK
* Optional: Sysmon for real Windows event log collection

Check your .NET installation:

```powershell
dotnet --version
```

---

## Build and Test

```powershell
dotnet build
dotnet test
```

---

## Basic Usage

Run with the sample config:

```powershell
dotnet run --project src\PurpleTrace.Agent -- --config config\purpletrace.sample.json
```

Expected result:

```text
Detected alerts before filtering: 3
Exported alerts: 3
```

---

## List Rules

```powershell
dotnet run --project src\PurpleTrace.Agent -- --list-rules --rules rules
```

---

## Validate Rules

```powershell
dotnet run --project src\PurpleTrace.Agent -- --validate-rules --rules rules
```

---

## Run with JSONL Events

```powershell
dotnet run --project src\PurpleTrace.Agent -- --source sample --rules rules --event samples\sample-event-batch.jsonl --out samples\jsonl-alerts.local.json --report samples\jsonl-report.local.md --csv samples\jsonl-alerts.local.csv --html samples\jsonl-report.local.html --summary samples\jsonl-summary.local.json
```

---

## Filters

Minimum severity filter:

```powershell
dotnet run --project src\PurpleTrace.Agent -- --config config\purpletrace.sample.json --min-severity High
```

MITRE technique filter:

```powershell
dotnet run --project src\PurpleTrace.Agent -- --config config\purpletrace.sample.json --mitre-technique T1082
```

Rule ID filter:

```powershell
dotnet run --project src\PurpleTrace.Agent -- --config config\purpletrace.sample.json --rule-id PT-RULE-003
```

Rule tag filter:

```powershell
dotnet run --project src\PurpleTrace.Agent -- --config config\purpletrace.sample.json --tag discovery
```

---

## Output Files

The sample config generates local output files under `samples/`:

```text
samples/config-alerts.local.json
samples/config-report.local.md
samples/config-alerts.local.csv
samples/config-report.local.html
samples/config-summary.local.json
```

These local generated files are intended for testing and are ignored by Git.

---

## Detection Rules

Rules are stored as JSON files in the `rules/` directory.

Each rule can include:

* Rule ID
* Title
* Description
* Severity
* MITRE tactic
* MITRE technique ID
* MITRE technique name
* Author
* Creation date
* Tags
* References
* Process matching conditions
* Command line matching conditions
* Parent process matching conditions

Example rule intent:

```text
Detect suspicious PowerShell execution patterns from endpoint process creation telemetry.
```

---

## Reports

PurpleTrace Lab can generate multiple output formats:

| Format       | Purpose                                             |
| ------------ | --------------------------------------------------- |
| JSON         | Machine-readable alert output                       |
| Markdown     | Analyst-readable report                             |
| CSV          | Spreadsheet-friendly alert export                   |
| HTML         | Visual report for review and portfolio presentation |
| Summary JSON | Machine-readable run summary                        |

---

## Safety Scope

This project is designed for defensive security education and portfolio demonstration.

It does not perform exploitation, persistence, privilege escalation, credential theft, evasion, malware execution, or unauthorized activity.

The included sample events are static telemetry examples used to test detection logic safely.

---

## Documentation

Additional documentation:

* [Project Overview](docs/project-overview.md)
* [How to Explain This Project](docs/how-to-explain.md)
* [Safety Scope](docs/safety.md)
* [Roadmap](docs/roadmap.md)

---

## Current Status

PurpleTrace Lab is currently a portfolio-ready defensive detection engineering lab.

The next improvements are focused on documentation, screenshots, rule coverage, and release packaging rather than adding unnecessary features.

---

## Synthetic Telemetry Simulator

PurpleTrace Lab includes a safe synthetic telemetry simulator.

The simulator writes JSON or JSONL endpoint-style events without executing commands or performing system changes.

Generate simulated telemetry:

```powershell
dotnet run --project src\PurpleTrace.Simulator -- --scenario all --format jsonl --out samples\simulated-events.local.jsonl
```

Analyze simulated telemetry:

```powershell
dotnet run --project src\PurpleTrace.Agent -- --source sample --rules rules --event samples\simulated-events.local.jsonl --out samples\simulator-alerts.local.json --report samples\simulator-report.local.md --csv samples\simulator-alerts.local.csv --html samples\simulator-report.local.html --summary samples\simulator-summary.local.json
```

More details: [PurpleTrace Simulator](docs/simulator.md)

---

## Investigation Workflow

PurpleTrace Lab includes a structured investigation workflow for reviewing detection alerts.

The workflow helps move from raw alert output to analyst-style review by documenting:

* Alert details
* Rule context
* MITRE ATT&CK mapping
* Process evidence
* Matched fields and matched values
* Source event review
* Analyst questions
* Recommended next steps
* Final decision

Investigation workflow documentation:

```text
docs/investigation-workflow.md
```

Reusable case template:

```text
templates/investigation-case-template.md
```

Sample investigation case:

```text
samples/sample-investigation-case.md
```

This component is designed for defensive alert review and portfolio demonstration.

---

## PurpleTrace Lab v0.2.0

PurpleTrace Lab v0.2.0 turns the project into a more complete defensive detection engineering lab.

The project now includes:

* Safe synthetic telemetry generation
* JSON-based detection rules
* MITRE ATT&CK metadata
* JSON, Markdown, CSV, HTML, summary, and investigation report exports
* Local browser-based detection dashboard
* Structured analyst investigation workflow
* Rule coverage documentation
* Defensive-only demo flow

Recommended demo flow:

```text
Simulator -> Agent -> Reports -> Dashboard -> Investigation Report
```

Generate safe synthetic telemetry:

```powershell
dotnet run --project src\PurpleTrace.Simulator -- --scenario all --format jsonl --out samples\simulated-events.local.jsonl
```

Analyze telemetry and generate reports:

```powershell
dotnet run --project src\PurpleTrace.Agent -- --source sample --rules rules --event samples\simulated-events.local.jsonl --out samples\simulator-alerts.local.json --report samples\simulator-report.local.md --csv samples\simulator-alerts.local.csv --html samples\simulator-report.local.html --summary samples\simulator-summary.local.json --investigation samples\simulator-investigation.local.md
```

Open the local dashboard:

```powershell
start dashboard\index.html
```

Then load:

```text
samples\simulator-alerts.local.json
```

Useful documentation:

```text
docs/demo-guide.md
docs/v0.2.0-release-notes.md
docs/investigation-workflow.md
docs/rule-coverage.md
docs/simulator.md
```

---

## Current Capabilities

PurpleTrace Lab currently supports:

* Loading sample endpoint telemetry from JSON and JSONL files
* Reading Sysmon process creation events on Windows
* Loading JSON-based detection rules
* Rule validation
* Rule catalog listing
* Rule coverage export
* Severity filtering
* MITRE technique filtering
* Rule ID filtering
* Rule tag filtering
* JSON alert export
* Markdown report export
* CSV alert export
* HTML report export
* JSON run summary export
* Markdown investigation report export
* Safe synthetic telemetry simulation
* Local browser-based dashboard
* Structured investigation workflow

Current detection coverage includes:

* Suspicious PowerShell execution
* Command shell started PowerShell
* Windows discovery commands
* Encoded PowerShell command
* Certutil download pattern
* Registry discovery command
* Rundll32 URL handler usage
* Windows service discovery command
