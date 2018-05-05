using SoundMixerServer.Networking.Connections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundMixerServer.Networking
{
    class ListenerFactory
    {
        public static Listener listener
        {
            get {
                if (_listener == null)
                    _listener = new SocketListener();

                return _listener;
            }
        }

        private static Listener _listener;
    }
}
