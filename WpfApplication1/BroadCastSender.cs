using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SoundMixerServer
{
    class BroadcastSender
    {
        public static void sendServerStartedBroadcast()
        {
            UdpClient client = new UdpClient();
            IPEndPoint ip = new IPEndPoint(IPAddress.Broadcast, Constants.REVERSE_DISCOVERY_UDP_PORT);
            byte[] bytes = Encoding.ASCII.GetBytes("VCHELLO");
            client.Send(bytes, bytes.Length, ip);
            client.Close();
        }
    }
}
