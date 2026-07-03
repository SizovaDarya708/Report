using OfficeOpenXml;
using Reporter.Entities;
using Reporter.WorkSheetsHandlers.SprintReport;

namespace Reporter.ReportGenerators;

public class ExcelKpiReportGenerator
{
    private string _1ListName = "KPI 1.";
    private string _2_1ListName = "KPI 2.1.";
    private string _2_2ListName = "KPI 2.2.";
    private string _3ListName = "KPI 3.";
    private string _4ListName = "KPI 4.";
    private string _5ListName = "KPI 5.";
    private string _6ListName = "KPI 6.";
    private string _7ListName = "KPI 8.";
    private string _8ListName = "KPI 10.1.";
    private string _10_2ListName = "KPI 10.2.";
    private string _9ListName = "KPI 11.";

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

            Kpi2_1WorksheetHandler secondListHandler =
                new Kpi2_1WorksheetHandler(package, _2_1ListName, sprintReportEntity);
            secondListHandler.FillReportData();

            Kpi2_2WorksheetHandler second_2ListHandler =
                new Kpi2_2WorksheetHandler(package, _2_2ListName, sprintReportEntity);
            second_2ListHandler.FillReportData();

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

            Kpi10_1WorksheetHandler tenthListHandler =
                new Kpi10_1WorksheetHandler(package, _8ListName, sprintReportEntity);
            tenthListHandler.FillReportData();

            Kpi10_2WorksheetHandler tenth_2ListHandler =
                new Kpi10_2WorksheetHandler(package, _10_2ListName, sprintReportEntity);
            tenth_2ListHandler.FillReportData();

            Kpi11WorksheetHandler elevenListHandler =
                new Kpi11WorksheetHandler(package, _9ListName, sprintReportEntity);
            elevenListHandler.FillReportData();

            package.Save();
        }

    }
}

