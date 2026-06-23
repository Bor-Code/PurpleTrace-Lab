# PurpleTrace Simulator

PurpleTrace Simulator is a safe synthetic telemetry generator for PurpleTrace Lab.

It creates endpoint-style process creation events that can be analyzed by PurpleTrace Agent.

The simulator does not execute commands, launch processes, download files, or perform offensive activity. It only writes static JSON or JSONL telemetry examples.

---

## Purpose

The simulator helps test detection rules safely.

Instead of running real commands, it generates synthetic events that look like endpoint telemetry.

---

## Supported Scenarios

* `powershell`
* `encoded-powershell`
* `certutil`
* `registry-discovery`
* `rundll32-url`
* `service-discovery`
* `all`

---

## Generate JSONL Telemetry

```powershell
dotnet run --project src\PurpleTrace.Simulator -- --scenario all --format jsonl --out samples\simulated-events.local.jsonl
```

---

## Generate JSON Telemetry

```powershell
dotnet run --project src\PurpleTrace.Simulator -- --scenario all --format json --out samples\simulated-events.local.json
```

---

## Analyze Simulated Telemetry

```powershell
dotnet run --project src\PurpleTrace.Agent -- --source sample --rules rules --event samples\simulated-events.local.jsonl --out samples\simulator-alerts.local.json --report samples\simulator-report.local.md --csv samples\simulator-alerts.local.csv --html samples\simulator-report.local.html --summary samples\simulator-summary.local.json
```

---

## Safety Scope

PurpleTrace Simulator only creates synthetic telemetry files.

It does not:

* Execute PowerShell
* Execute certutil
* Execute rundll32
* Execute registry commands
* Execute service discovery commands
* Download files
* Contact external systems
* Run payloads
* Modify the operating system
