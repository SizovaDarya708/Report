﻿using Atlassian.Jira;
using Reporter.Dtos;
using Reporter.Extensions;

namespace Reporter.Entities;

public class IssueEntity
{
    public IssueEntity() { }

    public IssueEntity(Issue issue)
    {
        Key = issue.Key.Value;
        Title = issue.Summary;
        Description = issue.Description;
        //AuthorName = ;
        Status = issue.Status.Name;
        Type = issue.Type.Name;
        StoryPoints = Int32.TryParse(issue.GetFieldValue(JiraConstants.StoryPointsField), out var points) ? points : null;
        Priority = issue.Priority.Name;
        TimeSpentInHours = (int)(issue.TimeTrackingData?.TimeSpentInSeconds / 3600 ?? 0);
        LastAssignee = issue.AssigneeUser != null ? new IssueParticipantEntity(issue.AssigneeUser) : null;
    }

    public void SetWorkflows(IEnumerable<Worklog> workflows)
    {
        var newWorklogs = workflows
            .Select(w => new WorklogEntity(w));
        Workflows.AddRange(newWorklogs);
    }

    public void SetChangeLogs(IEnumerable<IssueChangeLog> issueChaneLogs)
    {
        var newChangeLogs = issueChaneLogs
            .Select(w => new ChangeLogEntity(w));
        ChangeLogs.AddRange(newChangeLogs);
    }

    public string Key { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? AuthorName { get; set; }

    public string Status { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public IssueParticipantEntity? LastAssignee { get; set; }

    public int TimeSpentInHours { get; set; }

    public int? StoryPoints { get; set; }

    public string? Priority { get; set; }

    //Все участники задачи (автор и исполнители)
    public List<IssueParticipantEntity> Participants { get; set; } = new List<IssueParticipantEntity>();

    public IssueParticipantEntity? GetParticipantByType(EmployeeType employeeType)
    {
        return Participants.Where(x => x.EmployeeType == employeeType).FirstOrDefault();
    }

    public void SetParticipants()
    {
        var statusChangedLogs = ChangeLogs
            .Where(x => x.Items.Any(i => i.FieldName.ToLower() == JiraConstants.Status.ToLower()));
        //Найти кто переводил задачку в Оценку - считать этого участника Аналитиком
        var analyst = statusChangedLogs
            .Where(x => x.Items.Any(i => i.FromValue?.ToLower() == JiraConstants.Analiz.ToLower() && i.ToValue?.ToLower() == JiraConstants.Estimate.ToLower()))
            .Select(x => x.Author)
            .FirstOrDefault();
        SetParticipantsByType(EmployeeType.Analyst, analyst);

        //Найти кто переводил задачку в Разработку - считать этого участника Разработчиком
        var developer  = statusChangedLogs
            .Where(x => x.Items.Any(i => i.FromValue?.ToLower() == JiraConstants.ToWork.ToLower() && i.ToValue?.ToLower() == JiraConstants.Work.ToLower()))
            .Select(x => x.Author)
            .FirstOrDefault();
        SetParticipantsByType(EmployeeType.Developer, developer);

        //Найти кто переводил задачку в оценку качества - считать этого участника Тестировщиком ????
        var tester = statusChangedLogs
            .Where(x => x.Items.Any(i => (i.FromValue?.ToLower() == JiraConstants.TestingOnBranch.ToLower() || i.FromValue?.ToLower() == JiraConstants.TestingOnMaster.ToLower())
            && i.ToValue?.ToLower() == JiraConstants.QualityAssessment.ToLower()))
            .Select(x => x.Author)
            .FirstOrDefault();
        SetParticipantsByType(EmployeeType.Tester, tester);
    }

    private void SetParticipantsByType(EmployeeType employeeType, IssueParticipantEntity? participant)
    {
        if (participant == null)
        {
            return;
        }
        participant.EmployeeType = employeeType;
        Participants.Add(participant);
    }

    /// <summary> Содержатся данные о записях о работе </summary>
    public List<WorklogEntity> Workflows { get; set; } = new List<WorklogEntity>();

    /// <summary> Содержатся данные о переходах из статуса в статус </summary>
    public List<ChangeLogEntity> ChangeLogs { get; set; } = new List<ChangeLogEntity>();

    public IssueReworkDto GetReworkInfo()
    {
        var reworkInfo = new IssueReworkDto(ChangeLogs, Workflows);
        return reworkInfo;
    }
}
