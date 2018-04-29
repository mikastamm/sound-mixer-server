using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundMixerServer.Networking
{
    interface Connection
    {
        void send(string data);
        string readLine();
        void disconnect();
    }
}
