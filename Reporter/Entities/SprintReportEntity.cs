using Atlassian.Jira;
using JiraInteraction.Dtos;
using Reporter.Extensions;

namespace Reporter.Entities;

public class SprintReportEntity
{
    public SprintReportEntity(DateTime startDate, DateTime endDate)
    {
        ReportPeriod = new ReportPeriodDto(startDate, endDate);
    }

    public void SetEstimateTimeData(List<EstimateDataDto> estimateData)
    {
        var estimateDict = estimateData
            .Where(x => x.EstimateTime != null)
            .GroupBy(x => x.Jiraidentifier)
            .ToDictionary(x => x.Key, v => v.Select(x => x.EstimateTime).First());

        if (estimateDict == null)
        {
            return;
        }

        WithoutSprintPool.SetEstimateData(estimateDict!);
        Parallel.ForEach(Sprints, sprint => sprint.SetEstimateData(estimateDict!));
    }

    public SprintEntity? GetSprintByIssueKey(string key)
    {
        return Sprints
            .Where(x => x.Issues.Any(y => y.Key == key))
            .FirstOrDefault();
    }

    public ReportPeriodDto ReportPeriod;

    public List<SprintEntity> Sprints { get; set; } = new List<SprintEntity>();

    public WithoutSprintPool WithoutSprintPool { get; set; } = new WithoutSprintPool();

    public List<WorklogEntity> Worklogs { get; set; } = new List<WorklogEntity>();

    public List<IssueParticipantEntity> IssueParticipants { get; set; } = new List<IssueParticipantEntity>();

    public string GetUserDepartmentByLogin(string login)
    {
        return IssueParticipants.Where(x => x.UserLogin == login).FirstOrDefault()?.Department ?? string.Empty;
    }

    public List<IssueEntity> GetAllIssues()
    {
        var sprintIssues = Sprints.SelectMany(x => x.Issues)
            .ToList();
        sprintIssues.AddRange(WithoutSprintPool.Issues);
        return sprintIssues;
    }

    public void SetParticipantDepartment(Dictionary<string, string> loginsPerDepartments)
    {
        foreach (var participant in IssueParticipants)
        {
            if (loginsPerDepartments.TryGetValue(participant.UserLogin, out var dep))
            {
                participant.Department = dep.Replace("БО_УНП_-_", "").Replace("_", " ");
            }
            else
            {
                participant.Department = "Сорудник не активен";
            }
        }
    }

    public void SetWorklogs()
    {
        var sprintWorkLogs = Sprints
            .SelectMany(x => x.Issues)
            .SelectMany(x => x.Workflows);
        var withoutSprintIssues = WithoutSprintPool.Issues
            .SelectMany(x => x.Workflows);

        Worklogs.AddRange(sprintWorkLogs);
        Worklogs.AddRange(withoutSprintIssues);
    }

    public void SetParticipants()
    {
        var sprintParticipants = Sprints
            .SelectMany(x => x.Issues)
            .SelectMany(x => x.Participants)
            .ToList();
        var withoutSprintParticipants = WithoutSprintPool.Issues
            .SelectMany(x => x.Participants);

        sprintParticipants.AddRange(withoutSprintParticipants);
        IssueParticipants = sprintParticipants.DistinctBy(x => x.UserLogin).ToList();
    }

    public async Task FillDataAsync(Issue[] JiraIssues)
    {
        await Parallel.ForEachAsync(JiraIssues, async (issue, ct) =>
        {
            var sprintName = issue.GetFieldValue(JiraConstants.SprintField);

            if (sprintName != null)
            {
                TryAddSprint(sprintName, out var sprint);
                await sprint!.AddIssue(issue, ReportPeriod);
                return;
            }
            await WithoutSprintPool.AddIssue(issue, ReportPeriod);

        });

        SetWorklogs();
        SetParticipants();
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
