using OfficeOpenXml;
using Reporter.Entities;
using Reporter.Extensions;

namespace Reporter.WorkSheetsHandlers.SprintReport;

public class ErrorWorksheetHandler : WorksheetExportHandlerBase
{
    private SprintReportEntity _sprintReportEntity;

    public ErrorWorksheetHandler(ExcelPackage excelPackage, string listName, SprintReportEntity sprintReportEntity)
    {
        ListName = listName;
        CurrentExcelPackage = excelPackage;
        _sprintReportEntity = sprintReportEntity;
        SetWorksheet();
    }

    #region Колонки
    private static int headerRow = 1;
    private int currentRow = 2;

    private int issueNameColumn = 2;
    private int issueKeyColumn = 3;
    private int statusColumn = 4;
    private int typeColumn = 5;
    private int createDateColumn = 6;
    private int reasonColumn = 7;
    private int errorTypeColumn = 8;

    #endregion

    public void FillReportData()
    {
        FillHeaders();
        FillDataWithoutSprint();
        FillFormat();
    }

    private void FillCommonForSprintAndWithoutSprintIssues(IssueEntity issue)
    {
        CurrentWorksheet.SetValue(currentRow, issueNameColumn, issue.Title);
        CurrentWorksheet.SetValue(currentRow, issueKeyColumn, issue.Key);

        //Применить гиперссылку для ключа задачи
        CurrentWorksheet.Cells[currentRow, issueKeyColumn].SetHyperlink(new Uri($"https://jira.bars.group/browse/{issue.Key}"));
        CurrentWorksheet.SetValue(currentRow, statusColumn, issue.Status);
        CurrentWorksheet.SetValue(currentRow, typeColumn, issue.Type);
        CurrentWorksheet.Cells[currentRow, createDateColumn].SetDateTime(issue.CreateDate);
        CurrentWorksheet.SetValue(currentRow, reasonColumn, issue.ErrorReason);
        CurrentWorksheet.SetValue(currentRow, errorTypeColumn, issue.ErrorType);

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
        CurrentWorksheet.SetValue(headerRow, issueNameColumn, "Наименование задачи");
        CurrentWorksheet.SetValue(headerRow, issueKeyColumn, "Ключ задачи");
        CurrentWorksheet.SetValue(headerRow, statusColumn, "Статус");
        CurrentWorksheet.SetValue(headerRow, typeColumn, "Тип задачи");

        CurrentWorksheet.SetValue(headerRow, createDateColumn, "Дата создания");
        CurrentWorksheet.SetValue(headerRow, reasonColumn, "Причина ошибки");
        CurrentWorksheet.SetValue(headerRow, errorTypeColumn, "Тип ошибки");
    }
}
