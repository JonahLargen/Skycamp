using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Skycamp.ApiService.Data.Identity;
using Skycamp.ApiService.Data.Messaging;
using Skycamp.ApiService.Data.ProjectManagement;

namespace Skycamp.ApiService.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectUser> ProjectUsers { get; set; }
    public DbSet<ProjectRole> ProjectRoles { get; set; }
    public DbSet<Workspace> Workspaces { get; set; }
    public DbSet<WorkspaceUser> WorkspaceUsers { get; set; }
    public DbSet<WorkspaceRole> WorkspaceRoles { get; set; }
    public DbSet<Todo> Todos { get; set; }
    public DbSet<OutboxMessage> OutboxMessages { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}