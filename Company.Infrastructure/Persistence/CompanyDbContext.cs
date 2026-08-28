using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Company.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Company.Infrastructure.Persistence;
public class CompanyDbContext : DbContext
{
    public CompanyDbContext(DbContextOptions<CompanyDbContext> options): base(options)
    {

    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ArticleTag>()
            .HasKey(at => new { at.ArticleId, at.TagId });
    }

    public DbSet<Article> Articles { get; set; }

    public DbSet<Category> Categories { get; set; }

    public DbSet<Service> Services { get; set; }

    public DbSet<Project> Projects { get; set; }

    public DbSet<ProjectImage> ProjectImages { get; set; }

    public DbSet<Tag> Tags { get; set; }

    public DbSet<ArticleTag> ArticleTags { get; set; }

    public DbSet<TeamMember> TeamMembers { get; set; }

    public DbSet<ContactMessage> ContactMessages { get; set; }
}
