using Atlassian.Jira;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reporter;

public static class JiraIssueExtensions
{
    public static string? GetFieldValue(this Issue issue, string fieldName)
    {
        var field =  issue.CustomFields
            .FirstOrDefault(x => x.Name.ToLower().Contains(fieldName));

        if (field == null)
        {
            //тут может надо exception кидать, но пока так
            return null;
        }

        return field.Values.LastOrDefault();
    }

    //public static Tuple<DateTime, DateTime> TryParseSprintDates(this string)
    //{
        
    
    //}

    public static string? GetLastSprint(this Issue issue)
    {
        var sprint = issue.CustomFields
            .FirstOrDefault(x => x.Name.ToLower().Contains("sprint") || x.Name.Contains("-"));

        if (sprint == null)
        {
            //тут может надо exception кидать, но пока так
            return null;
        }

        return sprint.Values.LastOrDefault();
    }
}
