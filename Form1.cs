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


        public Form1()
        {
            InitializeComponent();
            table.Columns.Add("IP Address");
            table.Columns.Add("Status");
            table.Columns.Add("TimeOut");
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            defaultIP = textBox1.Text + "." + textBox2.Text + "." + textBox3.Text + ".";
            startingIndex = int.Parse(textBox4.Text);
            endingIndex = int.Parse(textBox5.Text);
         
            for(int i=startingIndex; i<=endingIndex; i++)
            {
                pingingOnTheWay(defaultIP + i);
            }
            
        }

        
        private void pingingOnTheWay(String ip)
        {

            Thread thread = new Thread(() => this.BeginInvoke((Action)delegate ()
            {
                Ping ping = new Ping();
                PingReply pingReply = ping.Send(ip, 5000);

                table.Rows.Add(ip, pingReply.Status.ToString(), pingReply.RoundtripTime.ToString());
                dataGridView1.DataSource = table;
                Console.WriteLine(DateTime.Now.TimeOfDay + " \t" + ip + " \t" + pingReply.Status.ToString() + " \t" + pingReply.RoundtripTime.ToString());

            }));
            thread.Start();

            }
        }

  
}
