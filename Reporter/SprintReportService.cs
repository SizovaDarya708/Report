using JiraInteraction;
using JiraInteraction.Dtos;
using Reporter.Entities;

namespace Reporter;

public class SprintReportService : ISprintReportService
{
    private IJiraService _jiraService;
    private ExcelReportGenerator _excelReportGenerator;

    public SprintReportService(IJiraService jiraService)
    {
        _jiraService = jiraService;
        _excelReportGenerator = new ExcelReportGenerator();
    }

    public async Task ExecuteAsync(
        SprintIssuesDataInput sprintReportDataInput,
        CancellationToken cancellationToken)
    {
        var issues = await _jiraService.GetIssuesForReportAsync(sprintReportDataInput, cancellationToken);

        var reportEntity = new SprintReportEntity(sprintReportDataInput.StartDate, sprintReportDataInput.EndDate);

        await reportEntity.FillDataAsync(issues);

        var userLogins = reportEntity.IssueParticipants
            .Select(x => x.UserLogin)
            .Distinct()
            .ToArray();
        var departments = await _jiraService.GetUsersDataAsync(userLogins, cancellationToken);
        reportEntity.SetParticipantDepartment(departments);

        _excelReportGenerator.GenerateReport(sprintReportDataInput.FilePath, reportEntity);
    }
}
