using OfficeOpenXml;
using Reporter.Entities;
using Reporter.Extensions;

namespace Reporter.WorkSheetsHandlers.SprintReport;

public class Kpi6WorksheetHandler : WorksheetExportHandlerBase
{
    private SprintReportEntity _sprintReportEntity;

    public Kpi6WorksheetHandler(ExcelPackage excelPackage, string listName, SprintReportEntity sprintReportEntity)
    {
        ListName = listName;
        CurrentExcelPackage = excelPackage;
        _sprintReportEntity = sprintReportEntity;
        SetWorksheet();
    }

    #region Колонки
    private static int headerRow = 1;
    private int currentRow = 2;

    private int projectKeyColumn = 1;
    private int allIssueCountColumn = 2;
    private int techCountColumn = 3;
    private int techCountToAllIssuesColumn = 4;
    private int periodStartDateColumn = 5;
    private int periodEndDateColumn = 6;

    #endregion

    public void FillReportData()
    {
        FillHeaders();
        FillData();
        FillFormat();
    }

    private void FillData()
    {
        var allIssues = _sprintReportEntity.GetAllIssues();
        var projectGroupedIssues = allIssues
            .Where(x => x.Status == JiraConstants.Closed)
            .GroupBy(issue => issue.ProjectKey)
            .ToDictionary(k => k.Key, v => v.Select(i => i).ToList());

        foreach (var projectIssues in projectGroupedIssues)
        {
            FillByProjects(projectIssues);
        }
    }

    private void FillByProjects(KeyValuePair<string, List<IssueEntity>> projectIssues)
    {
        var projectKey = projectIssues.Key;

        CurrentWorksheet.SetValue(currentRow, projectKeyColumn, projectIssues.Key);
        var allIssueCount = projectIssues.Value.Count;
        CurrentWorksheet.SetValue(currentRow, allIssueCountColumn, allIssueCount);

        var techIssuesCount = projectIssues.Value.Where(i => i.Labels.Any(l => l == JiraConstants.TechDutyLabel)).Count();
        CurrentWorksheet.SetValue(currentRow, techCountColumn, techIssuesCount);
        CurrentWorksheet.Cells[currentRow, techCountToAllIssuesColumn].Style.Numberformat.Format = "0.##";
        CurrentWorksheet.SetValue(currentRow, techCountToAllIssuesColumn, techIssuesCount/(decimal)allIssueCount);

        CurrentWorksheet.Cells[currentRow, periodStartDateColumn].SetDateTime(_sprintReportEntity.ReportPeriod.StartDate);
        CurrentWorksheet.SetValue(currentRow, periodStartDateColumn, _sprintReportEntity.ReportPeriod.StartDate);
        CurrentWorksheet.Cells[currentRow, periodEndDateColumn].SetDateTime(_sprintReportEntity.ReportPeriod.EndDate);
        CurrentWorksheet.SetValue(currentRow, periodEndDateColumn, _sprintReportEntity.ReportPeriod.EndDate);
        currentRow++;

    }

    private void FillHeaders()
    {
        //Заголовки данных
        CurrentWorksheet.SetValue(headerRow, projectKeyColumn, "Проект");
        CurrentWorksheet.SetValue(headerRow, allIssueCountColumn, "Количество всех задач");
        CurrentWorksheet.SetValue(headerRow, techCountColumn, "Количество задач ТехДолг");
        CurrentWorksheet.SetValue(headerRow, techCountToAllIssuesColumn, "Отношение задач техдолга к общему объему задач");
        CurrentWorksheet.SetValue(headerRow, periodStartDateColumn, "Начало периода");
        CurrentWorksheet.SetValue(headerRow, periodEndDateColumn, "Конец периода");
    }
}
