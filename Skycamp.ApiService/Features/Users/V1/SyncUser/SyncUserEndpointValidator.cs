using FastEndpoints;
using FluentValidation;

namespace Skycamp.ApiService.Features.Users.V1.SyncUser;

public class SyncUserEndpointValidator : Validator<SyncUserEndpointRequest>
{
    public SyncUserEndpointValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}
