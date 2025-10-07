using FastEndpoints;
using FluentValidation;

namespace Skycamp.ApiService.Features.Users.V1.SyncUser;

public class SyncUserEndpointValidator : Validator<SyncUserEndpointRequest>
{
    public SyncUserEndpointValidator()
    {
        RuleFor(x => x.LoginProvider)
            .NotEmpty();

        RuleFor(x => x.ProviderKey)
            .NotEmpty();

        RuleFor(x => x.Email)
            .EmailAddress()
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.DisplayName)
            .MaximumLength(100)
            .When(x => !string.IsNullOrEmpty(x.DisplayName));

        RuleFor(x => x.AvatarUrl)
            .MaximumLength(256)
            .When(x => !string.IsNullOrEmpty(x.AvatarUrl));
    }
}
