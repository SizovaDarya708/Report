using OfficeOpenXml;
using Reporter.Entities;

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
            IssuesFirstWorkSheetHandler firstListHandler = 
                new IssuesFirstWorkSheetHandler(package, firstListName, sprintReportEntity);
            firstListHandler.FillReportData();


            package.Save();
        }

    }
}
