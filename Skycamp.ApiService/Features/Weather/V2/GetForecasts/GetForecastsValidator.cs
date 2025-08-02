using FastEndpoints;
using FluentValidation;

namespace Skycamp.ApiService.Features.Weather.V2.GetForecasts;

public class GetForecastsValidator : Validator<GetForecastsRequest>
{
    public GetForecastsValidator()
    {
        RuleFor(x => x.City).NotEmpty().WithMessage("City is required.");
        RuleFor(x => x.Days).GreaterThanOrEqualTo(1).LessThanOrEqualTo(7)
            .When(x => x.Days.HasValue)
            .WithMessage("Days must be between 1 and 7.");
    }
}