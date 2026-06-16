using Atlassian.Jira;
using JiraInteraction.Dtos;

namespace Reporter.Entities;

public abstract class IssuesBase
{
    public virtual async Task AddIssue(Issue issue, ReportPeriodDto reportPeriod)
    {
        var newIssue = new IssueEntity(issue);

        var workLogs = await issue.GetWorklogsAsync();
        newIssue.SetWorkflows(workLogs);

        var changeLogs = await issue.GetChangeLogsAsync();
        newIssue.SetChangeLogs(changeLogs);

        newIssue.SetParticipants();

        AddIssue(newIssue, reportPeriod);
    }

    private void AddIssue(IssueEntity issue, ReportPeriodDto reportPeriod)
    {
        if (!issue.IsExistChangesInReportPeriod(reportPeriod))
        {
            return;
        }

        Issues.Add(issue);
    }

    public List<IssueEntity> Issues { get; set; } = new List<IssueEntity>();

    public void SetEstimateData(Dictionary<string, List<EstimateByWorklogTypeDto>> estimateData)
    {
        foreach (var issue in Issues)
        {
            estimateData.TryGetValue(issue.Key, out var estimates);

            if (estimates == null)
            {
                continue;
            }

            issue.SetEstimateData(estimates);
        }
    }
}
