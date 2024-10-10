using Atlassian.Jira;
using Reporter.Extensions;
using static Microsoft.IO.RecyclableMemoryStreamManager;

namespace Reporter.Entities;

public class SprintReportEntity
{
    public SprintReportEntity(DateTime startDate, DateTime endDate)
    { 
        StartDate = startDate;
        EndDate = endDate;
    }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public List<SprintEntity> Sprints { get; set; } = new List<SprintEntity>();

    public WithoutSprintPool WithoutSprintPool { get; set; } = new WithoutSprintPool();

    public List<WorklogEntity> Worklogs { get; set; } = new List<WorklogEntity>();

    public List<IssueParticipantEntity> IssueParticipants { get; set; } = new List<IssueParticipantEntity>();

    public void SetParticipantDepartment(Dictionary<string, string> loginsPerDepartments)
    {
        foreach (var participant in IssueParticipants)
        {
            if (loginsPerDepartments.TryGetValue(participant.UserLogin, out var dep))
            {
                participant.Department = dep;
            }
        
        }
    
    }

    public void SetWorklogs()
    {
        var sprintWorkLogs = Sprints
            .SelectMany(x => x.Issues)
            .SelectMany(x => x.Workflows)
            .Where(x => x.UpdateDate >= StartDate && x.UpdateDate <= EndDate);
        var withoutSprintIssues = WithoutSprintPool.Issues
            .SelectMany(x => x.Workflows)
            .Where(x => x.UpdateDate >= StartDate && x.UpdateDate <= EndDate);

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
                await sprint!.AddIssue(issue);
            }
            await WithoutSprintPool.AddIssue(issue);
            
        });
        //    foreach (var issue in JiraIssues)
        //{
        //    var sprintName = issue.GetFieldValue(JiraConstants.SprintField);

        //    if (sprintName == null)
        //    {
        //        await WithoutSprintPool.AddIssue(issue);
        //        continue;
        //    }

        //    TryAddSprint(sprintName, out var sprint);
        //    await sprint!.AddIssue(issue);
        //}

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
