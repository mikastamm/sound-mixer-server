using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SoundMixerServer.Networking.Connections
{
    public class SslListener : Listener
    {
        private TcpListener listener = new TcpListener(IPAddress.Any, Constants.CLIENT_COMMUNICATION_TCP_PORT);

        public void startListening()
        {
            Task.Factory.StartNew(() => {
                Console.WriteLine("Listening for SSL Connection on port " + Constants.CLIENT_COMMUNICATION_TCP_PORT + "...");
                listener.Start();

                while (true)
                {
                    TcpClient client = listener.AcceptTcpClient();
                }
            });
        }

        private SslStream getSslStream(TcpClient client)
        {
            SslStream sslStream = new SslStream(client.GetStream(), false);
            byte[] certBytes = CertificateManager.CreateSelfSignCertificatePfx("CN=" + Constants.APPLICATION_NAME, DateTime.Now, DateTime.Now + TimeSpan.FromDays(15000));
            X509Certificate cert = new X509Certificate(certBytes);

            try
            {
                sslStream.AuthenticateAsServer(cert, false, SslProtocols.Tls, true);
                // Set timeouts for the read and write to 5 seconds.
                sslStream.ReadTimeout = 5000;
                sslStream.WriteTimeout = 5000;
            }
            catch(AuthenticationException e)
            {
                sslStream.Close();
                client.Close();
                throw e;
            }

            return sslStream;
        }

        public void stopListening()
        {
            listener.Stop();
        }
    }
}
