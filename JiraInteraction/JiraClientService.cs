using Atlassian.Jira;
using JiraInteraction.Dtos;
using RestSharp;
using System.Text.RegularExpressions;

namespace JiraInteraction;

public class JiraClientService : IJiraService
{
    public JiraClientService(JiraClientInitData initData)
    {
        var uri = "https://jira.bars.group";
        client = Jira.CreateRestClient(uri, initData.JiraLogin, initData.JiraPassword);
        client.Issues.MaxIssuesPerRequest = 3000; 
    }

    public async Task<bool> CheckClientConnection(CancellationToken cancellationToken)
    {
        //В библиотеке нет проверки успешного соединения, поэтому так:
        if (client == null)
        {
            return false;
        }
        var testIssue = await client.Issues.GetIssueAsync("GPREGION-808", cancellationToken);
        return testIssue != null;
    }

    private Jira client;
    
    public async Task<Issue[]> GetIssuesForReportAsync(BaseReportInput input, CancellationToken cancellationToken)
    {
        var jql = input.GetJql();

        var issuePages = await client.Issues.GetIssuesFromJqlAsync(jql, 1000, 0, cancellationToken);

        var issues = issuePages.ToArray();
        return issues;
    }

    public async Task<Dictionary<string, string>> GetUsersDepartmentAsync(string[] userLogins, CancellationToken cancellationToken)
    {
        var dict = new Dictionary<string, string>();

        var pattern = @"(Отдел_|БО_УНП_-)[\w]{0,25}";
        Regex rg = new Regex(pattern);

        await Parallel.ForEachAsync(userLogins, async (login, ct) => 
        {
            var info = await client.RestClient.RestSharpClient.ExecuteGetAsync(
            new RestRequest($"/rest/api/2/user?username={login}&expand=groups&expand=applicationRoles", Method.GET), ct);
            if (info.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var department = rg.Match(info.Content);
                dict.TryAdd(login, department.Value);
            }
        });

        return dict;
    }

    public async Task<Dictionary<string, List<EstimateByWorklogTypeDto>>> GetEstimateDataPerIssuesAsync(IssueApiRequestDto[] jiraKeys, CancellationToken cancellationToken)
    {
        var estimatesPerIssue = new Dictionary<string, List<EstimateByWorklogTypeDto>>();

        var estimateByWorklogTypePattern = @"<select\s+style=""display:none;""[\s\S]*?<\/option>";
        Regex worklogEstimateDataRg = new Regex(estimateByWorklogTypePattern);

        var workTypePattern = @"<option\s+value\s*=\s*""([^""]*)""";
        Regex wkTypeRg = new Regex(workTypePattern);

        var wklogIdPattern = "data-worklog\\s*=\\s*[\"'](\\d+)[\"']";
        var wkIdRg = new Regex(wklogIdPattern);

        Console.WriteLine($"Start parallel foreach issue");
        await Parallel.ForEachAsync(jiraKeys, async (issue, ct) =>
        {
            Console.WriteLine($"{issue.Key}  in thread {Thread.CurrentThread.ManagedThreadId}");

            var info = await client.RestClient.RestSharpClient.ExecuteGetAsync(
            new RestRequest($"/secure/TempoIssueBoard!report.jspa?v=1&issue={issue.Key}&show_worklog_attribute:_Тип_=true" +
            $"&periodType=FLEX&periodView=DATES&from={issue.CreatedDate.ToString("yyyy-MM-dd")}&to={DateTime.Now.ToString("yyyy-MM-dd")}",
            Method.GET));
            
            if (info.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var estimates = new List<EstimateByWorklogTypeDto>();

                var infoContent = info.Content;
                var worklogDataStrMatches = worklogEstimateDataRg.Matches(infoContent);
                var worklogDataStrCollection = worklogDataStrMatches;

                foreach (var worklog in worklogDataStrCollection)
                {
                    var workType = wkTypeRg.Match(worklog.ToString());
                    var workTypeStr = workType.Groups.Values.Last().Value;

                    var worklogId = wkIdRg.Match(worklog.ToString());
                    var wkIdStr = worklogId.Groups.Values.Last().Value;

                    estimates.Add(new EstimateByWorklogTypeDto(wkIdStr, workTypeStr));
                }
                estimatesPerIssue.TryAdd(issue.Key, estimates);
            }            
        });

        return estimatesPerIssue;
    }
}
