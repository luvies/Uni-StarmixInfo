using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;

namespace StarmixInfo.Services
{
    public class AdminLogon : IAdminLogon
    {
        const string AdminPasswordStoreID = "admin_password_hash";
        const string AdminSettingID = "admin_session_id";
        const string AdminCookieID = "session_id";

        readonly HttpContext _httpContext;
        readonly IDbSettings _dbSettings;

        public AdminLogon(IHttpContextAccessor httpContextAccessor, IDbSettings dbSettings)
        {
            _httpContext = httpContextAccessor.HttpContext;
            _dbSettings = dbSettings;
        }

        public bool LoggedIn
        {
            get
            {
#if (TEST_ADMIN || !DEBUG)
                if (_httpContext.Request.Cookies.ContainsKey(AdminCookieID) &&
                    _dbSettings[AdminSettingID] == _httpContext.Request.Cookies[AdminCookieID])
                        return true;
                return false;
#elif (!TEST_ADMIN && DEBUG)
                return true;
#endif
            }
        }

        public bool AttemptLogin(string password)
        {
            if (MatchPassword(password))
            {
                var guid = Guid.NewGuid();
                _httpContext.Response.Cookies.Append(AdminCookieID, guid.ToString());
                _dbSettings[AdminSettingID] = guid.ToString();
                return true;
            }
            return false;
        }

        #region Password Management

        public bool HasSetPassword => _dbSettings[AdminPasswordStoreID] != null;

        public void SetPassword(string password)
        {
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            string hash = HashPassword(password, salt);
            _dbSettings[AdminPasswordStoreID] = String.Format("{0}${1}",
                                                              hash,
                                                              Convert.ToBase64String(salt));
        }

        Tuple<string, byte[]> GetStoredPasswordHash()
        {
            string[] stored = _dbSettings[AdminPasswordStoreID].Split("$", StringSplitOptions.None);
            return Tuple.Create(stored[0], Convert.FromBase64String(stored[1]));
        }

        string HashPassword(string password, byte[] salt)
        {
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password,
                salt,
                KeyDerivationPrf.HMACSHA1,
                10000,
                256 / 8));
        }

        bool MatchPassword(string given)
        {
            Tuple<string, byte[]> stored = GetStoredPasswordHash();
            return stored.Item1 == HashPassword(given, stored.Item2);
        }

        #endregion

        public void Dispose()
        {
            // nothing to dispose of
        }
    }
}
