using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StarmixInfo.Models;
using StarmixInfo.Models.Data;
using StarmixInfo.Models.Unity;
using StarmixInfo.Services;

namespace StarmixInfo.Controllers
{
    public class ProjectsController : Controller
    {
        readonly DataContext _dbContext;
        readonly ILogger<ProjectsController> _logger;
        readonly IConfigHelper _configHelper;
        readonly IUnityApiHelper _unityApiHelper;

        public ProjectsController(DataContext dbContext, ILogger<ProjectsController> logger,
                                  IConfigHelper configHelper, IUnityApiHelper unityApiHelper)
        {
            _dbContext = dbContext;
            _logger = logger;
            _configHelper = configHelper;
            _unityApiHelper = unityApiHelper;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View(((IEnumerable<Project>)_dbContext.Projects.ToList(), _configHelper.CurrentProject));
        }

        // GET: /<controller>/{id}
        [HttpGet("[controller]/{id}")]
        public IActionResult Project(int id)
        {
            return View(_dbContext.Projects.SingleOrDefault(q => q.ProjectID == id));
        }

        // GET: /<controller>/{id}/Builds
        [HttpGet("[controller]/{id}/Builds")]
        async public Task<IActionResult> ProjectBuilds(int id)
        {
            _logger.LogInformation("Fetching builds from project {0}", id);
            Project proj = _dbContext.Projects.SingleOrDefault(q => q.ProjectID == id);
            if (proj == null)
            {
                _logger.LogInformation("Project does not exist, redirecting...");
                return View("ProjectBuilds", Tuple.Create<Project, Dictionary<Platform, List<BuildModel>>>(null, null));
            }
            Dictionary<Platform, List<BuildModel>> allBuilds = new Dictionary<Platform, List<BuildModel>>();
            int total = 0;
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Stop();
            foreach (var platform in new[] {
                Platform.Windows64,
                Platform.Windows32,
                Platform.OSX,
                Platform.Linux64,
                Platform.Linux32,
                Platform.Android
            })
            {
                allBuilds.Add(platform, await _unityApiHelper.GetBuilds(proj.UnityOrgID, proj.UnityProjectID, platform));
                total += allBuilds[platform].Count;
            }
            stopwatch.Stop();
            _logger.LogInformation("Found {0} builds in total in {1} seconds", total, stopwatch.Elapsed.TotalMilliseconds / 1000);
            return View("ProjectBuilds", (proj, allBuilds));
        }
    }
}
