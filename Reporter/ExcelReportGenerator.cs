using OfficeOpenXml;
using Reporter.Entities;
using Reporter.WorkSheetsHandlers;

namespace Reporter;

public class ExcelReportGenerator
{
    private string firstListName = "Задачи";
    private string secondListName = "Списания времени по задачам";
    private string thirdListName = "Сотрудники";

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

            package.Save();
        }

    }
}
