using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Skycamp.ApiService.Data.ProjectManagement;

[Table("ProjectRoles", Schema = "projectmgmt")]
public class ProjectRole
{
    [Key]
    public string Name { get; set; } = null!;
}