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
            richTextBox1.Clear();
            Ping ping = new Ping();
            PingReply pingReply = ping.Send(textBox1.Text, 10000);
            richTextBox1.AppendText("Status: " + pingReply.Status.ToString() + "\nAddress: " + pingReply.Address.ToString() + "\nTime: " + pingReply.RoundtripTime.ToString()  + "\nBuffer: " + pingReply.Buffer.ToString());

        }
    }
}
