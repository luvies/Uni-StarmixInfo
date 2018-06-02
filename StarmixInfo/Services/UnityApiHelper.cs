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
        const string ApiEndpoint_ShareLink = "/api/v1/orgs/{0}/projects/{1}/buildtargets/{2}/builds/{3}/share";
        const string Url_ShareLink = "https://developer.cloud.unity3d.com/share/{0}/";
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

        public class BuildData
        {
            public string platform;
            public string buildStatus;
            public string created;
            public string buildtargetid;
            public int build;
            public Links links;
            public string finished;

            public class Links
            {
                public DownloadPrimary download_primary;

                public class DownloadPrimary
                {
                    public string href = "";
                }
            }
        }

        public async Task<Dictionary<Platform, List<BuildModel>>> GetBuilds(string orgId, string projId, bool shareLinks = false)
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
                var parsedBuilds = JsonConvert.DeserializeObject<IEnumerable<BuildData>>(content);
                var workers = new List<Task>();
                foreach (var item in parsedBuilds)
                {
                    // if platform for build exists as a Platform, add it
                    if (_platformDict.TryGetValue(item.platform, out Platform platform))
                    {
                        // set up build model
                        var build = new BuildModel
                        {
                            BuildStatus = GetBuildStatusFromString(item.buildStatus),
                            Created = DateTime.Parse(item.created),
                            BuildTargetId = item.buildtargetid,
                            BuildNumber = item.build
                        };
                        // if build was a success, get download link
                        if (build.BuildStatus == BuildStatus.Success)
                        {
                            build.DownloadLink = item.links.download_primary.href;
                            build.Finished = DateTime.Parse(item.finished);
                            if (shareLinks)
                                workers.Add(FinishBuildFetch(orgId, projId, build));
                        }
                        // if builds were already added to this platform, add them to the list
                        // otherwise, init the list
                        if (builds.TryGetValue(platform, out List<BuildModel> buildModels))
                            buildModels.Add(build);
                        else
                            builds.Add(platform, new List<BuildModel> { build });
                    }
                }
                await Task.WhenAll(workers);
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

        async Task FinishBuildFetch(string orgId, string projId, BuildModel build)
        {
            string shareEndpoint = string.Format(ApiEndpoint_ShareLink,
                                                 orgId, projId, build.BuildTargetId, build.BuildNumber);
            var resp = await _httpClient.GetAsync(shareEndpoint);
            _logger.LogInformation("got status {0} from API (endpoint: {1})", resp.StatusCode, shareEndpoint);
            var def = new { shareid = "" };
            if (resp.IsSuccessStatusCode)
            {
                string content = await resp.Content.ReadAsStringAsync();
                var shareResp = JsonConvert.DeserializeAnonymousType(content, def);
                build.DownloadLink = string.Format(Url_ShareLink, shareResp.shareid);
            }
            else
            {
                var respCreate = await _httpClient.PostAsync(shareEndpoint,
                                                             new FormUrlEncodedContent(new KeyValuePair<string, string>[] { }));
                if (respCreate.IsSuccessStatusCode)
                {
                    string content = await respCreate.Content.ReadAsStringAsync();
                    var shareResp = JsonConvert.DeserializeAnonymousType(content, def);
                    build.DownloadLink = string.Format(Url_ShareLink, shareResp.shareid);
                }
                else
                {
                    _logger.LogWarning("failed to create share link, defaulting to direct download");
                }
            }
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
