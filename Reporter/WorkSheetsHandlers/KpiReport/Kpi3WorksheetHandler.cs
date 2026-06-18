using OfficeOpenXml;
using Reporter.Entities;

namespace Reporter.WorkSheetsHandlers.SprintReport;

public class Kpi3WorksheetHandler : WorksheetExportHandlerBase
{
    private SprintReportEntity _sprintReportEntity;

    private List<WorkEstimateTypeEnum> DeveloperEstimateTypes =
        new List<WorkEstimateTypeEnum> { WorkEstimateTypeEnum.Develop, WorkEstimateTypeEnum.Rework, WorkEstimateTypeEnum.Review };

    public Kpi3WorksheetHandler(ExcelPackage excelPackage, string listName, SprintReportEntity sprintReportEntity)
    {
        ListName = listName;
        CurrentExcelPackage = excelPackage;
        _sprintReportEntity = sprintReportEntity;
        SetWorksheet();
    }

    #region Колонки
    private static int headerRow = 1;
    private int currentRow = 2;

    private int issueColumn = 2;
    private int SpColumn = 3;
    private int HColumn = 4;
    private int HtoSpColumn = 5;

    #endregion

    public void FillReportData()
    {
        FillHeaders();
        FillData();
        FillFormat();
    }

    private void FillData()
    {
        var allClosedIssues = _sprintReportEntity.GetAllIssues()
            .Where(x => x.Status == JiraConstants.Closed);

        foreach (var issue in allClosedIssues)
        {
            FillIssueData(issue);
        }
    }
    private void FillIssueData(IssueEntity issue)
    {
        CurrentWorksheet.SetValue(currentRow, issueColumn, issue.Key);
        CurrentWorksheet.SetValue(currentRow, SpColumn, issue.StoryPoints);

        var storyPoint = issue.StoryPoints;

        var allIssueTimeSpent = issue.Estimates
           .Where(estimate => estimate.WorkEstimateType != null && DeveloperEstimateTypes.Contains(estimate.WorkEstimateType!.Value))
           .Sum(estimate => estimate.Worklog.TimeSpendInSeconds);

        var timeSpentInHours = allIssueTimeSpent / 60 / 60;
        CurrentWorksheet.SetValue(currentRow, HColumn, timeSpentInHours);
        CurrentWorksheet.SetValue(currentRow, HtoSpColumn, timeSpentInHours/storyPoint);
       
        currentRow++;
    }

    private void FillHeaders()
    {
        //Заголовки данных
        CurrentWorksheet.SetValue(headerRow, issueColumn, "Задача");
        CurrentWorksheet.SetValue(headerRow, SpColumn, "Story points");
        CurrentWorksheet.SetValue(headerRow, HColumn, "Списанное время разработки");
        CurrentWorksheet.SetValue(headerRow, HtoSpColumn, "r = H/Sp");
    }
}
