using Reporter.Entities;

namespace Reporter.WorkSheetsHandlers.KpiReport.Dtos;

public class ReworkInfoDto
{
    public ReworkInfoDto(IssueEntity issue, IssueParticipantEntity participant)
    {
        Issue = issue;

        //Проверить, были ли доработки по задаче в принципе
        var rework = issue.GetReworkInfo();
        if (rework == null || rework.CountOfRework == 0)
        {
            return;
        }

        //Проверить наличие доработки у конкретного участника

        //Кейс 1: Сотрудник сам разрабатывал задачу и у нее есть доработка

        var workChanelogs = issue.GetDevelopersChangeLogs();
        var participantIsDeveloper = workChanelogs.Where(x => x.Author!.Name == participant.Name).Any();
        if (participantIsDeveloper)
        {
            HasReworks = true;
            return;
        }

        //Кейс 2: Сотрудник дорабатывал задачу и после его работы существуют переходы в доработку
        var reworkChangelogs = issue.GetReworksChangeLogs();
        var participantRework = reworkChangelogs
            .Where(x => x.Author!.Name == participant.Name)
            .OrderBy(x => x.CreateDate)
            .FirstOrDefault();
        if (participantRework != null)
        {
            var reworksAfterParticipantReworks = reworkChangelogs.Where(x => x.CreateDate > participantRework.CreateDate);
            HasReworks = reworksAfterParticipantReworks.Any();
            return;
        }

        HasReworks = false;    
    }

    public IssueEntity Issue { get; set; }

    public bool HasReworks { get; set; } = false;
}
