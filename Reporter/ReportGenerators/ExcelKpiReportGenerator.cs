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
    private string allLogsListName = "Записи о списаниях";
    private string issueInfoListName = "Информация о задачах";
    private string changeLogsListName = "Переходы по задачам";

    public void GenerateReport(string filePath, SprintReportEntity sprintReportEntity)
    {
        Console.WriteLine($"Start generate report");
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        var dateString = $"{sprintReportEntity.ReportPeriod.StartDate.ToString("dd.MM")}-{sprintReportEntity.ReportPeriod.EndDate.ToString("dd.MM.yy")}";
        var reportPath = Path.Combine(filePath, $"Отчет KPI ({dateString}).xlsx");

        using (var package = new ExcelPackage(new FileInfo(reportPath)))
        {
            Console.WriteLine($"KPI1");
            Kpi1WorksheetHandler firstListHandler =
                new Kpi1WorksheetHandler(package, _1ListName, sprintReportEntity);
            firstListHandler.FillReportData();

            Console.WriteLine($"KPI2");
            Kpi2_1WorksheetHandler secondListHandler =
                new Kpi2_1WorksheetHandler(package, _2_1ListName, sprintReportEntity);
            secondListHandler.FillReportData();

            Console.WriteLine($"KPI2_2");
            Kpi2_2WorksheetHandler second_2ListHandler =
                new Kpi2_2WorksheetHandler(package, _2_2ListName, sprintReportEntity);
            second_2ListHandler.FillReportData();

            Console.WriteLine($"KPI3");
            Kpi3WorksheetHandler thirdListHandler =
                new Kpi3WorksheetHandler(package, _3ListName, sprintReportEntity);
            thirdListHandler.FillReportData();

            Console.WriteLine($"KPI4");
            Kpi4WorksheetHandler fourthListHandler =
                new Kpi4WorksheetHandler(package, _4ListName, sprintReportEntity);
            fourthListHandler.FillReportData();

            Console.WriteLine($"KPI5");
            Kpi5WorksheetHandler fifthListHandler =
                new Kpi5WorksheetHandler(package, _5ListName, sprintReportEntity);
            fifthListHandler.FillReportData();

            Console.WriteLine($"KPI6");
            Kpi6WorksheetHandler sixthListHandler =
                new Kpi6WorksheetHandler(package, _6ListName, sprintReportEntity);
            sixthListHandler.FillReportData();

            Console.WriteLine($"KPI8");
            Kpi8WorksheetHandler eightListHandler =
                new Kpi8WorksheetHandler(package, _7ListName, sprintReportEntity);
            eightListHandler.FillReportData();

            Console.WriteLine($"KPI10_1");
            Kpi10_1WorksheetHandler tenthListHandler =
                new Kpi10_1WorksheetHandler(package, _8ListName, sprintReportEntity);
            tenthListHandler.FillReportData();

            // Console.WriteLine($"KPI10_2");
            // Kpi10_2WorksheetHandler tenth_2ListHandler =
            //     new Kpi10_2WorksheetHandler(package, _10_2ListName, sprintReportEntity);
            // tenth_2ListHandler.FillReportData();

            Console.WriteLine($"KPI11");
            Kpi11WorksheetHandler elevenListHandler =
                new Kpi11WorksheetHandler(package, _9ListName, sprintReportEntity);
            elevenListHandler.FillReportData();

            AllLogsWorksheetHandler allLogsHandler =
                new AllLogsWorksheetHandler(package, allLogsListName, sprintReportEntity);
            allLogsHandler.FillReportData();

            AllIssueWorksheetHandler allIssueWorksheetHandler =
                new AllIssueWorksheetHandler(package, issueInfoListName, sprintReportEntity);
            allIssueWorksheetHandler.FillReportData();

            ChangeLogsWorksheetHandler changeLogsWorksheetHandler =
                new ChangeLogsWorksheetHandler(package, changeLogsListName, sprintReportEntity);
            changeLogsWorksheetHandler.FillReportData();

            package.Save();
        }
    }
}

