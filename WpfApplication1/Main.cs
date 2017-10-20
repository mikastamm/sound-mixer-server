using System;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.IO;
using CoreAudio;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SoundMixerServer
{
    public class Main
    {
        public bool buttonPressed = false;

        public const int sleepTime = 5000;

        public IPAddress ipAddress = IPAddress.Any;

        public const int maxServerConnections = 3; //TODO: Add to settings

        private Type serverClass;

        public bool checkSEND_ICON = false;

        public static string serverName;

        public static VolumeServer info;

        public AudioManager audioManager;

        public Thread broadcastThread;


        public static Main Instance { get { return _instance; } }
        private static Main _instance;

        public static bool LOG_EVENTS = true;

        public enum AudioEventType
        {
            Added,
            Edited,
            Removed
        }

        public Main()
        {
            _instance = this;
            Console.WriteLine("##Starting Server##");
            Console.WriteLine("APPLICATION_DATA_ROOT_DIR" + Constants.APPLICATION_DATA_ROOT_DIR);

            serverName = Environment.MachineName;
            info = new VolumeServer() { name = serverName, hasPassword = AuthentificationManager.Instance.usesPassword, RSAPublicKey = VCCryptography.getPublicKey() };
            audioManager = new AudioManager();

            ClientListener.StartListener();

            if (!BroadcastReceiver.running)
            {
                broadcastThread = new Thread(new ThreadStart(BroadcastReceiver.findClients));
                broadcastThread.Start();
            }
            else
            {
                BroadcastReceiver.respondToNdRequests = true;
            }
        }

        public void Close()
        {
            Console.WriteLine("\n##Stopping Server##");
            BroadcastReceiver.respondToNdRequests = false;
            Console.WriteLine("Broadcastreceiver disabled");
            ClientListener.StopListener();
            audioManager.Close();
            ClientListener.disconnectAllDevices();
            Console.WriteLine("##Server Stopped##\n");
        }

    }
}
