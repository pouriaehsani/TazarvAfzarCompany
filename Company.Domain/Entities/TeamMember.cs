using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Company.Domain.Common;

namespace Company.Domain.Entities;

public class TeamMember : BaseEntity
{
    public string FullName { get; set; } = string.Empty;

    public string Position { get; set; } = string.Empty;

    public string ImageName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}
