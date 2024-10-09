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
    
    public async Task<Issue[]> GetIssuesForReportAsync(SprintIssuesDataInput input, CancellationToken cancellationToken)
    {
        var jql = $"project = \"Планы по достижению показателей ГП РФ\" AND " + input.GetJql();

        var issuePages = await client.Issues.GetIssuesFromJqlAsync(jql, 1000, 0, cancellationToken);

        var issues = issuePages.ToArray();
        return issues;
    }

    public async Task<Dictionary<string, string>> GetUsersDataAsync(string[] userLogins, CancellationToken cancellationToken)
    {
        //var userGroup = userLogins.AsParallel().Select(async name => await client.RestClient.RestSharpClient.ExecuteGetAsync(
        //    new RestRequest($"/rest/api/2/user?username={name}&expand=groups&expand=applicationRoles", Method.GET),
        //     cancellationToken));

        var dict = new Dictionary<string, string>();

        foreach (var login in userLogins)
        {
            var info = await client.RestClient.RestSharpClient.ExecuteGetAsync(
            new RestRequest($"/rest/api/2/user?username={login}&expand=groups&expand=applicationRoles", Method.GET),
             cancellationToken);
            if (info.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var pattern = @"(Отдел_|БО_УНП_-)[\w]{0,25}";
                Regex rg = new Regex(pattern);
                var department = rg.Match(info.Content);
                dict.TryAdd(login, department.Value);
            }
        }
        return dict;
    }
}
