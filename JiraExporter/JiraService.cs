// Services/JiraService.cs
using System.Net;
using System.Text;
using System.Text.Json;
using JiraTimesheetExporter.Models;

namespace JiraTimesheetExporter.Services
{
    public interface IJiraService
    {
        Task<List<TimesheetRecord>> GetTimesheetDataAsync(DateTime weekStartDate, string project = "UNP");
    }

    public class JiraService : IJiraService
    {
        private readonly HttpClient _httpClient;
        private readonly string _jiraUrl;
        private readonly string _username;
        private readonly string _apiToken;
        private readonly SemaphoreSlim _semaphore;

        public JiraService(string jiraUrl, string username, string apiToken, int maxConcurrentRequests = 10)
        {
            _jiraUrl = jiraUrl.TrimEnd('/');
            _username = username;
            _apiToken = apiToken;
            _semaphore = new SemaphoreSlim(maxConcurrentRequests, maxConcurrentRequests);

            var handler = new HttpClientHandler
            {
                MaxConnectionsPerServer = 50
            };

            _httpClient = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromMinutes(5)
            };

            var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{apiToken}"));
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authToken);
        }

        public async Task<List<TimesheetRecord>> GetTimesheetDataAsync(DateTime weekStartDate, string project = "UNP")
        {
            var weekEndDate = weekStartDate.AddDays(7);

            // Получаем все задачи с ворклогами за неделю за один запрос
            var issues = await GetAllIssuesWithWorklogsAsync(weekStartDate, weekEndDate, project);

            Console.WriteLine($"Найдено {issues.Count} задач с ворклогами за указанную неделю");

            // Параллельно обрабатываем задачи для получения деталей ворклогов
            var timesheetRecords = new List<TimesheetRecord>();
            var tasks = issues.Select(async issue =>
            {
                await _semaphore.WaitAsync();
                try
                {
                    var worklogs = await GetWorklogsForIssueAsync(issue.Key);
                    var records = ProcessWorklogs(worklogs, issue, weekStartDate, weekEndDate);
                    return records;
                }
                finally
                {
                    _semaphore.Release();
                }
            });

            var results = await Task.WhenAll(tasks);
            foreach (var records in results)
            {
                timesheetRecords.AddRange(records);
            }

            return timesheetRecords;
        }

        public async Task<List<TimesheetRecord>> GetIssuesAfterReview(DateTime weekStartDate, string project = "UNP")
        {
            var weekEndDate = weekStartDate.AddDays(7);

            // Получаем все задачи с ворклогами за неделю за один запрос
            var issues = await GetAllIssuesWithWorklogsAsync(weekStartDate, weekEndDate, project);

            Console.WriteLine($"Найдено {issues.Count} задач с ворклогами за указанную неделю");

            // Параллельно обрабатываем задачи для получения деталей ворклогов
            var timesheetRecords = new List<TimesheetRecord>();
            var tasks = issues.Select(async issue =>
            {
                await _semaphore.WaitAsync();
                try
                {
                    var worklogs = await GetWorklogsForIssueAsync(issue.Key);
                    var records = ProcessWorklogs(worklogs, issue, weekStartDate, weekEndDate);
                    return records;
                }
                finally
                {
                    _semaphore.Release();
                }
            });

            var results = await Task.WhenAll(tasks);
            foreach (var records in results)
            {
                timesheetRecords.AddRange(records);
            }

            return timesheetRecords;
        }

        private async Task<List<JiraIssue>> GetAllIssuesWithWorklogsAsync(DateTime fromDate, DateTime toDate, string project)
        {
            var allIssues = new List<JiraIssue>();
            int startAt = 0;
            const int maxResults = 100; // Увеличили размер страницы

            while (true)
            {
                // Оптимизированный JQL - ищем задачи с worklog'ами за период и сразу расширяем worklogs
                var jql = $"worklogDate >= \"{fromDate:yyyy-MM-dd}\" AND worklogDate < \"{toDate:yyyy-MM-dd}\" and project = {project}";
                // Запрашиваем только нужные поля и расширяем worklogs
                var fields = "key,issuetype,summary,worklog";
                var expand = "worklog";

                var url = $"{_jiraUrl}/rest/api/2/search?jql={WebUtility.UrlEncode(jql)}" +
                         $"&startAt={startAt}&maxResults={maxResults}" +
                         $"&fields={WebUtility.UrlEncode(fields)}" +
                         $"&expand={WebUtility.UrlEncode(expand)}";

                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var searchResponse = JsonSerializer.Deserialize<JiraSearchResponse>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (searchResponse?.Issues == null || !searchResponse.Issues.Any())
                    break;

                allIssues.AddRange(searchResponse.Issues);

                // Если получили меньше запрошенного количества, значит это последняя страница
                if (searchResponse.Issues.Count < maxResults)
                    break;

                startAt += maxResults;

                // Небольшая задержка чтобы не превысить лимиты API
                if (startAt % 500 == 0) // После каждых 500 задач
                {
                    await Task.Delay(100);
                }
            }

            return allIssues;
        }

        private async Task<List<JiraWorklog>> GetWorklogsForIssueAsync(string issueKey)
        {
            try
            {
                // Пытаемся получить все worklogs за один запрос с увеличенным лимитом
                var url = $"{_jiraUrl}/rest/api/2/issue/{issueKey}/worklog?maxResults=1000";

                var response = await _httpClient.GetAsync(url);

                if (response.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    // Если превысили лимиты - ждем и повторяем
                    await Task.Delay(2000);
                    response = await _httpClient.GetAsync(url);
                }

                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var worklogResponse = JsonSerializer.Deserialize<WorklogList>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return worklogResponse?.Worklogs ?? new List<JiraWorklog>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении worklogs для задачи {issueKey}: {ex.Message}");
                return new List<JiraWorklog>();
            }
        }

        private List<TimesheetRecord> ProcessWorklogs(List<JiraWorklog> worklogs, JiraIssue issue,
            DateTime weekStartDate, DateTime weekEndDate)
        {
            var records = new List<TimesheetRecord>();

            foreach (var worklog in worklogs)
            {
                try
                {
                    var worklogDate = DateTime.Parse(worklog.Started).Date;

                    if (worklogDate >= weekStartDate.Date && worklogDate < weekEndDate.Date)
                    {
                        var record = new TimesheetRecord
                        {
                            EmployeeName = worklog.Author.DisplayName,
                            Date = worklogDate,
                            TimeSpent = FormatTimeSpentToHour(worklog.TimeSpentSeconds),
                            TimeSpentInMinutes = FormatTimeSpentToMinutes(worklog.TimeSpentSeconds),
                            TaskKey = issue.Key,
                            TaskSummary = issue.Fields.Summary,
                            Comment = worklog.Comment ?? string.Empty,
                            TaskType = issue.Fields.IssueType.Name
                        };

                        records.Add(record);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка обработки worklog для задачи {issue.Key}: {ex.Message}");
                }
            }

            return records;
        }

        private string FormatTimeSpentToHour(int seconds)
        {
            var hours = seconds / 3600;
            var minutes = (seconds % 3600) / 60;

            return minutes == 0 ? $"{hours}h" : $"{hours}h {minutes}m";
        }

        private int FormatTimeSpentToMinutes(int seconds)
        {
            var minutes = seconds / 60;

            return minutes;
        }
    }
}