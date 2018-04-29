using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundMixerServer.Networking
{
    interface MessageHandler
    {
        string Tag { get; set; }
        void handleMessage(string message);
    }
}
