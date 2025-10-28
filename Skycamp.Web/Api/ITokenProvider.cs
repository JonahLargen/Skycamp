namespace Skycamp.Web.Api;

public interface ITokenProvider
{
    Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default);
}
