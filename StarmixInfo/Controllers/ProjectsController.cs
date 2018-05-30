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
        public async Task<IActionResult> ProjectBuilds(int id)
        {
            _logger.LogInformation("Fetching builds from project {0}", id);
            Project proj = _dbContext.Projects.SingleOrDefault(q => q.ProjectID == id);
            if (proj == null)
            {
                _logger.LogInformation("Project does not exist, redirecting...");
                return View("ProjectBuilds", Tuple.Create<Project, Dictionary<Platform, List<BuildModel>>>(null, null));
            }
            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            Dictionary<Platform, List<BuildModel>> allBuilds = await _unityApiHelper.GetBuilds(proj.UnityOrgID, proj.UnityProjectID);

            stopwatch.Stop();
#if DEBUG
            _logger.LogInformation("Found {0} builds in total in {1:N4} seconds", allBuilds.Values.SelectMany(b => b).Count(), stopwatch.Elapsed.TotalMilliseconds / 1000);
#endif
            return View("ProjectBuilds", (proj, allBuilds));
        }
    }
}
