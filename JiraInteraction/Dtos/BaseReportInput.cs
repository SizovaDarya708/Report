namespace JiraInteraction.Dtos;

public class BaseReportInput
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public string? AdditionalJiraFilter { get; set; }

    public string FilePath { get; set; } = string.Empty;

    public List<string> ProjectKeys { get; set; } = new List<string>();

    public string GetJql()
    {
        //Как вытягивать данные по задачам из старых спринтов, которые были обновлены позже спринта
        // если убрать фильтрацию по дате обновления или создания - данных будет слишком много, обработка станет медленной
        //Jira блокирует объемные и частые запросы
        var jql = $"project in ({String.Join(",", ProjectKeys)}) AND " +
            $"((updatedDate >= '{StartDate.AddDays(-3).ToString("yyyy-MM-dd")}' " +
            $"AND updatedDate < '{EndDate.AddDays(3).ToString("yyyy-MM-dd")}') OR " +
            $"(createdDate >= '{StartDate.AddDays(-7).ToString("yyyy-MM-dd")}' " +
            $"AND createdDate < '{EndDate.AddDays(1).ToString("yyyy-MM-dd")}'))" +
            $"AND status = 'Закрыт'";

        if (!string.IsNullOrEmpty(AdditionalJiraFilter))
        {
            jql += $" AND {AdditionalJiraFilter}";
        }
        return jql;
    }
}
