using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ping_Checking_System
{
    public partial class Form1 : Form
    {

        String defaultIP;
        int startingIndex;
        int endingIndex;
        
        DataTable table = new DataTable();
        int countSuccess = 0;

        public Form1()
        {
            InitializeComponent();
            table.Columns.Add("Reporting Time");
            table.Columns.Add("IP Address");
            table.Columns.Add("Status");
            table.Columns.Add("TimeOut");
        }
        

        private void button1_Click(object sender, EventArgs e)
        {
            countSuccess = 0;
            table.Clear();
            defaultIP = textBox1.Text + "." + textBox2.Text + "." + textBox3.Text + ".";
            startingIndex = int.Parse(textBox4.Text);
            endingIndex = int.Parse(textBox5.Text);

            List<Task> tasks = new List<Task>();
            
            for(int i=startingIndex; i<=endingIndex; i++)
            {
               Task currentTask = pingingOnTheWay(defaultIP + i);
            tasks.Add(currentTask);
            }
            
        }

        
        private async Task pingingOnTheWay(String ip)
        {
            
                Ping ping = new Ping();
                PingReply pingReply = await ping.SendPingAsync(ip, 5000);

                if (pingReply.Status == IPStatus.Success)
                    countSuccess++;

                table.Rows.Add(DateTime.Now.TimeOfDay, ip, pingReply.Status.ToString(), pingReply.RoundtripTime.ToString());
                dataGridView1.DataSource = table;
                label7.Text = "Successful Pings: " + countSuccess;
                Console.WriteLine(DateTime.Now.TimeOfDay + " \t" + ip + " \t" + pingReply.Status.ToString() + " \t" + pingReply.RoundtripTime.ToString());
                Console.WriteLine("Successful Pings: " + countSuccess);
            }
        
    }

  
}
