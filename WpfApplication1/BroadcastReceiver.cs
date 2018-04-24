using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace SoundMixerServer
{
    public class BroadcastReceiver
    {
        public static bool respondToNdRequests = true;
        public static bool running = false;

        public static void findClients()
        {
            running = true;
                using (var udpClient = new UdpClient(Constants.NETWORK_DISCOVERY_UDP_PORT))
                {

                    Console.WriteLine("Starting NetworkDiscovery receiver");
                    udpClient.EnableBroadcast = true;

                    while (true)
                    {
                        string loggingEvent = "";
                        //IPEndPoint object will allow us to read datagrams sent from any source.
                        var remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                        var receivedResults = udpClient.Receive(ref remoteEndPoint);
                        loggingEvent += Encoding.ASCII.GetString(receivedResults);
                        Console.WriteLine("Received ND Request: " + loggingEvent + " from " + remoteEndPoint.ToString());

                        if (loggingEvent == "VC_HELLO")
                            if(respondToNdRequests)
                                sendServerInfo(new IPEndPoint(remoteEndPoint.Address, Constants.NETWORK_DISCOVERY_TCP_PORT));
                            else
                            Console.WriteLine("Did not respond to broadcast; Receiver disabled");
                        else
                            Console.WriteLine("Did not respond to broadcast; Wrong request");
                    }
                }
            running = false;
        }

        public static void sendServerInfo(IPEndPoint clientEP)
        {
            TcpClient server;

            try
            {
                server = new TcpClient();
                server.Connect(clientEP);
            }
            catch (SocketException e)
            {
                Console.WriteLine("Unable to connect to server: " + e.Message);
                
                return;
            }
            NetworkStream ns = server.GetStream();

            string serverInfo = JSONManager.serialize(Main.info);
            byte[] serverInfoBytes = Encoding.ASCII.GetBytes(serverInfo);

            ns.Write(serverInfoBytes, 0, serverInfoBytes.Length);
            Console.WriteLine("Sent Server Info to: " + clientEP.ToString());
            ns.Close();
            server.Close();
        }
    }

    public class VolumeServer
    {
        public bool hasPassword { get; set; }
        public string name { get; set; }
        public string id { get; set; }
    }
}
