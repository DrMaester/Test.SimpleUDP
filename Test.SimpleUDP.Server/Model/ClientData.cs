using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;

namespace Test.SimpleUDP.Server.Model
{
    public class ClientData
    {
        public Guid Id { get; set; }
        public IPEndPoint IPEndPoint { get; set; }
        public bool Connected { get; set; }

        public ClientData(IPEndPoint iPEndPoint)
        {
            Id = Guid.NewGuid();
            IPEndPoint = iPEndPoint;
            Connected = true;
        }

    }
}
