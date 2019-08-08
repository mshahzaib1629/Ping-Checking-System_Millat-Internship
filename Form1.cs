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
using System.Diagnostics;

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

        private void button2_Click(object sender, EventArgs e)
        {
            // -------------------- Parameters used for Route Trace -----------------------
            string hostname = textBox6.Text;
            int timeOut = 1000; // 1000ms or 1 second
            int max_ttl = 30; //max number of servers allowed to be found
            const int bufferSize = 32;
            
            traceOut(hostname, timeOut, max_ttl, bufferSize);
           
        }

        private void traceOut(String hostname, int timeOut, int max_ttl, int bufferSize)
        {
            int current_ttl = 0; //used for tracking how many servers have been found.
            Stopwatch s1 = new Stopwatch();
            Stopwatch s2 = new Stopwatch();

            byte[] buffer = new byte[bufferSize];
            new Random().NextBytes(buffer);

            Ping pinger = new Ping();
            Task.Factory.StartNew(() => this.BeginInvoke((Action)delegate ()
            {
                WriteListBox($"Started ICMP Trace route on {hostname}");
                for (int ttl = 1; ttl <= max_ttl; ttl++)
                {
                    current_ttl++;
                    s1.Start();
                    s2.Start();
                    PingOptions options = new PingOptions(ttl, true);
                    PingReply reply = null;
                    try
                    {
                        reply = pinger.Send(hostname, timeOut, buffer, options);
                    }
                    catch
                    {
                        WriteListBox("Error");
                        break; //the rest of the code relies on reply not being null so...
                    }
                    if (reply != null) //dont need this but anyway...
                    {
                        //the traceroute bits :)
                        if (reply.Status == IPStatus.TtlExpired)
                        {
                            //address found after yours on the way to the destination
                            WriteListBox($"[{ttl}] - Route: {reply.Address} - Time: {s1.ElapsedMilliseconds} ms - Total Time: {s2.ElapsedMilliseconds} ms");
                            continue; //continue to the other bits to find more servers
                        }
                        if (reply.Status == IPStatus.TimedOut)
                        {
                            //this would occour if it takes too long for the server to reply or if a server has the ICMP port closed (quite common for this).
                            WriteListBox($"Timeout on {hostname}. Continuing.");
                            continue;
                        }
                        if (reply.Status == IPStatus.Success)
                        {
                            //the ICMP packet has reached the destination (the hostname)
                            WriteListBox($"Successful Trace route to {hostname} in {s1.ElapsedMilliseconds} ms - Total Time: {s2.ElapsedMilliseconds} ms");
                            s1.Stop();
                            s2.Stop();
                        }
                    }
                    break;
                }
            }));
        }

        private void WriteListBox(String text)
        {
            Console.WriteLine(text);
            listBox1.Items.Add(text);
        }
    }

  
}
