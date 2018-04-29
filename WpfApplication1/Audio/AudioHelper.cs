using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundMixerServer.Audio
{
    class AudioHelper
    {
        public static void registerAudioSessions()
        {
            AudioSession[] array = Main.Instance.audioManager.getDisplayableAudioSessions();
            foreach (var p in array)
            {
                AudioSession.registerSessionID(p.id);
            }
        
        }
    }
}
