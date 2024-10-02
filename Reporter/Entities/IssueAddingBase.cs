using Atlassian.Jira;

namespace Reporter.Entities;

public abstract class IssueAddingBase
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
}
