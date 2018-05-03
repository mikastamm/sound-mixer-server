using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundMixerServer.Networking.MessageHandlers
{
    class TrackStartMessageHandler : MessageHandler
    {
        public const string Tag = "TRACK";
        private ClientLogic logic;

        public TrackStartMessageHandler(ClientLogic logic)
        {
            this.logic = logic;
        }

        public void handleMessage(string message)
        {
            message = message.Substring(Tag.Length+1);
            logic.sendVolumeBlacklist.Add(AudioSession.getSessionId(message));
        }
    }
}
