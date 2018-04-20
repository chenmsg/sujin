using Castle.Windsor;
using System;

namespace ITOrm.Core.Utility.Castle
{
    public class ContainerHelper
    {
        /// <summary>
        /// 容器实例对象
        /// </summary>
        private static volatile WindsorContainer instance = null;

        /// <summary>
        /// 锁
        /// </summary>
        private static volatile object olock = new object();

        public static T Get<T>()
        {
            try
            {

                if (instance == null)
                {
                    lock (olock)
                    {
                        if (instance == null)
                        {
                            instance = new WindsorContainer("config://castle/");
                        }
                    }
                }
                return instance.Resolve<T>();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 重新加载容器的实例
        /// </summary>
        public static void ReloadWinsorContainer()
        {
            lock (olock)
            {
                if (instance != null)
                {
                    instance.Dispose();
                }
                instance = null;
            }
        }
    }
}
