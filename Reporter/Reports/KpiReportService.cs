using JiraInteraction;
using JiraInteraction.Dtos;
using Reporter.Entities;
using Reporter.ReportGenerators;
namespace Reporter.Reports;

public class KpiReportService : IKpiReportService
{
    private IJiraService _jiraService;
    private ExcelKpiReportGenerator _excelReportGenerator;

    public KpiReportService(IJiraService jiraService)
    {
        _jiraService = jiraService;
        _excelReportGenerator = new ExcelKpiReportGenerator();
    }

    public async Task ExecuteAsync(
        KpiReportInput input,
        CancellationToken cancellationToken)
    {
        var issues = await _jiraService.GetIssuesForReportAsync(input, cancellationToken);

        var reportEntity = new SprintReportEntity(input.StartDate, input.EndDate);

        await reportEntity.FillDataAsync(issues);

        //Получение и заполнение данных о списаниях времени (не приходит по API)        
        var jiraKeys = reportEntity.GetAllIssueInfoForApiRequest();
        var estimateDto = await _jiraService.GetEstimateDataPerIssuesAsync(jiraKeys, cancellationToken);
        reportEntity.SetEstimateTimeData(estimateDto);

        _excelReportGenerator.GenerateReport(input.FilePath, reportEntity);
    }
}
