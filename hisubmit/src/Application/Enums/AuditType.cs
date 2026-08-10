using System.ComponentModel.DataAnnotations;

namespace HiSubmit.Application.Enums;

public enum AuditType : byte
{
    None = 0,
    [Display(Name = "create")]
    Create = 1,
    Update = 2,
    Delete = 3
}