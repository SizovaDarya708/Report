﻿using OfficeOpenXml;

namespace Reporter.WorkSheetsHandlers;

public abstract class WorksheetExportHandlerBase
{
    public string ListName { get; set; } = string.Empty;

    public ExcelPackage? CurrentExcelPackage { get; set; }

    public ExcelWorksheet? CurrentWorksheet { get; set; }

    public void SetWorksheet()
    {
        var existWorksheets = CurrentExcelPackage.Workbook.Worksheets;

        var worksheet = existWorksheets.FirstOrDefault(x => x.Name == ListName);

        if (worksheet != null)
        {
            CurrentExcelPackage.Workbook.Worksheets.Delete(worksheet);
        }

        worksheet = CurrentExcelPackage.Workbook.Worksheets.Add(ListName);
        CurrentWorksheet = worksheet;
    }

    public void FillFormat()
    {
        CurrentWorksheet.Cells.AutoFitColumns();
    }

}
