using System;

namespace StarmixInfo.Models.Unity
{
    public class BuildModel
    {
        public BuildStatus BuildStatus { get; set; }
        public string DownloadLink { get; set; }
        public DateTime Created { get; set; }
        public DateTime Finished { get; set; }
        public string BuildTargetId { get; set; }
        public int BuildNumber { get; set; }
    }
}
