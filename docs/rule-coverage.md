# PurpleTrace Rule Coverage

Generated UTC: 2026-06-23 13:46:14

Total Rules: 4

## Severity Coverage

| Severity | Count |
|---|---|
| High | 2 |
| Medium | 2 |

## MITRE Coverage

| Technique ID | Technique Name | Rule Count |
|---|---|---|
| T1059 | Command and Scripting Interpreter | 1 |
| T1059.001 | PowerShell | 2 |
| T1082 | System Information Discovery | 1 |

## Tag Coverage

| Tag | Rule Count |
|---|---|
| cmd | 2 |
| command-line | 2 |
| discovery | 1 |
| encoded-command | 1 |
| execution | 3 |
| powershell | 3 |
| reconnaissance | 1 |
| windows | 4 |

## Rule Table

| Rule ID | Title | Severity | MITRE | Tags |
|---|---|---|---|---|
| PT-RULE-001 | Suspicious PowerShell Execution | High | T1059.001 PowerShell | powershell, execution, windows, command-line |
| PT-RULE-002 | Command Shell Started PowerShell | Medium | T1059 Command and Scripting Interpreter | powershell, cmd, execution, windows |
| PT-RULE-003 | Windows Discovery Commands | Medium | T1082 System Information Discovery | discovery, windows, cmd, reconnaissance |
| PT-RULE-004 | Encoded PowerShell Command | High | T1059.001 PowerShell | powershell, encoded-command, execution, windows, command-line |

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

