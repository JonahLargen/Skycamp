using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Skycamp.ApiService.Data.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Skycamp.ApiService.Data.ProjectManagement;

[Table("WorkspaceUsers", Schema = "projectmgmt")]
public class WorkspaceUser
{
    public Guid WorkspaceId { get; set; }

    public string UserId { get; set; } = null!;

    public string RoleName { get; set; } = null!;

    public DateTime JoinedUtc { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(UserId))]
    public ApplicationUser User { get; set; } = null!;

    [ForeignKey(nameof(WorkspaceId))]
    public Workspace Workspace { get; set; } = null!;

    [ForeignKey(nameof(RoleName))]
    public WorkspaceRole Role { get; set; } = null!;
}

public class WorkspaceUserConfiguration : IEntityTypeConfiguration<WorkspaceUser>
{
    public void Configure(EntityTypeBuilder<WorkspaceUser> builder)
    {
        builder.HasKey(wu => new { wu.WorkspaceId, wu.UserId });

        builder.HasOne(wu => wu.User)
            .WithMany()
            .HasForeignKey(wu => wu.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(wu => wu.Workspace)
            .WithMany()
            .HasForeignKey(wu => wu.WorkspaceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(wu => wu.Role)
            .WithMany()
            .HasForeignKey(wu => wu.RoleName)
            .OnDelete(DeleteBehavior.Restrict);
    }
}