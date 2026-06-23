# PurpleTrace Investigation Case Template

Use this template to document a structured alert investigation.

---

## 1. Case Summary

**Case ID:**
**Investigation Date:**
**Analyst:**
**Status:** Open / In Review / Closed
**Outcome:** Expected Activity / Suspicious Activity / Needs More Context / False Positive

---

## 2. Alert Details

**Alert ID:**
**Timestamp UTC:**
**Rule ID:**
**Rule Name:**
**Severity:**
**Hostname:**
**Username:**

---

## 3. MITRE ATT&CK Mapping

**Tactic:**
**Technique ID:**
**Technique Name:**

---

## 4. Rule Context

**Rule Author:**
**Rule Created UTC:**
**Rule Tags:**
**Rule References:**

---

## 5. Process Evidence

**Process Name:**
**Process Path:**
**Command Line:**

**Parent Process Name:**
**Parent Process Path:**
**Parent Command Line:**

---

## 6. Matched Evidence

**Evidence Summary:**

**Matched Fields:**

*

**Matched Values:**

*

---

## 7. Source Event Review

**Event Source:**
**Event ID:**
**Event Type:**
**Process ID:**
**Parent Process ID:**
**Destination IP:**
**Destination Port:**
**Protocol:**

---

## 8. Analyst Questions

Answer the following questions during review:

* Was this command expected for the user?
* Was the parent process normal?
* Is the command line suspicious?
* Does this match expected administrator behavior?
* Are multiple rules firing on the same source event?
* Is there repeated activity from the same host?
* Is the MITRE mapping accurate?
* Does the evidence summary explain the alert clearly?

---

## 9. Analyst Notes

Write investigation notes here.

---

## 10. Recommended Next Steps

* Review related alerts from the same host.
* Review related alerts from the same user.
* Check whether the command line is expected.
* Compare the parent process with normal behavior.
* Validate whether this is admin activity or suspicious execution.
* Document the final decision.

---

## 11. Final Decision

**Classification:** Expected Activity / Suspicious Activity / Needs More Context / False Positive

**Reason:**

**Closed By:**
**Closed Date:**
