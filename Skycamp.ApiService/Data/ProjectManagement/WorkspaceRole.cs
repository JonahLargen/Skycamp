using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Skycamp.ApiService.Data.ProjectManagement;

[Table("WorkspaceRoles", Schema = "projectmgmt")]
public class WorkspaceRole
{
    [Key]
    public string Name { get; set; } = null!;
}

public class WorkspaceRoleConfiguration : IEntityTypeConfiguration<WorkspaceRole>
{
    public void Configure(EntityTypeBuilder<WorkspaceRole> builder)
    {
        builder.HasData(
            new WorkspaceRole { Name = "Owner" },
            new WorkspaceRole { Name = "Admin" },
            new WorkspaceRole { Name = "Member" },
            new WorkspaceRole { Name = "Viewer" }
        );
    }
}