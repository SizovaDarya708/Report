using Atlassian.Jira;
using Reporter.Extensions;

namespace Reporter.Entities;

public class SprintReportEntity
{
    public List<SprintEntity> Sprints { get; set; } = new List<SprintEntity>();

    public WithoutSprintPool WithoutSprintPool { get; set; } = new WithoutSprintPool();

    public async Task FillDataAsync(Issue[] JiraIssues)
    {
        foreach (var issue in JiraIssues)
        {
            var sprintName = issue.GetFieldValue(JiraConstants.SprintField);

            if (sprintName == null)
            {
                await WithoutSprintPool.AddIssue(issue);
                continue;
            }

            TryAddSprint(sprintName, out var sprint);
            await sprint!.AddIssue(issue);
        }
    }

    public void TryAddSprint(string sprintName, out SprintEntity? sprint)
    {
        sprint = Sprints.FirstOrDefault(sp => sp.Name == sprintName);
        if (sprint == null)
        {
            sprint = new SprintEntity(sprintName);
            Sprints.Add(sprint);
        }
    }
}
