using OfficeOpenXml;
using Reporter.Entities;
using Reporter.Extensions;

namespace Reporter.WorkSheetsHandlers.SprintReport;

public class Kpi8WorksheetHandler : WorksheetExportHandlerBase
{
    private SprintReportEntity _sprintReportEntity;

    public Kpi8WorksheetHandler(ExcelPackage excelPackage, string listName, SprintReportEntity sprintReportEntity)
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
    private int authorNameColumn = 2;
    private int complexIssueCountColumn = 3;
    private int highPriorityIssueCountColumn = 4;
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
        var developerPerIssues = new Dictionary<IssueParticipantEntity, List<IssueEntity>>(new IssueParticipantEntityComparer()) {};
        var allIssues = _sprintReportEntity.GetAllIssues();

        var allIssuesInfo = allIssues
            .Where(i => i.Status.ToLower() == JiraConstants.Closed.ToLower());

        //группируем задачи по разработчику
        foreach (var issue in allIssuesInfo)
        {
            var developer = issue.GetParticipantByType(EmployeeType.Developer);

            if (developer == null)
            {
                continue;
            }

            if (developerPerIssues.ContainsKey(developer))
            {
                developerPerIssues[developer].Add(issue);
            }
            else
            {
                developerPerIssues.Add(developer, new List<IssueEntity> { issue });
            }
        }

        foreach (var developer in developerPerIssues)
        {
            FillDeveloperIssues(developer);
        }
    }
    private void FillDeveloperIssues(KeyValuePair<IssueParticipantEntity, List<IssueEntity>> developersWorks)
    {
        var randomIssueForKey = developersWorks.Value.FirstOrDefault();

        if (randomIssueForKey == null)
        {
            return;        
        }

        CurrentWorksheet.SetValue(currentRow, projectKeyColumn, randomIssueForKey.ProjectKey);
        CurrentWorksheet.SetValue(currentRow, authorNameColumn, developersWorks.Key.Name);

        var complexIssueCount = developersWorks.Value
            .Where(x => x.StoryPoints >= 8)
            .Count();
        CurrentWorksheet.SetValue(currentRow, complexIssueCountColumn, complexIssueCount);

        var highPriorityIssueCount = developersWorks.Value
            .Where(i => i.Priority == JiraConstants.Urgent)
            .Count();
        CurrentWorksheet.SetValue(currentRow, highPriorityIssueCountColumn, highPriorityIssueCount);
       
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
        CurrentWorksheet.SetValue(headerRow, authorNameColumn, "Сотрудник");
        CurrentWorksheet.SetValue(headerRow, complexIssueCountColumn, "Количество задач с Sp >= 8");
        CurrentWorksheet.SetValue(headerRow, highPriorityIssueCountColumn, "Количество задач с приоритетом Неотложный");

        CurrentWorksheet.SetValue(headerRow, periodStartDateColumn, "Начало периода");
        CurrentWorksheet.SetValue(headerRow, periodEndDateColumn, "Конец периода");
    }
}
