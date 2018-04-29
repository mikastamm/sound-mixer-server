using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundMixerServer.Networking.MessageHandlers
{
    class DeviceInfoMessageHandler : MessageHandler
    {
        public const string Tag = "DEVINFO";

        private ClientLogic logic;

        public DeviceInfoMessageHandler(ClientLogic logic)
        {
            this.logic = logic;
        }

        public void handleMessage(string message)
        {
            message = message.Substring("DEVINFO".Length);

            ClientInformation dev = new ClientInformation();
            dev = JSONManager.deserialize<ClientInformation>(message);
            dev.LastConnected = DateTime.Now;
            //dev.IP = clientEP.Address.ToString();
            dev.Connected = true;

            if(ClientListener.knownDevices.ContainsKey(dev.id))
            {
                ClientListener.knownDevices[dev.id] = dev;
            }
            else
            {
                ClientListener.knownDevices.Add(dev.id, dev);
            }

            ClientListener.saveDevices();
            logic.device = dev;
            MainWindow.Instance.NotifyDeviceDatasetChanged();
        }
    }
}
