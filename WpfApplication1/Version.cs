using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SoundMixerServer
{
    public class Version
    {
        /// <summary>
        /// Gets the latest Version
        /// Format: *.*
        /// </summary>
        public static string latestVersion { get {
                if(string.IsNullOrEmpty(_latestVersion))
                {
                    _latestVersion = getLatestVersion();
                }

                return _latestVersion;
        } }
        private static string _latestVersion;

        /// <summary>
        /// Extracts the current Version from the AssemblyVersion.
        /// Format: Major.Minor
        /// </summary>
        public static string currentVersion { get {
                string productVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion;
                return productVersion.Substring(0, productVersion.IndexOf(".", productVersion.IndexOf(".") + 1));
        } }

        public static bool isUpToDate { get
            {
                bool ret = false;
                if(currentVersion == latestVersion)
                {
                    ret = true;
                }
                else if(latestVersion == "0.0.0.0")
                {
                    ret = true;
                }

                return ret;
            } }

        /// <summary>
        /// Downloads the latest Windows client version
        /// </summary>
        /// <returns>The latest Windows client version</returns>
        public static string getLatestVersion()
        {
            string version;
            using (WebClient client = new WebClient())
            {
                try
                {
                    version = client.DownloadString(Constants.SERVER_VERSION_URL);
                }
                catch
                {
                    version = "0.0.0.0";
                }
            }
            return version;
        }

    }
}
