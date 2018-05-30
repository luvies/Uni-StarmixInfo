using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StarmixInfo.Models.Unity;

namespace StarmixInfo.Services
{
    public class UnityApiHelper : IUnityApiHelper
    {
        const string ApiEndpoint = "https://build-api.cloud.unity3d.com";
        const string ApiEndpoint_ListBuilds = "/api/v1/orgs/{0}/projects/{1}/buildtargets/_all/builds?per_page=100";
        readonly HttpClient _httpClient;
        readonly ILogger<UnityApiHelper> _logger;
        readonly Dictionary<string, Platform> _platformDict;

        public string AuthToken { get; }

        public UnityApiHelper(string authToken, ILogger<UnityApiHelper> logger)
        {
            AuthToken = authToken;
            _logger = logger;

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(ApiEndpoint)
            };
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", AuthToken);

            // a hashset of all available platform strings
            _platformDict = new Dictionary<string, Platform>();
            foreach (var (desc, platform) in
                     ((IEnumerable<Platform>)Enum.GetValues(typeof(Platform)))
                     .Select(e => (GetDescString(e), e)))
                _platformDict.Add(desc, platform);
        }

        public async Task<Dictionary<Platform, List<BuildModel>>> GetBuilds(string orgId, string projId)
        {
            // fetch result from unity API
            string endpoint = string.Format(ApiEndpoint_ListBuilds,
                                            orgId, projId);
            var resp = await _httpClient.GetAsync(endpoint);
            _logger.LogInformation("got status {0} from API (endpoint: {1})", resp.StatusCode, endpoint);
            if (resp.IsSuccessStatusCode)
            {
                // if got result, walk through it
                var builds = new Dictionary<Platform, List<BuildModel>>();
                // read content
                string content = await resp.Content.ReadAsStringAsync();
                _logger.LogTrace("response content: {0}", content);
                // parse content
                dynamic dybuilds = JsonConvert.DeserializeObject(content);
                foreach (dynamic item in dybuilds)
                {
                    // if platform for build exists as a Platform, add it
                    if (_platformDict.TryGetValue((string)item.platform, out Platform platform))
                    {
                        // set up build model
                        var build = new BuildModel
                        {
                            BuildStatus = GetBuildStatusFromString((string)item.buildStatus),
                            Created = DateTime.Parse((string)item.created)
                        };
                        // if build was a success, get download link
                        if (build.BuildStatus == BuildStatus.Success)
                        {
                            build.DownloadLink = (string)item.links.download_primary.href;
                            build.Finished = DateTime.Parse((string)item.finished);
                        }
                        // if builds were already added to this platform, add them to the list
                        // otherwise, init the list
                        if (builds.TryGetValue(platform, out List<BuildModel> buildModels))
                            buildModels.Add(build);
                        else
                            builds.Add(platform, new List<BuildModel> { build });
                    }
                }
                return builds;
            }
            return null;
        }

        static string GetDescString<T>(T item)
        {
            return typeof(T).GetTypeInfo().GetMember(item.ToString())[0].GetCustomAttribute<DescriptionAttribute>().Description;
        }

        static BuildStatus GetBuildStatusFromString(string buildStatus)
        {
            foreach (BuildStatus status in Enum.GetValues(typeof(BuildStatus)))
                if (GetDescString(status) == buildStatus)
                    return status;
            throw new KeyNotFoundException(string.Format("status: {0}", buildStatus));
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
