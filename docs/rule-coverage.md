# PurpleTrace Rule Coverage

Generated UTC: 2026-06-23 14:35:36

Total Rules: 7

## Severity Coverage

| Severity | Count |
|---|---|
| High | 3 |
| Medium | 4 |

## MITRE Coverage

| Technique ID | Technique Name | Rule Count |
|---|---|---|
| T1012 | Query Registry | 1 |
| T1059 | Command and Scripting Interpreter | 1 |
| T1059.001 | PowerShell | 2 |
| T1082 | System Information Discovery | 1 |
| T1105 | Ingress Tool Transfer | 1 |
| T1218.011 | Rundll32 | 1 |

## Tag Coverage

| Tag | Rule Count |
|---|---|
| certutil | 1 |
| cmd | 2 |
| command-line | 5 |
| discovery | 2 |
| download | 1 |
| encoded-command | 1 |
| execution | 3 |
| living-off-the-land | 2 |
| powershell | 3 |
| reconnaissance | 1 |
| reg | 1 |
| registry | 1 |
| rundll32 | 1 |
| url-handler | 1 |
| windows | 7 |

## Rule Table

| Rule ID | Title | Severity | MITRE | Tags |
|---|---|---|---|---|
| PT-RULE-001 | Suspicious PowerShell Execution | High | T1059.001 PowerShell | powershell, execution, windows, command-line |
| PT-RULE-002 | Command Shell Started PowerShell | Medium | T1059 Command and Scripting Interpreter | powershell, cmd, execution, windows |
| PT-RULE-003 | Windows Discovery Commands | Medium | T1082 System Information Discovery | discovery, windows, cmd, reconnaissance |
| PT-RULE-004 | Encoded PowerShell Command | High | T1059.001 PowerShell | powershell, encoded-command, execution, windows, command-line |
| PT-RULE-005 | Certutil Download Pattern | High | T1105 Ingress Tool Transfer | certutil, download, living-off-the-land, windows, command-line |
| PT-RULE-006 | Windows Registry Discovery Command | Medium | T1012 Query Registry | registry, discovery, windows, reg, command-line |
| PT-RULE-007 | Rundll32 URL Handler Usage | Medium | T1218.011 Rundll32 | rundll32, living-off-the-land, windows, url-handler, command-line |

## Rule Details

### PT-RULE-001 - Suspicious PowerShell Execution

- Severity: High
- MITRE Tactic: Execution
- MITRE Technique: T1059.001 - PowerShell
- Author: PurpleTrace Lab
- Created UTC: 2026-06-22T00:00:00Z
- Tags: powershell, execution, windows, command-line
- Description: Detects PowerShell execution with suspicious command-line arguments.

ProcessNameContains:
- `powershell.exe`
- `pwsh.exe`

CommandLineContains:
- `-NoProfile`
- `ExecutionPolicy Bypass`
- `-enc`
- `-nop`

References:
- MITRE ATT&CK T1059.001 - PowerShell

### PT-RULE-002 - Command Shell Started PowerShell

- Severity: Medium
- MITRE Tactic: Execution
- MITRE Technique: T1059 - Command and Scripting Interpreter
- Author: PurpleTrace Lab
- Created UTC: 2026-06-22T00:00:00Z
- Tags: powershell, cmd, execution, windows
- Description: Detects PowerShell launched from the Windows command shell.

ProcessNameContains:
- `powershell.exe`
- `pwsh.exe`

ParentProcessNameContains:
- `cmd.exe`

References:
- MITRE ATT&CK T1059 - Command and Scripting Interpreter

### PT-RULE-003 - Windows Discovery Commands

- Severity: Medium
- MITRE Tactic: Discovery
- MITRE Technique: T1082 - System Information Discovery
- Author: PurpleTrace Lab
- Created UTC: 2026-06-22T00:00:00Z
- Tags: discovery, windows, cmd, reconnaissance
- Description: Detects common Windows discovery commands such as whoami, ipconfig, systeminfo, and net user.

ProcessNameContains:
- `cmd.exe`

CommandLineContains:
- `whoami`
- `ipconfig`
- `systeminfo`
- `net user`

References:
- MITRE ATT&CK T1082 - System Information Discovery

### PT-RULE-004 - Encoded PowerShell Command

- Severity: High
- MITRE Tactic: Execution
- MITRE Technique: T1059.001 - PowerShell
- Author: PurpleTrace Lab
- Created UTC: 2026-06-22T00:00:00Z
- Tags: powershell, encoded-command, execution, windows, command-line
- Description: Detects PowerShell execution using encoded command arguments.

ProcessNameContains:
- `powershell.exe`

CommandLineContains:
- `-EncodedCommand`
- `-enc`

References:
- MITRE ATT&CK T1059.001 - PowerShell

### PT-RULE-005 - Certutil Download Pattern

- Severity: High
- MITRE Tactic: Command and Control
- MITRE Technique: T1105 - Ingress Tool Transfer
- Author: PurpleTrace Lab
- Created UTC: 2026-06-22T00:00:00Z
- Tags: certutil, download, living-off-the-land, windows, command-line
- Description: Detects certutil execution with command-line patterns commonly associated with file download activity.

ProcessNameContains:
- `certutil.exe`

CommandLineContains:
- `-urlcache`
- `http`
- `https`

References:
- MITRE ATT&CK T1105 - Ingress Tool Transfer

### PT-RULE-006 - Windows Registry Discovery Command

- Severity: Medium
- MITRE Tactic: Discovery
- MITRE Technique: T1012 - Query Registry
- Author: PurpleTrace Lab
- Created UTC: 2026-06-22T00:00:00Z
- Tags: registry, discovery, windows, reg, command-line
- Description: Detects registry query commands commonly used for Windows system or persistence discovery.

ProcessNameContains:
- `reg.exe`

CommandLineContains:
- `query`
- `HKLM`
- `HKCU`
- `CurrentVersion\Run`

References:
- MITRE ATT&CK T1012 - Query Registry

### PT-RULE-007 - Rundll32 URL Handler Usage

- Severity: Medium
- MITRE Tactic: Defense Evasion
- MITRE Technique: T1218.011 - Rundll32
- Author: PurpleTrace Lab
- Created UTC: 2026-06-22T00:00:00Z
- Tags: rundll32, living-off-the-land, windows, url-handler, command-line
- Description: Detects rundll32 execution patterns involving URL handler behavior.

ProcessNameContains:
- `rundll32.exe`

CommandLineContains:
- `url.dll,FileProtocolHandler`
- `http`
- `https`

References:
- MITRE ATT&CK T1218.011 - Rundll32

