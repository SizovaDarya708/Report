using OfficeOpenXml;
namespace Reporter.Extensions;

public static class ExcelRangeExtension
{
    public static void SetDateTime(this ExcelRange cell, DateTime? dateTime)
    {
        if (!dateTime.HasValue)
        {
            return;
        }
        cell.Style.Numberformat.Format = "dd.MM.yyyy";
        cell.Formula = $"=DATE({dateTime?.ToString("yyyy,MM,dd")})";
    }
}