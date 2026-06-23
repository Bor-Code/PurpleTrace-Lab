# Sample Investigation Case: Suspicious PowerShell Execution

This sample shows how a PurpleTrace alert can be reviewed using the investigation workflow.

---

## 1. Case Summary

**Case ID:** PT-CASE-001
**Investigation Date:** 2026-06-23
**Analyst:** PurpleTrace Lab
**Status:** Closed
**Outcome:** Needs More Context

---

## 2. Alert Details

**Alert ID:** Sample alert generated from PurpleTrace Agent
**Timestamp UTC:** Generated during local test
**Rule ID:** PT-RULE-001
**Rule Name:** Suspicious PowerShell Execution
**Severity:** High
**Hostname:** BORAN
**Username:** nonmr

---

## 3. MITRE ATT&CK Mapping

**Tactic:** Execution
**Technique ID:** T1059.001
**Technique Name:** PowerShell

---

## 4. Rule Context

**Rule Author:** PurpleTrace Lab
**Rule Created UTC:** 2026-06-22T00:00:00Z

**Rule Tags:**

* powershell
* execution
* windows
* command-line

**Rule References:**

* MITRE ATT&CK T1059.001 - PowerShell

---

## 5. Process Evidence

**Process Name:** powershell.exe
**Process Path:** C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe

**Command Line:**

```text
powershell.exe -NoProfile -ExecutionPolicy Bypass
```

**Parent Process Name:** cmd.exe
**Parent Process Path:** C:\Windows\System32\cmd.exe

**Parent Command Line:**

```text
cmd.exe
```

---

## 6. Matched Evidence

**Evidence Summary:**

```text
ProcessName contains powershell.exe; CommandLine contains -NoProfile; CommandLine contains ExecutionPolicy Bypass
```

**Matched Fields:**

* ProcessName
* CommandLine

**Matched Values:**

* ProcessName contains powershell.exe
* CommandLine contains -NoProfile
* CommandLine contains ExecutionPolicy Bypass

---

## 7. Source Event Review

**Event Source:** Sample
**Event ID:** 1
**Event Type:** Process creation
**Process ID:** 4321
**Parent Process ID:** 1234
**Destination IP:** N/A
**Destination Port:** N/A
**Protocol:** N/A

---

## 8. Analyst Questions

### Was this command expected for the user?

Unknown. More context is needed.

### Was the parent process normal?

PowerShell was started from cmd.exe. This can be normal for administrator activity, but it should be reviewed.

### Is the command line suspicious?

Yes. The command line contains flags commonly reviewed in security investigations:

* `-NoProfile`
* `-ExecutionPolicy Bypass`

### Does this match expected administrator behavior?

Possibly, but confirmation is required.

### Are multiple rules firing on the same source event?

Yes. This event may also match command shell to PowerShell behavior.

### Is the MITRE mapping accurate?

Yes. The alert maps to PowerShell execution under MITRE ATT&CK T1059.001.

### Does the evidence summary explain the alert clearly?

Yes. The evidence summary shows which fields matched and why the alert was generated.

---

## 9. Analyst Notes

This alert was generated from a safe sample event in PurpleTrace Lab.

The command line includes PowerShell execution with suspicious flags. In a real environment, an analyst should check whether the user or administrator intentionally ran this command.

The parent process is cmd.exe, which may be legitimate but should be reviewed together with user activity, host history, and surrounding alerts.

---

## 10. Recommended Next Steps

* Review other alerts from the same host.
* Review other alerts from the same user.
* Check whether this command was expected.
* Review command history or endpoint telemetry if available.
* Compare with normal administrator behavior.
* Check whether similar alerts occurred repeatedly.
* Document whether the activity is expected, suspicious, or a false positive.

---

## 11. Final Decision

**Classification:** Needs More Context

**Reason:**
The command line contains suspicious PowerShell flags, but the sample alone does not prove malicious activity. More host and user context would be required in a real investigation.

**Closed By:** PurpleTrace Lab
**Closed Date:** 2026-06-23
