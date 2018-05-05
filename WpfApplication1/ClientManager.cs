using SoundMixerServer.Networking;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SoundMixerServer
{
    public static class ClientMangager
    {
        public static bool acceptNewDevices = true;
        private static bool listenerRunning = false;
        public static Dictionary<IPAddress, ClientLogic> connectedClients = new Dictionary<IPAddress, ClientLogic>();

        /*Model*/
        public static Dictionary<string, ClientInformation> knownDevices = new Dictionary<string, ClientInformation>();

        /*ViewModel*/
        public static ObservableCollection<ClientInformation> deviceObservables { get {
                ObservableCollection<ClientInformation> col = new ObservableCollection<ClientInformation>();
                foreach (ClientInformation info in knownDevices.Values)
                {
                    col.Add(info.copy());
                }

                return col;
            }
        }

        public static void disconnectAllDevices()
        {
            foreach(ClientLogic c in connectedClients.Values)
            {
                c.disconnect();
            }
        }

        public static void saveDevices()
        {
            Devices dev = new Devices();
            dev.devices = knownDevices.Values.ToArray();

                XMLManager.WriteToXmlFile(Constants.DEVICES_DATA_PATH, dev);
            

        }

        private static void loadDevices()
        {
            if (File.Exists(Constants.DEVICES_DATA_PATH))
            {
                Devices dev = XMLManager.ReadFromXmlFile<Devices>(Constants.DEVICES_DATA_PATH);
                foreach (ClientInformation d in dev.devices)
                {
                    d.Connected = false;
                    if(!knownDevices.ContainsKey(d.id))
                    knownDevices.Add(d.id, d);
                }
                MainWindow.Instance.NotifyDeviceDatasetChanged();
            }
        }

        


    }

    [XmlRoot("Devices")]
    public class Devices
    {
        [XmlArray("devs")]
        [XmlArrayItem("ClientInfo")]
        public ClientInformation[] devices{ get; set; }
    }
}
