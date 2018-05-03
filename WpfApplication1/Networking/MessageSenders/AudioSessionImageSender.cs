using SoundMixerServer.Networking.Connections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundMixerServer.Networking.MessageSenders
{
    class AudioSessionImageSender
    {
        private Connection connection;
        private string tag = "IMG";

        public AudioSessionImageSender(Connection connection)
        {
            this.connection = connection;
        }

        public void send(ApplicationIcon toSend)
        {
            toSend.id = AudioSession.getCode(toSend.id);
            string data = tag + JSONManager.serialize(toSend);
            connection.send(data);
        }
    }
}
