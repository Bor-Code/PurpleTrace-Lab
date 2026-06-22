@"
# PurpleTrace Lab

PurpleTrace Lab is a Windows endpoint telemetry and detection engineering lab built for safe Purple Team training.

It loads detection rules from JSON files, analyzes endpoint events, maps alerts to MITRE ATT&CK techniques, and exports both machine-readable JSON alerts and human-readable Markdown reports.

> This project does not provide malware, bypass payloads, shellcode loaders, or working evasion implementations. It focuses on defensive telemetry, detection engineering, MITRE ATT&CK mapping, and safe behavior simulation.

---

## Features

- Endpoint event data model
- Detection alert model
- Rule-based detection engine
- JSON rule loader
- JSON alert exporter
- Markdown report exporter
- CLI options for rules, event source, output, and report paths
- Sample event loader
- Sysmon Process Create collector
- MITRE ATT&CK metadata in alerts
- Sample PowerShell and discovery command detections

---

## Current Detection Rules

| Rule ID | Rule Name | Severity | MITRE |
|---|---|---|---|
| PT-RULE-001 | Suspicious PowerShell Execution | High | T1059.001 PowerShell |
| PT-RULE-002 | Command Shell Started PowerShell | Medium | T1059 Command and Scripting Interpreter |
| PT-RULE-003 | Windows Discovery Commands | Medium | T1082 System Information Discovery |

---

## Project Structure

```text
PurpleTrace-Lab/
├── src/
│   ├── PurpleTrace.Agent/
│   │   ├── Collectors/
│   │   ├── Detection/
│   │   ├── Exporters/
│   │   ├── Mitre/
│   │   ├── Models/
│   │   └── Program.cs
│   └── PurpleTrace.Simulator/
├── rules/
├── samples/
├── docs/
├── dashboard/
└── .github/workflows/