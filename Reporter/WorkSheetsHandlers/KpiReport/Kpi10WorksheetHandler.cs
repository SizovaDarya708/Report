using Atlassian.Jira;
using OfficeOpenXml;
using Reporter.Entities;
using Reporter.Extensions;

namespace Reporter.WorkSheetsHandlers.SprintReport;

public class Kpi10WorksheetHandler : WorksheetExportHandlerBase
{
    private SprintReportEntity _sprintReportEntity;

    private List<WorkEstimateTypeEnum> DeveloperEstimateTypes =
        new List<WorkEstimateTypeEnum> { WorkEstimateTypeEnum.Develop, WorkEstimateTypeEnum.Rework, WorkEstimateTypeEnum.Review };

    public Kpi10WorksheetHandler(ExcelPackage excelPackage, string listName, SprintReportEntity sprintReportEntity)
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
    private int resolveIssueTimeColumn = 3;
    private int reworkTimeSpentColumn = 4;
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

    private void FillReworksByDeveloper(KeyValuePair<IssueParticipantEntity, List<IssueEntity>> issuesPerParticipant)
    {
        var randomIssueForKey = issuesPerParticipant.Value.FirstOrDefault();

        if (randomIssueForKey == null)
        {
            return;
        }

        CurrentWorksheet.SetValue(currentRow, projectKeyColumn, randomIssueForKey.ProjectKey);
        CurrentWorksheet.SetValue(currentRow, authorNameColumn, issuesPerParticipant.Key.Name);

        //Посчитать всю работу над задачами по сотруднику
        //у каждой задачи посчитать списания по видам работ - разработка, ревью, доработка, (анализ?)
        var allIssueTimeSpent = issuesPerParticipant.Value
            .SelectMany(issue => issue.Estimates)
            .Where(estimate => estimate.Worklog.Participant.Name == issuesPerParticipant.Key.Name)
            .Where(estimate => estimate.WorkEstimateType != null && DeveloperEstimateTypes.Contains(estimate.WorkEstimateType!.Value))
            .Sum(estimate => estimate.Worklog.TimeSpendInSeconds);
        CurrentWorksheet.SetValue(currentRow, resolveIssueTimeColumn, allIssueTimeSpent);

        //Подсчитать все время на дорботки по задачам
        long allReworksTimeSpent = 0;
        foreach (var issue in issuesPerParticipant.Value)
        {
            var reworksInfo = issue.GetReworkInfo();
            allReworksTimeSpent += (reworksInfo.TimeSpendInSeconds ?? 0);                    
        }

        CurrentWorksheet.SetValue(currentRow, reworkTimeSpentColumn, allReworksTimeSpent);
        var reworkPercentage = allReworksTimeSpent == 0 ? allReworksTimeSpent : ((decimal)allReworksTimeSpent / (decimal)allIssueTimeSpent) * 100;
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
        CurrentWorksheet.SetValue(headerRow, authorNameColumn, "Сотрудник");
        CurrentWorksheet.SetValue(headerRow, resolveIssueTimeColumn, "Время на решение задачи");
        CurrentWorksheet.SetValue(headerRow, reworkTimeSpentColumn, "Время на доработки");
        CurrentWorksheet.SetValue(headerRow, issuesWithReworkPercentageColumn, "Процент времени затраченное на доработки от всего времени задачи");

        CurrentWorksheet.SetValue(headerRow, periodStartDateColumn, "Начало периода");
        CurrentWorksheet.SetValue(headerRow, periodEndDateColumn, "Конец периода");
    }
}
