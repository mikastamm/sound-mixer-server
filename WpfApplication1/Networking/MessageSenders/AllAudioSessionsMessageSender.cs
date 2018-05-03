using SoundMixerServer.Networking.Connections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundMixerServer.Networking.MessageSenders
{
    class AllAudioSessionsMessageSender
    {
        private Connection conn;
        private string tag = "REP";

        public AllAudioSessionsMessageSender(Connection conn)
        {
            this.conn = conn;
        }

        public void send(AudioSession[] toSend)
        {
            AudioSession[] finalSend = new AudioSession[toSend.Length];
            for (int i = 0; i < toSend.Length; i++)
            {
                toSend[i].volume = (float)Math.Round(toSend[i].volume, 2);
                finalSend[i] = toSend[i].toCodeId();
            }

            string data = tag + JSONManager.serialize(finalSend);
            conn.send(data);
        }
    }
}
