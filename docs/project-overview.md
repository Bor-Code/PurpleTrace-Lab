# Project Overview

PurpleTrace Lab is a defensive endpoint telemetry and detection engineering lab.

The goal of the project is to show how endpoint events can be loaded, analyzed with detection rules, enriched with useful metadata, filtered, and exported into reports.

---

## Main Idea

The project simulates a small detection pipeline:

```text
Input telemetry → Detection rules → Alerts → Reports
```

The telemetry can come from sample JSON files, JSONL files, or Sysmon process creation logs.

Rules are written as JSON documents. This makes the detection logic easy to review, update, and extend without changing application code.

---

## Core Components

### Event Loading

PurpleTrace can load endpoint-style events from:

* Single JSON event files
* JSON arrays
* JSONL / NDJSON files
* Sysmon Event ID 1 process creation logs

### Detection Rules

Rules describe suspicious or important process behavior.

A rule can match on:

* Process name
* Command line
* Parent process name

### Detection Engine

The detection engine receives endpoint events and detection rules.

When an event matches a rule, the engine creates a detection alert.

### Alert Enrichment

Generated alerts include:

* Rule ID
* Rule name
* Severity
* MITRE ATT&CK tactic
* MITRE ATT&CK technique
* Rule author
* Rule tags
* Rule references
* Hostname
* User
* Process details
* Evidence summary
* Matched fields
* Matched values

### Filtering

Alerts can be filtered by:

* Minimum severity
* MITRE technique ID
* Rule ID
* Rule tag

### Exporting

The project supports multiple output formats:

* JSON
* Markdown
* CSV
* HTML
* Summary JSON

---

## Why This Project Is Useful

This project is useful as a portfolio project because it connects several real-world defensive security concepts:

* Endpoint telemetry
* Detection rules
* Alert triage
* MITRE ATT&CK mapping
* Evidence-based reporting
* CLI-based security tooling
* Safe lab design

It is not a malware tool and it does not perform offensive actions.
