using Atlassian.Jira;

namespace Reporter.Entities;

public class SprintEntity
{
    public SprintEntity() { }

    public SprintEntity(string sprintName, Issue? issue = null)
    { 
        Name = sprintName;
        //тут какой-нибудь TryParseDateStartEndByName
        //По DateStart and DateEnd calc IsCurrent
        
    }
    public bool IsCurrent { get; set; } = false;

    public string Name { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public List<IssueEntity> Issues { get; set; } = new List<IssueEntity>();

    public void AddIssue(IssueEntity issue)
    { 
        Issues.Add(issue);
    }

    public async Task AddIssue(Issue issue)
    {
        var newIssue = new IssueEntity(issue);
        var workLogs = await issue.GetWorklogsAsync();
        newIssue.SetWorkflows(workLogs.ToArray());
        AddIssue(newIssue);
    }
}
