using OfficeOpenXml;
using Reporter.Entities;
using Reporter.WorkSheetsHandlers;

namespace Reporter;

public class ExcelReportGenerator
{
    private string firstListName = "Лист 1";
    private string secondListName = "Лист 2";
    private string thirdListName = "Лист 3";

    public void GenerateReport(string filePath, SprintReportEntity sprintReportEntity)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        var reportPath = Path.Combine(filePath, "SprintReport.xlsx");

        using (var package = new ExcelPackage(new FileInfo(reportPath)))
        {
            IssuesWorksheetHandler firstListHandler = 
                new IssuesWorksheetHandler(package, firstListName, sprintReportEntity);
            firstListHandler.FillReportData();

            WorklogsWorksheetHandler secondListHandler =
                new WorklogsWorksheetHandler(package, secondListName, sprintReportEntity);
            secondListHandler.FillReportData();

            package.Save();
        }

    }
}
