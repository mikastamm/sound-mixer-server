using SoundMixerServer.Networking.Connections;
using SoundMixerServer.Networking.MessageSenders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundMixerServer.Networking.MessageHandlers
{
    public class AuthenticationMessageHandlerRunner : MessageHandler
    {
        private MessageHandler handler;
        private Authentication auth;
        private Connection connection;

        public AuthenticationMessageHandlerRunner(MessageHandler handler, Authentication auth, Connection connection)
        {
            this.handler = handler;
            this.auth = auth;
            this.connection = connection;
        }

        public void handleMessage(string message)
        {
            if(handler.GetType() == typeof(DeviceInfoMessageHandler) || auth.isAuthenticated())
            {
                handler.handleMessage(message);
            }
            else
            {
                new NotAuthenticatedMessageSender(connection).send();   
            }
        }
    }
}
