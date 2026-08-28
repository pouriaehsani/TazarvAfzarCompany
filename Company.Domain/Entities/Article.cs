using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Company.Domain.Common;

namespace Company.Domain.Entities;

public class Article : BaseEntity
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public string ImagePath { get; set; } = string.Empty;

    public int CategoryId { get; set; }

    public Category Category { get; set; } = null!;

    public ICollection<ArticleTag> ArticleTags { get; set; } = new List<ArticleTag>();
}
