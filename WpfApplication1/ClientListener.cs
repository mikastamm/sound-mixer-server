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
            } }


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

        private static void mainListener()
        {
            listenerRunning = true;
            Console.WriteLine("MainListener: Thread Started");
            // Alle Netzwerk-Schnittstellen abhören
            TcpListener listener = new TcpListener(IPAddress.Any, Constants.CLIENT_COMMUNICATION_TCP_PORT);
            Console.WriteLine("Listening on port " + Constants.CLIENT_COMMUNICATION_TCP_PORT + "...");
            loadDevices();

                listener.Start();
                // Solange Clients akzeptieren, bis das
                // angegebene Maximum erreicht ist
                while (true)
                {
                    //while (!listener.Pending()) { Thread.Sleep(sleepTime); }
                    Socket newSocket = listener.AcceptSocket();
                    if (newSocket != null)
                    {
                        // Mitteilung bzgl. neuer Clientverbindung
                        Console.WriteLine("New Client (" +
                            "IP: " + newSocket.RemoteEndPoint + ", " +
                            "Port " + ((IPEndPoint)newSocket.LocalEndPoint).Port.ToString() + ")");

                        //Connected to device, übergabe an ClientConnection object for authentification and sending of data
                        if (acceptNewDevices)
                        {
                    /*        ClientConnection client = new ClientConnection(newSocket);
                        if (connectedClients.ContainsKey(client.clientEP.Address))
                        {
                            connectedClients[client.clientEP.Address].disconnect();
                            connectedClients.Remove(client.clientEP.Address);
                        }
                            connectedClients.Add(client.clientEP.Address, client);

                            Task.Factory.StartNew(() => { client.initializeConnection(); });*/
                            //Moved to ClientLogic
                        }
                    }
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
