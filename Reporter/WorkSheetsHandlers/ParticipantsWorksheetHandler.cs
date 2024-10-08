using OfficeOpenXml;
using Reporter.Entities;

namespace Reporter.WorkSheetsHandlers;

public class ParticipantsWorksheetHandler : WorksheetExportHandlerBase
{
    private SprintReportEntity _sprintReportEntity;

    public ParticipantsWorksheetHandler(ExcelPackage excelPackage, string listName, SprintReportEntity sprintReportEntity)
    {
        ListName = listName;
        CurrentExcelPackage = excelPackage;
        _sprintReportEntity = sprintReportEntity;
        SetWorksheet();
    }

    #region Колонки
    private static int headerRow = 1;
    private int currentRow = 2;

    private int ParticipantNameColumn = 1;
    private int ParticipantTypeColumn = 2;


    #endregion

    public void FillReportData()
    {
        FillHeaders();
        FillData();
        FillFormat();
    }

    private void FillData()
    {
        //var workflows = _sprintReportEntity.GetAllWorklogs();

        //foreach (var workflow in workflows)
        //{
        //    //CurrentWorksheet.SetValue(currentRow, ParticipantNameColumn, );
        //    //CurrentWorksheet.SetValue(currentRow, ParticipantTypeColumn, );

        //    currentRow++;
        //}
    }

    private void FillHeaders()
    {
        //Заголовки данных
        CurrentWorksheet.SetValue(headerRow, ParticipantNameColumn, "Сотрудник");
        CurrentWorksheet.SetValue(headerRow, ParticipantTypeColumn, "Отдел");
    }
}
