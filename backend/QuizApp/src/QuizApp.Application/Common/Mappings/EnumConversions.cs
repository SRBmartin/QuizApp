namespace QuizApp.Application.Common.Mappings;

public static class EnumConversions
{
    public static Domain.Enums.QuestionType ToDomain(this DTOs.Questions.Emums.QuestionType type)
    {
        return type switch
        {
            DTOs.Questions.Emums.QuestionType.Single => Domain.Enums.QuestionType.Single,
            DTOs.Questions.Emums.QuestionType.Multi => Domain.Enums.QuestionType.Multi,
            DTOs.Questions.Emums.QuestionType.TrueFalse => Domain.Enums.QuestionType.TrueFalse,
            DTOs.Questions.Emums.QuestionType.FillIn => Domain.Enums.QuestionType.FillIn,
            _ => throw new InvalidDataException("Type of question provided was not found.")
        };
    }
}
