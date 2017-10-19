using System.Linq;

namespace StarmixInfo.Services
{
    public class DbSettings : IDbSettings
    {
        readonly Models.DataContext _dbContext;

        public DbSettings(Models.DataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public string this[string setting]
        {
            get
            {
                return _dbContext.Config.SingleOrDefault(q => q.ConfigID == setting)?.Value;
            }
            set
            {
                try
                {
                    _dbContext.Config.SingleOrDefault(q => q.ConfigID == setting).Value = value;
                }
                catch
                {
                    _dbContext.Config.Add(new Models.Data.Config() { ConfigID = setting, Value = value });
                }
                _dbContext.SaveChanges();
            }
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}
