using OfficeOpenXml;
using Reporter.Entities;

namespace Reporter.WorkSheetsHandlers;

public class WorklogsWorksheetHandler : WorksheetExportHandlerBase
{
    private SprintReportEntity _sprintReportEntity;

    public WorklogsWorksheetHandler(ExcelPackage excelPackage, string listName, SprintReportEntity sprintReportEntity)
    {
        ListName = listName;
        CurrentExcelPackage = excelPackage;
        _sprintReportEntity = sprintReportEntity;
        SetWorksheet();
    }

    #region Колонки
    private static int headerRow = 1;
    private int currentRow = 2;

    private int issueKeyColumn = 1;
    private int CreateDateColumn = 2;
    private int TimeSpentInSecondsColumn = 3;
    private int WorklogAuthorDisplayName = 4;


    #endregion

    public void FillReportData()
    {
        FillHeaders();
        FillData();
        FillFormat();
    }

    private void FillData()
    {
        var workflows = _sprintReportEntity.Worklogs;

        foreach (var workflow in workflows)
        {
            CurrentWorksheet.SetValue(currentRow, issueKeyColumn, workflow.IssueKey);
            CurrentWorksheet.SetValue(currentRow, TimeSpentInSecondsColumn, workflow.TimeSpendInSeconds);
            CurrentWorksheet.SetValue(currentRow, CreateDateColumn, workflow.UpdateDate?.ToString("dd.MM.yy HH:mm"));
            CurrentWorksheet.SetValue(currentRow, WorklogAuthorDisplayName, workflow.Participant?.Name);

            currentRow++;
        }
    }

    private void FillHeaders()
    {
        //Заголовки данных
        CurrentWorksheet.SetValue(headerRow, issueKeyColumn, "Ключ задачи");
        CurrentWorksheet.SetValue(headerRow, CreateDateColumn, "Дата создания лога");
        CurrentWorksheet.SetValue(headerRow, TimeSpentInSecondsColumn, "Время списанное");
        CurrentWorksheet.SetValue(headerRow, WorklogAuthorDisplayName, "Автор лога");
    }
}
