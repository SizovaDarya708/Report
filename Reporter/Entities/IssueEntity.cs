using Atlassian.Jira;

namespace Reporter.Entities;

public class IssueEntity
{
    public IssueEntity() { }

    public IssueEntity(Issue issue)
    {
        Key = issue.Key.Value;
        Title = issue.Summary;
        Description = issue.Description;
        //AuthorName = ;
        Status = issue.Status.Name;
        Type = issue.Type.Name;
        StoryPoints = Int32.TryParse(issue.GetFieldValue(JiraConstants.StoryPointsField), out var points) ? points : null;
        Priority = issue.Priority.Name;
        TimeSpentInHours = (int)(issue.TimeTrackingData?.TimeSpentInSeconds / 3600 ?? 0);
        LastAssignee = issue.AssigneeUser != null ? new IssueParticipantEntity(issue.AssigneeUser) : null;
    }

    public void SetWorkflows(Worklog[] workflows)
    {
        var newWorklogs = workflows
            .Select(w => new WorklogEntity(w));
        Worklogs.AddRange(newWorklogs);
    }

    public string Key { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? AuthorName { get; set; }

    public string Status { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public IssueParticipantEntity? LastAssignee { get; set; }

    public int TimeSpentInHours { get; set; }

    public int? StoryPoints { get; set; }

    public string? Priority { get; set; }

    //Все участники задачи (автор и исполнители)
    public IssueParticipantEntity[] Participants { get; set; } = Array.Empty<IssueParticipantEntity>();

    /// <summary> Содержатся данные о переходах из статуса в статус </summary>
    public List<WorklogEntity> Worklogs { get; set; } = new List<WorklogEntity>();

}
