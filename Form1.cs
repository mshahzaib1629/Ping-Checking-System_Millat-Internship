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
using System.Net;

namespace Ping_Checking_System
{
    public partial class Form1 : Form
    {

        String defaultIP;
        int startingIndex;
        int endingIndex;

        bool ipTraceActive = true;
        
        DataTable table = new DataTable();
        int countSuccess = 0;
        TimeSpan ping_timeElapsed;

        public Form1()
        {
            InitializeComponent();
            table.Columns.Add("Reporting Time");
            table.Columns.Add("IP Address");
            table.Columns.Add("Status");
            table.Columns.Add("TimeOut");
        }
        

        private void button1_Click_1(object sender, EventArgs e)
        {
            countSuccess = 0;
            table.Clear();
            defaultIP = textBox1.Text + "." + textBox2.Text + "." + textBox3.Text + ".";
            startingIndex = int.Parse(textBox4.Text);
            endingIndex = int.Parse(textBox5.Text);

            Stopwatch stopwatch = new Stopwatch();

            List<Task> tasks = new List<Task>();
            stopwatch.Start();
            for(int i=startingIndex; i<=endingIndex; i++)
            {
               Task currentTask = pingingOnTheWay(defaultIP + i);
            tasks.Add(currentTask);
            }
            stopwatch.Stop();
            ping_timeElapsed = stopwatch.Elapsed;
        }

        
        private async Task pingingOnTheWay(String ip)
        {
            int countCurrentSuccess = 0;
            long currentRoundTripTime = 0;
            MyPIng currentPing = new MyPIng();
            currentPing.Ip = ip;

            for (int i = 0; i < 4; i++)
            {
                Ping ping = new Ping();
                PingReply pingReplyTest = await ping.SendPingAsync(ip, 5000);
                currentRoundTripTime += pingReplyTest.RoundtripTime;

                Console.WriteLine(DateTime.Now.TimeOfDay + " \t" + ip + " \t" + pingReplyTest.Status.ToString() + " \t" + pingReplyTest.RoundtripTime.ToString());
                
                    if (pingReplyTest.Status == IPStatus.Success)
                {
                    countCurrentSuccess++;
                }
                if (countCurrentSuccess == 4)
                {
                    currentPing.Status = IPStatus.Success;
                }
                else
                    currentPing.Status = pingReplyTest.Status;
            }
            currentPing.TimeOut = currentRoundTripTime;

                if (currentPing.Status == IPStatus.Success)
                    countSuccess++;

                table.Rows.Add(DateTime.Now.TimeOfDay, ip, currentPing.Status.ToString(), currentPing.TimeOut.ToString());
                dataGridView1.DataSource = table;
                label7.Text = "Successful Pings: " + countSuccess;
                label10.Text = "Time Elapsed: " + ping_timeElapsed.ToString();

                Console.WriteLine(DateTime.Now.TimeOfDay + " \t" + ip + " \t" + currentPing.Status.ToString() + " \t" + currentPing.TimeOut.ToString());
                Console.WriteLine("Successful Pings: " + countSuccess);
            }

        private void button2_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            // -------------------- Parameters used for Route Trace -----------------------
            string hostname = textBox6.Text;
            int timeOut = int.Parse( textBox8.Text ); // 1000ms or 1 second
            int max_ttl = Int32.Parse(textBox7.Text); //max number of servers allowed to be found
            const int bufferSize = 32;
            
            ipTraceActive = true;
            traceOut(hostname, timeOut, max_ttl, bufferSize);
           
        }

        private async void traceOut(String hostname, int timeOut, int max_ttl, int bufferSize)
        {
            int current_ttl = 0; //used for tracking how many servers have been found.
            Stopwatch s1 = new Stopwatch();
            Stopwatch s2 = new Stopwatch();

            byte[] buffer = new byte[bufferSize];
            new Random().NextBytes(buffer);

            Ping pinger = new Ping();
            
                WriteListBox($"Started ICMP Trace route on {hostname}");
                for (int ttl = 1; ttl <= max_ttl; ttl++)
                {

                if (ipTraceActive)
                {
                    current_ttl++;
                    s1.Start();
                    s2.Start();
                    PingOptions options = new PingOptions(ttl, true);
                    PingReply reply = null;
                    try
                    {
                        reply = await pinger.SendPingAsync(hostname, timeOut, buffer, options);
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
                else
                {

                    WriteListBox("Tracing Stopped! Sorry for Missing Results");
                    break;
                }
                }
            
        }

        private void WriteListBox(String text)
        {
            Console.WriteLine(text);
            listBox1.Items.Add(text);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ipTraceActive = false;
        }

    }

  
}
