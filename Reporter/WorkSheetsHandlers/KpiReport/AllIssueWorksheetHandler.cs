using OfficeOpenXml;
using Reporter.Entities;
using Reporter.Extensions;

namespace Reporter.WorkSheetsHandlers.SprintReport;

public class AllIssueWorksheetHandler : WorksheetExportHandlerBase
{
    private SprintReportEntity _sprintReportEntity;

    public AllIssueWorksheetHandler(ExcelPackage excelPackage, string listName, SprintReportEntity sprintReportEntity)
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
    private int issueTypeColumn = 3;
    private int SpColumn = 4;
    private int PriorityColumn = 5;
    private int ReworkCountColumn = 6;
    private int AllEstimatesColumn = 7;
    private int StatusColumn = 8;
    private int CreatedDateColumn = 9;
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
            CurrentWorksheet.SetValue(currentRow, projectKeyColumn, issue.ProjectKey);
            CurrentWorksheet.SetValue(currentRow, issueNameColumn, issue.Key);
            CurrentWorksheet.SetValue(currentRow, issueTypeColumn, issue.Type);
            CurrentWorksheet.SetValue(currentRow, SpColumn, issue.StoryPoints ?? 0);
            CurrentWorksheet.SetValue(currentRow, PriorityColumn, issue.Priority ?? "-");
            var reworks = issue.GetReworkInfo();
            var reworkCount = reworks.CountOfRework;
            CurrentWorksheet.SetValue(currentRow, ReworkCountColumn, reworkCount);
            var allEstimates = issue.GetAllDeveloperEstimatesInHours();
            CurrentWorksheet.SetValue(currentRow, AllEstimatesColumn, allEstimates);
            CurrentWorksheet.SetValue(currentRow, StatusColumn, issue.Status);
            CurrentWorksheet.Cells[currentRow, CreatedDateColumn].SetDateTime(issue.CreateDate);
            CurrentWorksheet.SetValue(currentRow, CreatedDateColumn, issue.CreateDate);
            currentRow++;
        }
    }   

    private void FillHeaders()
    {
        //Заголовки данных
        CurrentWorksheet.SetValue(headerRow, projectKeyColumn, "Проект");
        CurrentWorksheet.SetValue(headerRow, issueTypeColumn, "Тип задачи");
        CurrentWorksheet.SetValue(headerRow, issueNameColumn, "Задача");
        CurrentWorksheet.SetValue(headerRow, AllEstimatesColumn, "Списанное время в часах разработки");
        CurrentWorksheet.SetValue(headerRow, ReworkCountColumn, "Количество доработок");

        CurrentWorksheet.SetValue(headerRow, SpColumn, "SP");
        CurrentWorksheet.SetValue(headerRow, PriorityColumn, "Приоритет");
        CurrentWorksheet.SetValue(headerRow, CreatedDateColumn, "Дата создания задачи");
        CurrentWorksheet.SetValue(headerRow, StatusColumn, "Статус задачи");
    }
}
