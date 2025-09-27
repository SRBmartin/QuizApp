using System.Reflection;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using QuizApp.Application.Common.Result;

namespace QuizApp.Application.Common.Behaviours;

public class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
        => _validators = validators;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);
        var results = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = results
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        if (failures.Count == 0)
            return await next();

        return BuildValidationFailureResponse<TResponse>(failures);
    }

    private static TOut BuildValidationFailureResponse<TOut>(IList<ValidationFailure> failures)
    {
        var msg = string.Join(" | ", failures.Select(f => $"{f.PropertyName}: {f.ErrorMessage}"));
        var error = new Error("validation.failed", msg);

        var t = typeof(TOut);

        if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Result<>))
        {
            var inner = t.GetGenericArguments()[0];
            var failureMethod = typeof(Result<>)
                .MakeGenericType(inner)
                .GetMethod("Failure", BindingFlags.Public | BindingFlags.Static, new[] { typeof(Error) });

            if (failureMethod is not null)
                return (TOut)failureMethod.Invoke(null, new object[] { error })!;
        }


        if (t == typeof(Result.Result))
        {
            return (TOut)(object)Result.Result.Failure(error);
        }

        throw new ValidationException(failures);
    }
}
