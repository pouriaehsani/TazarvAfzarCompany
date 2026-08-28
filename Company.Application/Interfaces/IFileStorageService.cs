using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Application.Interfaces;

public interface IFileStorageService
{
    Task<string> SaveAsync(IFormFile file, string folder);
    Task DeleteAsync(string filePath);
}
