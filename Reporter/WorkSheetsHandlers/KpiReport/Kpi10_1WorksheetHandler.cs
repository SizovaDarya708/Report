using Atlassian.Jira;
using OfficeOpenXml;
using Reporter.Entities;
using Reporter.Extensions;

namespace Reporter.WorkSheetsHandlers.SprintReport;

public class Kpi10_1WorksheetHandler : WorksheetExportHandlerBase
{
    private SprintReportEntity _sprintReportEntity;

    private List<WorkEstimateTypeEnum> DeveloperEstimateTypes =
        new List<WorkEstimateTypeEnum> { WorkEstimateTypeEnum.Develop, WorkEstimateTypeEnum.Rework, WorkEstimateTypeEnum.Review };

    public Kpi10_1WorksheetHandler(ExcelPackage excelPackage, string listName, SprintReportEntity sprintReportEntity)
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
    private int issueColumn = 2;
    private int resolveIssueTimeColumn = 3;
    private int reworkTimeSpentColumn = 4;
    private int issuesWithReworkPercentageColumn = 5;
    private int periodStartDateColumn = 6;
    private int periodEndDateColumn = 7;

    #endregion

    public void FillReportData()
    {
        FillHeaders();
        FillDataByProjects();
        FillFormat();
    }

    private void FillDataByProjects()
    {
        var allIssues = _sprintReportEntity.GetAllIssues();
        var projectGroupedIssues = allIssues.GroupBy(issue => issue.ProjectKey)
           .ToDictionary(k => k.Key, v => v.Select(i => i).ToList());

        foreach (var projectIssues in projectGroupedIssues)
        {
            FillData(projectIssues);
        }
    }

    private void FillData(KeyValuePair<string, List<IssueEntity>> projectIssues)
    {
        var developerPerIssues = new Dictionary<IssueParticipantEntity, List<IssueEntity>>(new IssueParticipantEntityComparer()) {};

        var allIssuesInfo = projectIssues.Value
            .Where(i => i.Status.ToLower() == JiraConstants.Closed.ToLower());

        //группируем задачи по разработчику
        foreach (var issue in allIssuesInfo)
        {
            FillReworks(issue);
        }
    }

    private void FillReworks(IssueEntity issue)
    {
        //Посчитать всю работу над задачей по разработке(разработка, ревью, доработка)
        var allIssueTimeSpent = issue.Estimates
            .Where(estimate => estimate.WorkEstimateType != null && DeveloperEstimateTypes.Contains(estimate.WorkEstimateType!.Value))
            .Sum(estimate => estimate.Worklog.TimeSpendInSeconds);
        CurrentWorksheet.SetValue(currentRow, resolveIssueTimeColumn, allIssueTimeSpent/60/60);

        if (allIssueTimeSpent == 0)
        {
            return;
        }

        CurrentWorksheet.SetValue(currentRow, projectKeyColumn, issue.ProjectKey);
        CurrentWorksheet.SetValue(currentRow, issueColumn, issue.Key);

        //Подсчитать все время на дорботки по задаче
        var reworksInfo = issue.GetReworkInfo();
        var allReworksTimeSpent = reworksInfo.TimeSpendInSeconds != null && reworksInfo.TimeSpendInSeconds != 0 ?
            reworksInfo.TimeSpendInSeconds / 60 / 60 : 0;
        CurrentWorksheet.SetValue(currentRow, reworkTimeSpentColumn, allReworksTimeSpent);
        var reworkPercentage = allReworksTimeSpent == 0 ? (decimal)allReworksTimeSpent : ((decimal)allReworksTimeSpent / (decimal)allIssueTimeSpent) * 100;
        CurrentWorksheet.Cells[currentRow, issuesWithReworkPercentageColumn].Style.Numberformat.Format = "0.#";
        CurrentWorksheet.SetValue(currentRow, issuesWithReworkPercentageColumn, reworkPercentage);
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
        CurrentWorksheet.SetValue(headerRow, issueColumn, "Задача");
        CurrentWorksheet.SetValue(headerRow, resolveIssueTimeColumn, "Время на решение задачи в часах");
        CurrentWorksheet.SetValue(headerRow, reworkTimeSpentColumn, "Время на доработки в часах");
        CurrentWorksheet.SetValue(headerRow, issuesWithReworkPercentageColumn, "Процент времени затраченное на доработки от всего времени задачи");

        CurrentWorksheet.SetValue(headerRow, periodStartDateColumn, "Начало периода");
        CurrentWorksheet.SetValue(headerRow, periodEndDateColumn, "Конец периода");
    }
}
