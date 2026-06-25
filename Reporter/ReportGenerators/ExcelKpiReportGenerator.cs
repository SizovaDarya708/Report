using OfficeOpenXml;
using Reporter.Entities;
using Reporter.WorkSheetsHandlers.SprintReport;

namespace Reporter.ReportGenerators;

public class ExcelKpiReportGenerator
{
    private string _1ListName = "KPI 1.";
    private string _2ListName = "KPI 2.";
    private string _3ListName = "KPI 3";
    private string _4ListName = "KPI 4";
    private string _5ListName = "KPI 5";
    private string _6ListName = "KPI 6";
    private string _7ListName = "KPI 8";

    private string tenthListName = "KPI 10";

    public void GenerateReport(string filePath, SprintReportEntity sprintReportEntity)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        var dateString = $"{sprintReportEntity.ReportPeriod.StartDate.ToString("dd.MM")}-{sprintReportEntity.ReportPeriod.EndDate.ToString("dd.MM.yy")}";
        var reportPath = Path.Combine(filePath, $"Отчет KPI ({dateString}).xlsx");

        using (var package = new ExcelPackage(new FileInfo(reportPath)))
        {
            Kpi1WorksheetHandler firstListHandler =
                new Kpi1WorksheetHandler(package, _1ListName, sprintReportEntity);
            firstListHandler.FillReportData();

            Kpi2WorksheetHandler secondListHandler =
                new Kpi2WorksheetHandler(package, _2ListName, sprintReportEntity);
            secondListHandler.FillReportData();

            Kpi3WorksheetHandler thirdListHandler =
                new Kpi3WorksheetHandler(package, _3ListName, sprintReportEntity);
            thirdListHandler.FillReportData();

            Kpi4WorksheetHandler fourthListHandler =
                new Kpi4WorksheetHandler(package, _4ListName, sprintReportEntity);
            fourthListHandler.FillReportData();

            Kpi5WorksheetHandler fifthListHandler =
                new Kpi5WorksheetHandler(package, _5ListName, sprintReportEntity);
            fifthListHandler.FillReportData();

            Kpi6WorksheetHandler sixthListHandler =
                new Kpi6WorksheetHandler(package, _6ListName, sprintReportEntity);
            sixthListHandler.FillReportData();

            Kpi8WorksheetHandler eightListHandler =
                new Kpi8WorksheetHandler(package, _7ListName, sprintReportEntity);
            eightListHandler.FillReportData();

            Kpi10WorksheetHandler tenthListHandler =
                new Kpi10WorksheetHandler(package, tenthListName, sprintReportEntity);
            tenthListHandler.FillReportData();

            package.Save();
        }

    }
}

