using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StarmixInfo.Services
{
    public interface IAdminLogon : IDisposable
    {
        bool LoggedIn { get; }
        bool HasSetPassword { get; }

        bool AttemptLogin(string password);
        void SetPassword(string password);
    }
}
