using Atlassian.Jira;

namespace Reporter.Entities;

public class SprintEntity : IssuesBase
{
    public SprintEntity() { }

    public SprintEntity(string sprintName, Issue? issue = null)
    { 
        Name = sprintName;
        //тут какой-нибудь TryParseDateStartEndByName
        //По DateStart and DateEnd calc IsCurrent
        
    }
    public bool IsCurrent { get; set; } = false;

    public string Name { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }
}
