using FastEndpoints;
using JetBrains.Annotations;

namespace Skycamp.ApiService.Common.Endpoints;

/// <summary>
/// use this base class for defining endpoints that use request dtos that map to a command and return nothing.
/// </summary>
/// <typeparam name="TRequest">the type of the request dto</typeparam>
/// <typeparam name="TCommand">the type of the command</typeparam>
[UsedImplicitly(ImplicitUseTargetFlags.WithInheritors)]
public abstract class EndpointWithoutResponseWithCommandMapping<TRequest, TCommand> : Endpoint<TRequest>
    where TRequest : notnull
    where TCommand : ICommand
{
    /// <summary>
    /// place the logic for mapping the request dto to the desired command
    /// </summary>
    /// <param name="r">the request dto</param>
    public abstract TCommand MapToCommand(TRequest r);

    /// <summary>
    /// send a response by mapping the supplied request to a command, executing the command, and mapping the result to a response
    /// </summary>
    /// <param name="request">the request to send</param>
    /// <param name="statusCode">the status code to send</param>
    /// <param name="ct">optional cancellation token</param>
    protected async Task SendMappedAsync(TRequest request, int statusCode = 200, CancellationToken ct = default)
    {
        var command = MapToCommand(request);

        await command.ExecuteAsync(ct);
        await Send.ResultAsync(Results.StatusCode(statusCode));
    }
}