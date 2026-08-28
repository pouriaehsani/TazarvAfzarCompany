namespace Company.Application.Exceptions;

public class ValidationException : Exception
{
    public ValidationException(IEnumerable<FluentValidation.Results.ValidationFailure> errors)
        : base("One or more validation errors occurred.")
    {
        Errors = errors;
    }

    public IEnumerable<FluentValidation.Results.ValidationFailure> Errors { get; }
}