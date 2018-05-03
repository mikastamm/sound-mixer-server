using SoundMixerServer.Networking.Connections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SoundMixerServer.Networking
{
    class SocketListener : Listener
    {
        private TcpListener listener = new TcpListener(IPAddress.Any, Constants.CLIENT_COMMUNICATION_TCP_PORT);

        public void startListening()
        {
            Task.Factory.StartNew(() => {
                Console.WriteLine("Listening on port " + Constants.CLIENT_COMMUNICATION_TCP_PORT + "...");
                listener.Start();

                while (true)
                {
                    Socket newSocket = listener.AcceptSocket();
                    if (newSocket != null)
                    {
                        Console.WriteLine("New Client (" +
                            "IP: " + newSocket.RemoteEndPoint + ", " +
                            "Port " + ((IPEndPoint)newSocket.LocalEndPoint).Port.ToString() + ")");

                        //Connected to device, übergabe an ClientConnection object for authentification and sending of data
                        ClientLogic client = new ClientLogic(new SocketConnection(newSocket));
                        Task.Factory.StartNew(() => { client.init(); });

                    }
                }
            });
        }

        public void stopListening()
        {
            listener.Stop();
        }
    }
}
