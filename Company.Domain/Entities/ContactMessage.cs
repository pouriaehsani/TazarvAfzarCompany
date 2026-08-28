using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Company.Domain.Common;

namespace Company.Domain.Entities;

public class ContactMessage : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public bool IsRead { get; set; }
}
