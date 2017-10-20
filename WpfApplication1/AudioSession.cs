using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace SoundMixerServer
{
    public class AudioSession
   {
        public float volume { get; set; }
        public bool mute { get; set; }
        public string title { get; set; }
        public string id { get; set; }

        [ScriptIgnore]
        public int pid;

        private static Pair<string, string>[] sessionIDCodes = new Pair<string, string>[256];

        public AudioSession toCodeId()
        {
            AudioSession ret = new AudioSession();
            ret.volume = volume;
            ret.mute = mute;
            ret.title = title;
            ret.pid = pid;
            ret.id = getCode(id);
            return ret;
        }

        public AudioSession toSessionId()
        {
            AudioSession ret = new AudioSession();
            ret.volume = volume;
            ret.mute = mute;
            ret.title = title;
            ret.pid = pid;
            ret.id = getSessionId(id);
            return ret;
        }

        public static string getCode(string id)
        {
            Pair<string, string> current;
            for (byte i = 0; i < sessionIDCodes.Length; i++)
            {
                if ((current = sessionIDCodes[i]) != null && current.v1 == id)
                {
                    return current.v2.ToString();
                }
            }
            return "-1";
        }

        public static string getSessionId(string code)
        {
            Pair<string, string> current;
            for (byte i = 0; i < sessionIDCodes.Length; i++)
            {
                if ((current = sessionIDCodes[i]) != null && current.v2 == code)
                {
                    return current.v1;
                }
            }
            return "";
        }

        public static bool registerSessionID(string id)
        {
            bool isSet = false;
            for (byte i = 0; i < sessionIDCodes.Length; i++)
            {
                if (sessionIDCodes[i] == null)
                {
                    sessionIDCodes[i] = new Pair<string, string>(id, i.ToString());
                    isSet = true;
                    break;
                }
            }
            return isSet;
        }

        public static bool removeSessionID(string id)
        {
            bool hasRemoved = false;

            for (byte i = 0; i < sessionIDCodes.Length; i++)
            {
                if (sessionIDCodes[i] != null && sessionIDCodes[i].v1 == id)
                {
                    sessionIDCodes[i] = null;
                    hasRemoved = true;
                    break;
                }
            }

            return hasRemoved;
        }

        public override string ToString()
        {
            return ("Title=" + title + "\nVolume=" + volume + "\nPid=" + pid + "\nMute=" + mute + "\nGuid="+id);
        }

        public override bool Equals(object obj)
        {
            AudioSession s;
            if((s = obj as AudioSession) == null)
            {
                return base.Equals(obj);
            }
            else
            {
                return s.volume == volume && s.mute == mute && s.title == title && s.id == id && s.pid == pid;
            }
        }
    }


    public class Pair<T, K>
    {
        public T v1;
        public K v2;

        public Pair(T v1, K v2)
        {
            this.v1 = v1;
            this.v2 = v2;
        }

        public Pair()
        {

        }
     }
}


