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
    private int AuthorLoginColumn = 4;
    private int TimeSpentInHoursColumn = 5;
    private int AuthorNameColumn = 6;
    private int Department = 7;
    private int SprintNameColumn = 8;
    private int IsCreatedInReportIntervalColumn = 9;
    private int IssueStatusColumn = 10;
    private int IssueNameColumn = 11;
    //private int EstimateColumn = 12;


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
            //Применить гиперссылку для ключа задачи
            CurrentWorksheet.Cells[headerRow, issueKeyColumn].SetHyperlink(new Uri($"https://jira.bars.group/browse/{workflow.IssueKey}"));

            CurrentWorksheet.SetValue(currentRow, TimeSpentInSecondsColumn, workflow.TimeSpendInSeconds);
            CurrentWorksheet.SetValue(currentRow, CreateDateColumn, workflow.UpdateDate);
            CurrentWorksheet.SetValue(currentRow, AuthorLoginColumn, workflow.Participant?.UserLogin);

            CurrentWorksheet.SetValue(currentRow, TimeSpentInHoursColumn, workflow.TimeSpendInSeconds / 60 / 60);
            CurrentWorksheet.SetValue(currentRow, AuthorNameColumn, workflow.Participant?.Name);
            CurrentWorksheet.SetValue(currentRow, Department, workflow.Participant ?.Department);
            CurrentWorksheet.SetValue(currentRow, SprintNameColumn, "Спринт");//TODO спринт как-то прокинуть
            CurrentWorksheet.SetValue(currentRow, IsCreatedInReportIntervalColumn, "");//TODO прокинуть и проверять входит ли лог в интервал выгрузки
            CurrentWorksheet.SetValue(currentRow, IssueStatusColumn, workflow.IssueStatus);
            CurrentWorksheet.SetValue(currentRow, IssueNameColumn, workflow.IssueName);
            //CurrentWorksheet.SetValue(currentRow, EstimateColumn, "Оценка");

            currentRow++;
        }
    }

    private void FillHeaders()
    {
        //Заголовки данных
        CurrentWorksheet.SetValue(headerRow, issueKeyColumn, "Ключ задачи");
        CurrentWorksheet.SetValue(headerRow, CreateDateColumn, "Дата создания лога");
        CurrentWorksheet.SetValue(headerRow, TimeSpentInSecondsColumn, "Время списанное");
        CurrentWorksheet.SetValue(headerRow, AuthorLoginColumn, "Логин автора лога");

        CurrentWorksheet.SetValue(headerRow, TimeSpentInHoursColumn, "Время списанное в часах");
        CurrentWorksheet.SetValue(headerRow, AuthorNameColumn, "Сотрудник");
        CurrentWorksheet.SetValue(headerRow, Department, "Отдел");
        CurrentWorksheet.SetValue(headerRow, SprintNameColumn, "Спринт");
        CurrentWorksheet.SetValue(headerRow, IsCreatedInReportIntervalColumn, "Логи интерисующие нас по промежутку времени (те, что были введены при выгрузке)");
        CurrentWorksheet.SetValue(headerRow, IssueStatusColumn, "Статус задачи");
        CurrentWorksheet.SetValue(headerRow, IssueNameColumn, "Имя задачи");
        //CurrentWorksheet.SetValue(headerRow, EstimateColumn, "Оценка");
    }
}
