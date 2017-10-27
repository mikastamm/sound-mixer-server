using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreAudio;
using System.Diagnostics;
using System.Drawing;
using System.ComponentModel;

namespace SoundMixerServer 
{
    public class AudioManager
    {
        public enum AudioSessionChangedType
        {
            DisplayName,
            Volume
        }

        private Dictionary<string, AudioSession> AudioSessions { get; set; } = new Dictionary<string, AudioSession>();

        private HashSet<string> expiredSessions = new HashSet<string>();

        public delegate void AudioSessionChangedDelegate(AudioSession session);
        public event AudioSessionChangedDelegate OnAudioSessionAdded;
        public event AudioSessionChangedDelegate OnAudioSessionEdited;
        public event AudioSessionChangedDelegate OnAudioSessionRemoved;
        public event AudioSessionChangedDelegate OnAudioSessionIconChanged;

        AudioSessionControl2.SessionDisconnectedDelegate disconnectedDelegate;
        AudioSessionControl2.DisplayNameChangedDelegate displayNameChangedDelegate;
        AudioSessionControl2.IconPathChangedDelegate iconPathChangedDelegate;
        AudioSessionControl2.SimpleVolumeChangedDelegate volumeChangedDelegate;
        AudioSessionControl2.StateChangedDelegate stateChangedDelegate;

        AudioSessionManager2.SessionCreatedDelegate sessionCreatedDelegate;
        AudioEndpointVolumeNotificationDelegate masterVolumeChangedDelegate;

        MMDeviceEnumerator DevEnum;
        MMDevice device;

        public void Close()
        {
            Console.WriteLine("Unsubscribing Audio Events");
            device.AudioSessionManager2.OnSessionCreated -= sessionCreatedDelegate;
            device.AudioEndpointVolume.OnVolumeNotification -= masterVolumeChangedDelegate;

            for (int i = 0; i < device.AudioSessionManager2.Sessions.Count; i++)
            {
                try
                {
                    device.AudioSessionManager2.Sessions[i].OnSessionDisconnected -= disconnectedDelegate;
                    device.AudioSessionManager2.Sessions[i].OnDisplayNameChanged -= displayNameChangedDelegate;
                    device.AudioSessionManager2.Sessions[i].OnIconPathChanged -= iconPathChangedDelegate;
                    device.AudioSessionManager2.Sessions[i].OnSimpleVolumeChanged -= volumeChangedDelegate;
                    device.AudioSessionManager2.Sessions[i].OnStateChanged -= stateChangedDelegate;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Coudlnt unsubscribe a delegate from " + device.AudioSessionManager2.Sessions[i].DisplayName + ": " + ex.ToString());
                }
            }
        }

        public AudioManager()
        {
            Console.WriteLine("Initializing audio sessions");
            DevEnum = new MMDeviceEnumerator();
            device = DevEnum.GetDefaultAudioEndpoint(CoreAudio.EDataFlow.eRender, CoreAudio.ERole.eMultimedia);

            populateAudioSessions(device);

            masterVolumeChangedDelegate = new AudioEndpointVolumeNotificationDelegate(HandleMasterVolumeChanged);
            device.AudioEndpointVolume.OnVolumeNotification += masterVolumeChangedDelegate;

            sessionCreatedDelegate = new AudioSessionManager2.SessionCreatedDelegate(HandleSessionCreated);
            device.AudioSessionManager2.OnSessionCreated += sessionCreatedDelegate;

            Console.Write("\n");
        }

        public void updateChangedSession(AudioSession changedSession)
        {
            if (changedSession.id == Constants.MASTER_AUDIO_SESSION_ID)
            {
                device.AudioEndpointVolume.MasterVolumeLevelScalar = changedSession.volume;
                device.AudioEndpointVolume.Mute = changedSession.mute;
            }
            else {
                AudioSessionControl2 changedSessionControl;
                for (int i = 0; i < device.AudioSessionManager2.Sessions.Count; i++)
                {
                    if (device.AudioSessionManager2.Sessions[i].GetID() == changedSession.id)
                    {
                        changedSessionControl = device.AudioSessionManager2.Sessions[i];
                        changedSessionControl.SimpleAudioVolume.MasterVolume = changedSession.volume;
                        changedSessionControl.SimpleAudioVolume.Mute = changedSession.mute;
                    }
                }

                Console.WriteLine(changedSession == null ? "ChangeSession: Session not found" : "Changed Session " + changedSession.title + ":" + changedSession.id);
            }
        }

        /// <summary>
        /// Populates the session dictionary
        /// </summary>
        /// <param name="device">Speaker MMDevice</param>
        private void populateAudioSessions(MMDevice device)
        {
            //Master volume (Audio Device) session
            AudioSession deviceSession = getMasterAudioSessionObject(device);
            AudioSessions.Add(deviceSession.id, deviceSession);

            //Process sessions
            for (int i = 0; i < device.AudioSessionManager2.Sessions.Count; i++)
            {
                AudioSessionControl2 sessionControl = device.AudioSessionManager2.Sessions[i];
                if (setAudioSessionEventHandlers(sessionControl))
                {
                    AudioSession foundSession = getAudioSessionObject(sessionControl);
                    Console.WriteLine("\n" + foundSession.ToString());
                    Console.WriteLine("state: " + sessionControl.State.ToString());

                    if (!AudioSessions.ContainsKey(foundSession.id))
                    {
                        AudioSessions.Add(foundSession.id, foundSession);
                    }
                    else
                    {
                        if(sessionControl.State == AudioSessionState.AudioSessionStateActive)
                        {
                            AudioSessions[foundSession.id] = foundSession;
                        }
                    }
                }
            }
        }

        public ApplicationIcon[] getSessionIcons()
        {
            List<ApplicationIcon> icons = new List<ApplicationIcon>();

            for (int i = 0; i < device.AudioSessionManager2.Sessions.Count; i++)
            {
                AudioSessionControl2 sessionControl = device.AudioSessionManager2.Sessions[i];
                if (sessionControl.State != AudioSessionState.AudioSessionStateExpired)
                {
                    ApplicationIcon icon = getSessionIcon(sessionControl.GetID());
                    if (icon != null)
                        icons.Add(icon);
                }
            }


            return icons.ToArray();
        }

        public ApplicationIcon getSessionIcon(string sessionID)
        {
            int i = 0;
            foreach (AudioSession session in AudioSessions.Values)
            {
                if (session.pid != 0 && session.id == sessionID)
                {
                    i++;
                    ApplicationIcon icon = new ApplicationIcon();
                    Process proc = Process.GetProcessById(session.pid);
                    Bitmap bmp = Icon.ExtractAssociatedIcon(proc.MainModule.FileName).ToBitmap();
                    ImageConverter converter = new ImageConverter();
                    icon.icon = Convert.ToBase64String((byte[])converter.ConvertTo(bmp, typeof(byte[])));
                    icon.id = session.id;
                    return icon;
                }
            }
            return null;
        }

        public AudioSession[] getDisplayableAudioSessions()
        {
            return AudioSessions.Values.Where(x => !expiredSessions.Contains(x.id)).ToArray();
        }

        #region EventHandlers

        private void Session_OnStateChanged(object sender, AudioSessionState newState)
        {
            AudioSessionControl2 senderSession = (AudioSessionControl2)sender;
            AudioSession changedSession = getAudioSessionObject(senderSession);

            if (Main.LOG_EVENTS)
                Console.WriteLine(changedSession.ToString() + "\nState=" + newState.ToString());

            if (newState == AudioSessionState.AudioSessionStateExpired)
            {
                expiredSessions.Add(changedSession.id);
            }
            else 
            {
                if (expiredSessions.Contains(changedSession.id))
                {
                    expiredSessions.Remove(changedSession.id);
                }
            }
        }

        //Wrapper for SessionChanged
        public void HandleDisplayNameChanged(object sender, string newName)
        {
            HandleSessionChanged(sender, AudioSessionChangedType.DisplayName);
        }

        //Wrapper for SessionChanged
        public void HandleVolumeChanged(object sender, float newVolume, bool newMute)
        {
            HandleSessionChanged(sender, AudioSessionChangedType.Volume);
        }

        public void HandleSessionChanged(object sender, AudioSessionChangedType type)
        {
            AudioSessionControl2 sessionControl = (AudioSessionControl2)sender;
            AudioSession changedSession = getAudioSessionObject(sessionControl);

            if (type == AudioSessionChangedType.DisplayName)
            {
                AudioSessions[changedSession.id].title = (changedSession.title == "" ? Process.GetProcessById(changedSession.pid).ProcessName : sessionControl.DisplayName);
            }
            else if (type == AudioSessionChangedType.Volume)
            {
                AudioSessions[changedSession.id].volume = changedSession.volume;
                AudioSessions[changedSession.id].mute = changedSession.mute;
            }

            if (Main.LOG_EVENTS)
                Console.WriteLine("\nSession Volume Object Changed: " + changedSession.ToString());

            if (OnAudioSessionEdited != null)
                OnAudioSessionEdited(changedSession);
        }

        public void HandleIconPathChanged(object sender, string newItemPath)
        {
            AudioSessionControl2 senderSession = (AudioSessionControl2)sender;
            AudioSession changedSession = getAudioSessionObject(senderSession);

            if (Main.LOG_EVENTS)
                Console.WriteLine("\nSession Volume Object icon path Changed: " + changedSession.ToString());

            if (OnAudioSessionIconChanged != null)
                OnAudioSessionIconChanged(changedSession);
        }

        public void HandleSessionCreated(object sender, CoreAudio.Interfaces.IAudioSessionControl2 newSession)
        {
            AudioSessionControl2 sessionControl = new AudioSessionControl2(newSession);
            if (setAudioSessionEventHandlers(sessionControl))
            {
                AudioSession createdSession = getAudioSessionObject(sessionControl);
                AudioSessions.Add(createdSession.id, createdSession);

                if (Main.LOG_EVENTS)
                    Console.WriteLine("\n New Session Created: " + createdSession.ToString());

                if (OnAudioSessionAdded != null)
                    OnAudioSessionAdded(createdSession);
            }
        }

        public void HandleSessionDisconnected(object sender, CoreAudio.AudioSessionDisconnectReason reason) //NOT WORKING YET
        {
            AudioSessionControl2 sessionControl = (AudioSessionControl2)sender;
            AudioSession removedSession = getAudioSessionObject(sessionControl);

            if (Main.LOG_EVENTS)
                Console.WriteLine("\nSession Removed:" + removedSession.ToString());

            if (OnAudioSessionRemoved != null)
                OnAudioSessionRemoved(removedSession);
        }

        private void HandleProcessExited(object sender, EventArgs e)
        {
            Process process = (Process)sender;
            Console.WriteLine("\nProcess:" + process.Id + " has exited, removing all his audio sessions\n");
            removeSessionsWithPid(process.Id);
        }

        public void HandleMasterVolumeChanged(AudioVolumeNotificationData data)
        {
            AudioSession masterSession = getMasterAudioSessionObject(device);

            AudioSessions[masterSession.id].volume = device.AudioEndpointVolume.MasterVolumeLevelScalar;
            AudioSessions[masterSession.id].mute = device.AudioEndpointVolume.Mute;

            if (Main.LOG_EVENTS)
                Console.WriteLine("\nMaster Volume Changed: " + masterSession.ToString());

            if (OnAudioSessionEdited != null)
                OnAudioSessionEdited(masterSession);
        }

        #endregion


        public AudioSession getAudioSessionWithID(string id)
        {
            return AudioSessions.Values.FirstOrDefault(x => x.id == id);
        }

        /// <summary>
        /// Subscribes audio Session to events
        /// </summary>
        /// <param name="sessionControl"></param>
        /// <returns>Returns false if the parent process of the audio session is dead</returns>
        private bool setAudioSessionEventHandlers(AudioSessionControl2 sessionControl)
        {
            bool sucess = true;

            Process sessionProcess;
            int pid = (int)sessionControl.GetProcessID;
            try
            {
                if (pid != 0 && AudioSessions.Values.FirstOrDefault(x => x.pid == pid) == null)
                {
                    sessionProcess = Process.GetProcessById(pid);
                    try
                    {
                        sessionProcess.EnableRaisingEvents = true;
                    }
                    catch(Win32Exception e)
                    {
                        //TODO: Find another way to detect wether a process is alive or not
                        Console.WriteLine("Access for Process " + sessionProcess.ProcessName + " Denied :(" );
                    }
                    sessionProcess.Exited += HandleProcessExited;
                }

                disconnectedDelegate = new AudioSessionControl2.SessionDisconnectedDelegate(HandleSessionDisconnected);
                displayNameChangedDelegate = new AudioSessionControl2.DisplayNameChangedDelegate(HandleDisplayNameChanged);
                iconPathChangedDelegate = new AudioSessionControl2.IconPathChangedDelegate(HandleIconPathChanged);
                volumeChangedDelegate = new AudioSessionControl2.SimpleVolumeChangedDelegate(HandleVolumeChanged);
                stateChangedDelegate = new AudioSessionControl2.StateChangedDelegate(Session_OnStateChanged);

                sessionControl.OnSessionDisconnected += disconnectedDelegate; // (object _sender, CoreAudio.AudioSessionDisconnectReason reason) => { HandleSessionDisconnected(_sender, reason); };
                sessionControl.OnDisplayNameChanged += displayNameChangedDelegate; // (object _sender, string newName) => { HandleSessionChanged(_sender, AudioSessionChangedType.DisplayName); };
                sessionControl.OnIconPathChanged += iconPathChangedDelegate; // (object _sender, string newItemPath) => { HandleIconPathChanged(_sender); };
                sessionControl.OnSimpleVolumeChanged += volumeChangedDelegate;//(object _sender, float newVolume, bool newMute) => { HandleSessionChanged(_sender, AudioSessionChangedType.Volume); };
                sessionControl.OnStateChanged += stateChangedDelegate; //Session_OnStateChanged;
            }
            catch (ArgumentException argex)
            {//Remove all Sessions which are of the dead process from AudioSessions
                sucess = false;
                string sessionID = sessionControl.GetID();
                Console.WriteLine("\nProcess with ID " + pid + " dead, removing sessions of this process");
                removeSessionsWithPid(pid);
            }
            return sucess;
        }

        private void removeSessionsWithPid(int pid)
        {
            List<KeyValuePair<string, AudioSession>> sessionsUsingPid = AudioSessions.Where(x => x.Value.pid == pid).ToList();
            for (int i = sessionsUsingPid.Count - 1; i >= 0; i--)
            {
                Console.WriteLine("Removing session: " + sessionsUsingPid[i].Value.title + " : " + sessionsUsingPid[i].Value.id);
                if (OnAudioSessionRemoved != null)
                    OnAudioSessionRemoved(sessionsUsingPid[i].Value);

                AudioSessions.Remove(sessionsUsingPid[i].Key);
            }
        }

        private AudioSession getAudioSessionObject(AudioSessionControl2 asc)
        {
            int _pid = (int)asc.GetProcessID;
            string displayName = asc.DisplayName;

            if (_pid == 0 || displayName.Contains(@"\AudioSrv.Dll"))
            {
                displayName = "System";
            }
            else if (displayName == "")
            {
                try
                {
                    displayName = Process.GetProcessById(_pid).ProcessName;
                }
                catch
                {
                    displayName = "Application";
                }

            }

            return new AudioSession()
            {
                mute = asc.SimpleAudioVolume.Mute,
                pid = _pid,
                title = displayName,
                volume = asc.SimpleAudioVolume.MasterVolume,
                id = asc.GetID()
            };

        }

        private AudioSession getMasterAudioSessionObject(MMDevice device)
        {
            return new AudioSession()
            {
                volume = device.AudioEndpointVolume.MasterVolumeLevelScalar,
                mute = device.AudioEndpointVolume.Mute,
                pid = -1,
                title = "Master",
                id = Constants.MASTER_AUDIO_SESSION_ID
            };
        }
    }
}


