using Reporter.Entities;

namespace Reporter.Dtos;

public class IssueReworkDto
{
    public IssueReworkDto() { }

    public IssueReworkDto(IEnumerable<ChangeLogEntity> changeLogs, IEnumerable<WorklogEntity> workflows)
    {
        //найти changeLog, где есть нужные переводы и возможно рядом в item есть списания
        //пока по логике такой, что когда переводят в ревью - возможно тогда и списывают (я хз как формируется changeLog)
        var reworkChanges = changeLogs
            .Where(x => x.Items.Any(i => 
            i.FieldName.ToLower() == JiraConstants.Status.ToLower()
            && i.FromValue.ToLower() == JiraConstants.Rework.ToLower()
            && i.ToValue.ToLower() == JiraConstants.Review.ToLower()))
            .SelectMany(x => x.Items)
            //.Where(x => x.FieldName.ToLower() == JiraConstants.Status)
            //.Where(x => x.FromValue.ToLower() == JiraConstants.ToRework.ToLower()
            //    && x.ToValue.ToLower() == JiraConstants.Rework.ToLower())
            .ToArray();

        var statusChangedChangeLogs = reworkChanges.Where(i =>
            i.FieldName.ToLower() == JiraConstants.Status.ToLower()
            && i.FromValue.ToLower() == JiraConstants.Rework.ToLower()
            && i.ToValue.ToLower() == JiraConstants.Review.ToLower())
            .ToArray();

        var timeSpent = reworkChanges.Where(i => i.FieldName.ToLower() == JiraConstants.TimeSpent.ToLower()).ToArray();
        CountOfRework = statusChangedChangeLogs.Count();
    }

    public long TimeSpendInSeconds { get; set; } = 0;

    public int CountOfRework { get; set; } = 0;
}
