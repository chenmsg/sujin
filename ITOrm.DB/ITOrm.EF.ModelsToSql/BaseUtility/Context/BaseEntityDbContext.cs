using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
namespace ITOrm.EF.Models.Context
{
    public class BaseEntityDbContext<T> : DbContext where T : BaseEntity, new()
    {
        public BaseEntityDbContext() : base(new T().DataName)
        {
            base.Configuration.ProxyCreationEnabled = false;
        }

        //CodeFirst提供了一种先从代码开始工作，并根据代码直接生成数据库的工作方式。
        //Entity Framework 4.1在你的实体不派生自任何基类、不添加任何特性的时候正常的附加数据库。
        //另外呢，实体的属性也可以添加一些标签，但这些标签不是必须的。

        //覆盖默认约定[拦截模型的构建器，使用Fluent API 来修改模型]
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //数据表映射文件，根据不同的T实体映射，局限：实体必须继承BaseEntity，且主键必须是ID命名，其他字段无法做映射限制
            EntityTypeConfiguration<T> mp = new EntityTypeConfiguration<T>();
            mp.ToTable(new T().DataTableName);//映射数据库表名称
            //mp.HasKey<int>(h => h.ID);//映射表主键
            /**
            *为满足通用性，此处屏蔽部分，将无法实现
            *mp.Property((Expression<Func<T, string>>)(h => h.GAppid)).HasMaxLength(100);
            *mp.Property((Expression<Func<T, string>>)(h => h.GAppkey)).HasMaxLength(200);
            *mp.Property((Expression<Func<T, string>>)(h => h.GAppsecret)).HasMaxLength(300);
            *mp.Property((Expression<Func<T, string>>)(h => h.GMasterKey)).HasMaxLength(600);
            *如需更改EF默认数据库字段限制(大部分时候，默认的就能满足)，可用注解方式[Data Annotations]实现
            *注:使用Data Annotations 注解方式，需要引用 System.ComponentModel.DataAnnotations 和 EntityFramework
            **/
            modelBuilder.Configurations.Add<T>(mp);
        }
        public DbSet<T> BaseDbSet { get; set; }
    }

}
