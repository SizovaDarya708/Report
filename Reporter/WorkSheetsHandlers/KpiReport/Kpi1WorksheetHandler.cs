using Atlassian.Jira;
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
        FillData();
        FillFormat();
    }

    private void FillData()
    {
        var developerPerIssues = new Dictionary<IssueParticipantEntity, List<IssueEntity>>(new IssueParticipantEntityComparer()) {};
        var allIssues = _sprintReportEntity.GetAllIssues();

        //TODO взять задачи, которые вообще интересуют issuetype in (Инцидент, "New Feature", Bug, Improvement, ошибка)

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
            FillReworksByDeveloper(developer);
        }
    }
    private void FillReworksByDeveloper(KeyValuePair<IssueParticipantEntity, List<IssueEntity>> developersReworks)
    {
        CurrentWorksheet.SetValue(currentRow, authorNameColumn, developersReworks.Key.Name);
        var allIssueCount = developersReworks.Value.Count;
        CurrentWorksheet.SetValue(currentRow, issueCountColumn, allIssueCount);

        //Подсчитать все переработки по задачам

        var allReworksCount = 0;
        foreach (var issue in developersReworks.Value)
        {
            var reworksInfo = issue.GetReworkInfo();
            reworksInfo.ReworksPerParticipantList.TryGetValue(developersReworks.Key, out var issueReworks);

            if (issueReworks == null)
            {
                continue;            
            }

            var allReworksInPeriodCount = issueReworks.Count();

            allReworksCount += allReworksInPeriodCount;                    
        }

        CurrentWorksheet.SetValue(currentRow, issuesWithReworkCountColumn, allReworksCount);
        //CurrentWorksheet.Cells[currentRow, issuesWithReworkPercentageColumn].Style.Numberformat.Format = "0.###";
        //Тут дебилизм надо смотреть как вывести дроби в excel 
        CurrentWorksheet.SetValue(currentRow, issuesWithReworkPercentageColumn, (allReworksCount == 0 ? allReworksCount : (allReworksCount/allIssueCount)).ToString());
        CurrentWorksheet.Cells[currentRow, periodStartDateColumn].SetDateTime(_sprintReportEntity.ReportPeriod.StartDate);
        CurrentWorksheet.SetValue(currentRow, periodStartDateColumn, _sprintReportEntity.ReportPeriod.StartDate);
        CurrentWorksheet.Cells[currentRow, periodEndDateColumn].SetDateTime(_sprintReportEntity.ReportPeriod.EndDate);
        CurrentWorksheet.SetValue(currentRow, periodEndDateColumn, _sprintReportEntity.ReportPeriod.EndDate);
        currentRow++;
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
