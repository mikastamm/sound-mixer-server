using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SoundMixerServer
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            ConsoleManager.Show();
        }
        public Main volumeServices;

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            if(volumeServices != null)
            {
                volumeServices.Close();
            }
        }
    }
}
