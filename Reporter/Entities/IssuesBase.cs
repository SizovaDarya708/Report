using Atlassian.Jira;
using JiraInteraction.Dtos;

namespace Reporter.Entities;

public abstract class IssuesBase
{
    public virtual async Task AddIssue(Issue issue)
    {
        var newIssue = new IssueEntity(issue);

        var workLogs = await issue.GetWorklogsAsync();
        newIssue.SetWorkflows(workLogs);

        var changeLogs = await issue.GetChangeLogsAsync();
        newIssue.SetChangeLogs(changeLogs);

        newIssue.SetParticipants();

        AddIssue(newIssue);
    }

    public virtual void AddIssue(IssueEntity issue)
    {
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
