using OfficeOpenXml;
using Reporter.Entities;
using Reporter.Extensions;

namespace Reporter.WorkSheetsHandlers.SprintReport;

public class Kpi1WorksheetHandler : WorksheetExportHandlerBase
{
    private SprintReportEntity _sprintReportEntity;

    public Kpi1WorksheetHandler(ExcelPackage excelPackage, string listName, SprintReportEntity sprintReportEntity)
    {
        ListName = listName;
        CurrentExcelPackage = excelPackage;
        _sprintReportEntity = sprintReportEntity;
        SetWorksheet();
    }

    #region Колонки
    private static int headerRow = 1;
    private int currentRow = 2;

    private int authorNameColumn = 2;
    private int issueCountColumn = 3;
    private int issuesWithReworkCountColumn = 4;
    private int issuesWithReworkPercentageColumn = 5;
    private int periodStartDateColumn = 6;
    private int periodEndDateColumn = 7;

    #endregion

    public void FillReportData()
    {
        FillHeaders();
        FillDataWithoutSprint();
        FillFormat();
    }

    private void FillCommonForSprintAndWithoutSprintIssues(IssueEntity issue)
    {
        CurrentWorksheet.SetValue(currentRow, authorNameColumn, issue.Title);
        CurrentWorksheet.SetValue(currentRow, issueCountColumn, issue.Key);

        CurrentWorksheet.SetValue(currentRow, issuesWithReworkCountColumn, issue.Status);
        CurrentWorksheet.SetValue(currentRow, issuesWithReworkPercentageColumn, issue.Type);
        CurrentWorksheet.Cells[currentRow, periodStartDateColumn].SetDateTime(issue.CreateDate);
        CurrentWorksheet.SetValue(currentRow, periodStartDateColumn, issue.ErrorReason);
        CurrentWorksheet.Cells[currentRow, periodEndDateColumn].SetDateTime(issue.CreateDate);
        CurrentWorksheet.SetValue(currentRow, periodEndDateColumn, issue.ErrorReason);

    }

    private void FillDataWithoutSprint()
    {
        var errors = _sprintReportEntity.WithoutSprintPool.Issues.Where(iss => iss.Type == JiraConstants.Error || iss.Type == JiraConstants.Incident);
        foreach (var issue in errors)
        {
            FillCommonForSprintAndWithoutSprintIssues(issue);
            currentRow++;
        }
    }

    private void FillHeaders()
    {
        //Заголовки данных
        CurrentWorksheet.SetValue(headerRow, authorNameColumn, "Сотрудник");
        CurrentWorksheet.SetValue(headerRow, issueCountColumn, "Количество задач");
        CurrentWorksheet.SetValue(headerRow, issuesWithReworkCountColumn, "Количество задач с доработками");
        CurrentWorksheet.SetValue(headerRow, issuesWithReworkPercentageColumn, "Процент задач с доработками");

        CurrentWorksheet.SetValue(headerRow, periodStartDateColumn, "Начало периода");
        CurrentWorksheet.SetValue(headerRow, periodEndDateColumn, "Конец периода");
    }
}
