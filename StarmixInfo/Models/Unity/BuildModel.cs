using System;
using Newtonsoft.Json;

namespace StarmixInfo.Models.Unity
{
    public class BuildModel
    {
        public BuildStatus BuildStatus { get; set; }
        public string DownloadLink { get; set; }
        public DateTime Created { get; set; }
        public DateTime Finished { get; set; }
    }
}
