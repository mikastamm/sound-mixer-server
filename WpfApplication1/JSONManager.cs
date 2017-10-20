using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace SoundMixerServer
{
    public class JSONManager
    {
        public static string serialize(object toSerialize)
        {
            return new JavaScriptSerializer().Serialize(toSerialize);
        }

        public static T deserialize<T>(string json)
        {
            return new JavaScriptSerializer().Deserialize<T>(json);
        }
    }
}
