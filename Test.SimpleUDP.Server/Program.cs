using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Test.SimpleUDP.Shared.Model;
using Test.SimpleUDP.Shared.PaketTools;
using System.Linq;

namespace Test.SimpleUDP.Server
{
    // SERVER
    class Program
    {
        private static List<IPEndPoint> _endPoints = new List<IPEndPoint>();

        static void Main(string[] args)
        {
            var port = 27005;

            var chatServer = new ChatServer(port);
            chatServer.Start();

            Console.ReadKey();
        }
    }
}
