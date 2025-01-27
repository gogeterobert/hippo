using System.ComponentModel.DataAnnotations;

namespace Hippo.Application.Common.Interfaces.BindleService;

public class RevisionSpinToml
{
    [Required]
    public RevisionTrigger? Trigger { get; set; }

    [Required]
    public List<RevisionComponent> Component { get; set; } = new();
}
