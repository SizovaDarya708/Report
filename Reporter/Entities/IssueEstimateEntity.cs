using JiraInteraction.Dtos;

namespace Reporter.Entities;

public class IssueEstimateEntity
{
    public IssueEstimateEntity(EstimateByWorklogTypeDto estimate, WorklogEntity worklog)
    {
        WorkEstimateType = WorkEstimateExtensions.GetWorkEstimateType(estimate.WorklogType);
        Worklog = worklog;
    }

    public WorkEstimateTypeEnum? WorkEstimateType { get; set; }

    public WorklogEntity Worklog { get; set; }
}
