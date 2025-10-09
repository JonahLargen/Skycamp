using FastEndpoints;
using FluentValidation;

namespace Skycamp.ApiService.Features.Weather.V2.GetForecasts;

public class GetForecastsRequestValidator : Validator<GetForecastsRequest>
{
    public GetForecastsRequestValidator()
    {
        RuleFor(x => x.City)
            .NotEmpty();

        RuleFor(x => x.Days)
            .InclusiveBetween(1, 7)
            .When(x => x.Days.HasValue);
    }
}