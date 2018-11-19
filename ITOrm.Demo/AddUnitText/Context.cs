using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddUnitText
{
    public class Context : DbContext
    {
        public Context()
            : base("name=FirstCodeFirstApp")
        {
        }

        public DbSet<Donator> Donators { get; set; }
        public DbSet<PayWay> PayWays { get; set; }
    }

    public class Initializer : DropCreateDatabaseIfModelChanges<Context>
    {

    }
}
