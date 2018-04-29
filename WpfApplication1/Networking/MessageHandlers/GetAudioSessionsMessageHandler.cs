using SoundMixerServer.Networking.MessageSenders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundMixerServer.Networking.MessageHandlers
{
    class GetAudioSessionsMessageHandler : MessageHandler
    {
        public const string Tag  = "GETAUDIOSESSIONS";
        private Connection conn;

        public GetAudioSessionsMessageHandler(Connection conn)
        {
            this.conn = conn;
        }

        public void handleMessage(string message)
        {
            new AllAudioSessionsMessageSender(conn).send(Main.Instance.audioManager.getDisplayableAudioSessions());
            ApplicationIcon[] icons = Main.Instance.audioManager.getSessionIcons();
            foreach (var icon in icons)
            {
                new AudioSessionImageSender(conn).send(icon);
            }
        }
    }
}
