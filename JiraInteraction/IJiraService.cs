using Atlassian.Jira;
using JiraInteraction.Dtos;

namespace JiraInteraction;

public interface IJiraService
{
    Task<Issue[]> GetIssuesForReportAsync(SprintIssuesDataInput input, CancellationToken cancellationToken = default);

    Task<bool> CheckClientConnection(CancellationToken cancellationToken = default);

    Task<Dictionary<string, string>> GetUsersDataAsync(string[] userLogins, CancellationToken cancellationToken);
}
