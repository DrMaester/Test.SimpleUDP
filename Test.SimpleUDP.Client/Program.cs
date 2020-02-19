using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Test.SimpleUDP.Shared.Model;
using Test.SimpleUDP.Shared.PaketTools;

namespace Test.SimpleUDP.Client
{
    // CLIENT
    class Program
    {
        static void Main(string[] args)
        {
            int paketSize = 8 * 1024;
            int port = 27005;
            IPAddress serverIP = IPAddress.Parse("127.0.0.1");
            IPEndPoint ipEndPoint = new IPEndPoint(serverIP, port);

            using (UdpClient client = new UdpClient())
            {
                Task.Run(() => Receive(client, ref ipEndPoint));

                while (true)
                {
                    var message = Console.ReadLine();
                    var pakets = PaketSplitter.GetPakets(paketSize, PaketType.Message, Encoding.UTF8.GetBytes(message));
                    foreach (var paket in pakets)
                    {
                        var paketBuffer = paket.ToBytes();
                        client.Send(paketBuffer, paketBuffer.Length);
                    }
                }
            }
        }

        private static void Receive(UdpClient client, ref IPEndPoint iPEndPoint)
        {
            client.Connect(iPEndPoint);
            while (true)
            {
                var paketBuffer = client.Receive(ref iPEndPoint);
                var paket = new Paket(paketBuffer);
                var message = Encoding.UTF8.GetString(paket.Data);
                Console.WriteLine(">> " + message);
            }
        }
    }
}
