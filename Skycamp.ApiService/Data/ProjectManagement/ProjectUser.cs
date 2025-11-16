using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Skycamp.ApiService.Data.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Skycamp.ApiService.Data.ProjectManagement;

[Table("ProjectUsers", Schema = "projectmgmt")]
public class ProjectUser
{
    public Guid ProjectId { get; set; }

    public string UserId { get; set; } = null!;

    public string RoleName { get; set; } = null!;

    public DateTime JoinedUtc { get; set; } = DateTime.UtcNow;

    public bool IsFavorite { get; set; } = false;

    [ForeignKey(nameof(UserId))]
    public ApplicationUser User { get; set; } = null!;

    [ForeignKey(nameof(ProjectId))]
    public Project Project { get; set; } = null!;

    [ForeignKey(nameof(RoleName))]
    public ProjectRole Role { get; set; } = null!;
}

public class ProjectUserConfiguration : IEntityTypeConfiguration<ProjectUser>
{
    public void Configure(EntityTypeBuilder<ProjectUser> builder)
    {
        builder.HasKey(pu => new { pu.ProjectId, pu.UserId });

        builder.HasIndex(pu => pu.ProjectId);
        builder.HasIndex(pu => pu.UserId);
        builder.HasIndex(pu => pu.RoleName);
        builder.HasIndex(pu => pu.JoinedUtc);

        builder.HasOne(pu => pu.User)
            .WithMany()
            .HasForeignKey(pu => pu.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(pu => pu.Project)
            .WithMany(p => p.Users)
            .HasForeignKey(pu => pu.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(pu => pu.Role)
            .WithMany()
            .HasForeignKey(pu => pu.RoleName)
            .OnDelete(DeleteBehavior.Restrict);
    }
}