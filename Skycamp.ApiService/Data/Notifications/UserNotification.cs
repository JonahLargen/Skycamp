using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Skycamp.ApiService.Data.Identity;
using Skycamp.ApiService.Data.ProjectManagement;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Skycamp.ApiService.Data.Notifications;

[Table("UserNotifications", Schema = "notifications")]
public class UserNotification
{
    [Key]
    public Guid Id { get; set; }

    public Guid WorkspaceId { get; set; }

    public Guid? ProjectId { get; set; }

    public string UserId { get; set; } = null!;

    [MaxLength(100)]
    public string NotificationType { get; set; } = null!;

    [MaxLength(200)]
    public string Title { get; set; } = null!;

    [MaxLength(1000)]
    public string Message { get; set; } = null!;

    public string? ActorUserId { get; set; }

    [MaxLength(200)]
    public string? ActorUserDisplayName { get; set; }

    [MaxLength(500)]
    public string? ActorUserAvatarUrl { get; set; }

    public DateTime OccurredUtc { get; set; } = DateTime.UtcNow;

    public bool IsDismissed { get; set; } = false;

    public DateTime? DismissedUtc { get; set; }

    [ForeignKey(nameof(WorkspaceId))]
    public Workspace Workspace { get; set; } = null!;

    [ForeignKey(nameof(ProjectId))]
    public Project? Project { get; set; }

    [ForeignKey(nameof(UserId))]
    public ApplicationUser User { get; set; } = null!;

    [ForeignKey(nameof(ActorUserId))]
    public ApplicationUser? ActorUser { get; set; }
}

public class UserNotificationConfiguration : IEntityTypeConfiguration<UserNotification>
{
    public void Configure(EntityTypeBuilder<UserNotification> builder)
    {
        builder.Property(m => m.Id)
           .HasValueGenerator<SequentialGuidValueGenerator>();

        builder.HasOne(n => n.Workspace)
            .WithMany()
            .HasForeignKey(n => n.WorkspaceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(n => n.Project)
            .WithMany()
            .HasForeignKey(n => n.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(n => n.User)
            .WithMany()
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(n => n.ActorUser)
            .WithMany()
            .HasForeignKey(n => n.ActorUserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(n => new { n.UserId, n.WorkspaceId, n.IsDismissed, n.OccurredUtc })
            .HasDatabaseName("IX_UserNotifications_UserId_WorkspaceId_IsDismissed_OccurredUtc");
    }
}
