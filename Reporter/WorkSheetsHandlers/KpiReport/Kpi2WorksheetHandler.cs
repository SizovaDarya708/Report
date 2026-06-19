using OfficeOpenXml;
using Reporter.Entities;
using Reporter.Extensions;

namespace Reporter.WorkSheetsHandlers.SprintReport;

public class Kpi2WorksheetHandler : WorksheetExportHandlerBase
{
    private SprintReportEntity _sprintReportEntity;

    public Kpi2WorksheetHandler(ExcelPackage excelPackage, string listName, SprintReportEntity sprintReportEntity)
    {
        ListName = listName;
        CurrentExcelPackage = excelPackage;
        _sprintReportEntity = sprintReportEntity;
        SetWorksheet();
    }

    #region Колонки
    private static int headerRow = 1;
    private int currentRow = 2;

    private int projectNameColumn = 2;
    private int totalClosedIssuesColumn = 3;
    private int ResolveIssuesTimeColumn = 4;
    private int AverageResolvingTimeForIssues = 5;
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
        //тут надо группировку по проектам
        var allIssues = _sprintReportEntity.GetAllIssues();
        var projectGroupedIssues = allIssues.GroupBy(issue => issue.ProjectKey)
            .ToDictionary(k => k.Key, v => v.Select(i => i).ToList());

        foreach (var projectIssues in projectGroupedIssues)
        {
            FillReworksByProjects(projectIssues);
        }
    }
    private void FillReworksByProjects(KeyValuePair<string, List<IssueEntity>> projectIssues)
    {
        var projectKey = projectIssues.Key;
        var allClosedIssues = projectIssues.Value.Where(i => i.Status == JiraConstants.Closed);

        CurrentWorksheet.SetValue(currentRow, projectNameColumn, projectKey);

        TimeSpan allResolvingTime = TimeSpan.Zero;
        foreach (var issue in allClosedIssues)
        {
            //Взять все переходы начало разработки и переходы в реализовано.
            var statusChangedChangelogs = issue.ChangeLogs
                .Where(x => x.Items.Any(
                    ch => 
                    ch.FieldName.ToLower() == JiraConstants.Status.ToLower()
                    &&
                    //Все переходы К разработке -> Разработка
                    (ch.FromValue.ToLower() == JiraConstants.ToWork.ToLower() && ch.ToValue.ToLower() == JiraConstants.Work.ToLower()
                    ||
                    //Все переходы К доработке -> Доработка
                    ch.FromValue.ToLower() == JiraConstants.ToRework.ToLower() && ch.ToValue.ToLower() == JiraConstants.Rework.ToLower()
                    ||
                    //Все переходы Ревью -> Реализован
                    ch.FromValue.ToLower() == JiraConstants.Review.ToLower() && ch.ToValue.ToLower() == JiraConstants.Ready.ToLower()
                    )))
                //отсортировать по времени перехода
                .OrderBy(ch => ch.CreateDate)
                .ToArray();

            for (int i = 0; i < statusChangedChangelogs.Count() -1; i++)
            {
                var time = statusChangedChangelogs[i].CreateDate;
                allResolvingTime += (statusChangedChangelogs[i+1].CreateDate - statusChangedChangelogs[i].CreateDate) ?? TimeSpan.Zero;
                i++;
            }
        }
        CurrentWorksheet.SetValue(currentRow, totalClosedIssuesColumn, allClosedIssues.Count());
        CurrentWorksheet.SetValue(currentRow, ResolveIssuesTimeColumn, allResolvingTime.TotalHours);
        var average = allResolvingTime / allClosedIssues.Count();
        CurrentWorksheet.SetValue(currentRow, AverageResolvingTimeForIssues, average.TotalHours);

        CurrentWorksheet.Cells[currentRow, periodStartDateColumn].SetDateTime(_sprintReportEntity.ReportPeriod.StartDate);
        CurrentWorksheet.SetValue(currentRow, periodStartDateColumn, _sprintReportEntity.ReportPeriod.StartDate);
        CurrentWorksheet.Cells[currentRow, periodEndDateColumn].SetDateTime(_sprintReportEntity.ReportPeriod.EndDate);
        CurrentWorksheet.SetValue(currentRow, periodEndDateColumn, _sprintReportEntity.ReportPeriod.EndDate);
        currentRow++;
    }

    private void FillHeaders()
    {
        //Заголовки данных
        CurrentWorksheet.SetValue(headerRow, projectNameColumn, "Проект");
        CurrentWorksheet.SetValue(headerRow, totalClosedIssuesColumn, "Количество задач");
        CurrentWorksheet.SetValue(headerRow, ResolveIssuesTimeColumn, "Суммарное время решения задач в часах");
        CurrentWorksheet.SetValue(headerRow, AverageResolvingTimeForIssues, "Среднее время решения дефекта в часах");

        CurrentWorksheet.SetValue(headerRow, periodStartDateColumn, "Начало периода");
        CurrentWorksheet.SetValue(headerRow, periodEndDateColumn, "Конец периода");
    }
}
