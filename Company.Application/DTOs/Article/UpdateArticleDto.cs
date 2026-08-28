using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Application.DTOs.Article
{
    public class UpdateArticleDto
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Content { get; set; }

        public int CategoryId { get; set; }

        public IFormFile? Image { get; set; }

        public List<int> TagIds { get; set; } = new();
    }
}
