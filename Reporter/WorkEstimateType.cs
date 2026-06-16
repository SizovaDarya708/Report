namespace Reporter;

/// <summary>
/// Вид работ для списаний
/// </summary>
public enum WorkEstimateTypeEnum
{
    Develop = 0,
    
    Rework = 1,

    Analys = 2,

    Testing = 3,

    Review = 4,
}

public static class WorkEstimateExtensions
{
    public static WorkEstimateTypeEnum? GetWorkEstimateType(string workEstimates)
    {
        if (workEstimates == JiraConstants.EstimateTypeWorkDevelop)
        {
            return WorkEstimateTypeEnum.Develop;
        }
        else if (workEstimates == JiraConstants.EstimateTypeRework)
        {
            return WorkEstimateTypeEnum.Rework;
        }
        else if (workEstimates == JiraConstants.EstimateTypeAnaliz)
        {
            return WorkEstimateTypeEnum.Analys;
        }
        else if (workEstimates == JiraConstants.EstimateTypeReview)
        {
            return WorkEstimateTypeEnum.Review;
        }
        return null;
    }

}
