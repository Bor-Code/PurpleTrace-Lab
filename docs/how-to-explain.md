# How to Explain PurpleTrace Lab

This document is a short guide for explaining PurpleTrace Lab in an interview, portfolio review, or LinkedIn discussion.

---

## Short Explanation

PurpleTrace Lab is a defensive Windows endpoint telemetry and detection engineering project.

It loads endpoint-style process events, applies JSON-based detection rules, maps alerts to MITRE ATT&CK, enriches detections with evidence, and exports results as JSON, Markdown, CSV, HTML, and summary reports.

---

## 30-Second Pitch

PurpleTrace Lab is a safe Purple Team portfolio project focused on detection engineering. I built a CLI-based detection pipeline that can read sample endpoint events, JSONL telemetry, or Sysmon process creation logs. It applies JSON detection rules, enriches matching alerts with MITRE ATT&CK metadata and evidence details, supports filtering, and generates multiple report formats for analysis.

---

## What Problem Does It Solve?

Security teams need to understand endpoint activity and detect suspicious behavior.

This project demonstrates a simplified version of that workflow:

```text
Collect or load telemetry
Analyze it with detection rules
Generate alerts
Add useful context
Export reports
```

---

## Main Technical Parts

### Endpoint Events

The project uses endpoint-style process creation events.

These events include fields like:

* Hostname
* Username
* Process name
* Process path
* Command line
* Parent process name
* Parent command line

### Rules

Rules are written in JSON.

This makes the rules easy to edit and review.

A rule can define:

* Severity
* MITRE technique
* Process matching conditions
* Command line matching conditions
* Parent process matching conditions
* Tags and references

### Detection Engine

The detection engine checks each event against each rule.

When a rule matches, the engine creates an alert.

### Evidence

Alerts include evidence details.

For example:

```text
ProcessName contains powershell.exe
CommandLine contains -NoProfile
```

This helps explain why the alert was generated.

### Reports

The project generates:

* JSON for machine-readable alerts
* Markdown for analyst notes
* CSV for spreadsheet review
* HTML for visual reporting
* Summary JSON for run statistics

---

## What I Learned

This project helped me practice:

* C# and .NET CLI development
* File loading and JSON parsing
* Detection rule design
* MITRE ATT&CK mapping
* Report generation
* Unit testing
* Git branching and pull request workflow
* GitHub Actions CI

---

## Safe Scope Explanation

This project is defensive-only.

It does not include:

* Malware
* Exploits
* Shellcode
* Process injection
* Credential theft
* Persistence
* Evasion
* Unauthorized activity

The sample events are static telemetry examples used for safe detection testing.

---

## If Someone Asks Whether AI Was Used

A good honest answer:

```text
I used AI and documentation as development support while building this project, but I understand the architecture and can explain the detection pipeline, rule format, filtering, alert enrichment, and report outputs.
```

This is better than claiming every line was written without help.

---

## Best Way to Demo It

Run:

```powershell
dotnet build
dotnet test
dotnet run --project src\PurpleTrace.Agent -- --config config\purpletrace.sample.json
start samples\config-report.local.html
```

Then explain:

```text
This command loads sample endpoint telemetry, applies the rules, generates alerts, enriches them with MITRE and evidence details, and opens the HTML report.
```
