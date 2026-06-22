using Atlassian.Jira;
using OfficeOpenXml;
using Reporter.Entities;
using Reporter.Extensions;
using System.ComponentModel.DataAnnotations;

namespace Reporter.WorkSheetsHandlers.SprintReport;

public class Kpi5WorksheetHandler : WorksheetExportHandlerBase
{
    private SprintReportEntity _sprintReportEntity;

    public Kpi5WorksheetHandler(ExcelPackage excelPackage, string listName, SprintReportEntity sprintReportEntity)
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
    private int reviewTimeColumn = 4;
    private int averageReviewTimeColumn = 5;
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

        //TODO взять задачи, которые вообще интересуют issuetype in (Инцидент, Bug)

        var allIssuesInfo = allIssues
            .Where(i => i.Type == JiraConstants.Incident || i.Type == JiraConstants.Bug)
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
            FillDataByDeveloper(developer);
        }
    }
    private void FillDataByDeveloper(KeyValuePair<IssueParticipantEntity, List<IssueEntity>> developersWorks)
    {
        //Подсчитать все переходы от (разработка -> ревью) к (ревью -> реализован)
        // + от (доработка -> ревью) к (ревью -> реализован)

        var allReviewTime = TimeSpan.Zero;
        foreach (var issue in developersWorks.Value)
        {
            var statusChangedChangelogs = issue.ChangeLogs
                .Where(x => x.Items.Any(
                    ch =>
                    ch.FieldName.ToLower() == JiraConstants.Status.ToLower()
                    &&
                    //Все переходы разработка -> ревью
                    (ch.FromValue.ToLower() == JiraConstants.Work.ToLower() && ch.ToValue.ToLower() == JiraConstants.Review.ToLower()
                    ||
                    //Все переходы доработка -> ревью
                    ch.FromValue.ToLower() == JiraConstants.Rework.ToLower() && ch.ToValue.ToLower() == JiraConstants.Review.ToLower()
                    ||
                    //Все переходы Ревью -> Реализован
                    ch.FromValue.ToLower() == JiraConstants.Review.ToLower() && ch.ToValue.ToLower() == JiraConstants.Ready.ToLower()
                    )))
                //отсортировать по времени перехода
                .OrderBy(ch => ch.CreateDate)
                .ToArray();

            for (int i = 0; i < statusChangedChangelogs.Count() - 1; i++)
            {
                var time = statusChangedChangelogs[i].CreateDate;
                allReviewTime += (statusChangedChangelogs[i + 1].CreateDate - statusChangedChangelogs[i].CreateDate) ?? TimeSpan.Zero;
                i++;
            }
        }


        CurrentWorksheet.SetValue(currentRow, authorNameColumn, developersWorks.Key.Name);
        var allIssueCount = developersWorks.Value.Count;
        CurrentWorksheet.SetValue(currentRow, issueCountColumn, allIssueCount);
        CurrentWorksheet.SetValue(currentRow, reviewTimeColumn, allReviewTime.TotalHours);
        var averageReviewTime = allReviewTime.TotalHours == 0 ? allReviewTime.TotalHours : (allReviewTime.TotalHours / allIssueCount);
        CurrentWorksheet.Cells[currentRow, averageReviewTimeColumn].Style.Numberformat.Format = "0.##";
        CurrentWorksheet.SetValue(currentRow, averageReviewTimeColumn, averageReviewTime);

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
        CurrentWorksheet.SetValue(headerRow, reviewTimeColumn, "Время по задачам в статусе ревью в часах");
        CurrentWorksheet.SetValue(headerRow, averageReviewTimeColumn, "Среднее время прохождения Ревью");

        CurrentWorksheet.SetValue(headerRow, periodStartDateColumn, "Начало периода");
        CurrentWorksheet.SetValue(headerRow, periodEndDateColumn, "Конец периода");
    }
}
