using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
namespace ITOrm.Ms.Models.Context
{
    public class BaseViewDbContext<T> : DbContext where T : BaseView, new()
    {
        public BaseViewDbContext()
            : base(new T().DataName)
        {
            base.Configuration.ProxyCreationEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            EntityTypeConfiguration<T> mp = new EntityTypeConfiguration<T>();
            mp.ToTable(new T().DataTableName);//映射数据库视图名称
            modelBuilder.Configurations.Add<T>(mp);
        }
        public DbSet<T> BaseDbSet { get; set; }
    }

}
