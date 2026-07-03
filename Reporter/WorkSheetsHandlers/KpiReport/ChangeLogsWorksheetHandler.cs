using OfficeOpenXml;
using Reporter.Entities;
using Reporter.Extensions;

namespace Reporter.WorkSheetsHandlers.SprintReport;

public class ChangeLogsWorksheetHandler : WorksheetExportHandlerBase
{
    private SprintReportEntity _sprintReportEntity;

    public ChangeLogsWorksheetHandler(ExcelPackage excelPackage, string listName, SprintReportEntity sprintReportEntity)
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
    private int issueNameColumn = 2;
    private int authorNameColumn = 3;
    private int fromStatusColumn = 4;
    private int toStatusColumn = 5;
    private int CreatedDateColumn = 6;

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

        foreach (var issue in allIssues)
        {
            FillChangeLogs(issue);
        }
    }

    private void FillChangeLogs(IssueEntity issue)
    {
        var statusChangedChangelogs = issue.ChangeLogs
                .Where(x => x.Items.Any(
                    ch =>
                    ch.FieldName.ToLower() == JiraConstants.Status.ToLower()));

        foreach (var log in statusChangedChangelogs)
        {
            var workflows = log.Items;
            foreach (var workflow in workflows)
            {
                if (workflow.FromValue == null || workflow.FromValue == string.Empty ||
                    workflow.ToValue == null || workflow.ToValue == string.Empty)
                {
                    continue;                
                }

                CurrentWorksheet.SetValue(currentRow, projectKeyColumn, issue.ProjectKey);
                CurrentWorksheet.SetValue(currentRow, issueNameColumn, issue.Key);
                CurrentWorksheet.SetValue(currentRow, authorNameColumn, log.Author?.Name ?? "-");
                CurrentWorksheet.SetValue(currentRow, fromStatusColumn, workflow.FromValue);
                CurrentWorksheet.SetValue(currentRow, toStatusColumn, workflow.ToValue);
                CurrentWorksheet.Cells[currentRow, CreatedDateColumn].SetDateTime(log.CreateDate);
                CurrentWorksheet.SetValue(currentRow, CreatedDateColumn, log.CreateDate);
                currentRow++;
            }
        }
    }

    private void FillHeaders()
    {
        //Заголовки данных
        CurrentWorksheet.SetValue(headerRow, projectKeyColumn, "Проект");
        CurrentWorksheet.SetValue(headerRow, authorNameColumn, "Сотрудник");
        CurrentWorksheet.SetValue(headerRow, issueNameColumn, "Задача");
        CurrentWorksheet.SetValue(headerRow, fromStatusColumn, "Из статуса");
        CurrentWorksheet.SetValue(headerRow, toStatusColumn, "В статус");
        CurrentWorksheet.SetValue(headerRow, CreatedDateColumn, "Время перехода");
    }
}
