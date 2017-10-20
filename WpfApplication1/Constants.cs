using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundMixerServer
{
    public static class Constants
    {
        //Application
        readonly public static string APPLICATION_NAME = "VolumeControlDesktop";

        //File Paths
        readonly public static string APPLICATION_DATA_ROOT_DIR = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\" + APPLICATION_NAME + @"\";
        readonly public static string AUTH_DATA_PATH = APPLICATION_DATA_ROOT_DIR + "AUTH.xml";
        readonly public static string DEVICES_DATA_PATH = APPLICATION_DATA_ROOT_DIR + "DEVICES.xml";

        //Ports
        readonly public static int NETWORK_DISCOVERY_UDP_PORT = 11045;
        readonly public static int NETWORK_DISCOVERY_TCP_PORT = NETWORK_DISCOVERY_UDP_PORT;
        readonly public static int AUTHENTIFICATION_TCP_PORT = 11046;
        readonly public static int CLIENT_COMMUNICATION_TCP_PORT = 11047;

        //Audio stuff
        public const string MASTER_AUDIO_SESSION_ID = "DEVICE"; //If changed, also change on android application
    }
}
