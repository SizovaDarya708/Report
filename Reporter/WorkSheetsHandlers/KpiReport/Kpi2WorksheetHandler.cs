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

        TimeSpan allResolvingTime = TimeSpan.FromSeconds(0);
        foreach (var issue in allClosedIssues)
        {
            //надо посчитать сколько по времени задача была в к разработке от реализовано
            //время когда задача попала от "к разработке" в "разработка"
            var fromToDevelop = issue.ChangeLogs.Where(x => x.Items.Any(i =>
                i.FieldName.ToLower() == JiraConstants.Status.ToLower()
                && i.FromValue.ToLower() == JiraConstants.ToWork.ToLower()
                && i.ToValue.ToLower() == JiraConstants.Work.ToLower())).First().CreateDate;

            //время от разработка в реализовано - за счет OrderBy и First это будет первый переход в реализовано
            var fromReviewToReady = issue.ChangeLogs.Where(x => x.Items.Any(i =>
                i.FieldName.ToLower() == JiraConstants.Status.ToLower()
                && i.FromValue.ToLower() == JiraConstants.Review.ToLower()
                && i.ToValue.ToLower() == JiraConstants.Ready.ToLower())).OrderBy(x => x.CreateDate).First().CreateDate;

            //время чистого решения без доработок
            var resolveWithoutReworkTime = fromToDevelop - fromReviewToReady;

            //теперь надо считать время взятие в доработку - таких может быть много
            var fromToRework = issue.ChangeLogs.Where(x => x.Items.Any(i =>
                i.FieldName.ToLower() == JiraConstants.Status.ToLower()
                && i.FromValue.ToLower() == JiraConstants.ToRework.ToLower()
                && i.ToValue.ToLower() == JiraConstants.Rework.ToLower()));

            //все переходы в реализовано - их много
            var fromToReady = issue.ChangeLogs.Where(x => x.Items.Any(i =>
                i.FieldName.ToLower() == JiraConstants.Status.ToLower()
                && i.FromValue.ToLower() == JiraConstants.Review.ToLower()
                && i.ToValue.ToLower() == JiraConstants.Ready.ToLower())).OrderBy(x => x.CreateDate);

            //после этого надо как-то группировать переходы и считать время, после суммировать
            if (resolveWithoutReworkTime == null)
            {
                continue;
            }
            allResolvingTime += resolveWithoutReworkTime!.Value; // + все что было по доработкам
        }
        CurrentWorksheet.SetValue(currentRow, totalClosedIssuesColumn, allClosedIssues.Count());
        CurrentWorksheet.SetValue(currentRow, ResolveIssuesTimeColumn, allResolvingTime);
        CurrentWorksheet.SetValue(currentRow, AverageResolvingTimeForIssues, allResolvingTime / allClosedIssues.Count());

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
        CurrentWorksheet.SetValue(headerRow, ResolveIssuesTimeColumn, "Суммарное время решения задач");
        CurrentWorksheet.SetValue(headerRow, AverageResolvingTimeForIssues, "Среднее время решения дефекта");

        CurrentWorksheet.SetValue(headerRow, periodStartDateColumn, "Начало периода");
        CurrentWorksheet.SetValue(headerRow, periodEndDateColumn, "Конец периода");
    }
}
