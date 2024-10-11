using Atlassian.Jira;

namespace Reporter.Entities;

public class IssueParticipantEntity
{
    public IssueParticipantEntity() { }

    public IssueParticipantEntity(JiraUser user)
    {
        Name = user.DisplayName;
        UserLogin = user.Username;
        IsActual = user.IsActive;
    }

    public string Name { get; set; }

    public string UserLogin {  get; set; }

    public EmployeeType? EmployeeType { get; set; }

    public string Department { get; set; } = string.Empty;

    public bool IsActual { get; set; } = true;
}
