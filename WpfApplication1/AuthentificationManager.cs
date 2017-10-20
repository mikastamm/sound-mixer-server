using System;


namespace SoundMixerServer
{
    public class AuthentificationManager
    {
        public bool usesPassword { get { return authData.usesPassword; } }
        public bool passwordDisabled { get { return !authData.usesPassword && !string.IsNullOrEmpty(authData.PasswordHash); } }

        private AuthentificationData authData;

        public static AuthentificationManager Instance { get { if (_instance == null) _instance = new AuthentificationManager(); return _instance; }}
        private static AuthentificationManager _instance;

        private const string SALT = "1AQQB-90KXZ-Z1Y91-UINT8";

        private AuthentificationManager()
        {
            authData = AuthentificationData.load();
            if(authData == null)
            {
                Console.WriteLine("Auth data not found, creating file...");
                authData = new AuthentificationData();

                authData.usesPassword = false;
                authData.PasswordHash = "";
                authData.save();
            }

        }

        public string getPasswordHash() //TODO: Remove
        {
            return authData.PasswordHash;
        }

        public bool checkPasswordHash(string passwordHash)
        {
            return authData.PasswordHash.Equals(passwordHash, StringComparison.InvariantCultureIgnoreCase);
        }

        public void setNewPassword(string password)
        {
            authData.PasswordHash = (SALT + password).GetMD5Hash();
            authData.usesPassword = true;
            authData.save(); 
        }

        public void enablePassword()
        {
            authData.usesPassword = true;
            authData.save();
        }

        public void disablePassword()
        {
            authData.usesPassword = false;
            authData.save();
        }

        public void removePassword()
        {
            authData.usesPassword = false;
            authData.PasswordHash = "";
            authData.save();
        }
    }

    public class AuthentificationData
    {
        public bool usesPassword { get; set; }
        public string PasswordHash { get; set; }
    
        /// <summary>
        /// Gets the AuthentificationData object from AppData
        /// </summary>
        /// <returns></returns>
        public static AuthentificationData load()
        {
            return XMLManager.ReadFromXmlFile<AuthentificationData>(Constants.AUTH_DATA_PATH);
        }

        /// <summary>
        /// Saves this AuthentificationData object to AppData.
        /// </summary>
        /// <returns>Success</returns>
        public bool save()
        {
            return XMLManager.WriteToXmlFile(Constants.AUTH_DATA_PATH, this);
        }
    }
}
