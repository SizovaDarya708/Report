using OfficeOpenXml;
using Reporter.Entities;

namespace Reporter.WorkSheetsHandlers;

/// <summary>
/// Лист "Задачи"
/// </summary>
public class IssuesWorksheetHandler : WorksheetExportHandlerBase
{
    public IssuesWorksheetHandler(ExcelPackage excelPackage, string listName, SprintReportEntity sprintReportEntity)
    {
        ListName = listName;
        CurrentExcelPackage = excelPackage;
        _sprintReportEntity = sprintReportEntity;
        SetWorksheet();
    }

    #region Колонки
    private static int headerRow = 1;
    private int currentRow = 2;

    private int sprintNameColumn = 1;
    private int issueNameColumn = 2;
    private int issueKeyColumn = 3;
    private int lastExecutorColumn = 4;
    private int statusColumn = 5;
    private int typeColumn = 6;
    private int PriorityColumn = 7;
    private int EstimateTimeColumn = 8;
    private int StoryPointsColumn = 9;
    private int CountOfReworkColumn = 10;
    private int ResolutionColumn = 11;
    private int ResolutionDateColumn = 12;
    private int CreateDateColumn = 13;
    private int UpdateDateColumn = 14;

    private int ReworkDescriptionColumn = 15;
    private int SpentTimeDeveloperColumn = 16;
    private int TimeSpentColumn = 17;
    private int DepartmentColumn = 18;
    private int SprintStartColumn = 19;
    private int SprintEndColumn = 20;

    #endregion


    private SprintReportEntity _sprintReportEntity;

    public void FillReportData()
    {
        FillSprintData();
        FillDataWithoutSprint();
        FillFormat();
    }

    private void FillSprintData()
    {
        FillHeaders();
        foreach (var sprint in _sprintReportEntity.Sprints)
        {
            foreach (var issue in sprint.Issues)
            {
                CurrentWorksheet.SetValue(currentRow, sprintNameColumn, sprint.Name);
                CurrentWorksheet.SetValue(currentRow, SprintStartColumn, sprint.StartDate);
                CurrentWorksheet.SetValue(currentRow, SprintEndColumn, sprint.EndDate);

                FillCommonForSprintAndWithoutSprintIssues(issue);


                currentRow++;
            }
        }
    }

    private void FillCommonForSprintAndWithoutSprintIssues(IssueEntity issue)
    {
        CurrentWorksheet.SetValue(currentRow, issueNameColumn, issue.Title);
        CurrentWorksheet.SetValue(currentRow, issueKeyColumn, issue.Key);
        CurrentWorksheet.SetValue(currentRow, lastExecutorColumn, issue.LastAssignee?.Name);
        CurrentWorksheet.SetValue(currentRow, statusColumn, issue.Status);
        CurrentWorksheet.SetValue(currentRow, typeColumn, issue.Type);
        CurrentWorksheet.SetValue(currentRow, PriorityColumn, issue.Priority);
        CurrentWorksheet.SetValue(currentRow, EstimateTimeColumn, issue.TimeEstimateInSeconds);
        CurrentWorksheet.SetValue(currentRow, ResolutionDateColumn, issue.ResolutionDate);
        CurrentWorksheet.SetValue(currentRow, ResolutionColumn, issue.Resolution);
        CurrentWorksheet.SetValue(currentRow, CreateDateColumn, issue.CreateDate);
        CurrentWorksheet.SetValue(currentRow, UpdateDateColumn, issue.UpdateDate);

        var rework = issue.GetReworkInfo();
        CurrentWorksheet.SetValue(currentRow, CountOfReworkColumn, rework.CountOfRework);

        CurrentWorksheet.SetValue(currentRow, ReworkDescriptionColumn, issue.ReworkDescription);

        CurrentWorksheet.SetValue(currentRow, SpentTimeDeveloperColumn, "");
        CurrentWorksheet.SetValue(currentRow, TimeSpentColumn, "");
        CurrentWorksheet.SetValue(headerRow, DepartmentColumn, "");
    }

    private void FillDataWithoutSprint()
    {
        foreach (var issue in _sprintReportEntity.WithoutSprintPool.Issues)
        {
            FillCommonForSprintAndWithoutSprintIssues(issue);
            currentRow++;
        }
    }

    private void FillHeaders()
    {
        //Заголовки данных
        CurrentWorksheet.SetValue(headerRow, sprintNameColumn, "Спринт");
        CurrentWorksheet.SetValue(headerRow, issueNameColumn, "Наименование задачи");
        CurrentWorksheet.SetValue(headerRow, issueKeyColumn, "Ключ задачи");
        //Применить гиперссылку для ключа задачи
        CurrentWorksheet.Cells[headerRow, issueKeyColumn].SetHyperlink(new Uri($"https://jira.bars.group/browse/{issueKeyColumn}"));

        CurrentWorksheet.SetValue(headerRow, lastExecutorColumn, "Последний исполнитель");
        CurrentWorksheet.SetValue(headerRow, statusColumn, "Статус");
        CurrentWorksheet.SetValue(headerRow, typeColumn, "Тип задачи");
        CurrentWorksheet.SetValue(headerRow, PriorityColumn, "Приоритет");
        CurrentWorksheet.SetValue(headerRow, StoryPointsColumn, "Оценка SP");
        CurrentWorksheet.SetValue(headerRow, CountOfReworkColumn, "Количество доработок");

        CurrentWorksheet.SetValue(headerRow, EstimateTimeColumn, "Оценка времени");
        CurrentWorksheet.SetValue(headerRow, ResolutionDateColumn, "Резолюция");
        CurrentWorksheet.SetValue(headerRow, ResolutionColumn, "Дата резолюции");
        CurrentWorksheet.SetValue(headerRow, CreateDateColumn, "Дата создания");
        CurrentWorksheet.SetValue(headerRow, UpdateDateColumn, "Дата обновления");

        CurrentWorksheet.SetValue(headerRow, ReworkDescriptionColumn, "Описание доработки");
        CurrentWorksheet.SetValue(headerRow, SpentTimeDeveloperColumn, "Списано времени (Разработка)");
        CurrentWorksheet.SetValue(headerRow, TimeSpentColumn, "Списано времени всего");
        CurrentWorksheet.SetValue(headerRow, DepartmentColumn, "Отдел");
        CurrentWorksheet.SetValue(headerRow, SprintStartColumn, "Старт спринта");
        CurrentWorksheet.SetValue(headerRow, SprintEndColumn, "Окончание спринта");
    }
}
