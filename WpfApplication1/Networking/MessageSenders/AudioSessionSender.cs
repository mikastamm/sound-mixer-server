using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundMixerServer.Networking.MessageSenders
{
    class AudioSessionSender
    {
        private Connection conn;

        public AudioSessionSender(Connection conn)
        {
            this.conn = conn;
        }

        public void send(string tag, AudioSession toSend)
        {
            toSend = toSend.toCodeId();
            toSend.volume = (float)Math.Round(toSend.volume, 2);
            string data = tag + JSONManager.serialize(toSend);
            conn.send(data);
        }
    }
}
