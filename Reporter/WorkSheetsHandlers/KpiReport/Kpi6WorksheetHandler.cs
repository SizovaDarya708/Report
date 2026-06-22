using Atlassian.Jira;
using OfficeOpenXml;
using Reporter.Entities;
using Reporter.Extensions;
using System.ComponentModel.DataAnnotations;

namespace Reporter.WorkSheetsHandlers.SprintReport;

public class Kpi6WorksheetHandler : WorksheetExportHandlerBase
{
    private SprintReportEntity _sprintReportEntity;

    public Kpi6WorksheetHandler(ExcelPackage excelPackage, string listName, SprintReportEntity sprintReportEntity)
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
    private int storyPointsColumn = 3;
    private int hoursColumn = 4;
    private int pWeightColumn = 5;
    private int periodStartDateColumn = 6;
    private int periodEndDateColumn = 7;

    #endregion

    public void FillReportData()
    {
        FillHeaders();
        FillData();
        FillFormat();
    }

    private void FillData()
    {
    }
    private void FillReworksByDeveloper(KeyValuePair<IssueParticipantEntity, List<IssueEntity>> developersReworks)
    {
       
        currentRow++;
    }

    private void FillHeaders()
    {
        //Заголовки данных
        CurrentWorksheet.SetValue(headerRow, issueColumn, "Сотрудник");
        CurrentWorksheet.SetValue(headerRow, storyPointsColumn, "Количество задач");
        CurrentWorksheet.SetValue(headerRow, hoursColumn, "Количество задач с доработками");
        CurrentWorksheet.SetValue(headerRow, pWeightColumn, "Процент задач с доработками");

        CurrentWorksheet.SetValue(headerRow, periodStartDateColumn, "Начало периода");
        CurrentWorksheet.SetValue(headerRow, periodEndDateColumn, "Конец периода");
    }
}
