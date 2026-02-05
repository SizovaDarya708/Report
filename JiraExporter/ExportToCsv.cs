// Services/CsvExportService.cs
using JiraTimesheetExporter.Models;

namespace JiraTimesheetExporter.Services
{
    public interface ICsvExportService
    {
        Task ExportToCsvAsync(List<TimesheetRecord> records, string filePath);
    }

    public class CsvExportService : ICsvExportService
    {
        public async Task ExportToCsvAsync(List<TimesheetRecord> records, string filePath)
        {
            var csvLines = new List<string>
            {
                "ФИО;Дата;Списанное время (минуты);Задача;Тип задачи;Описание задачи;Комментарий"
            };

            foreach (var record in records.OrderBy(r => r.TaskKey).ThenBy(r => r.Date))
            {
                var line = $"\"{EscapeCsvField(record.EmployeeName)}\";" +
                          $"\"{record.Date:yyyy-MM-dd}\";" +
                          $"\"{EscapeCsvField(record.TimeSpentInMinutes.ToString())}\";" +
                          $"\"{EscapeCsvField(record.TaskKey)}\";" +
                          $"\"{EscapeCsvField(record.TaskType)}\";" +
                          $"\"{EscapeCsvField(record.TaskSummary)}\";" +
                          $"\"{EscapeCsvField(record.Comment)}\"";

                csvLines.Add(line);
            }

            await File.WriteAllLinesAsync(filePath, csvLines, System.Text.Encoding.UTF8);
        }

        private string EscapeCsvField(string field)
        {
            if (string.IsNullOrEmpty(field))
                return string.Empty;

            // Экранируем кавычки и убираем переносы строк
            return field.Replace("\"", "\"\"").Replace("\n", " ").Replace("\r", " ");
        }
    }
}