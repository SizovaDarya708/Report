namespace JiraInteraction.Dtos;

public class EstimateByWorklogTypeDto
{
    public EstimateByWorklogTypeDto(string worklogId, string worklogType)
    {
        WorklogId = worklogId;
        WorklogType = worklogType;
    }
    public string WorklogId { get; set; }

    public string WorklogType { get; set; }
}
