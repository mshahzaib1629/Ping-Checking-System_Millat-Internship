using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ping_Checking_System
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

            String defaultIP = textBox1.Text + "." + textBox2.Text + "." + textBox3.Text + ".";
            int startingIndex = int.Parse(textBox4.Text);
            int endingIndex = int.Parse(textBox5.Text);

            Ping ping = new Ping();

            DataTable table = new DataTable();
            table.Columns.Add("IP Address");
            table.Columns.Add("Status");
            table.Columns.Add("TimeOut");

            dataGridView1.DataSource = table;

            for (int i = startingIndex; i <= endingIndex; i++)
            {
                PingReply pingReply = ping.Send(defaultIP + i, 10000);

                table.Rows.Add(defaultIP + i, pingReply.Status.ToString(), pingReply.RoundtripTime.ToString());
                
            }
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
