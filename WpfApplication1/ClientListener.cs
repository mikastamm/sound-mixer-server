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
    public static class ClientListener
    {
        private static bool acceptNewDevices = true;
        private static bool listenerRunning = false;
        public static Thread listenerThread;
        public static Dictionary<IPEndPoint, ClientConnection> connectedClients = new Dictionary<IPEndPoint, ClientConnection>();

        /*Model*/
        public static Dictionary<string, ClientInformation> knownDevices = new Dictionary<string, ClientInformation>();

        /*ViewModel*/
        public static ObservableCollection<ClientInformation> deviceObservables { get {
                ObservableCollection<ClientInformation> col = new ObservableCollection<ClientInformation>();
                foreach (ClientInformation info in knownDevices.Values)
                {
                    col.Add(info.copy());
                }
                ////Fill the empty space in the DataGrid with empty ClientInformation objects, purely visual
                //for(int i = 0; i < 10; i++)
                //{
                //    col.Add(ClientInformation.Empty);   
                //}

                return col;
            } }


        public static void disconnectAllDevices()
        {
            foreach(ClientConnection c in connectedClients.Values)
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
                    if(!knownDevices.ContainsKey(d.ID))
                    knownDevices.Add(d.ID, d);
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
                            ClientConnection client = new ClientConnection(newSocket);
                            connectedClients.Add(client.clientEP, client);
                            Task.Factory.StartNew(() => { client.initializeConnection(); });
                        }
                    }
                }


            // catch (Exception ex)
            // {
            //    throw new Exception("Fehler bei Verbindungserkennung", ex);
            // }
            listenerRunning = false;
        }

        public static void StopListener()
        {
            if (listenerThread != null)
            {
                acceptNewDevices = false;
                Console.WriteLine("Listener Thread Stopped");
            }
            else
            {
                Console.WriteLine("Listener Thread is Null");
            }
        }

        public static void StartListener()
        {
            try
            {
                if (!listenerRunning)
                {
                    Thread thread = new Thread(new ThreadStart(mainListener));
                    thread.Start();
                    Console.WriteLine("Listener Thread started");
                    listenerThread = thread;
                }
                else
                {
                    acceptNewDevices = true;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
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
