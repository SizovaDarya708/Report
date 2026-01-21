using Atlassian.Jira;

namespace Reporter.Extensions;

public static class JiraIssueExtensions
{
    public static string? GetFieldValue(this Issue issue, string fieldName)
    {
        var field = issue.CustomFields
            .FirstOrDefault(x => x.Name.ToLower().Contains(fieldName.ToLower()));

        if (field == null)
        {
            //тут может надо exception кидать, но пока так
            return null;
        }

        return field.Values.LastOrDefault();
    }
}
