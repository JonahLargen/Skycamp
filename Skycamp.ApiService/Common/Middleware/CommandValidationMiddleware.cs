using FastEndpoints;
using FluentValidation;
using FluentValidation.Results;

namespace Skycamp.ApiService.Common.Middleware;

public class CommandValidationMiddleware<TCommand, TResult> : ICommandMiddleware<TCommand, TResult>
    where TCommand : ICommand<TResult>
{
    private readonly ILogger<CommandValidationMiddleware<TCommand, TResult>> _logger;
    private readonly IEnumerable<IValidator<TCommand>> _validators;

    public CommandValidationMiddleware(ILogger<CommandValidationMiddleware<TCommand, TResult>> logger, IEnumerable<IValidator<TCommand>> validators)
    {
        _logger = logger;
        _validators = validators;
    }

    public async Task<TResult> ExecuteAsync(TCommand command, CommandDelegate<TResult> next, CancellationToken ct)
    {
        if (_validators == null || !_validators.Any())
        {
            _logger.LogWarning("No validators found for command: {name}", command.GetType().Name);

            return await next();
        }

        _logger.LogInformation("Validating command: {name}", command.GetType().Name);

        var failures = new List<ValidationFailure>();

        foreach (var validator in _validators)
        {
            var validationResult = await validator.ValidateAsync(command, ct);

            if (!validationResult.IsValid)
            {
                failures.AddRange(validationResult.Errors);
            }
        }

        if (failures.Count != 0)
        {
            _logger.LogInformation("Validation failed for {name}: {@errors}", command.GetType().Name, failures);

            foreach (var failure in failures)
            {
                ValidationContext.Instance.AddError(failure);
            }

            ValidationContext.Instance.ThrowIfAnyErrors();
        }

        var result = await next();

        _logger.LogInformation("Validated Command: {@value}", command.GetType().Name);

        return result;
    }
}