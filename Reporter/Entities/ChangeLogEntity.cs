using Atlassian.Jira;

namespace Reporter.Entities;

public class ChangeLogEntity
{
    public DateTime? CreateDate { get; set; }

    public IssueParticipantEntity? Author { get; set; }

    public List<ChangeLogEntityItem> Items { get; set; } = new List<ChangeLogEntityItem>();

    public ChangeLogEntity(IssueChangeLog issueChangeLog)
    {
        CreateDate = issueChangeLog.CreatedDate;
        Author = new IssueParticipantEntity(issueChangeLog.Author);
        Items = issueChangeLog.Items
            .Select(x => new ChangeLogEntityItem(x))
            .ToList();
    }
}

public class ChangeLogEntityItem
{
    public ChangeLogEntityItem(IssueChangeLogItem issueChangeLogItem)
    {
        FieldName = issueChangeLogItem.FieldName;
        FromValue = issueChangeLogItem.FromValue;
        ToValue = issueChangeLogItem.ToValue;
    }

    public string FieldName { get; set; } = string.Empty;

    public string FromValue { get; set; } = string.Empty;

    public string ToValue { get; set; } = string.Empty;
}
