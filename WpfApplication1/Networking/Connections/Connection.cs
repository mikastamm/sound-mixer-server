using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SoundMixerServer.Networking.Connections
{
    public interface Connection
    {
        IPEndPoint IP { get; set; }
        void send(string data);
        string readLine();
        void disconnect();
    }
}
