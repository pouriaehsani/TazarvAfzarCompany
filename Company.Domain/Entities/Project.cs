using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Company.Domain.Common;

namespace Company.Domain.Entities;

public class Project : BaseEntity
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string ClientName { get; set; } = string.Empty;

    public ICollection<ProjectImage> Images { get; set; } = new List<ProjectImage>();
}