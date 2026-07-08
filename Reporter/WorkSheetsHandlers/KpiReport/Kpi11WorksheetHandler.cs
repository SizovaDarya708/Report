using OfficeOpenXml;
using Reporter.Entities;
using Reporter.Extensions;

namespace Reporter.WorkSheetsHandlers.SprintReport;

public class Kpi11WorksheetHandler : WorksheetExportHandlerBase
{
    private SprintReportEntity _sprintReportEntity;

    public Kpi11WorksheetHandler(ExcelPackage excelPackage, string listName, SprintReportEntity sprintReportEntity)
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
    private int allSpentTimeColumn = 3;
    private int featureSpentTimeColumn = 4;
    private int notFeaturesSpentTimePercentageColumn = 5;
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
        var allIssues = projectIssues.Value;

        //группируем задачи по разработчику
        foreach (var issue in allIssues)
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
            FillDeveloperIssues(developer, projectIssues.Key);
        }
    }
    private void FillDeveloperIssues(KeyValuePair<IssueParticipantEntity, List<IssueEntity>> developersWorks, string projectKey)
    {
        CurrentWorksheet.SetValue(currentRow, projectKeyColumn, projectKey);
        CurrentWorksheet.SetValue(currentRow, authorNameColumn, developersWorks.Key.Name);

        
        var allSpentTime = developersWorks.Value
            .SelectMany(i => i.Estimates)
            .Where(es => es.Worklog.Participant.Name == developersWorks.Key.Name)
            .Sum(es => es.Worklog.TimeSpendInSeconds);
        var allSpentTimeInHours = (allSpentTime == 0 || allSpentTime == null) ? 0 : (decimal)allSpentTime / 60 / 60;
        CurrentWorksheet.SetValue(currentRow, allSpentTimeColumn, allSpentTimeInHours);

        var featureWorkSpentTime = developersWorks.Value
            .Where(x => x.Type == JiraConstants.Features || x.Type == JiraConstants.NewDevelop)
            .SelectMany(i => i.Estimates)
            .Where(es => es.Worklog.Participant.Name == developersWorks.Key.Name)
            .Sum(es => es.Worklog.TimeSpendInSeconds);
        var featureWorkSpentTimeInHours = (featureWorkSpentTime == 0 || featureWorkSpentTime == null) ? 0 : (decimal)featureWorkSpentTime / 60 / 60;
        CurrentWorksheet.SetValue(currentRow, featureSpentTimeColumn, featureWorkSpentTimeInHours);

        var percentage = allSpentTimeInHours != 0 ? (1 - (featureWorkSpentTimeInHours / allSpentTimeInHours)) * 100 : 0;       
        CurrentWorksheet.SetValue(currentRow, notFeaturesSpentTimePercentageColumn, percentage);
       
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
        CurrentWorksheet.SetValue(headerRow, allSpentTimeColumn, "Все списанное время разработчика");
        CurrentWorksheet.SetValue(headerRow, featureSpentTimeColumn, "Время списанное на Тип Новая разработка, Улучшения");
        CurrentWorksheet.SetValue(headerRow, notFeaturesSpentTimePercentageColumn, "Процент времени НЕ на улучшения");

        CurrentWorksheet.SetValue(headerRow, periodStartDateColumn, "Начало периода");
        CurrentWorksheet.SetValue(headerRow, periodEndDateColumn, "Конец периода");
    }
}
