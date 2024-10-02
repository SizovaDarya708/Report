using OfficeOpenXml;
using Reporter.Entities;

namespace Reporter;

public class ExcelReportGenerator
{
    private static int headerRow = 1;
    private int currentRow = 2;
    private int sprintNameColumn = 1;
    private int issueNameColumn = 2;
    private int issueKeyColumn = 3;
    private int lastExecutorColumn = 4;
    private int statusColumn = 5;
    private int typeColumn = 6;
    private int PriorityColumn = 7;
    private int EstimateColumn = 8;
    private int StoryPointsColumn = 9;
    private int DeveloperColumn = 10;
    private int CountOfReworkColumn = 11;

    private string firstListName = "Лист 1";

    public void GenerateReport(string filePath, SprintReportEntity sprintReportEntity)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        var reportPath = Path.Combine(filePath, "SprintReport.xlsx");
        using (var package = new ExcelPackage(new FileInfo(reportPath)))
        {
            var existWorksheets = package.Workbook.Worksheets;

            var firstWorksheet = existWorksheets.FirstOrDefault(x => x.Name == firstListName);

            if (firstWorksheet != null)
            {
                package.Workbook.Worksheets.Delete(firstWorksheet);
            }

            firstWorksheet = package.Workbook.Worksheets.Add(firstListName);

            FillSprintData(firstWorksheet, sprintReportEntity);
            FillDataWithoutSprint(firstWorksheet, sprintReportEntity);
            FillFormat(firstWorksheet);

            package.Save();
        }

    }

    private void FillFormat(ExcelWorksheet worksheet)
    {
        var cells = worksheet.Cells;
        cells.AutoFitColumns();
    }


    private void FillSprintData(ExcelWorksheet worksheet, SprintReportEntity sprintReportEntity)
    {
        FillHeaders(worksheet);
        foreach (var sprint in sprintReportEntity.Sprints)
        {
            foreach (var issue in sprint.Issues)
            {
                worksheet.SetValue(currentRow, sprintNameColumn, sprint.Name);
                worksheet.SetValue(currentRow, issueNameColumn, issue.Title);
                worksheet.SetValue(currentRow, issueKeyColumn, issue.Key);
                worksheet.SetValue(currentRow, lastExecutorColumn, issue.LastAssignee?.Name);
                worksheet.SetValue(currentRow, statusColumn, issue.Status);
                worksheet.SetValue(currentRow, typeColumn, issue.Type);
                worksheet.SetValue(currentRow, PriorityColumn, issue.Priority);
                worksheet.SetValue(currentRow, EstimateColumn, issue.TimeSpentInHours.ToString());
                worksheet.SetValue(currentRow, StoryPointsColumn, issue.StoryPoints?.ToString());

                var rework = issue.GetReworkInfo();
                worksheet.SetValue(currentRow, CountOfReworkColumn, rework.CountOfRework);

                var developer = issue.GetParticipantByType(EmployeeType.Developer);
                worksheet.SetValue(currentRow, DeveloperColumn, developer != null ? developer.Name : string.Empty);

                currentRow++;
            }
        }
    }

    private void FillDataWithoutSprint(ExcelWorksheet worksheet, SprintReportEntity sprintReportEntity)
    {
        foreach (var issue in sprintReportEntity.WithoutSprintPool.Issues)
        {
            worksheet.SetValue(currentRow, issueNameColumn, issue.Title);
            worksheet.SetValue(currentRow, issueKeyColumn, issue.Key);
            worksheet.SetValue(currentRow, lastExecutorColumn, issue.LastAssignee?.Name);
            worksheet.SetValue(currentRow, statusColumn, issue.Status);
            worksheet.SetValue(currentRow, typeColumn, issue.Type);
            worksheet.SetValue(currentRow, PriorityColumn, issue.Priority);
            worksheet.SetValue(currentRow, EstimateColumn, issue.TimeSpentInHours.ToString());

            var rework = issue.GetReworkInfo();
            worksheet.SetValue(currentRow, CountOfReworkColumn, rework.CountOfRework);

            var developer = issue.GetParticipantByType(EmployeeType.Developer);
            worksheet.SetValue(currentRow, DeveloperColumn, developer != null ? developer.Name : string.Empty);

            currentRow++;
        }

    }

    private void FillHeaders(ExcelWorksheet worksheet)
    {
        //Заголовки данных
        worksheet.SetValue(headerRow, sprintNameColumn, "Спринт");
        worksheet.SetValue(headerRow, issueNameColumn, "Наименование задачи");
        worksheet.SetValue(headerRow, issueKeyColumn, "Задача");
        worksheet.SetValue(headerRow, lastExecutorColumn, "Последний исполнитель");
        worksheet.SetValue(headerRow, statusColumn, "Статус");
        worksheet.SetValue(headerRow, typeColumn, "Тип задачи");
        worksheet.SetValue(headerRow, PriorityColumn, "Приоритет");
        worksheet.SetValue(headerRow, EstimateColumn, "Затрачено в часах");
        worksheet.SetValue(headerRow, StoryPointsColumn, "Оценка SP");
        worksheet.SetValue(headerRow, CountOfReworkColumn, "Количество доработок");
        worksheet.SetValue(headerRow, DeveloperColumn, "Разработчик");
    }
}
