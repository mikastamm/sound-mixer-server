using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundMixerServer.Networking.MessageHandlers
{
    class EditAudioSessionMessageHandler : MessageHandler
    {
        public const string Tag = "EDIT";

        public void handleMessage(string message)
        {
            message = message.Substring(Tag.Length);
            AudioSession session = JSONManager.deserialize<AudioSession>(message).toSessionId();
            Main.Instance.audioManager.updateChangedSession(session);
        }
    }
}
