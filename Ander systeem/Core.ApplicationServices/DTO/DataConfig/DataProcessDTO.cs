using System.ComponentModel.DataAnnotations;

namespace Leap.ApplicationServices.DTO.DataConfig
{
    public abstract class DataProcessDTO : IDTO
    {
        [Required]
        public Guid WorkspaceId { get; set; }
        public Guid ProcessId { get; set; } = Guid.NewGuid();
    }
}
