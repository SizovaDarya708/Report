using OfficeOpenXml;
using Reporter.Entities;
using Reporter.WorkSheetsHandlers.SprintReport;

namespace Reporter.ReportGenerators;

public class ExcelKpiReportGenerator
{
    private string firstListName = "KPI 1. Кол-во доработок";
    private string secondListName = "KPI 2.";
    private string thirdListName = "KPI 3";
    private string fourthListName = "KPI 4";

    public void GenerateReport(string filePath, SprintReportEntity sprintReportEntity)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        var dateString = $"{sprintReportEntity.ReportPeriod.StartDate.ToString("dd.MM")}-{sprintReportEntity.ReportPeriod.EndDate.ToString("dd.MM.yy")}";
        var reportPath = Path.Combine(filePath, $"Отчет KPI ({dateString}).xlsx");

        using (var package = new ExcelPackage(new FileInfo(reportPath)))
        {
            IssuesWorksheetHandler firstListHandler =
                new IssuesWorksheetHandler(package, firstListName, sprintReportEntity);
            firstListHandler.FillReportData();           

            package.Save();
        }

    }
}

