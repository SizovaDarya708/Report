using OfficeOpenXml;
using Reporter.Entities;
using Reporter.Extensions;
using Reporter.WorkSheetsHandlers.KpiReport.Dtos;

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

    private int projectKeyColumn = 1;
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
        var developerPerIssues = new Dictionary<IssueParticipantEntity, List<ReworkInfoDto>>(new IssueParticipantEntityComparer()) {};

        var allIssuesInfo = projectIssues.Value
            .Where(i => i.Type.ToLower() == JiraConstants.Bug.ToLower() ||
                i.Type.ToLower() == JiraConstants.Incident.ToLower() ||
                i.Type.ToLower() == JiraConstants.NewDevelop.ToLower() ||
                i.Type.ToLower() == JiraConstants.Features.ToLower())
            .Where(i => i.Status.ToLower() == JiraConstants.Closed.ToLower());

        foreach (var issue in allIssuesInfo)
        {
            var developers = issue.GetParticipantsByType(EmployeeType.Developer);

            foreach (var developer in developers)
            {
                if (developer == null)
                {
                    continue;
                }

                var reworkInfo = new ReworkInfoDto(issue, developer);

                if (developerPerIssues.ContainsKey(developer))
                {
                    developerPerIssues[developer].Add(reworkInfo);
                }
                else
                {
                    developerPerIssues.Add(developer, new List<ReworkInfoDto> { reworkInfo });
                }
            }
        }

        foreach (var developer in developerPerIssues)
        {
            FillReworksByDeveloper(developer, projectIssues.Key);
        }
    }

    
    private void FillReworksByDeveloper(KeyValuePair<IssueParticipantEntity, List<ReworkInfoDto>> developersReworks, string projectKey)
    {
        var participant = developersReworks.Key;
        var issues = developersReworks.Value;

        CurrentWorksheet.SetValue(currentRow, projectKeyColumn, projectKey);
        CurrentWorksheet.SetValue(currentRow, authorNameColumn, participant.Name);
        CurrentWorksheet.SetValue(currentRow, issueCountColumn, issues.Count);
        
        var issueWithReworksCount = issues.Where(x => x.HasReworks == true).Count();
        CurrentWorksheet.SetValue(currentRow, issuesWithReworkCountColumn, issueWithReworksCount);
        var reworkPercentage = issueWithReworksCount == 0 ? 0 : ((decimal)issueWithReworksCount / (decimal)issues.Count) * 100;
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
        CurrentWorksheet.SetValue(headerRow, issueCountColumn, "Количество задач");
        CurrentWorksheet.SetValue(headerRow, issuesWithReworkCountColumn, "Количество задач с доработками");
        CurrentWorksheet.SetValue(headerRow, issuesWithReworkPercentageColumn, "Процент задач с доработками");

        CurrentWorksheet.SetValue(headerRow, periodStartDateColumn, "Начало периода");
        CurrentWorksheet.SetValue(headerRow, periodEndDateColumn, "Конец периода");
    }
}
