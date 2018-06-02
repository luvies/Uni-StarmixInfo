using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StarmixInfo.Models.Unity;

namespace StarmixInfo.Services
{
    public interface IUnityApiHelper : IDisposable
    {
        string AuthToken { get; }
        Task<Dictionary<Platform, List<BuildModel>>> GetBuilds(string orgId, string projId, bool shareLinks = false);
    }
}
