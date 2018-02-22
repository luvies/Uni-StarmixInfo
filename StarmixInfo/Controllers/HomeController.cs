using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StarmixInfo.Models;
using StarmixInfo.Services;

namespace StarmixInfo.Controllers
{
    public class HomeController : Controller
    {
        readonly DataContext _dbContext;
        readonly ILogger<HomeController> _logger;
        readonly IConfigHelper _configHelper;

        public HomeController(DataContext dbContext, ILogger<HomeController> logger, IConfigHelper configHelper)
        {
            _dbContext = dbContext;
            _logger = logger;
            _configHelper = configHelper;
        }

        public IActionResult Index()
        {
            if (_configHelper.CurrentProject == null)
                return View();
            return View(_dbContext.Projects.SingleOrDefault(q => q.ProjectID == _configHelper.CurrentProject));
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View(GetErrorObject());
        }

        [HttpGet("/Home/Error/{code}")]
        public IActionResult Error(string code)
        {
            return View(GetErrorObject(code));
        }

        ErrorViewModel GetErrorObject(string code = null) => new ErrorViewModel
        {
            Code = code,
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        };
    }
}
