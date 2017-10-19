using System;

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
