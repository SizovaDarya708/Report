using Atlassian.Jira;

namespace Reporter.Entities;

public class SprintReportEntity
{
    public List<SprintEntity> Sprints { get; set; } = new List<SprintEntity>();

    public List<IssueEntity> IssuesWithoutSprint { get; set; } = new List<IssueEntity>();

    public async Task FillDataAsync(Issue[] JiraIssues)
    {
        foreach (var issue in JiraIssues)
        {
            var sprintName = issue.GetFieldValue(JiraConstants.SprintField);

            if (sprintName == null)
            {
                IssuesWithoutSprint.Add(new IssueEntity(issue));
                continue;
            }

            TryAddSprint(sprintName, out var sprint);
            sprint.AddIssue(issue);
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
