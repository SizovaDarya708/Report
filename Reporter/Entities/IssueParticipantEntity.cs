using Atlassian.Jira;

namespace Reporter.Entities;

public class IssueParticipantEntity
{
    public IssueParticipantEntity() { }

    public IssueParticipantEntity(JiraUser user)
    {
        Name = user.DisplayName;
    }

    public string Name { get; set; }

    public EmployeeType? EmployeeType { get; set; }
}
