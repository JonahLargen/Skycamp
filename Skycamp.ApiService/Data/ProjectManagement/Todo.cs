using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Skycamp.ApiService.Data.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Skycamp.ApiService.Data.ProjectManagement;

[Table("Todos", Schema = "projectmgmt")]
public class Todo
{
    [Key]
    public Guid Id { get; set; }

    public Guid ProjectId { get; set; }

    [MaxLength(500)]
    public string Text { get; set; } = null!;

    public DateOnly? DueDate { get; set; }

    public string? PrimaryAssigneeId { get; set; }

    [MaxLength(2000)]
    public string? Notes { get; set; }

    public bool IsCompleted { get; set; } = false;

    public DateTime? CompletedUtc { get; set; }

    public string? CreateUserId { get; set; }

    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

    public DateTime LastUpdatedUtc { get; set; } = DateTime.UtcNow;

    [Timestamp]
    public byte[] RowVersion { get; set; } = null!;

    [ForeignKey(nameof(ProjectId))]
    public Project Project { get; set; } = null!;

    [ForeignKey(nameof(PrimaryAssigneeId))]
    public ApplicationUser? PrimaryAssignee { get; set; }

    [ForeignKey(nameof(CreateUserId))]
    public ApplicationUser? CreateUser { get; set; }
}

public class TodoConfiguration : IEntityTypeConfiguration<Todo>
{
    public void Configure(EntityTypeBuilder<Todo> builder)
    {
        builder.Property(m => m.Id)
           .HasValueGenerator<SequentialGuidValueGenerator>();

        builder.HasOne(t => t.Project)
            .WithMany()
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(t => t.PrimaryAssignee)
            .WithMany()
            .HasForeignKey(t => t.PrimaryAssigneeId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(t => t.CreateUser)
            .WithMany()
            .HasForeignKey(t => t.CreateUserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
