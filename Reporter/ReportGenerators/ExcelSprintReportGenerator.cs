using OfficeOpenXml;
using Reporter.Entities;
using Reporter.WorkSheetsHandlers.SprintReport;

namespace Reporter.ReportGenerators;

public class ExcelSprintReportGenerator
{
    private string firstListName = "Задачи";
    private string secondListName = "Списания времени по задачам";
    private string thirdListName = "Сотрудники";
    private string fourthListName = "Ошибки";

    public void GenerateReport(string filePath, SprintReportEntity sprintReportEntity)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        var dateString = $"{sprintReportEntity.ReportPeriod.StartDate.ToString("dd.MM")}-{sprintReportEntity.ReportPeriod.EndDate.ToString("dd.MM.yy")}";
        var reportPath = Path.Combine(filePath, $"Отчет спринта({dateString}).xlsx");

        using (var package = new ExcelPackage(new FileInfo(reportPath)))
        {
            IssuesWorksheetHandler firstListHandler = 
                new IssuesWorksheetHandler(package, firstListName, sprintReportEntity);
            firstListHandler.FillReportData();

            WorklogsWorksheetHandler secondListHandler =
                new WorklogsWorksheetHandler(package, secondListName, sprintReportEntity);
            secondListHandler.FillReportData();

            ParticipantsWorksheetHandler thirdListHandler =
                new ParticipantsWorksheetHandler(package, thirdListName, sprintReportEntity);
            thirdListHandler.FillReportData();

            ErrorWorksheetHandler fourthListHandler =
                new ErrorWorksheetHandler(package, fourthListName, sprintReportEntity);
            fourthListHandler.FillReportData();

            package.Save();
        }

    }
}
