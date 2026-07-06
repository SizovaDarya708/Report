namespace JiraInteraction.Dtos;

public class BaseReportInput
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public string? AdditionalJiraFilter { get; set; }

    public string FilePath { get; set; } = string.Empty;

    public List<string> ProjectKeys { get; set; } = new List<string>();

    public bool OnlyClosedIssues { get; set; } = true;
    

    public string GetJql()
    {
        var _endDate = new  DateTime(
            EndDate.Year, EndDate.Month, EndDate.Day).AddDays(1);
        var _startDate = new DateTime(
            StartDate.Year, StartDate.Month, StartDate.Day).AddDays(-1);
        
        //Как вытягивать данные по задачам из старых спринтов, которые были обновлены позже спринта
        // если убрать фильтрацию по дате обновления или создания - данных будет слишком много, обработка станет медленной
        //Jira блокирует объемные и частые запросы
        var jql = $"project in ({String.Join(",", ProjectKeys)}) AND " +
            $"status CHANGED TO \"Закрыт\" AFTER \"{_startDate.ToString("yyyy-MM-dd")}\" BEFORE \"{_endDate.ToString("yyyy-MM-dd")}\"";

        if (OnlyClosedIssues)
        {
            //jql+= $"AND status = 'Закрыт'";
        }

        if (!string.IsNullOrEmpty(AdditionalJiraFilter))
        {
            jql += $" AND {AdditionalJiraFilter}";
        }
        return jql;
    }
}
