using ITOrm.Core.Helper;
using ITOrm.Host.BLL;
using ITOrm.Host.Models;
using ITOrm.Utility.Cache;
using ITOrm.Utility.Log;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ITOrm.AutoService
{
    public partial class Form1 : Form
    {
        private delegate void FlushClient();//代理 
        Operation objO = new Operation();//主处理程序
        public Form1()
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            #region 缓存管理
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
            #endregion
            InitializeComponent();
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            objO.Start(
                 StartButton
                , tbxImggeSuccess
                , tbxImageFail
                , tbxAuditSuccess
                , tbxAuditFail
                , tbxSetRateSuccess
                , tbxSetRateFail
                ,tbxWithDrawApiSuccess
                , tbxWithDrawApiFail
                ,tbxTimedTaskSuccess
                ,tbxTimedTaskFail
                , StartTime
                );
        }

        private void Form1_Load(object sender, EventArgs e)
        {



        }

        private void button1_Click(object sender, EventArgs e)
        {
            MemcachHelper.Set("ITOrm.AutoService", $"测试{DateTime.Now}", 100);
            MessageBox.Show("成功");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string str= MemcachHelper.Get("ITOrm.AutoService") as string ;
            MessageBox.Show(str);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            WithDrawBLL wi = new WithDrawBLL();
            WithDraw model = wi.Single(100004);
            model.UTime = DateTime.Now;
            bool flag= wi.Update(model);
            MessageBox.Show(flag.ToString());
        }
    }
}
