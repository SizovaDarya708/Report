using JiraInteraction.Dtos;

namespace Reporter;

public interface ISprintReportService
{
    Task ExecuteAsync(
        SprintIssuesDataInput sprintReportDataInput,
        CancellationToken cancellationToken = default);
}
