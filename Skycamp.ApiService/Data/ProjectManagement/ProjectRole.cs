using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Skycamp.ApiService.Data.ProjectManagement;

[Table("ProjectRoles", Schema = "projectmgmt")]
public class ProjectRole
{
    [Key]
    public string Name { get; set; } = null!;
}

public class ProjectRoleConfiguration : IEntityTypeConfiguration<ProjectRole>
{
    public void Configure(EntityTypeBuilder<ProjectRole> builder)
    {
        builder.HasData(
            new ProjectRole { Name = "Owner" },
            new ProjectRole { Name = "Admin" },
            new ProjectRole { Name = "Member" },
            new ProjectRole { Name = "Viewer" }
        );
    }
}