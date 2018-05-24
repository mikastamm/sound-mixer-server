using SoundMixerServer.Networking.Connections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundMixerServer.Networking.MessageSenders
{
    class NotAuthenticatedMessageSender
    {
        private Connection conn;
        private string Tag = "ACCESSDENIED";

        public NotAuthenticatedMessageSender(Connection conn)
        {
            this.conn = conn;
        }

        public void send()
        {
            conn.send(Tag);
        }
    }
}
