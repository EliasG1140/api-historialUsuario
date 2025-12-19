using FluentValidation;
using MediatR;

namespace Application.Behaviors;

public sealed class ValidationBehavior<TReq, TRes> : IPipelineBehavior<TReq, TRes> where TReq : notnull
{
    private readonly IEnumerable<IValidator<TReq>> _validators;
    public ValidationBehavior(IEnumerable<IValidator<TReq>> validators) => _validators = validators;

    public async Task<TRes> Handle(TReq request, RequestHandlerDelegate<TRes> next, CancellationToken ct)
    {
        if (!_validators.Any()) return await next();

        var context = new ValidationContext<TReq>(request);
        var failures = (await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, ct))))
                        .SelectMany(r => r.Errors)
                        .Where(f => f != null)
                        .ToList();

        if (failures.Count == 0) return await next();

        var error = Error.Validation(failures[0].ErrorMessage,
            string.Join(" | ", failures.Select(f => $"{f.PropertyName}: {f.ErrorMessage}")));

        if (typeof(TRes) == typeof(Result))
            return (TRes)(object)Result.Fail(error);

        if (typeof(TRes).IsGenericType && typeof(TRes).GetGenericTypeDefinition() == typeof(Result<>))
        {
            var failGeneric = typeof(Result<>).MakeGenericType(typeof(TRes).GetGenericArguments()[0])
                                .GetMethod("Fail", new[] { typeof(Error) })!;
            return (TRes)failGeneric.Invoke(null, new object[] { error })!;
        }

        throw new ValidationException(failures);
    }
}