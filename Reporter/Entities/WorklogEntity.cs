using Atlassian.Jira;

namespace Reporter.Entities;

public class WorklogEntity
{
    public WorklogEntity() { }

    public WorklogEntity(IssueEntity issue, Worklog worklog)
    {
        Participant = new IssueParticipantEntity(worklog.AuthorUser);
        Comment = worklog.Comment;
        TimeSpendInSeconds = worklog.TimeSpentInSeconds;
        UpdateDate = worklog.UpdateDate;
        IssueKey = issue.Key;
    }

    public string IssueKey { get; set; }

    public string? Status { get; set; }

    public IssueParticipantEntity Participant { get; set; }

    public long? TimeSpendInSeconds { get; set; }

    public DateTime? UpdateDate { get; set; }

    /// <summary>
    /// Комментарий к переводу задачи 
    /// </summary>
    public string? Comment { get; set; }
}
