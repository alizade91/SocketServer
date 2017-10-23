using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Sockets;
using ServerData;
using System.Net;
using System.Text;
using System.Threading;

namespace Server
{
    class Server
    {
        static Socket listenerSocket;
        static List<ClientData> _clients;


        static void Main(string[] args)
        {
            Console.WriteLine("Starting Server...");
            listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _clients = new List<ClientData>();

            IPEndPoint ip = new IPEndPoint(IPAddress.Parse(Packet.getIp4Address()), 5555);
            listenerSocket.Bind(ip);

            Task listener = new Task(ListenThread);
            listener.Start();
            Console.WriteLine("Success... Listening IP: " + Packet.getIp4Address() + ":5555");
            Console.ReadKey();
        }

        static void ListenThread()
        {
            while (true)
            {
                listenerSocket.Listen(0);
                _clients.Add(new ClientData(listenerSocket.Accept()));
            }
        }

        public static void SendData(object cSocket)
        {
            Socket clientSocket = (Socket)cSocket;

            byte[] Buffer;
            int readBytes;

            while (true)
            {

                try
                {
                    Buffer = new byte[clientSocket.SendBufferSize];
                    readBytes = clientSocket.Receive(Buffer);

                    if (readBytes > 0)
                    {
                        Packet p = new Packet(Buffer);
                        dataManager(p);
                    }
                }
                catch (SocketException ex)
                {
                    Console.WriteLine("Client Disconnected.");
                }
            }
        }

        public static void dataManager(Packet p)
        {
            switch (p.packetType)
            {
                case PacketType.chat:
                    foreach (ClientData c in _clients)
                    {
                        c.clientSocket.Send(p.toBytes());
                    }
                    break;
            }
        }
    }
}
