using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Skycamp.ApiService.Data.ProjectManagement;

[Table("WorkspaceRoles", Schema = "projectmgmt")]
public class WorkspaceRole
{
    [Key]
    public string Name { get; set; } = null!;
}