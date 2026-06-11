using Atlassian.Jira;

namespace Reporter.Entities;

public record IssueParticipantEntity
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


public class IssueParticipantEntityComparer : IEqualityComparer<IssueParticipantEntity>
{
    public bool Equals(IssueParticipantEntity x, IssueParticipantEntity y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null || y is null) return false;
        return string.Equals(x.Name, y.Name, StringComparison.OrdinalIgnoreCase);
    }

    public int GetHashCode(IssueParticipantEntity obj)
    {
        if (obj is null) return 0;
        // Комбинируем хэш-коды значимых полей
        unchecked
        {
            int hash = 17;
            hash = hash * 31 + (obj.Name != null ? obj.Name.GetHashCode() : 0);
            return hash;
        }
    }
}
