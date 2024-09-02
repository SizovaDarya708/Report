using Atlassian.Jira;
using JiraInteraction.Dtos;

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
}
