using FluentValidation;
using System.Text.RegularExpressions;

namespace Skycamp.ApiService.Features.Weather.Shared.GetForecasts;

public partial class GetForecastsValidator : AbstractValidator<GetForecastsCommand>
{
    private static readonly Regex CityNameRegex = MyCityNameRegex();

    public GetForecastsValidator()
    {
        RuleFor(x => x.City)
            .NotEmpty()
            .Matches(CityNameRegex).WithMessage("City name contains invalid characters. Only letters, spaces, hyphens, periods, and apostrophes are allowed.");

        RuleFor(x => x.Days)
            .InclusiveBetween(1, 7)
            .When(x => x.Days.HasValue);
    }

    [GeneratedRegex(@"^[\p{L} .'-]+$", RegexOptions.Compiled)]
    private static partial Regex MyCityNameRegex();
}
