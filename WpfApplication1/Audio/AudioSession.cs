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

        private static Dictionary<string, string> sessionIDCodes = new Dictionary<string, string>();

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
            return sessionIDCodes.ContainsKey(id) ? sessionIDCodes[id] : "-1";
        }

        public static string getSessionId(string code)
        {
            return sessionIDCodes.ContainsValue(code) ? sessionIDCodes.FirstOrDefault(x => x.Value == code).Key : "";
        }

        public static bool registerSessionID(string id)
        {
            bool isSet = false;

            if(getCode(id) == "-1")
            {
                byte? idCode = IDCodes.claim();
                if (idCode != null)
                {
                    sessionIDCodes.Add(id, idCode.ToString());
                    isSet = true;
                }
            }
            else
            {
                isSet = true;
            }

            return isSet;
        }

        public static bool removeSessionID(string id)
        {
            bool hasRemoved = false;

            if(sessionIDCodes.ContainsKey(id))
            {
                string IDCode = sessionIDCodes[id];
                sessionIDCodes.Remove(id);
                IDCodes.free(byte.Parse(IDCode));
                hasRemoved = true;
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


