using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Test.SimpleUDP.Server.Model;
using Test.SimpleUDP.Shared.Model;

namespace Test.SimpleUDP.Server
{
    public class ChatServer
    {
        private Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        private EndPoint _endPoint;
        private readonly object _clientsLock;
        private List<ClientData> _clients;
        private const int _buffersize = 8 * 1024;

        public ChatServer(int port)
        {
            _clientsLock = new object();
            _clients = new List<ClientData>();
            _endPoint = new IPEndPoint(IPAddress.Any, port);

            _socket.Bind(_endPoint);
        }

        public void Start()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    byte[] buffer = new byte[_buffersize];
                    var readed = _socket.ReceiveFrom(buffer, ref _endPoint);
                    buffer = buffer.Take(readed).ToArray();

                    if (!_clients.Any(client => client.IPEndPoint.Address.ToString() == ((IPEndPoint)_endPoint).Address.ToString() &&
                        client.IPEndPoint.Port == ((IPEndPoint)_endPoint).Port))
                    {
                        lock (_clientsLock)
                        {
                            _clients.Add(new ClientData(((IPEndPoint)_endPoint)));
                        }
                    }

                    var senderId = GetIdFromEndPoint(((IPEndPoint)_endPoint));
                    BroadcastMessageAsync(buffer, senderId);
                    var paket = new Paket(buffer);
                    string message = Encoding.UTF8.GetString(paket.Data);
                    Console.WriteLine($"Message from: {senderId}({_endPoint}): {message}");
                }

            });
            Console.WriteLine($"server running on {_endPoint}");
        }
        
        private string GetIdFromEndPoint(IPEndPoint iPEndPoint)
        {
            lock (_clientsLock)
            {
                return _clients.FirstOrDefault(client => client.IPEndPoint.Address.ToString() == iPEndPoint.Address.ToString() &&
                        client.IPEndPoint.Port == iPEndPoint.Port).Id.ToString();
            }
        }

        private void BroadcastMessageAsync(byte[] buffer, string senderId)
        {
            lock (_clientsLock)
            {
                foreach (var client in _clients)
                {
                    if (!client.Connected || client.Id.ToString() == senderId)
                        continue;

                    try
                    {
                        _socket.SendTo(buffer, client.IPEndPoint);
                    }
                    catch (InvalidOperationException ex)
                    {
                        client.Connected = false;
                    }
                }
            }
        }
    }
}
