using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        const string ApiEndpoint_ListBuilds = "/api/v1/orgs/{0}/projects/{1}/buildtargets/_all/builds?platform={2}";
        readonly HttpClient _httpClient;
        readonly ILogger<UnityApiHelper> _logger;

        public string AuthToken { get; }

        public UnityApiHelper(string authToken, ILogger<UnityApiHelper> logger)
        {
            AuthToken = authToken;
            _logger = logger;

            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(ApiEndpoint);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("applicatin/json"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", AuthToken);
        }

        async public Task<List<BuildModel>> GetBuilds(string orgId, string projId, Platform platform)
        {
            string endpoint = string.Format(ApiEndpoint_ListBuilds,
                                            orgId, projId, GetDescString(platform));
            var resp = await _httpClient.GetAsync(endpoint);
            _logger.LogInformation("got status {0} from API (endpoint: {1}", resp.StatusCode, endpoint);
            if (resp.IsSuccessStatusCode)
            {
                List<BuildModel> builds = new List<BuildModel>();
                BuildModel build;
                string content = await resp.Content.ReadAsStringAsync();
                _logger.LogDebug("response content: {0}", content);
                dynamic dybuilds = JsonConvert.DeserializeObject(content);
                foreach (dynamic item in dybuilds)
                {
                    build = new BuildModel()
                    {
                        BuildStatus = Services.UnityApiHelper.GetBuildStatusFromString((string)item.buildStatus),
                        Created = DateTime.Parse((string)item.created)
                    };
                    if (build.BuildStatus == BuildStatus.Success)
                    {
                        build.DownloadLink = (string)item.links.download_primary.href;
                        build.Finished = DateTime.Parse((string)item.finished);
                    }
                    builds.Add(build);
                }
                return builds;
            }
            return null;
        }

        static string GetDescString<T>(T item)
        {
            //return typeof(T).GetTypeInfo().GetProperty(item.ToString()).GetCustomAttribute<DescriptionAttribute>().Description;
            return typeof(T).GetTypeInfo().GetMember(item.ToString())[0].GetCustomAttribute<DescriptionAttribute>().Description;
            //return item.GetType().GetTypeInfo().GetMember(item.ToString())[0].GetCustomAttributes()

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
