using FastEndpoints;

namespace Skycamp.ApiService.Features.ProjectManagement.CreateProject.Shared;

public record CreateProjectCommand : ICommand<CreateProjectResult>
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}