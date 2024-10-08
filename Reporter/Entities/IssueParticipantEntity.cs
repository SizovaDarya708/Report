using Atlassian.Jira;

namespace Reporter.Entities;

public class IssueParticipantEntity
{
    public IssueParticipantEntity() { }

    public IssueParticipantEntity(JiraUser user)
    {
        Name = user.DisplayName;
        UserLogin = user.Username;
    }

    public string Name { get; set; }

    public string UserLogin {  get; set; }

    public EmployeeType? EmployeeType { get; set; }
}
