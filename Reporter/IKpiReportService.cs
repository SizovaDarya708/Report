using JiraInteraction.Dtos;

namespace Reporter;

public interface IKpiReportService
{
    Task ExecuteAsync(
       KpiReportInput input,
       CancellationToken cancellationToken = default);
}
