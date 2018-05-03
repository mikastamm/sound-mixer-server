using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SoundMixerServer.Networking.Connections
{
    class SocketConnection : Connection, IDisposable
    {
        private Socket socket;
        public IPEndPoint IP { get; set; }
        private StreamReader sr;

        public SocketConnection(Socket socket)
        {
            this.socket = socket;
            sr = new StreamReader(new NetworkStream(socket, false), Encoding.UTF8);
            IP = socket.RemoteEndPoint as IPEndPoint;
        }

        public void disconnect()
        {
            socket.Close();
            sr.Close();
        }

        public void Dispose()
        {
            disconnect();
        }

        public string readLine()
        {
            return sr.ReadLine();
        }

        public void send(string data)
        {
            try
            {
                socket.Send(Encoding.ASCII.GetBytes(data + "\r\n"));
                Console.WriteLine("SENT:" + data);
            }
            catch (SocketException se)
            {
                Console.WriteLine("Couldnt send: " + se.ToString());
            }
            catch (ObjectDisposedException ex)
            {
                Console.WriteLine("Couldnt send: " + ex.ToString());
            }
        }
    }
}
