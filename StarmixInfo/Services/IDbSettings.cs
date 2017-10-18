using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StarmixInfo.Services
{
    public interface IDbSettings : IDisposable
    {
        string this[string setting] { get; set; }
    }
}
