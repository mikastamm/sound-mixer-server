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

            if(ClientMangager.knownDevices.ContainsKey(dev.id))
            {
                ClientMangager.knownDevices[dev.id] = dev;
            }
            else
            {
                ClientMangager.knownDevices.Add(dev.id, dev);
            }

            ClientMangager.saveDevices();
            logic.device = dev;
            MainWindow.Instance.NotifyDeviceDatasetChanged();
        }
    }
}
