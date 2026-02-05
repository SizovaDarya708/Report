// Models/JiraModels.cs (обновленные модели)
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace JiraTimesheetExporter.Models
{
    public class JiraWorklog
    {
        [JsonPropertyName("started")]
        public string Started { get; set; } = string.Empty;

        [JsonPropertyName("timeSpentSeconds")]
        public int TimeSpentSeconds { get; set; }

        [JsonPropertyName("author")]
        public JiraUser Author { get; set; } = new JiraUser();

        [JsonPropertyName("comment")]
        public string? Comment { get; set; }
    }

    public class JiraUser
    {
        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; } = string.Empty;

        [JsonPropertyName("accountId")]
        public string AccountId { get; set; } = string.Empty;
    }

    public class JiraIssue
    {
        [JsonPropertyName("key")]
        public string Key { get; set; } = string.Empty;

        [JsonPropertyName("fields")]
        public IssueFields Fields { get; set; } = new IssueFields();
    }

    public class IssueFields
    {
        [JsonPropertyName("summary")]
        public string Summary { get; set; } = string.Empty;

        [JsonPropertyName("worklog")]
        public WorklogList? Worklog { get; set; }

        [JsonPropertyName("issuetype")]
        public IssueTypeInfo? IssueType { get; set; }
    }

    public class IssueTypeInfo
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }

    public class WorklogList
    {
        [JsonPropertyName("worklogs")]
        public List<JiraWorklog> Worklogs { get; set; } = new List<JiraWorklog>();

        [JsonPropertyName("maxResults")]
        public int MaxResults { get; set; }

        [JsonPropertyName("total")]
        public int Total { get; set; }
    }

    public class JiraSearchResponse
    {
        [JsonPropertyName("issues")]
        public List<JiraIssue> Issues { get; set; } = new List<JiraIssue>();

        [JsonPropertyName("total")]
        public int Total { get; set; }
    }

    public class TimesheetRecord
    {
        public string EmployeeName { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string TimeSpent { get; set; } = string.Empty;
        public int TimeSpentInMinutes { get; set; } = 0;
        public string TaskKey { get; set; } = string.Empty;
        public string TaskSummary { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public string TaskType { get; set; } = string.Empty;
    }
}