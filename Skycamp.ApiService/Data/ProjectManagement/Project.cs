using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Skycamp.ApiService.Data.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Skycamp.ApiService.Data.ProjectManagement;

[Table("Projects", Schema = "projectmgmt")]
public class Project
{
    [Key]
    public Guid Id { get; set; }

    public Guid WorkspaceId { get; set; }

    [MaxLength(100)]
    public string Name { get; set; } = null!;

    [MaxLength(500)]
    public string? Description { get; set; }

    public bool IsAllAccess { get; set; } = false;

    public string? CreateUserId { get; set; }

    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

    public DateTime LastUpdatedUtc { get; set; } = DateTime.UtcNow;

    [Column(TypeName = "decimal(5,4)")]
    public decimal Progress { get; set; } = 0m;

    public DateTime? ArchivedUtc { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    [ForeignKey(nameof(WorkspaceId))]
    public Workspace Workspace { get; set; } = null!;

    [ForeignKey(nameof(CreateUserId))]
    public ApplicationUser? CreateUser { get; set; } = null!;
}

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.Property(m => m.Id)
           .HasValueGenerator<SequentialGuidValueGenerator>();

        builder.HasOne(p => p.CreateUser)
            .WithMany()
            .HasForeignKey(p => p.CreateUserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}