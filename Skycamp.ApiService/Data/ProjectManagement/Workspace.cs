using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Skycamp.ApiService.Data.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Skycamp.ApiService.Data.ProjectManagement;

[Table("Workspaces", Schema = "projectmgmt")]
public class Workspace
{
    [Key]
    public Guid Id { get; set; }

    [MaxLength(100)]
    public string Name { get; set; } = null!;

    [MaxLength(500)]
    public string? Description { get; set; }

    public string? CreateUserId { get; set; }

    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

    public DateTime LastUpdatedUtc { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(CreateUserId))]
    public ApplicationUser? CreateUser { get; set; } = null!;
}

public class WorkspaceConfiguration : IEntityTypeConfiguration<Workspace>
{
    public void Configure(EntityTypeBuilder<Workspace> builder)
    {
        builder.Property(m => m.Id)
            .HasValueGenerator<SequentialGuidValueGenerator>();

        builder.HasOne(w => w.CreateUser)
           .WithMany()
           .HasForeignKey(w => w.CreateUserId)
           .OnDelete(DeleteBehavior.SetNull);
    }
}
