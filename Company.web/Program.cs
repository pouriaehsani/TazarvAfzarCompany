using AutoMapper;
using Company.Application.Interfaces;
using Company.Application.Mappings;
using Company.Application.Services;
using Company.Application.Validators.Articles;
using Company.Infrastructure.FileStorage;
using Company.Infrastructure.Persistence;
using Company.Infrastructure.Repositories;
using Company.Infrastructure.UnitOfWorks;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<CompanyDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUnitOfWork,UnitOfWork>();
builder.Services.AddScoped<IArticleService, ArticleService>();
builder.Services.AddScoped<IFileStorageService, FileStorageService>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateArticleValidator>();
builder.Services.AddScoped<IBaseInfo, BaseInfo>();
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<ArticleProfile>();
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Article}/{action=ArticleList}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Admin}/{controller=Article}/{action=ArticleList}/{id?}");

app.Run();
