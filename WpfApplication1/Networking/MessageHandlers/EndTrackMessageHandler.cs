using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundMixerServer.Networking.MessageHandlers
{
    class EndTrackMessageHandler : MessageHandler
    {
        public const string Tag = "ENDTRACK";
        private ClientLogic logic;

        public EndTrackMessageHandler(ClientLogic logic)
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
