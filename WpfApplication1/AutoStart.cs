using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SoundMixerServer
{
    public static class AutoStart
    {
        static string AutostartDir = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
        static string ExePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
        static string shortcutFileName = "Sound Mixer Remote.lnk";

        /// <summary>
        /// Creates a shortcut in the Autostart Folder
        /// </summary>
        /// <returns>Wether the autostart File still exists</returns>
        public static bool MakeShortcut()
        {
            try
            {
                WshShell wsh = new WshShell();
                IWshShortcut shortcut = wsh.CreateShortcut(AutostartDir + "\\" + shortcutFileName) as IWshShortcut;
                shortcut.Arguments = "";
                shortcut.TargetPath = ExePath;
                // not sure about what this is for
                shortcut.WindowStyle = 1;
                shortcut.Description = "Sound Mixer Remote autostart link";
                shortcut.WorkingDirectory = Path.GetDirectoryName(ExePath);
                shortcut.IconLocation = "shell32.dll, 2";
                shortcut.Save();
            }
            catch{}
            return isInAutostart();
        }

        /// <summary>
        /// Removes the shortcut in the Autostart Folder
        /// </summary>
        /// <returns>Wether the autostart File still exists</returns>
        public static bool RemoveShortcut()
        {
            bool retVal = false;

            if(isInAutostart())
            {
                try
                {
                    System.IO.File.Delete(AutostartDir + "\\" + shortcutFileName);
                }
                catch
                {
                    retVal = true;
                }
            }

            return retVal;
        }

        public static bool isInAutostart()
        {
            return System.IO.File.Exists(AutostartDir + "\\" + shortcutFileName);
        }

    }
}
