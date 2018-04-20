using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using ITOrm.Core.Helper;

namespace ITOrm.Manage
{
    public static class CacheConfig
    {
        public static void RegisterMemcache()
        {
            char[] separator = { ',' };
            string[] serverlist = ConfigHelper.GetAppSettings("Memcached.ServerList").Split(separator);

            // initialize the pool for memcache servers
            try
            {
                Memcached.ClientLibrary.SockIOPool pool = Memcached.ClientLibrary.SockIOPool.GetInstance();
                pool.SetServers(serverlist);

                //设置cache权重（均衡负载用）
                pool.SetWeights(new int[] { 1 });
                //socket pool设置
                pool.InitConnections = 5; //初始化时创建的连接数
                pool.MinConnections = 5; //最小连接数
                pool.MaxConnections = 2000; //最大连接数

                //连接的最大空闲时间，下面设置为6个小时（单位ms），超过这个设置时间，连接会被释放掉
                pool.MaxIdle = 1000 * 60 * 60 * 6;
                //通讯的超时时间，下面设置为3秒（单位ms），.NET版本没有实现
                pool.SocketTimeout = 1000 * 3;
                //socket连接的超时时间，下面设置表示连接不超时，即一直保持连接状态
                pool.SocketConnectTimeout = 0;
                pool.Nagle = false; //是否对TCP/IP通讯使用Nalgle算法，.NET版本没有实现
                //维护线程的间隔激活时间，下面设置为60秒（单位s），设置为0表示不启用维护线程
                pool.MaintenanceSleep = 60;
                //socket单次任务的最大时间，超过这个时间socket会被强行中断掉（当前任务失败）
                pool.MaxBusy = 1000 * 10;

                pool.Failover = true;

                pool.Initialize();

            }
            catch (Exception ex)
            {
                //Logs.kufaLog( ex.Message + "<:::>",
                //                 "d:\\Log\\CacheConfig", "iis"); 
                //这里就可以用Log4Net记录Error啦！
            }

        }
    }
}