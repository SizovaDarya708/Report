namespace Reporter.Entities;

public class ReportPeriodDto
{
    public ReportPeriodDto(DateTime startDate, DateTime endDate)
    {
        StartDate = startDate;
        EndDate = endDate;
    }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
