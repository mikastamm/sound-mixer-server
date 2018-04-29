using SoundMixerServer.Audio;
using SoundMixerServer.Networking.MessageHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SoundMixerServer.Networking
{
    class ClientLogic
    {
        private Connection connection;
        private MessageSenderDispatcher messageSenderDispatcher;

        public ClientInformation device;

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
            receive();
        }

        public void disconnect()
        {
            if (device != null)
            {
                device.Connected = false;
                device.LastConnected = DateTime.Now;
                ClientListener.connectedClients.Remove(IPAddress.Parse(device.IP));
                MainWindow.Instance.NotifyDeviceDatasetChanged();
            }
            connection.disconnect();
        }

        private void receive()
        {
            string received;
            MessageHandlerFactory factory = new MessageHandlerFactory(connection, this);
            while((received = connection.readLine()) != null)
            {
                MessageHandler handler = factory.GetMessageHandler(received);
                handler.handleMessage(received);
            }
        }
    }
}
