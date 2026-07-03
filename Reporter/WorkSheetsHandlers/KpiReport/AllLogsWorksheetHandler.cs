using OfficeOpenXml;
using Reporter.Entities;

namespace Reporter.WorkSheetsHandlers.SprintReport;

public class AllLogsWorksheetHandler : WorksheetExportHandlerBase
{
    private SprintReportEntity _sprintReportEntity;

    public AllLogsWorksheetHandler(ExcelPackage excelPackage, string listName, SprintReportEntity sprintReportEntity)
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
    private int estimateTypeColumn = 4;
    private int estimateTimeColumn = 5;

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
            FillEstimates(issue);
        }
    }

    private void FillEstimates(IssueEntity issue)
    {
        var allEstimates = issue.Estimates;

        foreach (var est in allEstimates)
        {
            CurrentWorksheet.SetValue(currentRow, projectKeyColumn, issue.ProjectKey);
            CurrentWorksheet.SetValue(currentRow, issueNameColumn, issue.Key);
            CurrentWorksheet.SetValue(currentRow, authorNameColumn, est.Worklog.Participant.Name);
            CurrentWorksheet.SetValue(currentRow, estimateTypeColumn, WorkEstimateExtensions.GetWorkEstimateName(est.WorkEstimateType));
            var estimateTime = (est.Worklog.TimeSpendInSeconds != null && est.Worklog.TimeSpendInSeconds != 0) ?
                (decimal)est.Worklog.TimeSpendInSeconds / 60 / 60 : 0;
            CurrentWorksheet.SetValue(currentRow, estimateTimeColumn, estimateTime);
            currentRow++;
        }
    }

    private void FillHeaders()
    {
        //Заголовки данных
        CurrentWorksheet.SetValue(headerRow, projectKeyColumn, "Проект");
        CurrentWorksheet.SetValue(headerRow, authorNameColumn, "Сотрудник");
        CurrentWorksheet.SetValue(headerRow, issueNameColumn, "Задача");
        CurrentWorksheet.SetValue(headerRow, estimateTimeColumn, "Списанное время в часах");
        CurrentWorksheet.SetValue(headerRow, estimateTypeColumn, "Тип списания");
    }
}
