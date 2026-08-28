using Company.Application.DTOs.Article;
using Company.Application.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Company.web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BaseInfoController : Controller
    {
        private readonly IBaseInfo _BaseInfoService;

        public BaseInfoController(IBaseInfo BaseInfoService)
        {
            _BaseInfoService = BaseInfoService;            
        }
        [HttpGet]
        public async Task<IActionResult> BaseInfo()
        {
            var BaseInfo = await _BaseInfoService.GetAllAsync();
            return View(BaseInfo);
        }
    }
}
