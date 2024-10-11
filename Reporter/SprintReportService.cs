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

        //Заполнение данных об Отделах сотрудников (не приходит по API)
        var userLogins = reportEntity.IssueParticipants
            .Select(x => x.UserLogin)
            .Distinct()
            .ToArray();
        var departments = await _jiraService.GetUsersDepartmentAsync(userLogins, cancellationToken);
        reportEntity.SetParticipantDepartment(departments);

        //Получение и заполнение данных о списаниях времени (не приходит по API)
        var jiraIdentifiers = issues.Select(x => x.JiraIdentifier).ToArray();
        var estimateDto = await _jiraService.GetEstimateDataPerIssuesAsync(jiraIdentifiers, cancellationToken);
        reportEntity.SetEstimateTimeData(estimateDto);

        _excelReportGenerator.GenerateReport(sprintReportDataInput.FilePath, reportEntity);
    }
}
