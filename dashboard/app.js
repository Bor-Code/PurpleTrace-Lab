const state = {
  alerts: [],
  filteredAlerts: []
};

const elements = {
  alertFile: document.getElementById("alertFile"),
  searchBox: document.getElementById("searchBox"),
  totalAlerts: document.getElementById("totalAlerts"),
  highAlerts: document.getElementById("highAlerts"),
  mitreCount: document.getElementById("mitreCount"),
  ruleCount: document.getElementById("ruleCount"),
  severityChart: document.getElementById("severityChart"),
  mitreChart: document.getElementById("mitreChart"),
  ruleChart: document.getElementById("ruleChart"),
  alertList: document.getElementById("alertList"),
  barTemplate: document.getElementById("barTemplate"),
  alertTemplate: document.getElementById("alertTemplate")
};

elements.alertFile.addEventListener("change", handleFileChange);
elements.searchBox.addEventListener("input", handleSearch);

function handleFileChange(event) {
  const file = event.target.files[0];

  if (!file) {
    return;
  }

  const reader = new FileReader();

  reader.onload = () => {
    try {
      const parsed = JSON.parse(reader.result);
      const alerts = normalizeAlerts(parsed);

      state.alerts = alerts;
      state.filteredAlerts = alerts;

      elements.searchBox.value = "";

      renderDashboard();
    } catch (error) {
      showError(`Could not parse alert JSON file. ${error.message}`);
    }
  };

  reader.readAsText(file);
}

function normalizeAlerts(parsed) {
  if (Array.isArray(parsed)) {
    return parsed;
  }

  if (parsed && Array.isArray(parsed.Alerts)) {
    return parsed.Alerts;
  }

  if (parsed && Array.isArray(parsed.alerts)) {
    return parsed.alerts;
  }

  throw new Error("Expected a JSON array of alerts.");
}

function handleSearch(event) {
  const query = event.target.value.trim().toLowerCase();

  if (!query) {
    state.filteredAlerts = state.alerts;
    renderDashboard();
    return;
  }

  state.filteredAlerts = state.alerts.filter(alert => {
    const searchable = [
      alert.RuleId,
      alert.RuleName,
      alert.Severity,
      alert.MitreTactic,
      alert.MitreTechniqueId,
      alert.MitreTechniqueName,
      alert.Hostname,
      alert.UserName,
      alert.ProcessName,
      alert.CommandLine,
      alert.ParentProcessName,
      alert.EvidenceSummary,
      Array.isArray(alert.RuleTags) ? alert.RuleTags.join(" ") : ""
    ]
      .join(" ")
      .toLowerCase();

    return searchable.includes(query);
  });

  renderDashboard();
}

function renderDashboard() {
  renderMetrics();
  renderBarChart(elements.severityChart, countBy(state.filteredAlerts, alert => alert.Severity));
  renderBarChart(elements.mitreChart, countBy(state.filteredAlerts, alert => formatMitre(alert)));
  renderBarChart(elements.ruleChart, countBy(state.filteredAlerts, alert => formatRule(alert)));
  renderAlerts();
}

function renderMetrics() {
  const alerts = state.filteredAlerts;

  elements.totalAlerts.textContent = alerts.length;
  elements.highAlerts.textContent = alerts.filter(alert => isHigh(alert.Severity)).length;
  elements.mitreCount.textContent = uniqueCount(alerts, alert => alert.MitreTechniqueId);
  elements.ruleCount.textContent = uniqueCount(alerts, alert => alert.RuleId);
}

function renderBarChart(container, counts) {
  container.innerHTML = "";

  const entries = Object.entries(counts)
    .filter(([name]) => name)
    .sort((a, b) => b[1] - a[1]);

  if (entries.length === 0) {
    container.className = "bar-list empty";
    container.textContent = "No data available.";
    return;
  }

  container.className = "bar-list";

  const max = Math.max(...entries.map(([, count]) => count));

  for (const [name, count] of entries) {
    const node = elements.barTemplate.content.cloneNode(true);
    const width = Math.max(6, Math.round((count / max) * 100));

    node.querySelector(".bar-name").textContent = name;
    node.querySelector(".bar-count").textContent = count;
    node.querySelector(".bar-fill").style.width = `${width}%`;

    container.appendChild(node);
  }
}

function renderAlerts() {
  elements.alertList.innerHTML = "";

  if (state.filteredAlerts.length === 0) {
    elements.alertList.className = "alert-list empty";
    elements.alertList.textContent = state.alerts.length === 0
      ? "Load a PurpleTrace alert JSON file to display alerts."
      : "No alerts matched the current search.";
    return;
  }

  elements.alertList.className = "alert-list";

  const sortedAlerts = [...state.filteredAlerts].sort((a, b) => {
    return severityRank(b.Severity) - severityRank(a.Severity);
  });

  for (const alert of sortedAlerts) {
    const node = elements.alertTemplate.content.cloneNode(true);

    node.querySelector(".alert-rule").textContent = alert.RuleId || "Unknown Rule";
    node.querySelector(".alert-title").textContent = alert.RuleName || "Untitled Alert";

    const severityBadge = node.querySelector(".severity-badge");
    severityBadge.textContent = alert.Severity || "Unknown";
    severityBadge.classList.add(getSeverityClass(alert.Severity));

    node.querySelector(".mitre").textContent = formatMitre(alert) || "MITRE: N/A";
    node.querySelector(".host").textContent = `Host: ${alert.Hostname || "N/A"}`;
    node.querySelector(".user").textContent = `User: ${alert.UserName || "N/A"}`;
    node.querySelector(".command-line").textContent = alert.CommandLine || "N/A";
    node.querySelector(".evidence-summary").textContent = alert.EvidenceSummary || alert.Reason || "No evidence summary available.";

    const tagList = node.querySelector(".tag-list");
    tagList.innerHTML = "";

    const tags = Array.isArray(alert.RuleTags) ? alert.RuleTags : [];

    for (const tag of tags) {
      const span = document.createElement("span");
      span.textContent = tag;
      tagList.appendChild(span);
    }

    elements.alertList.appendChild(node);
  }
}

function countBy(items, selector) {
  return items.reduce((result, item) => {
    const key = selector(item) || "Unknown";
    result[key] = (result[key] || 0) + 1;
    return result;
  }, {});
}

function uniqueCount(items, selector) {
  return new Set(
    items
      .map(selector)
      .filter(value => value && value.trim())
  ).size;
}

function formatMitre(alert) {
  const id = alert.MitreTechniqueId || "";
  const name = alert.MitreTechniqueName || "";

  return `${id} ${name}`.trim();
}

function formatRule(alert) {
  const id = alert.RuleId || "";
  const name = alert.RuleName || "";

  return `${id} ${name}`.trim();
}

function isHigh(severity) {
  return ["high", "critical"].includes(String(severity || "").toLowerCase());
}

function severityRank(severity) {
  const normalized = String(severity || "").toLowerCase();

  if (normalized === "critical") {
    return 5;
  }

  if (normalized === "high") {
    return 4;
  }

  if (normalized === "medium") {
    return 3;
  }

  if (normalized === "low") {
    return 2;
  }

  if (normalized === "informational") {
    return 1;
  }

  return 0;
}

function getSeverityClass(severity) {
  const normalized = String(severity || "").toLowerCase();

  if (normalized === "critical") {
    return "severity-critical";
  }

  if (normalized === "high") {
    return "severity-high";
  }

  if (normalized === "medium") {
    return "severity-medium";
  }

  if (normalized === "low") {
    return "severity-low";
  }

  if (normalized === "informational") {
    return "severity-informational";
  }

  return "";
}

function showError(message) {
  state.alerts = [];
  state.filteredAlerts = [];

  elements.totalAlerts.textContent = "0";
  elements.highAlerts.textContent = "0";
  elements.mitreCount.textContent = "0";
  elements.ruleCount.textContent = "0";

  elements.severityChart.className = "bar-list empty";
  elements.severityChart.textContent = "No data available.";

  elements.mitreChart.className = "bar-list empty";
  elements.mitreChart.textContent = "No data available.";

  elements.ruleChart.className = "bar-list empty";
  elements.ruleChart.textContent = "No data available.";

  elements.alertList.className = "alert-list empty";
  elements.alertList.textContent = message;
}