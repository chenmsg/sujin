using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;

namespace ITOrm.Ms.Models.Context
{
    public class BaseSqlContext : DbContext
    {
        public BaseSqlContext() : base("clump_host")
        {
            base.Configuration.ProxyCreationEnabled = false;
        }
        public DbSet BaseSqlSet { get; set; }
    }

}
