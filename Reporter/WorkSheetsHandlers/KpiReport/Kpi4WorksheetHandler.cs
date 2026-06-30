using OfficeOpenXml;
using Reporter.Entities;
using Reporter.Extensions;


namespace Reporter.WorkSheetsHandlers.SprintReport;

public class Kpi4WorksheetHandler : WorksheetExportHandlerBase
{
    private SprintReportEntity _sprintReportEntity;

    private List<WorkEstimateTypeEnum> DeveloperEstimateTypes =
        new List<WorkEstimateTypeEnum> { WorkEstimateTypeEnum.Develop, WorkEstimateTypeEnum.Rework, WorkEstimateTypeEnum.Review };

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
    private int StartPeriodDateColumn = 6;
    private int EndPeriodDateColumn = 7;

    #endregion

    public void FillReportData()
    {
        FillHeaders();
        FillData();
        FillFormat();
    }

    private void FillData()
    {
        var developerPerIssues = new Dictionary<IssueParticipantEntity, List<IssueEntity>>(new IssueParticipantEntityComparer()) { };
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
            FillStorypointAccuracyByDeveloper(developer.Key, developer.Value);
        }
    }
    private void FillStorypointAccuracyByDeveloper(IssueParticipantEntity participant, List<IssueEntity> issues)
    {
        var randomIssueForKey = issues.FirstOrDefault();

        if (randomIssueForKey == null)
        {
            return;
        }

        var eS_i = 0;
        var rHtoSbyIssues = new List<decimal>();
        foreach (var issue in issues)
        {
            var s_i = issue.StoryPoints;

            if (s_i == null || s_i == 0)
            {
                continue;            
            }

            var h_i = issue.H_i();
                      

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

        decimal eAcuracity = 0;
        decimal eS_iP_i = 0;
        //теперь уже считать суммы с медианной
        foreach (var issue in issues)
        {
            var s_i = issue.StoryPoints;

            if (s_i == null || s_i == 0)
            {
                continue;
            }

            var h_i = issue.H_i();

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
        }

        var Accuracy = 1; // пока 100%. Надо взять из KPI3. Вообще вопрос стоит ли брать? Или как правильно учесть ошибку оценки в производительности?
        //скорее всего надо умножать на оценку раньше, чем ссчитается 
        
        var pWeighted = (eS_iP_i/eS_i);
        var pFinal = pWeighted * (Accuracy / 100);

        CurrentWorksheet.SetValue(currentRow, projectKeyColumn, randomIssueForKey.ProjectKey);
        CurrentWorksheet.SetValue(currentRow, participantColumn, participant.Name);
        CurrentWorksheet.SetValue(currentRow, AccuracyColumn, Accuracy);
        CurrentWorksheet.SetValue(currentRow, pWeightedColumn, pWeighted);
        CurrentWorksheet.SetValue(currentRow, pFinalColumn, pFinal);
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
