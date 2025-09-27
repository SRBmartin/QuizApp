using FluentValidation;

namespace QuizApp.Application.Features.Quizes.GetById;

public class GetQuizByIdQueryValidator : AbstractValidator<GetQuizByIdQuery>
{
    public GetQuizByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }

}
