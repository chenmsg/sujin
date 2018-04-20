using System.Data.Entity;

namespace ITOrm.EF.Models.Context
{
    public class BaseSqlContext : DbContext
    {
        public BaseSqlContext()
            : base("ITOrmdb")
        {
            base.Configuration.ProxyCreationEnabled = false;
        }
        public DbSet BaseSqlSet { get; set; }
    }

}
