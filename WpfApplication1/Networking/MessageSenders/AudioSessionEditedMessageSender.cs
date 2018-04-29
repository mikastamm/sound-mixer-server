using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundMixerServer.Networking.MessageSenders
{
    class AudioSessionEditedMessageSender
    {
        private Connection conn;
        private string tag = "EDIT";
        private ClientLogic logic;

        public AudioSessionEditedMessageSender(Connection conn, ClientLogic logic)
        {
            this.conn = conn;
            this.logic = logic;
        }

        public void send(AudioSession session)
        {
            AudioSession lastEdit;
            bool firstEdit = !logic.lastEditSent.TryGetValue(session.id, out lastEdit);

            if (!logic.sendVolumeBlacklist.Contains(session.id) && (firstEdit || !lastEdit.Equals(session)))
            {
                new AudioSessionSender(conn).send(tag, session);
                logic.lastEditSent[session.id] = session;
            }
        }
    }
}
