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

    private int ParticipantLoginColumn = 1;
    private int ParticipantNameColumn = 2;
    private int ParticipantTypeColumn = 3;


    #endregion

    public void FillReportData()
    {
        FillHeaders();
        FillData();
        FillFormat();
    }

    private void FillData()
    {
        var participants = _sprintReportEntity.IssueParticipants;

        foreach (var participant in participants)
        {
            CurrentWorksheet.SetValue(currentRow, ParticipantLoginColumn, participant.UserLogin);
            CurrentWorksheet.SetValue(currentRow, ParticipantNameColumn, participant.Name);
            CurrentWorksheet.SetValue(currentRow, ParticipantTypeColumn, participant.Department);

            currentRow++;
        }
    }

    private void FillHeaders()
    {
        //Заголовки данных
        CurrentWorksheet.SetValue(headerRow, ParticipantLoginColumn, "Логин сотрудника");
        CurrentWorksheet.SetValue(headerRow, ParticipantNameColumn, "Cотрудник");
        CurrentWorksheet.SetValue(headerRow, ParticipantTypeColumn, "Отдел");
    }
}
