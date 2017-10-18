using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StarmixInfo.Models.Unity;

namespace StarmixInfo.Services
{
    public interface IUnityApiHelper : IDisposable
    {
        string AuthToken { get; }
        Task<List<BuildModel>> GetBuilds(string orgId, string projId, Platform platform);
    }
}
