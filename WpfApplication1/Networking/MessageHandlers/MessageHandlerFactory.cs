using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundMixerServer.Networking.MessageHandlers
{
    class MessageHandlerFactory
    {
        private Connection conn;
        private ClientLogic logic;

        public MessageHandlerFactory(Connection conn, ClientLogic logic)
        {
            this.conn = conn;
            this.logic = logic;
        }

        public MessageHandler GetMessageHandler(string message)
        {
            MessageHandler handler = null;
            string messageTag = message.Substring(0, message.IndexOf("{"));
            switch (messageTag)
            {
                case GetAudioSessionsMessageHandler.Tag:
                    handler = new GetAudioSessionsMessageHandler(conn);
                    break;
                case DeviceInfoMessageHandler.Tag:
                    handler = new DeviceInfoMessageHandler(logic);
                    break;
                case EditAudioSessionMessageHandler.Tag:
                    handler = new EditAudioSessionMessageHandler();
                    break;
                case TrackStartMessageHandler.Tag:
                    handler = new TrackStartMessageHandler(logic);
                    break;
                case EndTrackMessageHandler.Tag:
                    handler = new EndTrackMessageHandler(logic);
                    break;
            }

            return handler;
        }
    }
}
