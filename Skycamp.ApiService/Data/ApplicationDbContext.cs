using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Skycamp.ApiService.Data.Identity;
using Skycamp.ApiService.Data.Messaging;
using Skycamp.ApiService.Data.ProjectManagement;
using System.Reflection.Emit;

namespace Skycamp.ApiService.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectUser> ProjectUsers { get; set; }
    public DbSet<ProjectRole> ProjectRoles { get; set; }
    public DbSet<Workspace> Workspaces { get; set; }
    public DbSet<WorkspaceUser> WorkspaceUsers { get; set; }
    public DbSet<WorkspaceRole> WorkspaceRoles { get; set; }
    public DbSet<OutboxMessage> OutboxMessages { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<WorkspaceRole>().HasData(
            new WorkspaceRole { Name = "Owner" },
            new WorkspaceRole { Name = "Admin" },
            new WorkspaceRole { Name = "Member" },
            new WorkspaceRole { Name = "Viewer" }
        );

        modelBuilder.Entity<ProjectRole>().HasData(
            new ProjectRole { Name = "Owner" },
            new ProjectRole { Name = "Admin" },
            new ProjectRole { Name = "Member" },
            new ProjectRole { Name = "Viewer" }
        );

        modelBuilder.Entity<Workspace>()
            .HasOne(w => w.CreateUser)
            .WithMany()
            .HasForeignKey(w => w.CreateUserId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Project>()
            .HasOne(p => p.CreateUser)
            .WithMany()
            .HasForeignKey(p => p.CreateUserId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<OutboxMessage>()
            .HasIndex(m => m.OccurredOnUtc)
            .HasDatabaseName("IX_Outbox_Unprocessed")
            .HasFilter("[ProcessedOnUtc] IS NULL");
    }
}