using System;
using System.Threading.Tasks;
using ServerData;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Collections.Generic;

namespace Client
{
    class Client
    {
        private static Socket socket;
        private static string name;
        private static string id;
        private static Random _random = new Random();
        private static bool _isDisconnected;


        static void Main(string[] args)
        {
            Console.WriteLine("Enter Your Name: ");
            name = Console.ReadLine();

            Console.Clear();
            Console.WriteLine("Enter your ip:");
            string hostIp = Console.ReadLine();

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPEndPoint ip = new IPEndPoint(IPAddress.Parse(hostIp), 5555);

            try
            {
                socket.Connect(ip);
            }
            catch
            {
                Console.WriteLine("Could not connect to server");
                Thread.Sleep(1000);
            }

            Initiate();
        }

        private static void Initiate()
        {
            Task t = new Task(SendData);
            t.Start();

            while (true)
            {
                if (_isDisconnected) break;
                Console.Write("--->");
                List<string> messages = MockMessages();
                string input = messages[_random.Next(messages.Count)];

                Packet p = new Packet(PacketType.chat, id);
                p.Gdata.Add(name);
                p.Gdata.Add(input);
                Thread.Sleep(2000);
                if (!_isDisconnected) socket.Send(p.toBytes());

            }
            Console.WriteLine("Press any key to exit.......");
        }

        private static List<string> MockMessages()
        {
            return new List<string>
            {
                "Privet",
                "Hi there",
                "How are you doing",
                "Is that okay?",
                "cool :)",
                "let's make a short chat"
            };
        }

        static void SendData()
        {
            byte[] buffer;
            int readBytes;

            while (true)
            {
                try
                {
                    buffer = new byte[socket.SendBufferSize];
                    readBytes = socket.Receive(buffer);

                    if (readBytes > 0)
                    {
                        DataManager(new Packet(buffer));
                    }
                }
                catch (SocketException ex)
                {
                    _isDisconnected = true;
                    Console.WriteLine("Server disconnected!");
                    Console.ReadLine();
                    Environment.Exit(0);
                }
            }

        }

        static void DataManager(Packet p)
        {
            switch (p.packetType)
            {
                case PacketType.Registration:
                    Console.WriteLine("Connected to Server!");
                    id = p.Gdata[0];
                    break;
                case PacketType.chat:
                    Console.WriteLine(p.Gdata[0] + ": " + p.Gdata[1]);
                    break;
            }
        }
    }


}

