namespace Reporter.Dtos;

public class IssueApiRequestDto
{
    public IssueApiRequestDto(string key, DateTime date)
    {
        Key = key;
        CreatedDate = date;
    }
    public string Key { get; set; }

    public DateTime CreatedDate { get; set; }
}
