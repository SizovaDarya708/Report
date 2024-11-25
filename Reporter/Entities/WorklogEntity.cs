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
        IssueStatus = issue.Status;
        IssueName = issue.Title;
       // SprintStartDate = issue.SprintStartDate;
       // SprintEndDate = issue.SprintEndDate;
    }

    public string IssueKey { get; set; }

    public string? IssueStatus { get; set; }

    public string? IssueName { get; set; }

    public DateTime? SprintStartDate { get; set; }

    public DateTime? SprintEndDate { get; set; }

    public IssueParticipantEntity Participant { get; set; }

    public long? TimeSpendInSeconds { get; set; }

    public DateTime? UpdateDate { get; set; }

    /// <summary>
    /// Комментарий к переводу задачи 
    /// </summary>
    public string? Comment { get; set; }
}
