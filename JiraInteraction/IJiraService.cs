using Atlassian.Jira;
using JiraInteraction.Dtos;

namespace JiraInteraction;

public interface IJiraService
{
    Task<Issue[]> GetIssuesForReportAsync(BaseReportInput input, CancellationToken cancellationToken = default);

    Task<bool> CheckClientConnection(CancellationToken cancellationToken = default);

    Task<Dictionary<string, string>> GetUsersDepartmentAsync(string[] userLogins, CancellationToken cancellationToken);

    Task<Dictionary<string, List<EstimateByWorklogTypeDto>>> GetEstimateDataPerIssuesAsync(IssueApiRequestDto[] jiraKeys, CancellationToken cancellationToken);
}
