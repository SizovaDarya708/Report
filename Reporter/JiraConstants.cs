namespace Reporter;

public static class JiraConstants
{
    public const string StoryPointsField = "story points";
    public const string SprintField = "sprint";

    #region Статусы задач

    public const string Rework = "Доработка";
    public const string ToRework = "К доработке";
    public const string Review = "Ревью";
    public const string Analiz = "Анализ";
    public const string Estimate = "Оценка";
    public const string ToWork = "К разработке";
    public const string Work = "Разработка";
    public const string Ready = "Реализован";
    public const string TestingOnMaster = "Тестирование в мастере";
    public const string TestingOnBranch = "Тестирование на ветке";
    public const string QualityAssessment = "Оценка качества";
    public const string Closed = "Закрыт";

    #endregion

    #region Наименования полей
    public const string Status = "status";
    public const string TimeSpent = "timespent";
    public const string ReworkDescriptionField = "причина доработки";
    public const string ErrorReasonField = "Причина ошибки";
    public const string ErrorTypeField = "Тип ошибки";
    #endregion

    #region Отделы сотрудников
    public const string DeveloperDepartmentName = "отдел разработки";
    #endregion

    #region Типы задач
    public const string Bug = "Ошибка";
    public const string Incident = "Инцидент";
    #endregion

    #region Виды работ для списания
    public const string EstimateTypeWorkDevelop = "Разработка";
    public const string EstimateTypeRework = "Доработка";
    public const string EstimateTypeReview = "Ревью";
    public const string EstimateTypeAnaliz = "Анализ";
    public const string EstimateTypeTesting = "Тестирование";

    #endregion

    #region Labels
    public const string TechDutyLabel = "ТехДолг";
    #endregion

    #region Приоритет
    public const string Urgent = "Неотложный";

    public const string High = "Высокий";
    #endregion
}
