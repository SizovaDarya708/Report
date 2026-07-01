using OfficeOpenXml;
using Reporter.Entities;
using Reporter.Extensions;


namespace Reporter.WorkSheetsHandlers.SprintReport;

public class Kpi3WorksheetHandler : WorksheetExportHandlerBase
{
    private SprintReportEntity _sprintReportEntity;

    public Kpi3WorksheetHandler(ExcelPackage excelPackage, string listName, SprintReportEntity sprintReportEntity)
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
    //private int participantColumn = 2;
    private int AccuracyColumn = 2;
    private int StartPeriodDateColumn = 3;
    private int EndPeriodDateColumn = 4;

    #endregion

    public void FillReportData()
    {
        FillHeaders();
        FillData();
        FillFormat();
    }

    private void FillData()
    {
        var allIssues = _sprintReportEntity.GetAllIssues();
        var projectGroupedIssues = allIssues.GroupBy(issue => issue.ProjectKey)
           .ToDictionary(k => k.Key, v => v.Select(i => i).ToList());

        foreach (var projectIssues in projectGroupedIssues)
        {
            CalcStoryPointAccuracy(projectIssues);
        }
    }
    private void CalcStoryPointAccuracy(KeyValuePair<string, List<IssueEntity>> projectIssues)
    {
        var eS_i = 0;
        var rHtoSbyIssues = new List<decimal>();
        var issues = projectIssues.Value;
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

            eS_i += s_i.Value; // сумма оценок
            var r_i = h_i / s_i!.Value; // часы делённые на сторПоинт 
            rHtoSbyIssues.Add(r_i);        
        }

        if (rHtoSbyIssues.Count == 0)
        {
            return;
        }

        //var r = CalculateMedian(rHtoSbyIssues); // большое R вес СторПоинта по медиане среди всех выполненных оценок
        var r = issues.Sum(x => x.H_i() / issues.Sum(x => x.StoryPoints ?? 3));
        var rES_i = r * eS_i; 

        decimal eAcuracity = 0;
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
            
            eAcuracity += Math.Abs(h_i - (r * (decimal)s_i)); // разница между оценкой и фактом с учётом средней взвешенной стоимости
        }

        var Accuracy = (1 - (eAcuracity/ rES_i)) * 100;

        CurrentWorksheet.SetValue(currentRow, projectKeyColumn, projectIssues.Key);
        //CurrentWorksheet.SetValue(currentRow, participantColumn, participant.Name);
        CurrentWorksheet.SetValue(currentRow, AccuracyColumn, Accuracy);
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
        //CurrentWorksheet.SetValue(headerRow, participantColumn, "Автор");
        CurrentWorksheet.SetValue(headerRow, AccuracyColumn, "Точность");
        CurrentWorksheet.SetValue(headerRow, StartPeriodDateColumn, "Начало периода");
        CurrentWorksheet.SetValue(headerRow, EndPeriodDateColumn, "Конец периода");
    }


    // public static decimal CalculateMedian<T>(IEnumerable<T> source) where T : IComparable<T>
    // {
    //     if (source == null)
    //         throw new ArgumentNullException(nameof(source));
    //
    //     var list = source.Cast<decimal>().OrderBy(x => x).ToList();
    //     int count = list.Count;
    //
    //     if (count == 0)
    //         throw new InvalidOperationException("Коллекция пуста");
    //
    //     int mid = count / 2;
    //     if (count % 2 == 0)
    //         return (list[mid - 1] + list[mid]) / (decimal)2;
    //     else
    //         return list[mid];
    // }
}
