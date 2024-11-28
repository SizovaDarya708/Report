using Atlassian.Jira;
using System.Text.RegularExpressions;

namespace Reporter.Entities;

public class SprintEntity : IssuesBase
{
    public SprintEntity() { }

    public SprintEntity(string sprintName, Issue? issue = null)
    { 
        Name = sprintName;
        SetSprintInterval(sprintName);
    }

    //public bool IsCurrent { get; set; } = false;

    public string Name { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    private void SetSprintInterval(string sprintName)
    {
        var startDatePattern = @" ([0-9]{1;2}|.)*-";
        Regex startDate = new Regex(startDatePattern);
        var startDateValue = startDate.Match(sprintName);

        StartDate = startDateValue != null && DateTime.TryParse(startDateValue.Value.Replace("-", "").Replace(" ", ""), out var start) ? start : null;

        var endDatePattern = @"-([0-9]{1;2}|.)*";
        Regex endDate = new Regex(endDatePattern);
        var endDateValue = endDate.Match(sprintName);

        EndDate = endDateValue != null && DateTime.TryParse(endDateValue.Value.Replace("-", ""), out var end) ? end : null;
    }
}
