using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.IO;
using System.Web.Script.Serialization;
using System.Resources;
using System.Collections.ObjectModel;

namespace SoundMixerServer
{

    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Instance;
        private bool isOpen = true;
        public System.Windows.Forms.NotifyIcon notify;

        public MainWindow()
        {
            Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
            InitializeComponent();
            cbRunning.IsChecked = true;

            closeToTray();

            Instance = this;
            ShowInTaskbar = false;

            ObservableCollection<ClientInformation> col = new ObservableCollection<ClientInformation>();
            for(int i = 0; i < 10; i++)
            {
                col.Add(ClientInformation.Empty);
            }
            dgPadding.DataContext = col;

            setTrayIcon();

            if(AuthentificationManager.Instance.usesPassword)
            {
                cbUsesPassword.IsChecked = true;
            }

            if (AutoStart.isInAutostart())
            {
                cbAutostart.IsChecked = true;
            }
        }

        private void setTrayIcon()
        {
            Icon trayIcon;
            if (SystemParameters.SmallIconWidth > 16)
            {
                trayIcon = Properties.Resources.ic_tray32;
            }
            else
            {
                trayIcon = Properties.Resources.ic_tray16;
            }

            notify = new System.Windows.Forms.NotifyIcon();
            notify.Text = "Sound Mixer Remote";
            notify.Icon = trayIcon;
            notify.Visible = true;

            notify.MouseClick += (object sender, System.Windows.Forms.MouseEventArgs e) =>
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Left)
                {
                    if (isOpen)
                    {
                        closeToTray();
                    }
                    else
                    {
                        restoreFromTray();
                    }
                }
                else if(e.Button == System.Windows.Forms.MouseButtons.Right)
                {
                    ContextMenu menu;
                    if (isOpen)
                    {
                        menu = FindResource("TrayContextMenuHide") as ContextMenu;
                    }
                    else
                    {
                        menu = FindResource("TrayContextMenuShow") as ContextMenu;
                    }
                   
                    menu.IsOpen = true;
                }
            };

            notify.BalloonTipClosed += (sender, e) => {
                var thisIcon = (System.Windows.Forms.NotifyIcon)sender;
                thisIcon.Visible = false;
                thisIcon.Dispose();
            };


        }

        private void closeToTray()
        {
            Hide();
            ConsoleManager.Hide();
            isOpen = false;
        }

        private void restoreFromTray()
        {
            Show();
            Activate();
            WindowState = WindowState.Normal;
            isOpen = true;

            ConsoleManager.Show();

            Rect desktopWorkingArea = SystemParameters.WorkArea;
            Left = desktopWorkingArea.Right - (Width + 40);
            Top = desktopWorkingArea.Bottom - (Height + 30);
        }

        public void NotifyDeviceDatasetChanged()
        {
            System.Windows.Media.Brush white = Application.Current.Resources["White"] as System.Windows.Media.Brush;
            System.Windows.Media.Brush alternative = Application.Current.Resources["BackgroundLight"] as System.Windows.Media.Brush;

            dgDevices.Dispatcher.Invoke(new Action(() =>
            {
                dgDevices.DataContext = ClientMangager.deviceObservables;
                
                if(ClientMangager.deviceObservables.Count % 2 == 0)
                {
                    dgPadding.AlternatingRowBackground = alternative;
                    dgPadding.RowBackground = white;
                }
                else
                {
                    dgPadding.AlternatingRowBackground = white;
                    dgPadding.RowBackground = alternative;
                }

            }));

        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)//Uses password cb
        {
            if (!cbUsesPassword.IsChecked ?? true)
            {
                AuthentificationManager.Instance.removePassword();
            }
            else
            {
                if(!AuthentificationManager.Instance.usesPassword)
                {
                    cbUsesPassword.IsChecked = false;
                    gridEnterPassword.Visibility = Visibility.Visible;
                }

            }
        }


        private void window_Deactivated(object sender, EventArgs e)
        {

        }

        //Show password link click
        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            if (gridEnterPassword.Visibility == Visibility.Collapsed)
                gridEnterPassword.Visibility = Visibility.Visible;
            else
                gridEnterPassword.Visibility = Visibility.Collapsed;

        }

        //Set password btn click
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(tbPassword.Text != "")
            {
                AuthentificationManager.Instance.setNewPassword(tbPassword.Text);
                cbUsesPassword.IsChecked = true;
            }

            gridEnterPassword.Visibility = Visibility.Collapsed;
        }

        private void cbRunning_Checked(object sender, RoutedEventArgs e)
        {
            if(((App)Application.Current).volumeServices == null)
            {
                ((App)Application.Current).volumeServices = new Main();
            }
        }

        private void cbRunning_Unchecked(object sender, RoutedEventArgs e)
        {
            if(((App)Application.Current).volumeServices != null)
            {
                ((App)Application.Current).volumeServices.Close();
                ((App)Application.Current).volumeServices = null;
            }
        }

        private void BorderSettingsHeader_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            closeToTray();
        }

        private void window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            notify.Visible = false;
            notify.Dispose();
        }

        private void Menu_Exit(object sender, RoutedEventArgs e)
        {
            Close();
            Application.Current.Shutdown();
        }

        private void Menu_Show(object sender, RoutedEventArgs e)
        {
            restoreFromTray();
        }

        private void Menu_Hide(object sender, RoutedEventArgs e)
        {
            closeToTray();
        }

        private void cbAutostart_Checked(object sender, RoutedEventArgs e)
        {
            bool autostart = AutoStart.MakeShortcut();

            if (!autostart)
            {
                cbAutostart.IsChecked = false;
            }
        }

        private void cbAutostart_Unchecked(object sender, RoutedEventArgs e)
        {
            bool autostart = AutoStart.RemoveShortcut();

            if(autostart)
            {
                cbAutostart.IsChecked = true;
            }
        }
    }

}
