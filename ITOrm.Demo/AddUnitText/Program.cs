
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AddUnitText
{

    public class aa
    {
        public string name;
    }
    class Program
    {
        static void Main(string[] args)
        {
            Context context = new Context();
            Database.SetInitializer(new Initializer());
            context.Database.Initialize(true);

            List<aa> list = new List<aa>();

            Console.WriteLine("end");
            Console.Read();
        }

        public class Initializer : DropCreateDatabaseIfModelChanges<Context>
        {

            protected override void Seed(Context context)
            {
                context.PayWays.AddRange(new List<PayWay>
                {
                    new PayWay{Name = "支付宝"},
                    new PayWay{Name = "微信"},
                    new PayWay{Name = "QQ红包"}
                });
            }
        }
    }


    public abstract class AbstractEquipment
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public abstract void Attack();
    }

    public class Gun : AbstractEquipment
    {
        public override void Attack()
        {
            Console.WriteLine("用枪攻击");
        }
    }

    class Sword : AbstractEquipment
    {
        public override void Attack()
        {
            Console.WriteLine("用剑攻击");
        }
    }

    public class BaseEquipmentDecorator : AbstractEquipment
    {
        private AbstractEquipment _equipment = null;
        public BaseEquipmentDecorator(AbstractEquipment equipment)
        {
            _equipment = equipment;
        }
        public override void Attack()
        {
            _equipment.Attack();
        }
    }


    public class EquipmentStrengthenDecorator : BaseEquipmentDecorator
    {
        //调用直接父类的指定构造函数
        public EquipmentStrengthenDecorator(AbstractEquipment equipment)
            : base(equipment)
        {

        }
        public override void Attack()
        {
            base.Attack();
            Strengthen();
        }
        public void Strengthen()
        {
            Console.WriteLine("武器被强化");
        }
    }


    public class MoStrengthenDecorator : BaseEquipmentDecorator
    {
        //调用直接父类的指定构造函数
        public MoStrengthenDecorator(AbstractEquipment equipment)
            : base(equipment)
        {

        }
        public override void Attack()
        {
            base.Attack();
            Strengthen();
        }
        public void Strengthen()
        {
            Console.WriteLine("摩肩接踵");
        }
    }
}