using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Company.Domain.Common;

namespace Company.Domain.Entities;

public class Tag : BaseEntity
{
    public string Title { get; set; } = string.Empty;

    public ICollection<ArticleTag> ArticleTags { get; set; } = new List<ArticleTag>();
}
