using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{

    delegate void AsynUpdateUI(int step);
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void btnWrite_Click(object sender, EventArgs e)
        {
            int taskCount = 10000; //任务量为10000
            this.pgbWrite.Maximum = taskCount;
            this.pgbWrite.Value = 0;

            DataWrite dataWrite = new DataWrite();//实例化一个写入数据的类
            dataWrite.UpdateUIDelegate += UpdataUIStatus;//绑定更新任务状态的委托
            dataWrite.TaskCallBack += Accomplish;//绑定完成任务要调用的委托

            Thread thread = new Thread(new ParameterizedThreadStart(dataWrite.Write));
            thread.IsBackground = true;
            thread.Start(taskCount);
        }

        //更新UI
        private void UpdataUIStatus(int step)
        {
            if (InvokeRequired)
            {
                this.Invoke(new AsynUpdateUI(s =>
                {
                    this.pgbWrite.Value += s;
                    this.lblWriteStatus.Text = this.pgbWrite.Value.ToString() + "/" + this.pgbWrite.Maximum.ToString();
                }), step);


                this.Invoke(new AsynUpdateUI(delegate (int s)
                {
                    this.pgbWrite.Value += s;
                    this.lblWriteStatus.Text = this.pgbWrite.Value.ToString() + "/" + this.pgbWrite.Maximum.ToString();
                }), step);
            }
            else
            {
                this.pgbWrite.Value += step;
                this.lblWriteStatus.Text = this.pgbWrite.Value.ToString() + "/" + this.pgbWrite.Maximum.ToString();
            }
        }

        //完成任务时需要调用
        private void Accomplish()
        {
            //还可以进行其他的一些完任务完成之后的逻辑处理
            MessageBox.Show("任务完成");
        }
    }

    public class DataWrite
    {
        public delegate void UpdateUI(int step);//声明一个更新主线程的委托
        public UpdateUI UpdateUIDelegate;

        public delegate void AccomplishTask();//声明一个在完成任务时通知主线程的委托
        public AccomplishTask TaskCallBack;

        public void Write(object lineCount)
        {
            StreamWriter writeIO = new StreamWriter("text.txt", false, Encoding.GetEncoding("gb2312"));
            string head = "编号,省,市";
            writeIO.Write(head);
            for (int i = 0; i < (int)lineCount; i++)
            {
                writeIO.WriteLine(i.ToString() + ",湖南,衡阳");
                //写入一条数据，调用更新主线程ui状态的委托
                UpdateUIDelegate(1);
            }
            //任务完成时通知主线程作出相应的处理
            TaskCallBack();
            writeIO.Close();
        }
    }

}
