using FluentValidation;

namespace Skycamp.ApiService.Features.Users.Shared.SyncUser;

public class SyncUserCommandValidator : AbstractValidator<SyncUserCommand>
{
    public SyncUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}