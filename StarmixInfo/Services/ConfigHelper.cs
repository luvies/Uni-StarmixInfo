using System;
namespace StarmixInfo.Services
{
    public class ConfigHelper : IConfigHelper
    {
        const string CurrentProjectConfigID = "current_project";

        readonly IDbSettings _dbSettings;

        public ConfigHelper(IDbSettings dbSettings)
        {
            _dbSettings = dbSettings;
        }

        public int? CurrentProject
        {
            get
            {
                string id = _dbSettings[CurrentProjectConfigID];
                if (id == null)
                    return null;
                return int.Parse(id);
            }
            set => _dbSettings[CurrentProjectConfigID] = value?.ToString();
        }
    }
}
