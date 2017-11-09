using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SoundMixerServer
{
    [XmlType("ClientInfo")]
    public class ClientInformation
    {
        public string Name { get; set; }
        public string RSAKeyJSON { get; set; }
        [XmlIgnore]
        public RSAKeyValue RSAKey { get { return JSONManager.deserialize<RSAKeyValue>(RSAKeyJSON); } }
        [XmlIgnore]
        public DateTime verifiedUntil { get; set; } = new DateTime(2000, 1, 1);
        public double Version { get; set; }
        [XmlIgnore]
        public string ID { get { return Name + RSAKeyJSON; } }
        public string IP { get; set; }
        public bool Connected { get; set; }
        public string ConnectedString { get { return isEmpty ? "" : (Connected ? "yes" : "no"); } }
        public DateTime LastConnected { get; set; }
        public string LastConnectedString { get { return isEmpty ? "" : LastConnected.ToString(); } }

        //Used to produce an empty ClientInformation object to fill DataGrid up with empty rows
        [XmlIgnore]
        private bool isEmpty = false; 
        [XmlIgnore]
        public static ClientInformation Empty = new ClientInformation() { Name = "", IP = "", isEmpty = true };


        public ClientInformation copy()
        {
            return new ClientInformation()
            {
                Connected = Connected,
                IP = IP,
                LastConnected = LastConnected,
                Name = Name,
                RSAKeyJSON = RSAKeyJSON,
                Version = Version,
                verifiedUntil = verifiedUntil
            };
            
        }

    }
}
