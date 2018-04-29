using SoundMixerServer.Networking.MessageSenders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundMixerServer.Networking
{
    class AudioSessionRemoveMessageSender
    {
        private Connection connection;
        private string tag = "DEL";

        public AudioSessionRemoveMessageSender(Connection connection)
        {
            this.connection = connection;
        }

        public void send(AudioSession toSend)
        {
            new AudioSessionSender(connection).send(tag, toSend);
            AudioSession.removeSessionID(toSend.id);
        }
    }
}
