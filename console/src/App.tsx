import { useState } from "react";
import {
  Activity,
  AlertTriangle,
  BarChart3,
  BookOpen,
  FileJson,
  FileText,
  Gauge,
  GitBranch,
  Layers,
  Monitor,
  Search,
  Shield,
  Terminal,
} from "lucide-react";
import {
  Bar,
  BarChart,
  CartesianGrid,
  Cell,
  Pie,
  PieChart,
  ResponsiveContainer,
  Tooltip,
  XAxis,
  YAxis,
} from "recharts";
import "./index.css";

type Severity = "Critical" | "High" | "Medium" | "Low" | "Informational";

type AlertItem = {
  alertId: string;
  ruleId: string;
  ruleName: string;
  severity: Severity;
  tactic: string;
  techniqueId: string;
  techniqueName: string;
  host: string;
  user: string;
  processName: string;
  commandLine: string;
  parentProcess: string;
  evidence: string;
  tags: string[];
};

type RuleItem = {
  id: string;
  name: string;
  severity: Severity;
  technique: string;
  tactic: string;
  tags: string[];
};

const sampleAlerts: AlertItem[] = [
  {
    alertId: "PT-ALERT-001",
    ruleId: "PT-RULE-001",
    ruleName: "Suspicious PowerShell Execution",
    severity: "High",
    tactic: "Execution",
    techniqueId: "T1059.001",
    techniqueName: "PowerShell",
    host: "PURPLETRACE-LAB",
    user: "lab-user",
    processName: "powershell.exe",
    commandLine: "powershell.exe -NoProfile -ExecutionPolicy Bypass",
    parentProcess: "cmd.exe",
    evidence:
      "ProcessName contains powershell.exe; CommandLine contains -NoProfile; CommandLine contains ExecutionPolicy Bypass",
    tags: ["powershell", "execution", "windows", "command-line"],
  },
  {
    alertId: "PT-ALERT-002",
    ruleId: "PT-RULE-004",
    ruleName: "Encoded PowerShell Command",
    severity: "High",
    tactic: "Execution",
    techniqueId: "T1059.001",
    techniqueName: "PowerShell",
    host: "PURPLETRACE-LAB",
    user: "lab-user",
    processName: "powershell.exe",
    commandLine: "powershell.exe -NoProfile -EncodedCommand SAFE_SAMPLE_ONLY",
    parentProcess: "cmd.exe",
    evidence:
      "ProcessName contains powershell.exe; CommandLine contains -EncodedCommand; CommandLine contains -enc",
    tags: ["powershell", "encoded-command", "execution", "windows"],
  },
  {
    alertId: "PT-ALERT-003",
    ruleId: "PT-RULE-005",
    ruleName: "Certutil Download Pattern",
    severity: "High",
    tactic: "Command and Control",
    techniqueId: "T1105",
    techniqueName: "Ingress Tool Transfer",
    host: "PURPLETRACE-LAB",
    user: "lab-user",
    processName: "certutil.exe",
    commandLine:
      "certutil.exe -urlcache -f https://example.com/SAFE_SAMPLE_ONLY.txt SAFE_SAMPLE_ONLY.txt",
    parentProcess: "cmd.exe",
    evidence:
      "ProcessName contains certutil.exe; CommandLine contains -urlcache; CommandLine contains http",
    tags: ["certutil", "download", "living-off-the-land", "windows"],
  },
  {
    alertId: "PT-ALERT-004",
    ruleId: "PT-RULE-002",
    ruleName: "Command Shell Started PowerShell",
    severity: "Medium",
    tactic: "Execution",
    techniqueId: "T1059",
    techniqueName: "Command and Scripting Interpreter",
    host: "PURPLETRACE-LAB",
    user: "lab-user",
    processName: "powershell.exe",
    commandLine: "powershell.exe -NoProfile -ExecutionPolicy Bypass",
    parentProcess: "cmd.exe",
    evidence: "ProcessName contains powershell.exe; ParentProcessName contains cmd.exe",
    tags: ["powershell", "cmd", "execution", "windows"],
  },
  {
    alertId: "PT-ALERT-005",
    ruleId: "PT-RULE-006",
    ruleName: "Windows Registry Discovery Command",
    severity: "Medium",
    tactic: "Discovery",
    techniqueId: "T1012",
    techniqueName: "Query Registry",
    host: "PURPLETRACE-LAB",
    user: "lab-user",
    processName: "reg.exe",
    commandLine: "reg.exe query HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\Run",
    parentProcess: "cmd.exe",
    evidence:
      "ProcessName contains reg.exe; CommandLine contains query; CommandLine contains HKLM",
    tags: ["registry", "discovery", "windows", "reg"],
  },
  {
    alertId: "PT-ALERT-006",
    ruleId: "PT-RULE-007",
    ruleName: "Rundll32 URL Handler Usage",
    severity: "Medium",
    tactic: "Defense Evasion",
    techniqueId: "T1218.011",
    techniqueName: "Rundll32",
    host: "PURPLETRACE-LAB",
    user: "lab-user",
    processName: "rundll32.exe",
    commandLine: "rundll32.exe url.dll,FileProtocolHandler https://example.com/SAFE_SAMPLE_ONLY",
    parentProcess: "cmd.exe",
    evidence:
      "ProcessName contains rundll32.exe; CommandLine contains url.dll,FileProtocolHandler",
    tags: ["rundll32", "living-off-the-land", "url-handler"],
  },
  {
    alertId: "PT-ALERT-007",
    ruleId: "PT-RULE-008",
    ruleName: "Windows Service Discovery Command",
    severity: "Medium",
    tactic: "Discovery",
    techniqueId: "T1007",
    techniqueName: "System Service Discovery",
    host: "PURPLETRACE-LAB",
    user: "lab-user",
    processName: "sc.exe",
    commandLine: "sc.exe query type= service state= all",
    parentProcess: "cmd.exe",
    evidence: "ProcessName contains sc.exe; CommandLine contains query",
    tags: ["service-discovery", "windows", "sc", "discovery"],
  },
];

const rules: RuleItem[] = [
  {
    id: "PT-RULE-001",
    name: "Suspicious PowerShell Execution",
    severity: "High",
    tactic: "Execution",
    technique: "T1059.001 PowerShell",
    tags: ["powershell", "execution", "windows"],
  },
  {
    id: "PT-RULE-002",
    name: "Command Shell Started PowerShell",
    severity: "Medium",
    tactic: "Execution",
    technique: "T1059 Command and Scripting Interpreter",
    tags: ["powershell", "cmd", "execution"],
  },
  {
    id: "PT-RULE-004",
    name: "Encoded PowerShell Command",
    severity: "High",
    tactic: "Execution",
    technique: "T1059.001 PowerShell",
    tags: ["encoded-command", "powershell"],
  },
  {
    id: "PT-RULE-005",
    name: "Certutil Download Pattern",
    severity: "High",
    tactic: "Command and Control",
    technique: "T1105 Ingress Tool Transfer",
    tags: ["certutil", "download"],
  },
  {
    id: "PT-RULE-006",
    name: "Windows Registry Discovery Command",
    severity: "Medium",
    tactic: "Discovery",
    technique: "T1012 Query Registry",
    tags: ["registry", "discovery"],
  },
  {
    id: "PT-RULE-007",
    name: "Rundll32 URL Handler Usage",
    severity: "Medium",
    tactic: "Defense Evasion",
    technique: "T1218.011 Rundll32",
    tags: ["rundll32", "living-off-the-land"],
  },
  {
    id: "PT-RULE-008",
    name: "Windows Service Discovery Command",
    severity: "Medium",
    tactic: "Discovery",
    technique: "T1007 System Service Discovery",
    tags: ["service-discovery", "windows"],
  },
];

const pages = [
  { id: "overview", label: "Overview", icon: Gauge },
  { id: "alerts", label: "Alerts", icon: AlertTriangle },
  { id: "rules", label: "Rules", icon: Shield },
  { id: "investigation", label: "Investigation", icon: BookOpen },
  { id: "reports", label: "Reports", icon: FileText },
  { id: "about", label: "About", icon: Layers },
];

const severityOrder: Record<string, number> = {
  Critical: 5,
  High: 4,
  Medium: 3,
  Low: 2,
  Informational: 1,
};

function App() {
  const [activePage, setActivePage] = useState("overview");
  const [alerts, setAlerts] = useState<AlertItem[]>(sampleAlerts);
  const [query, setQuery] = useState("");

  const filteredAlerts = alerts.filter((alert) => {
    const text = [
      alert.ruleId,
      alert.ruleName,
      alert.severity,
      alert.tactic,
      alert.techniqueId,
      alert.techniqueName,
      alert.host,
      alert.user,
      alert.processName,
      alert.commandLine,
      alert.evidence,
      alert.tags.join(" "),
    ]
      .join(" ")
      .toLowerCase();

    return text.includes(query.toLowerCase());
  });

  const highAlerts = alerts.filter((alert) => alert.severity === "High" || alert.severity === "Critical").length;
  const mitreCount = new Set(alerts.map((alert) => alert.techniqueId)).size;
  const ruleCount = new Set(alerts.map((alert) => alert.ruleId)).size;

  const severityData = toChartData(alerts, (alert) => alert.severity);
  const mitreData = toChartData(alerts, (alert) => `${alert.techniqueId} ${alert.techniqueName}`);
  const ruleData = toChartData(alerts, (alert) => alert.ruleId);

  async function handleFileUpload(file: File | null) {
    if (!file) {
      return;
    }

    const text = await file.text();
    const parsed = JSON.parse(text);
    const rawAlerts = Array.isArray(parsed) ? parsed : parsed.Alerts ?? parsed.alerts ?? [];

    const normalized = rawAlerts.map((item: any, index: number): AlertItem => ({
      alertId: item.AlertId ?? item.alertId ?? `LOCAL-${index + 1}`,
      ruleId: item.RuleId ?? item.ruleId ?? "UNKNOWN",
      ruleName: item.RuleName ?? item.ruleName ?? "Unknown Rule",
      severity: item.Severity ?? item.severity ?? "Medium",
      tactic: item.MitreTactic ?? item.tactic ?? "Unknown",
      techniqueId: item.MitreTechniqueId ?? item.techniqueId ?? "N/A",
      techniqueName: item.MitreTechniqueName ?? item.techniqueName ?? "N/A",
      host: item.Hostname ?? item.host ?? "N/A",
      user: item.UserName ?? item.user ?? "N/A",
      processName: item.ProcessName ?? item.processName ?? "N/A",
      commandLine: item.CommandLine ?? item.commandLine ?? "N/A",
      parentProcess: item.ParentProcessName ?? item.parentProcess ?? "N/A",
      evidence: item.EvidenceSummary ?? item.evidence ?? item.Reason ?? "No evidence summary available.",
      tags: item.RuleTags ?? item.tags ?? [],
    }));

    setAlerts(normalized);
    setActivePage("overview");
  }

  return (
    <div className="app">
      <aside className="sidebar">
        <div className="brand">
          <div className="brand-mark">
            <Shield size={24} />
          </div>
          <div>
            <strong>PurpleTrace</strong>
            <span>Console</span>
          </div>
        </div>

        <nav className="nav">
          {pages.map((page) => {
            const Icon = page.icon;

            return (
              <button
                key={page.id}
                className={activePage === page.id ? "nav-item active" : "nav-item"}
                onClick={() => setActivePage(page.id)}
              >
                <Icon size={18} />
                {page.label}
              </button>
            );
          })}
        </nav>

        <div className="sidebar-card">
          <p>Demo Flow</p>
          <strong>Simulator → Agent → Dashboard → Investigation</strong>
        </div>
      </aside>

      <main className="main">
        <header className="topbar">
          <div>
            <p className="eyebrow">Defensive Detection Engineering Lab</p>
            <h1>{getPageTitle(activePage)}</h1>
          </div>

          <label className="upload">
            <FileJson size={18} />
            Load Alert JSON
            <input
              type="file"
              accept=".json,application/json"
              onChange={(event) => handleFileUpload(event.target.files?.[0] ?? null)}
            />
          </label>
        </header>

        {activePage === "overview" && (
          <OverviewPage
            totalAlerts={alerts.length}
            highAlerts={highAlerts}
            mitreCount={mitreCount}
            ruleCount={ruleCount}
            severityData={severityData}
            mitreData={mitreData}
            ruleData={ruleData}
            recentAlerts={alerts}
          />
        )}

        {activePage === "alerts" && (
          <AlertsPage alerts={filteredAlerts} query={query} setQuery={setQuery} />
        )}

        {activePage === "rules" && <RulesPage rules={rules} />}

        {activePage === "investigation" && <InvestigationPage alerts={alerts} />}

        {activePage === "reports" && <ReportsPage />}

        {activePage === "about" && <AboutPage />}
      </main>
    </div>
  );
}

function OverviewPage(props: {
  totalAlerts: number;
  highAlerts: number;
  mitreCount: number;
  ruleCount: number;
  severityData: { name: string; value: number }[];
  mitreData: { name: string; value: number }[];
  ruleData: { name: string; value: number }[];
  recentAlerts: AlertItem[];
}) {
  return (
    <div className="page-grid">
      <MetricCard icon={Activity} label="Total Alerts" value={props.totalAlerts} />
      <MetricCard icon={AlertTriangle} label="High Alerts" value={props.highAlerts} danger />
      <MetricCard icon={GitBranch} label="MITRE Techniques" value={props.mitreCount} />
      <MetricCard icon={Shield} label="Detection Rules" value={props.ruleCount} />

      <section className="panel chart-panel">
        <PanelTitle icon={BarChart3} title="Severity Distribution" />
        <ResponsiveContainer width="100%" height={260}>
          <PieChart>
            <Pie data={props.severityData} dataKey="value" nameKey="name" outerRadius={90}>
              {props.severityData.map((entry) => (
                <Cell key={entry.name} />
              ))}
            </Pie>
            <Tooltip />
          </PieChart>
        </ResponsiveContainer>
      </section>

      <section className="panel chart-panel wide">
        <PanelTitle icon={Monitor} title="MITRE Technique Distribution" />
        <ResponsiveContainer width="100%" height={260}>
          <BarChart data={props.mitreData}>
            <CartesianGrid strokeDasharray="3 3" />
            <XAxis dataKey="name" hide />
            <YAxis />
            <Tooltip />
            <Bar dataKey="value" />
          </BarChart>
        </ResponsiveContainer>
      </section>

      <section className="panel wide">
        <PanelTitle icon={AlertTriangle} title="Recent Alerts" />
        <div className="alert-stack">
          {props.recentAlerts.slice(0, 4).map((alert) => (
            <AlertCard key={alert.alertId} alert={alert} compact />
          ))}
        </div>
      </section>

      <section className="panel">
        <PanelTitle icon={Shield} title="Rule Distribution" />
        <div className="rule-bars">
          {props.ruleData.map((item) => (
            <div key={item.name} className="rule-bar">
              <div>
                <span>{item.name}</span>
                <strong>{item.value}</strong>
              </div>
              <div className="bar-track">
                <div style={{ width: `${Math.min(100, item.value * 35)}%` }} />
              </div>
            </div>
          ))}
        </div>
      </section>
    </div>
  );
}

function AlertsPage(props: {
  alerts: AlertItem[];
  query: string;
  setQuery: (value: string) => void;
}) {
  return (
    <div className="single-column">
      <section className="panel">
        <div className="searchbox">
          <Search size={18} />
          <input
            value={props.query}
            onChange={(event) => props.setQuery(event.target.value)}
            placeholder="Search rule, MITRE, process, command line, evidence..."
          />
        </div>
      </section>

      <div className="alert-stack">
        {props.alerts.map((alert) => (
          <AlertCard key={alert.alertId} alert={alert} />
        ))}
      </div>
    </div>
  );
}

function RulesPage({ rules }: { rules: RuleItem[] }) {
  return (
    <div className="single-column">
      {rules.map((rule) => (
        <section key={rule.id} className="panel rule-card">
          <div>
            <p className="rule-id">{rule.id}</p>
            <h2>{rule.name}</h2>
            <p className="muted">{rule.tactic} · {rule.technique}</p>
          </div>

          <SeverityBadge severity={rule.severity} />

          <div className="tags">
            {rule.tags.map((tag) => (
              <span key={tag}>{tag}</span>
            ))}
          </div>
        </section>
      ))}
    </div>
  );
}

function InvestigationPage({ alerts }: { alerts: AlertItem[] }) {
  return (
    <div className="single-column">
      <section className="panel hero-panel">
        <PanelTitle icon={BookOpen} title="Investigation Workflow" />
        <p>
          PurpleTrace converts detection alerts into analyst-style cases with MITRE mapping,
          process evidence, matched fields, source event context, recommended next steps,
          and a final decision section.
        </p>
      </section>

      {alerts.slice(0, 3).map((alert, index) => (
        <section key={alert.alertId} className="panel case-card">
          <p className="case-id">PT-CASE-{String(index + 1).padStart(3, "0")}</p>
          <h2>{alert.ruleName}</h2>
          <div className="case-grid">
            <span>Severity</span>
            <strong>{alert.severity}</strong>
            <span>MITRE</span>
            <strong>{alert.techniqueId} {alert.techniqueName}</strong>
            <span>Process</span>
            <strong>{alert.processName}</strong>
            <span>Parent</span>
            <strong>{alert.parentProcess}</strong>
          </div>
          <div className="evidence">
            <strong>Analyst Review</strong>
            <p>{alert.evidence}</p>
          </div>
        </section>
      ))}
    </div>
  );
}

function ReportsPage() {
  const reports = [
    "JSON alert export",
    "Markdown report export",
    "CSV alert export",
    "HTML report export",
    "JSON run summary export",
    "Markdown investigation report export",
  ];

  return (
    <div className="page-grid">
      {reports.map((report) => (
        <section key={report} className="panel report-card">
          <FileText size={26} />
          <h2>{report}</h2>
          <p>Generated locally by PurpleTrace Agent for safe defensive review.</p>
        </section>
      ))}
    </div>
  );
}

function AboutPage() {
  return (
    <div className="single-column">
      <section className="panel hero-panel">
        <PanelTitle icon={Terminal} title="PurpleTrace Lab" />
        <p>
          PurpleTrace Lab is a defensive Windows endpoint telemetry and detection engineering
          project for safe Purple Team learning. It includes synthetic telemetry simulation,
          JSON detection rules, multi-format reporting, a local dashboard, and investigation
          workflow support.
        </p>
      </section>

      <section className="panel">
        <PanelTitle icon={Shield} title="Safety Scope" />
        <div className="safety-list">
          <span>No malware</span>
          <span>No exploit code</span>
          <span>No credential theft</span>
          <span>No persistence creation</span>
          <span>No evasion implementation</span>
          <span>No destructive behavior</span>
        </div>
      </section>
    </div>
  );
}

function MetricCard(props: {
  icon: React.ElementType;
  label: string;
  value: number;
  danger?: boolean;
}) {
  const Icon = props.icon;

  return (
    <section className={props.danger ? "metric danger" : "metric"}>
      <div>
        <span>{props.label}</span>
        <strong>{props.value}</strong>
      </div>
      <Icon size={28} />
    </section>
  );
}

function AlertCard({ alert, compact = false }: { alert: AlertItem; compact?: boolean }) {
  return (
    <article className={compact ? "alert-card compact" : "alert-card"}>
      <div className="alert-head">
        <div>
          <p>{alert.ruleId}</p>
          <h2>{alert.ruleName}</h2>
        </div>
        <SeverityBadge severity={alert.severity} />
      </div>

      <div className="alert-meta">
        <span>{alert.techniqueId} {alert.techniqueName}</span>
        <span>Host: {alert.host}</span>
        <span>User: {alert.user}</span>
      </div>

      {!compact && (
        <>
          <label>Command Line</label>
          <code>{alert.commandLine}</code>

          <div className="evidence">
            <strong>Evidence</strong>
            <p>{alert.evidence}</p>
          </div>
        </>
      )}

      <div className="tags">
        {alert.tags.map((tag) => (
          <span key={tag}>{tag}</span>
        ))}
      </div>
    </article>
  );
}

function PanelTitle(props: { icon: React.ElementType; title: string }) {
  const Icon = props.icon;

  return (
    <div className="panel-title">
      <Icon size={20} />
      <h2>{props.title}</h2>
    </div>
  );
}

function SeverityBadge({ severity }: { severity: Severity }) {
  return <span className={`severity ${severity.toLowerCase()}`}>{severity}</span>;
}

function toChartData<T>(items: T[], selector: (item: T) => string) {
  return Object.entries(
    items.reduce<Record<string, number>>((result, item) => {
      const key = selector(item);
      result[key] = (result[key] ?? 0) + 1;
      return result;
    }, {})
  )
    .map(([name, value]) => ({ name, value }))
    .sort((a, b) => {
      const severityDiff = (severityOrder[b.name] ?? 0) - (severityOrder[a.name] ?? 0);
      return severityDiff || b.value - a.value;
    });
}

function getPageTitle(page: string) {
  const map: Record<string, string> = {
    overview: "Security Overview",
    alerts: "Alert Review",
    rules: "Detection Rules",
    investigation: "Investigation Workspace",
    reports: "Reports",
    about: "About PurpleTrace",
  };

  return map[page] ?? "PurpleTrace Console";
}

export default App;