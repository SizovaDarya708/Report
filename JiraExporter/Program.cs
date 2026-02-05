using JiraTimesheetExporter.Services;
using Microsoft.Extensions.Configuration;

#region 
// Создание конфигурации
var configuration = BuildConfiguration();

// Пример чтения значений
var jiraUrl = configuration["JiraURL"];
var jiraLogin = configuration["JiraLogin"];
var jiraPassword = configuration["JiraPassword"];
#endregion


DateTime weekStartDate = new DateTime(2026, 1, 26);

try
{
    if (string.IsNullOrEmpty(jiraUrl) || string.IsNullOrEmpty(jiraLogin) || string.IsNullOrEmpty(jiraPassword))
    {
        Console.WriteLine("Ошибка: Не заданы переменные окружения JIRA_USERNAME и JIRA_API_TOKEN");
        return 1;
    }

    // Увеличиваем количество параллельных запросов
    var jiraService = new JiraService(jiraUrl, jiraLogin, jiraPassword, maxConcurrentRequests: 15);
    var csvExportService = new CsvExportService();

    Console.WriteLine($"Получение данных за неделю с {weekStartDate:yyyy-MM-dd}...");

    var startTime = DateTime.Now;
    var timesheetData = await jiraService.GetTimesheetDataAsync(weekStartDate, "UNP");
    var endTime = DateTime.Now;

    Console.WriteLine($"Найдено {timesheetData.Count} записей о списанном времени");
    Console.WriteLine($"Время выполнения: {(endTime - startTime).TotalSeconds:F2} секунд");

    var fileName = $"timesheet_{weekStartDate:yyyyMMdd}.csv";
    await csvExportService.ExportToCsvAsync(timesheetData, fileName);

    var incidentMinutes = timesheetData.Where(x => x.TaskType == "Инцидент").Select(x => x.TimeSpentInMinutes).Sum();
    var errorMinutes = timesheetData.Where(x => x.TaskType == "Ошибка").Select(x => x.TimeSpentInMinutes).Sum();
    var sprintMinutes = timesheetData.Where(x => x.TaskType != "Инцидент" && x.TaskType != "Ошибка").Select(x => x.TimeSpentInMinutes).Sum();


    Console.WriteLine($"Данные экспортированы в файл: {fileName}");

    Console.WriteLine($"На инциденты затрачено (минут): {incidentMinutes}, на ошибки затрачено (минут): {errorMinutes}, на прочее затрачено (минут): {sprintMinutes}");

    return 0;
}
catch (Exception ex)
{
    Console.WriteLine($"Ошибка: {ex.Message}");
    return 1;
}


static IConfiguration BuildConfiguration()
{
    return new ConfigurationBuilder()
        .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true)
        .AddEnvironmentVariables()
        .Build();
}

public class AppSettings
{
    public string JiraURL { get; set; }
    public bool JiraLogin { get; set; }
    public string JiraPassword { get; set; }
}