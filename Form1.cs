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
            table.Columns.Add("RoundTripTime (ms)");
        }
        

        private void button1_Click_1(object sender, EventArgs e)
        {
            countSuccess = 0;
            table.Clear();
            defaultIP = textBox1.Text + "." + textBox2.Text + "." + textBox3.Text + ".";
            startingIndex = int.Parse(textBox4.Text);
            endingIndex = int.Parse(textBox5.Text);

            int accuracyLevel = int.Parse(textBox9.Text);
            int timeOut = int.Parse(textBox11.Text);

            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();
            for (int i=startingIndex; i<=endingIndex; i++)
            {
               pingingOnTheWay(defaultIP + i, accuracyLevel, timeOut);
            }
            stopwatch.Stop();
            ping_timeElapsed = stopwatch.Elapsed;
        }

        
        private async Task pingingOnTheWay(String ip, int accuracyLevel, int timeOut)
        {

            long currentRoundTripTime = 0;
            MyPIng currentPing = new MyPIng();
            List<IPStatus> replyTestResults = new List<IPStatus>();
            currentPing.Ip = ip;

            Ping ping = new Ping();

            for (int i = 0; i < int.Parse(textBox9.Text); i++)
            {
                
                PingReply pingReplyTest = await ping.SendPingAsync(ip, timeOut);
                currentRoundTripTime += pingReplyTest.RoundtripTime;
                replyTestResults.Add(pingReplyTest.Status);

                Console.WriteLine(ip + " \t" + pingReplyTest.Status.ToString() + " \t" + pingReplyTest.RoundtripTime.ToString());
            }

            currentPing.Status = mode(replyTestResults);
            currentPing.RoundTripTime = currentRoundTripTime;

                if (currentPing.Status == IPStatus.Success)
                    countSuccess++;

                table.Rows.Add(DateTime.Now.TimeOfDay, ip, currentPing.Status.ToString(), currentPing.RoundTripTime.ToString());
                dataGridView1.DataSource = table;
                label7.Text = "Successful Pings: " + countSuccess;
                label10.Text = "Time Elapsed: " + ping_timeElapsed.ToString();

                Console.WriteLine(ip + " \t" + currentPing.Status.ToString() + " \t" + currentPing.RoundTripTime.ToString());
                Console.WriteLine("Successful Pings: " + countSuccess);
            }

        private void button2_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            // -------------------- Parameters used for Route Trace -----------------------
            string hostname = textBox6.Text;
            int timeOut = int.Parse( textBox8.Text );
            int max_ttl = Int32.Parse( textBox7.Text ); //max number of servers allowed to be found
            int accuracyLevel = int.Parse(textBox10.Text);  // no. of iterations of sending ping per ip
            const int bufferSize = 32;
            
            ipTraceActive = true;
            traceOut(hostname, accuracyLevel, timeOut, max_ttl, bufferSize);
           
        }

        private async void traceOut(String hostname, int accuracyLevel, int timeOut, int max_ttl, int bufferSize)
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
                    
                    MyPIng reply = new MyPIng();
                    List<IPStatus> replyTestResults = new List<IPStatus>();

                    try
                    {
                        for(int i=0; i< accuracyLevel; i++)
                        {
                            PingReply replyTest = null;
                            replyTest = await pinger.SendPingAsync(hostname, timeOut, buffer, options);

                            reply.Ip = replyTest.Address.ToString();

                            replyTestResults.Add(replyTest.Status);
                            Console.WriteLine(replyTest.Address + " \t" + replyTest.Status + "\t" + s1.ElapsedMilliseconds);
                        }
                        reply.Status = mode(replyTestResults);
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
                            WriteListBox($"[{ttl}] - Route: {reply.Ip} - Time: {s1.ElapsedMilliseconds} ms - Total Time: {s2.ElapsedMilliseconds} ms");
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

        static IPStatus mode(List<IPStatus> statusList)
        {
            IPStatus maxValue = 0;
            int maxCount = 0, i, j;

            for (i = 0; i < statusList.Count; ++i)
            {
                int count = 0;
                for (j = 0; j < statusList.Count; ++j)
                {
                    if (statusList[j] == statusList[i])
                        ++count;
                }

                if (count > maxCount)
                {
                    maxCount = count;
                    maxValue = statusList[i];
                }
            }
            return maxValue;
        }

    }

  
}
