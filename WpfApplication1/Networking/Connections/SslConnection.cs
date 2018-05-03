using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SoundMixerServer.Networking.Connections
{
    class SslConnection : Connection, IDisposable
    {
        public IPEndPoint IP { get; set; }
        private SslStream stream;
        private TcpClient client;

        public SslConnection(SslStream stream, TcpClient client) {this.stream = stream; this.client = client; }

        public void send(string data)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            stream.Write(bytes);
        }

        public string readLine()
        {
            // Read the  message sent by the client.
            // The client signals the end of the message using the
            // "<EOF>" marker.
            byte[] buffer = new byte[2048];
            StringBuilder messageData = new StringBuilder();
            int bytes = -1;
            do
            {
                bytes = stream.Read(buffer, 0, buffer.Length);

                Decoder decoder = Encoding.UTF8.GetDecoder();
                char[] chars = new char[decoder.GetCharCount(buffer, 0, bytes)];
                decoder.GetChars(buffer, 0, bytes, chars, 0);
                messageData.Append(chars);
                // Check for EOF or an empty message.
                if (messageData.ToString().IndexOf("<EOF>") != -1)
                {
                    break;
                }
            } while (bytes != 0);

            return messageData.ToString();
        }

        public void disconnect()
        {
            stream.Close();
            client.Close();
        }

        public void Dispose()
        {
            disconnect();
        }
    }
}
