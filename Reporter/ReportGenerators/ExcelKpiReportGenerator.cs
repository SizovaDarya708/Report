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
    private string fifthListName = "KPI 5";
    private string sixthListName = "KPI 6";

    private string tenthListName = "KPI 10";

    public void GenerateReport(string filePath, SprintReportEntity sprintReportEntity)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        var dateString = $"{sprintReportEntity.ReportPeriod.StartDate.ToString("dd.MM")}-{sprintReportEntity.ReportPeriod.EndDate.ToString("dd.MM.yy")}";
        var reportPath = Path.Combine(filePath, $"Отчет KPI ({dateString}).xlsx");

        using (var package = new ExcelPackage(new FileInfo(reportPath)))
        {
            Kpi1WorksheetHandler firstListHandler =
                new Kpi1WorksheetHandler(package, firstListName, sprintReportEntity);
            firstListHandler.FillReportData();

            Kpi2WorksheetHandler secondListHandler =
                new Kpi2WorksheetHandler(package, secondListName, sprintReportEntity);
            secondListHandler.FillReportData();

            Kpi3WorksheetHandler thirdListHandler =
                new Kpi3WorksheetHandler(package, thirdListName, sprintReportEntity);
            thirdListHandler.FillReportData();

            Kpi4WorksheetHandler fourthListHandler =
                new Kpi4WorksheetHandler(package, fourthListName, sprintReportEntity);
            fourthListHandler.FillReportData();

            Kpi5WorksheetHandler fifthListHandler =
                new Kpi5WorksheetHandler(package, fifthListName, sprintReportEntity);
            fifthListHandler.FillReportData();

            Kpi6WorksheetHandler sixthListHandler =
                new Kpi6WorksheetHandler(package, sixthListName, sprintReportEntity);
            sixthListHandler.FillReportData();

            Kpi10WorksheetHandler tenthListHandler =
                new Kpi10WorksheetHandler(package, tenthListName, sprintReportEntity);
            tenthListHandler.FillReportData();

            package.Save();
        }

    }
}

