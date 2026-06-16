using Reporter.Entities;

namespace Reporter.Dtos;

public class IssueReworkDto
{
    public IssueReworkDto() { }

    public IssueReworkDto(IEnumerable<ChangeLogEntity> changeLogs, IEnumerable<WorklogEntity> workflows, IEnumerable<IssueEstimateEntity> issueEstimate)
    {
        //найти changeLog, где есть нужные переводы и возможно рядом в item есть списания
        //пока по логике такой, что когда переводят в ревью со статуса Доработка
        //- возможно тогда и списывают (я хз как формируется changeLog)
        var reworkChanges = changeLogs
            .Where(x => x.Items.Any(i =>
            i.FieldName.ToLower() == JiraConstants.Status.ToLower()
            && i.FromValue.ToLower() == JiraConstants.Rework.ToLower()
            && i.ToValue.ToLower() == JiraConstants.Review.ToLower()));


        //группируем по автору - считаем сколько у автора статусов доработка по задаче
        ReworksPerParticipantList = reworkChanges.Any(c => c.Author != null)
            ?
            reworkChanges
            .Where(c => c.Author != null)
            .GroupBy(c => c.Author!.Name)
            .ToDictionary(k => k.First().Author!, v => v.ToList(), new IssueParticipantEntityComparer())
            : new Dictionary<IssueParticipantEntity, List<ChangeLogEntity>>(new IssueParticipantEntityComparer()) { };

        CountOfRework = reworkChanges.Count();
        TimeSpendInSeconds = issueEstimate
            .Where(es => es.WorkEstimateType == WorkEstimateTypeEnum.Rework)
            .Sum(es => es.Worklog.TimeSpendInSeconds);
    }
        
    /// <summary>
    /// Количество переработок по сотруднику
    /// </summary>
    public Dictionary<IssueParticipantEntity, List<ChangeLogEntity>> ReworksPerParticipantList { get; set; } =
        new Dictionary<IssueParticipantEntity, List<ChangeLogEntity>>(new IssueParticipantEntityComparer()) { };

    public long? TimeSpendInSeconds { get; set; } = 0;

    public int CountOfRework { get; set; } = 0;
}
