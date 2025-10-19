using FastEndpoints;
using JetBrains.Annotations;

namespace Skycamp.ApiService.Common.Endpoints;

/// <summary>
/// use this base class for defining endpoints that do not use request dtos, but still that map to a command and domain entity dtos that map to a response dto.
/// </summary>
/// <typeparam name="TRequest">the type of the request dto</typeparam>
/// <typeparam name="TResponse">the type of the response dto</typeparam>
/// <typeparam name="TCommand">the type of the command</typeparam>
/// <typeparam name="TEntity">the type of domain entity that will be mapped to/from</typeparam>
[UsedImplicitly(ImplicitUseTargetFlags.WithInheritors)]
public abstract class EndpointWithoutRequestWithCommandMapping<TResponse, TCommand, TEntity> : EndpointWithoutRequest<TResponse>
    where TCommand : ICommand<TEntity>
{
    /// <summary>
    /// place the logic for mapping to the desired command
    /// </summary>
    /// <param name="r">the request dto</param>
    public abstract TCommand MapToCommand();

    /// <summary>
    /// place the logic for mapping a domain entity to a response dto
    /// </summary>
    /// <param name="e">the domain entity to map from</param>
    public abstract TResponse MapFromEntity(TEntity e);

    /// <summary>
    /// send a response by mapping to a command, executing the command, and mapping the result to a response
    /// </summary>
    /// <param name="request">the request to send</param>
    /// <param name="statusCode">the status code to send</param>
    /// <param name="ct">optional cancellation token</param>
    protected async Task SendMappedAsync(int statusCode = 200, CancellationToken ct = default)
    {
        var command = MapToCommand();
        var result = await command.ExecuteAsync(ct);
        var response = MapFromEntity(result);

        await Send.ResponseAsync(response, statusCode, ct);
    }
}