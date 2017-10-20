using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SoundMixerServer
{
    public class ClientConnection
    {
        Socket socket;
        public IPEndPoint clientEP;

        HashSet<string> sendVolumeBlacklist = new HashSet<string>();
        Dictionary<string, AudioSession> lastEditSent = new Dictionary<string, AudioSession>();

        RSAKeyValue clientPublicKey;
        ClientInformation device;

        const bool usesEncryption = true;

        public void disconnect()
        {
            if (device != null)
            {
                Console.WriteLine("Disconnecting " + device.Name);
                device.Connected = false;
                device.LastConnected = DateTime.Now;
                MainWindow.Instance.NotifyDeviceDatasetChanged();
            }
            socket.Close();
        }

        public void initializeConnection() 
        {
            AudioSession[] array = Main.Instance.audioManager.getDisplayableAudioSessions();
            foreach (var p in array)
            {
                AudioSession.registerSessionID(p.id);
            }

                Main.Instance.audioManager.OnAudioSessionAdded += Form1_OnAudioSessionAdded;
                Main.Instance.audioManager.OnAudioSessionEdited += Form1_OnAudioSessionEdited;
                Main.Instance.audioManager.OnAudioSessionIconChanged += Form1_OnAudioSessionIconChanged;
                Main.Instance.audioManager.OnAudioSessionRemoved += Form1_OnAudioSessionRemoved;
                handleReceivedData();
        }

        //Event handlers

        private void Form1_OnAudioSessionRemoved(AudioSession session)
        {
            send("DEL", session);
            AudioSession.removeSessionID(session.id);
        }

        private void Form1_OnAudioSessionIconChanged(AudioSession session)
        {
         //   send("IMG", session);
        }

        private void Form1_OnAudioSessionEdited(AudioSession session)
        {
            AudioSession lastEdit;
            bool firstEdit = !lastEditSent.TryGetValue(session.id, out lastEdit);

            if (!sendVolumeBlacklist.Contains(session.id) && (firstEdit ||  !lastEdit.Equals(session)))
            {
                send("EDIT", session);
                lastEditSent[session.id] = session;
            }
        }

        private void Form1_OnAudioSessionAdded(AudioSession session)
        {
            AudioSession.registerSessionID(session.id);
            send("ADD", session);
        }

        private void send(string data)
        {
            if (clientPublicKey != null)
            {
                try
                {
                    socket.Send(Encoding.ASCII.GetBytes(VCCryptography.getEncryptedMessage(data, clientPublicKey) + "\r\n"));
                }
                catch (SocketException se)
                {
                    Console.WriteLine("Couldnt send: " + se.Message);
                }
                catch (ObjectDisposedException ex)
                {
                    Console.WriteLine("Couldnt send: " + ex.Message);
                }
            }
            else
            {
                Console.WriteLine("Did not send Data: Public key is null");
            }
        }

        private void send(string action, AudioSession toSend)
        {
            if (clientPublicKey != null)
            {
                try
                {
                    toSend = toSend.toCodeId();
                    string data = action + JSONManager.serialize(toSend);
                    socket.Send(Encoding.ASCII.GetBytes(VCCryptography.getEncryptedMessage(data, clientPublicKey) + "\r\n"));
                }
                catch (SocketException se)
                {
                    Console.WriteLine("Couldnt send: " + se.Message);
                }
                catch(ObjectDisposedException ex)
                {
                    Console.WriteLine("Couldnt send: " + ex.Message);
                }
            }
            else
            {
                Console.WriteLine("Did not send Data: Public key is null");
            }
        }

        private void send(string action, AudioSession[] toSend)
        {
            if (clientPublicKey != null)
            {
                try
                {
                    AudioSession[] finalSend = new AudioSession[toSend.Length];
                    for (int i = 0; i < toSend.Length; i++)
                    {
                        finalSend[i] = toSend[i].toCodeId();
                    }

                    string data = action + JSONManager.serialize(finalSend);
                    socket.Send(Encoding.ASCII.GetBytes(VCCryptography.getEncryptedMessage(data, clientPublicKey) + "\r\n"));
                }
                catch (SocketException se)
                {
                    Console.WriteLine("Couldnt send: " + se.Message);
                }
                catch (ObjectDisposedException ex)
                {
                    Console.WriteLine("Couldnt send: " + ex.Message);
                }
            }
            else
            {
                Console.WriteLine("Did not send Data: Public key is null");
            }
        }

        private void send(string action, ApplicationIcon[] toSend)
        {
            if (clientPublicKey != null)
            {
                try
                {
                    ApplicationIcon[] finalSend = new ApplicationIcon[toSend.Length];
                    for (int i = 0; i < toSend.Length; i++)
                    {
                        finalSend[i] = new ApplicationIcon();
                        finalSend[i].id = AudioSession.getCode(toSend[i].id);
                        finalSend[i].icon = toSend[i].icon;
                    }

                    string data = action + JSONManager.serialize(finalSend);
                    socket.Send(Encoding.ASCII.GetBytes(VCCryptography.getEncryptedMessage(data, clientPublicKey) + "\r\n"));
                }
                catch (SocketException se)
                {
                    Console.WriteLine("Couldnt send: " + se.Message);
                }
                catch (ObjectDisposedException ex)
                {
                    Console.WriteLine("Couldnt send: " + ex.Message);
                }
            }
            else
            {
                Console.WriteLine("Did not send Data: Public key is null");
            }
        }

        /// <summary>
        /// Sends all AudioSessions to the Client, used for app start of Client to populate the adapter
        /// </summary>
        /// <returns>count of sessions sent</returns>
        public int sendAllAudioSessions()
        {
            int sessionCount = 0;

            AudioSession[] sessions = Main.Instance.audioManager.getDisplayableAudioSessions();
            send("REP", sessions);
            sessionCount = sessions.Length;

            ApplicationIcon[] icons = Main.Instance.audioManager.getSessionIcons();
            send("IMGS", icons);
           
            return sessionCount;
        }

        public void handleReceivedData()
        {
            using (StreamReader reader = new StreamReader(new NetworkStream(socket, false), Encoding.UTF8))
            {
                string recv;
                try {
                    while ((recv = reader.ReadLine()) != null) 
                    {
                        Console.WriteLine("\nReceived " + recv);


                        if (recv.StartsWith("DEVINFO"))
                        {
                            recv = recv.Substring("DEVINFO".Length);

                            ClientInformation dev = new ClientInformation();
                            dev = JSONManager.deserialize<ClientInformation>(recv);
                            dev.LastConnected = DateTime.Now;
                            dev.IP = clientEP.Address.ToString();
                            dev.Connected = true;
                            device = dev;

                            clientPublicKey = dev.RSAKey;
                        }
                        else
                        {
                            string line, hash;
                            if (usesEncryption)
                            {
                                decryptMessage(recv, out hash, out line);
                            }
                            else
                            {
                                getUnencryptedMessage(recv, out hash, out line);
                            }

                            Console.WriteLine("Received " + (usesEncryption ? "Encrypted" : "Unencrypted") + " Data: \n\tHash=" + hash + "\n\tData=" + line);
                            if (!AuthentificationManager.Instance.usesPassword || AuthentificationManager.Instance.checkPasswordHash(hash))
                            {
                                if(ClientListener.knownDevices.ContainsKey(device.ID))
                                {
                                    ClientListener.knownDevices[device.ID] = device;
                                }
                                else
                                {
                                    ClientListener.knownDevices.Add(device.ID, device);
                                }
                                ClientListener.saveDevices();
                                MainWindow.Instance.NotifyDeviceDatasetChanged();

                                if (line.Equals("GETAUDIOSESSIONS"))
                                {
                                    int sessionsSent = sendAllAudioSessions();
                                    Console.WriteLine("\tSent " + sessionsSent + "Sessions");
                                }
                                else if (line.StartsWith("EDIT"))
                                {
                                    line = line.Substring(4);
                                    AudioSession session = JSONManager.deserialize<AudioSession>(line).toSessionId();
                                    Main.Instance.audioManager.updateChangedSession(session);
                                }
                                else if (line.StartsWith("TRACK"))
                                {
                                    line = line.Substring(5);
                                    sendVolumeBlacklist.Add(AudioSession.getSessionId(line));
                                }
                                else if (line.StartsWith("ENDTRACK"))
                                {
                                    line = line.Substring(8);
                                    sendVolumeBlacklist.Remove(AudioSession.getSessionId(line));
                                    AudioSession session = Main.Instance.audioManager.getAudioSessionWithID(AudioSession.getSessionId(line));

                                }
                                //else if(line == "NOENC")
                                //{
                                //    usesEncryption = false;
                                //}
                                //else if(line == "ENC")
                                //{
                                //    usesEncryption = true;
                                //}
                                else
                                {
                                    Console.WriteLine("\nReceived unknown command: " + line);
                                }
                            }
                            else
                            {
                                Console.WriteLine("Wrong password");
                                send("AUTHWPW");//Code for Wrong Password
                                disconnect();
                            }
                        }
                    }
                }
                catch(IOException e)
                {
                    Console.WriteLine("\nConnection lost: " + e.Message);
                }
                catch(ObjectDisposedException odex)
                {
                    //TODO: reconnect (Happened when audio changed on client)   
                }

                Console.WriteLine("INFO: Reached end of Stream in handleReceivedData(), wont receive new Data from this client   ( reader.ReadLine() == null )");
                disconnect();
            }
        } 

        public void getUnencryptedMessage(string line, out string passwordHash, out string data)
        {
            int seperatorIndex = line.IndexOf(";");
            passwordHash = seperatorIndex != -1 ? line.Substring(0, seperatorIndex) : "";
            data = line.Substring(seperatorIndex + 1);
        }

        public void decryptMessage(string encryptedString, out string passwordHash, out string data)
        {
            byte[] EncryptedData = Convert.FromBase64String(encryptedString);
            byte[] DecryptedData = VCCryptography.RSADecrypt(EncryptedData);
            string line = Encoding.UTF8.GetString(DecryptedData ?? new byte[0]);
            int seperatorIndex = line.IndexOf(";");
            passwordHash = seperatorIndex != -1 ? line.Substring(0, seperatorIndex) : "";
            data = line.Substring(seperatorIndex + 1);
        }

        public ClientConnection(Socket socket)
        {
            this.socket = socket;
            clientEP = socket.RemoteEndPoint as IPEndPoint;
        }

     
    }

   
    public class ApplicationIcon
    {
        public string id { get; set; }
        public string icon { get; set; }
    }
}
