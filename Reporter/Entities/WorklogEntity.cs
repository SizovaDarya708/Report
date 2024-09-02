using Atlassian.Jira;

namespace Reporter.Entities;

public class WorklogEntity
{
    public WorklogEntity() { }

    public WorklogEntity(Worklog worklog)
    {
        Participant = new IssueParticipantEntity(worklog.AuthorUser);
        //Status = worklog.WorklogStatus.Name;
        Comment = worklog.Comment;
    
    }
    public string? Status { get; set; }

    public IssueParticipantEntity Participant { get; set; }

    /// <summary>
    /// Комментарий к переводу задачи 
    /// </summary>
    public string? Comment { get; set; }
}
