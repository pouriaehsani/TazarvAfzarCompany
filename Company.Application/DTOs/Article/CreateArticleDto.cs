using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Company.Application.DTOs.Article
{
    public class CreateArticleDto
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string Content { get; set; }

        public int CategoryId { get; set; }

        public IFormFile Image { get; set; }

        public List<string> TagNames { get; set; } = new List<string>();
    }
}
