using ServerData;
using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    class ClientData
    {
        public Socket clientSocket;
        public Task clientTask;
        public string id;

        public ClientData()
        {
            id = Guid.NewGuid().ToString();
            Task task = new Task(() => Server.SendData(clientSocket));
            task.Start();
            SendRegistrationPacket();
        }

        public ClientData(Socket clientSocket)
        {
            this.clientSocket = clientSocket;
            id = Guid.NewGuid().ToString();
            clientTask = new Task(() => Server.SendData(clientSocket));
            clientTask.Start();
            SendRegistrationPacket();
        }

        public void SendRegistrationPacket()
        {
            Packet p = new Packet(PacketType.Registration, "server");
            p.Gdata.Add(id);
            clientSocket.Send(p.toBytes());
        }
    }
}
