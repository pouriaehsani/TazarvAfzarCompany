using Company.Application.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Company.Infrastructure.FileStorage;

public class FileStorageService : IFileStorageService
{
    private readonly IWebHostEnvironment _environment;

    public FileStorageService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<string> SaveAsync(
        IFormFile file,
        string folder)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("Invalid file.");

        var uploadFolder = Path.Combine(
            _environment.WebRootPath,
            folder);

        Directory.CreateDirectory(uploadFolder);

        var extension = Path.GetExtension(file.FileName);

        var fileName = $"{Guid.NewGuid()}{extension}";

        var filePath = Path.Combine(
            uploadFolder,
            fileName);

        await using var stream =
            new FileStream(filePath, FileMode.Create);

        await file.CopyToAsync(stream);

        return $"/{folder.Replace("\\", "/")}/{fileName}";
    }

    public Task DeleteAsync(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return Task.CompletedTask;

        var physicalPath = Path.Combine(
            _environment.WebRootPath,
            filePath.TrimStart('/'));

        if (File.Exists(physicalPath))
        {
            File.Delete(physicalPath);
        }

        return Task.CompletedTask;
    }
}