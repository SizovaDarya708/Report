namespace JiraInteraction.Dtos;

public class SprintIssuesDataInput
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public string? AdditionalJiraFilter { get; set; }

    public string FilePath { get; set; } = string.Empty;

    public string GetJql()
    {
        var jql = $"((updatedDate >= '{StartDate.ToString("yyyy-MM-dd")}' " +
            $"AND updatedDate < '{EndDate.AddDays(1).ToString("yyyy-MM-dd")}') OR " +
            $"(createdDate >= '{StartDate.AddDays(-7).ToString("yyyy-MM-dd")}' " +
            $"AND createdDate < '{EndDate.AddDays(1).ToString("yyyy-MM-dd")}'))";

        if (!string.IsNullOrEmpty(AdditionalJiraFilter))
        {
            jql += $" AND {AdditionalJiraFilter}";
        }
        return jql;
    }
}
