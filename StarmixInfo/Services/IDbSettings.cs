using System;

namespace StarmixInfo.Services
{
    public interface IDbSettings : IDisposable
    {
        string this[string setting] { get; set; }
    }
}
