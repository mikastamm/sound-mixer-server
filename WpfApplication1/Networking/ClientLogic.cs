using SoundMixerServer.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundMixerServer.Networking
{
    class ClientLogic
    {
        private Connection connection;
        private MessageSenderDispatcher messageSenderDispatcher;

        public HashSet<string> sendVolumeBlacklist = new HashSet<string>();
        public Dictionary<string, AudioSession> lastEditSent = new Dictionary<string, AudioSession>();

        public ClientLogic(Connection connection)
        {
            this.connection = connection;
        }

        public void init()
        {
            AudioHelper.registerAudioSessions();
            messageSenderDispatcher = new MessageSenderDispatcher(connection, this);
        }
    }
}
