using OfficeOpenXml;
using Reporter.Entities;
using Reporter.Extensions;


namespace Reporter.WorkSheetsHandlers.SprintReport;

public class Kpi4WorksheetHandler : WorksheetExportHandlerBase
{
    private SprintReportEntity _sprintReportEntity;

    public Kpi4WorksheetHandler(ExcelPackage excelPackage, string listName, SprintReportEntity sprintReportEntity)
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
    private int participantColumn = 2;
    private int AccuracyColumn = 3;
    private int pWeightedColumn = 4;
    private int pFinalColumn = 5;
    private int spColumn = 6;
    private int issueCountColumn = 7;
    private int issueEstimateTimeColumn = 8;
    private int StartPeriodDateColumn = 9;
    private int EndPeriodDateColumn = 10;

    #endregion

    public void FillReportData()
    {
        FillHeaders();
        FillDataByProject();
        FillFormat();
    }

    private void FillDataByProject()
    {
        var allIssues = _sprintReportEntity.GetAllIssues();
        var projectGroupedIssues = allIssues.GroupBy(issue => issue.ProjectKey)
            .ToDictionary(k => k.Key, v => v.Select(i => i).ToList());

        foreach (var projectIssues in projectGroupedIssues)
        {
            FillDataByParticipant(projectIssues);
        }
    }

    private void FillDataByParticipant(KeyValuePair<string, List<IssueEntity>> projectIssues)
    {
        var developerPerIssues = new Dictionary<IssueParticipantEntity, List<IssueEntity>>(new IssueParticipantEntityComparer()) { };       

        var allIssuesInfo = projectIssues.Value
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
            FillStorypointAccuracyByDeveloper(projectIssues.Key, developer.Key, developer.Value);
        }

    }
   
    private void FillStorypointAccuracyByDeveloper(string projectKey, IssueParticipantEntity participant, List<IssueEntity> issues)
    {
        var eS_i = 0;
        var rHtoSbyIssues = new List<decimal>();
        foreach (var issue in issues)
        {
            var s_i = issue.StoryPoints;

            if (s_i == null || s_i == 0)
            {
                continue;            
            }

            var h_i = issue.GetAllDeveloperEstimatesInHours();
                      

            if (h_i == null || h_i == 0)
            {
                continue;
            }           

            eS_i += s_i.Value;
            var r_i = h_i / s_i!.Value;
            rHtoSbyIssues.Add(r_i);        
        }

        if (rHtoSbyIssues.Count == 0)
        {
            return;
        }

        var r = CalculateMedian(rHtoSbyIssues);
        var rES_i = r * eS_i;

        //Похоже поняла в чем беда - как надо считать точность:
        //относительно оценок задач конкретного разработчика или относительно всего проекта?
        decimal eAcuracity = 0;
        decimal eS_iP_i = 0;
        decimal issuesEstimateTime = 0;
        //теперь уже считать суммы с медианной
        foreach (var issue in issues)
        {
            var s_i = issue.StoryPoints;

            if (s_i == null || s_i == 0)
            {
                continue;
            }

            var h_i = issue.GetAllDeveloperEstimatesInHours();

            if (h_i == null || h_i == 0)
            {
                continue;
            }

            var p_i = s_i / h_i;
            if (p_i == null || p_i == 0)
            {
                continue;
            }

            eS_iP_i += (decimal)s_i * p_i.Value;
            eAcuracity += Math.Abs(h_i - (r * (decimal)s_i));
            issuesEstimateTime += h_i;
        }

        var Accuracy = 1; // пока 100%. Надо взять из KPI3. Вообще вопрос стоит ли брать? Или как правильно учесть ошибку оценки в производительности?
        //скорее всего надо умножать на оценку раньше, чем считается 
        
        var pWeighted = (eS_iP_i/eS_i);
        var pFinal = pWeighted * (Accuracy / 100);

        var issuesCount = issues.Count;
        var issuesSp = issues.Where(i => i.StoryPoints != null).Sum(i => i.StoryPoints);

        CurrentWorksheet.SetValue(currentRow, projectKeyColumn, projectKey);
        CurrentWorksheet.SetValue(currentRow, participantColumn, participant.Name);
        CurrentWorksheet.SetValue(currentRow, AccuracyColumn, Accuracy);
        CurrentWorksheet.SetValue(currentRow, pWeightedColumn, pWeighted);
        CurrentWorksheet.SetValue(currentRow, pFinalColumn, pFinal);
        CurrentWorksheet.SetValue(currentRow, issueCountColumn, issuesCount);
        CurrentWorksheet.SetValue(currentRow, spColumn, issuesSp);
        CurrentWorksheet.SetValue(currentRow, issueEstimateTimeColumn, issuesEstimateTime);
        CurrentWorksheet.Cells[currentRow, StartPeriodDateColumn].SetDateTime(_sprintReportEntity.ReportPeriod.StartDate);
        CurrentWorksheet.SetValue(currentRow, StartPeriodDateColumn, _sprintReportEntity.ReportPeriod.StartDate);
        CurrentWorksheet.Cells[currentRow, EndPeriodDateColumn].SetDateTime(_sprintReportEntity.ReportPeriod.EndDate);
        CurrentWorksheet.SetValue(currentRow, EndPeriodDateColumn, _sprintReportEntity.ReportPeriod.EndDate);

        currentRow++;
    }

    private void FillHeaders()
    {
        //Заголовки данных
        CurrentWorksheet.SetValue(headerRow, projectKeyColumn, "Проект");
        CurrentWorksheet.SetValue(headerRow, participantColumn, "Автор");
        CurrentWorksheet.SetValue(headerRow, AccuracyColumn, "Точность");
        CurrentWorksheet.SetValue(headerRow, pWeightedColumn, "Взвешенная производительность");
        CurrentWorksheet.SetValue(headerRow, pFinalColumn, "Производительность с учётом точности");
        CurrentWorksheet.SetValue(headerRow, spColumn, "Сумма оценок в SP");
        CurrentWorksheet.SetValue(headerRow, issueCountColumn, "Количество задач на разработчика");
        CurrentWorksheet.SetValue(headerRow, issueEstimateTimeColumn, "Сумма затраченного времени разработкой в часах");
        CurrentWorksheet.SetValue(headerRow, StartPeriodDateColumn, "Начало периода");
        CurrentWorksheet.SetValue(headerRow, EndPeriodDateColumn, "Конец периода");
    }


    public static decimal CalculateMedian<T>(IEnumerable<T> source) where T : IComparable<T>
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        var list = source.Cast<decimal>().OrderBy(x => x).ToList();
        int count = list.Count;

        if (count == 0)
            throw new InvalidOperationException("Коллекция пуста");

        int mid = count / 2;
        if (count % 2 == 0)
            return (list[mid - 1] + list[mid]) / (decimal)2;
        else
            return list[mid];
    }
}
