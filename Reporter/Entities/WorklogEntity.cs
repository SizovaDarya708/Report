using Atlassian.Jira;

namespace Reporter.Entities;

public class WorklogEntity
{
    public WorklogEntity() { }

    public WorklogEntity(Worklog worklog)
    {
        Participant = new IssueParticipantEntity(worklog.AuthorUser);
        Comment = worklog.Comment;
        TimeSpendInSeconds = worklog.TimeSpentInSeconds;
        UpdateDate = worklog.UpdateDate;
    }

    public string? Status { get; set; }

    public IssueParticipantEntity Participant { get; set; }

    public long? TimeSpendInSeconds { get; set; }

    public DateTime? UpdateDate { get; set; }

    /// <summary>
    /// Комментарий к переводу задачи 
    /// </summary>
    public string? Comment { get; set; }
}
