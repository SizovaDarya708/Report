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
        //client = Jira.CreateRestClient(uri, initData.JiraLogin, initData.JiraPassword);
        client = Jira.CreateRestClient(uri, "zabegaev", "1q2w3eSD");
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
    
    public async Task<Issue[]> GetIssuesForReportAsync(SprintIssuesDataInput input, CancellationToken cancellationToken)
    {
        var jql = $"project = \"Планы по достижению показателей ГП РФ\" AND " + input.GetJql();

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

    public async Task<List<EstimateDataDto>> GetEstimateDataPerIssuesAsync(string[] jiraIdentifiers, CancellationToken cancellationToken)
    {
        var estimateData = new List<EstimateDataDto>();

        var remaningTimePattern = @"remaining time"">([0-9]*h [0-9]*m|[0-9]*h)";
        Regex rt = new Regex(remaningTimePattern);

        var estimateTimePattern = @"estimate"">([0-9]*h [0-9]*m|[0-9]*h)";
        Regex et = new Regex(estimateTimePattern);

        await Parallel.ForEachAsync(jiraIdentifiers, async (identifier, ct) =>
        {
            var info = await client.RestClient.RestSharpClient.ExecuteGetAsync(
            new RestRequest($"/secure/BarsEstimateIssue!default.jspa?id={identifier}", Method.GET), ct);
            if (info.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var infoContent = info.Content;
                var remainingTime = rt.Match(infoContent);
                var remainingTimeStr = remainingTime?.Value;

                var estimateTime = et.Match(infoContent);
                var estimateTimeStr = estimateTime?.Value;

                estimateData.Add(new EstimateDataDto(identifier, estimateTimeStr, remainingTimeStr));
            }
        });

        return estimateData;
    }
}
