using Company.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Domain.Entities;

public class ProjectImage : BaseEntity
{
    public string ImageName { get; set; } = string.Empty;

    public int ProjectId { get; set; }

    public Project Project { get; set; } = null!;
}
