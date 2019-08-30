﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Ping_Checking_System
{
    class MyPIng
    {
        String ip;
        String deviceName = "undefined";
        long roundTripTime;
        IPStatus status;

        public string Ip { get => ip; set => ip = value; }
        public string DeviceName { get => deviceName; set => deviceName = value; }
        public long RoundTripTime { get => roundTripTime; set => roundTripTime = value; }
        public IPStatus Status { get => status; set => status = value; }
    }
    
}
