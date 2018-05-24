using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundMixerServer
{
    public class Authentication
    {
        private string filePath;
        private ClientInformation information;

        public Authentication(ClientInformation information)
        {
            this.information = information;
            filePath = Constants.APPLICATION_DATA_ROOT_DIR + information.id + ".xml";
        }

        public bool isAuthenticated()
        {
            bool auth = false;
            AuthenticationState state = XMLManager.ReadFromXmlFile<AuthenticationState>(filePath);
            auth = state == null ? false : state.Authenticated;

            return auth;
        }

        public void setAuthentication(bool state)
        {
            AuthenticationState authenticationState = new AuthenticationState();
            authenticationState.Authenticated = state;
            XMLManager.WriteToXmlFile(filePath, authenticationState);
        }
    }
}
