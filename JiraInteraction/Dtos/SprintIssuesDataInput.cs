namespace JiraInteraction.Dtos;

public class SprintIssuesDataInput
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public string? AdditionalJiraFilter { get; set; }

    public string FilePath { get; set; } = string.Empty;

    public string GetJql()
    {
        //Как вытягивать данные по задачам из старых спринтов, которые были обновлены позже спринта
        // если убрать фильтрацию по дате обновления или создания - данных будет слишком много, обработка станет медленной
        //Jira блокирует объемные и частые запросы
        var jql = $"((updatedDate >= '{StartDate.AddDays(-3).ToString("yyyy-MM-dd")}' " +
            $"AND updatedDate < '{EndDate.AddDays(3).ToString("yyyy-MM-dd")}') OR " +
            $"(createdDate >= '{StartDate.AddDays(-7).ToString("yyyy-MM-dd")}' " +
            $"AND createdDate < '{EndDate.AddDays(1).ToString("yyyy-MM-dd")}'))";

        if (!string.IsNullOrEmpty(AdditionalJiraFilter))
        {
            jql += $" AND {AdditionalJiraFilter}";
        }
        return jql;
    }
}
