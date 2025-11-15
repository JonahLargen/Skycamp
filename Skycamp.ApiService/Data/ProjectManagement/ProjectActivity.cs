using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Skycamp.ApiService.Data.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Skycamp.ApiService.Data.ProjectManagement;

[Table("ProjectActivities", Schema = "projectmgmt")]
public class ProjectActivity
{
    [Key]
    public Guid Id { get; set; }

    public Guid ProjectId { get; set; }

    [MaxLength(100)]
    public string ActivityType { get; set; } = null!;

    [MaxLength(1000)]
    public string Message { get; set; } = null!;

    public string? UserId { get; set; }

    [MaxLength(200)]
    public string? UserDisplayName { get; set; }

    [MaxLength(500)]
    public string? UserAvatarUrl { get; set; }

    public DateTime OccurredUtc { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(ProjectId))]
    public Project Project { get; set; } = null!;

    [ForeignKey(nameof(UserId))]
    public ApplicationUser? User { get; set; }
}

public class ProjectActivityConfiguration : IEntityTypeConfiguration<ProjectActivity>
{
    public void Configure(EntityTypeBuilder<ProjectActivity> builder)
    {
        builder.Property(m => m.Id)
           .HasValueGenerator<SequentialGuidValueGenerator>();

        builder.HasOne(pa => pa.Project)
            .WithMany()
            .HasForeignKey(pa => pa.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(pa => pa.User)
            .WithMany()
            .HasForeignKey(pa => pa.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(pa => new { pa.ProjectId, pa.OccurredUtc })
            .HasDatabaseName("IX_ProjectActivities_ProjectId_OccurredUtc");
    }
}
