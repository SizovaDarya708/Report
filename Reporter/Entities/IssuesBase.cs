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
        if (!issue.IsExistCreatedWorkLogsInReportPeriod(reportPeriod))
        {
            return;
        }

        Issues.Add(issue);
    }

    public List<IssueEntity> Issues { get; set; } = new List<IssueEntity>();

    public void SetEstimateData(Dictionary<string, EstimateTime> estimateData)
    {
        foreach (var issue in Issues)
        {
            if (!estimateData.TryGetValue(issue.JiraIdentifier, out var estimates))
            {
                continue;
            }

            issue.TimeRemainingInSeconds = estimates.RemainingTimeInSeconds;
            issue.TimeEstimateInSeconds = estimates.EstimateTimeInSeconds;
        }
    }   
}
