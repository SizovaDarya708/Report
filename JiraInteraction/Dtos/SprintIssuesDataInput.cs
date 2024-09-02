namespace JiraInteraction.Dtos;

public class SprintIssuesDataInput
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public string? AdditionalJiraFilter { get; set; }

    public string FilePath { get; set; } = string.Empty;

    public string GetJql()
    {
        var jql = $"updatedDate >= '{StartDate.ToString("yyyy-MM-dd HH:mm")}' " +
            $"AND created <= '{EndDate.ToString("yyyy-MM-dd HH:mm")}'";
        
        if (!string.IsNullOrEmpty(AdditionalJiraFilter))
        {
            jql += $" AND {AdditionalJiraFilter}";
        }
        return jql;
    }
}
