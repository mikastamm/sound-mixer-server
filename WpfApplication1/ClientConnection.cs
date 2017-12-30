﻿using System;
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

        string unprocessedReceiveUntilAuthentication = "";

        public void disconnect()
        {
            if (device != null)
            {
                Console.WriteLine("Disconnecting " + device.Name);
                device.Connected = false;
                device.LastConnected = DateTime.Now;
                MainWindow.Instance.NotifyDeviceDatasetChanged();

                Main.Instance.audioManager.OnAudioSessionAdded -= Form1_OnAudioSessionAdded;
                Main.Instance.audioManager.OnAudioSessionEdited -= Form1_OnAudioSessionEdited;
                Main.Instance.audioManager.OnAudioSessionIconChanged -= Form1_OnAudioSessionIconChanged;
                Main.Instance.audioManager.OnAudioSessionRemoved -= Form1_OnAudioSessionRemoved;

                ClientListener.connectedClients.Remove(IPAddress.Parse(device.IP));
            }
            socket.Close();
        }

        public void initializeConnection() 
        {
            Console.WriteLine("Inititalizing Client session " + clientEP.ToString());
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
            Console.WriteLine("Audio session added " + session.title + ":" + session.id );
            AudioSession.registerSessionID(session.id);
            ApplicationIcon icon = Main.Instance.audioManager.getSessionIcon(session.id);
            send("ADD", session);
            send("IMG", icon);
        }

        private void send(string data)
        {
            if (clientPublicKey != null)
            {
                try
                {
                    socket.Send(Encoding.ASCII.GetBytes(data + "\r\n"));
                    Console.WriteLine("SENT:" + data);
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
                    toSend.volume = (float)Math.Round(toSend.volume, 2);
                    string data = action + JSONManager.serialize(toSend);
                    Console.WriteLine("SENT:" + data);
                    socket.Send(Encoding.ASCII.GetBytes(data + "\r\n"));
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
                        toSend[i].volume = (float)Math.Round(toSend[i].volume, 2);
                        finalSend[i] = toSend[i].toCodeId();
                    }

                    string data = action + JSONManager.serialize(finalSend);
                    Console.WriteLine("SENT:" + data);
                    socket.Send(Encoding.ASCII.GetBytes(data + "\r\n"));
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

        private void send(string action, ApplicationIcon toSend)
        {
            if (clientPublicKey != null)
            {
                try
                {
                    toSend.id = AudioSession.getCode(toSend.id);
                    string data = action + JSONManager.serialize(toSend);
                    Console.WriteLine("SENT:" + data);
                    socket.Send(Encoding.ASCII.GetBytes(data + "\r\n"));
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
                    Console.WriteLine("SENT:"+data.Substring(0, 20) + "...");
                    socket.Send(Encoding.ASCII.GetBytes(data + "\r\n"));
                }
                catch (SocketException se)
                {
                    Console.WriteLine("Couldnt send: " + se.ToString());
                }
                catch (ObjectDisposedException ex)
                {
                    Console.WriteLine("Couldnt send: " + ex.ToString());
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

            if(icons.Length == 0 || (icons.Length < sessions.Length-2))//TODO: Remove
                Console.WriteLine("FUUUUUUUUUUUUUUUUUUUUUUUUUUCK");

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
                        handleData(recv);
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

        private void handleData(string recv)
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
                if (recv.StartsWith("AUTH"))
                {
                    string encryptedPassword = recv.Remove(0, 4);
                    string decryptedPasswordHash = decryptMessage(encryptedPassword);

                    if (AuthentificationManager.Instance.checkPasswordHash(decryptedPasswordHash))
                    {
                        device.verifiedUntil = DateTime.Now + AuthentificationManager.timeToReAuth;
                        Console.WriteLine("Client authenticated for another period");

                        if(string.IsNullOrEmpty(unprocessedReceiveUntilAuthentication))
                        {
                            return; //No data to handle
                        }
                        else
                        {
                            recv = unprocessedReceiveUntilAuthentication;
                        }

                        unprocessedReceiveUntilAuthentication = "";
                        
                    }
                    else
                    {
                        Console.WriteLine("Wrong password\ngot:" + decryptedPasswordHash + "\nexp:" + AuthentificationManager.Instance.getPasswordHash());
                        send("AUTHWPW");//Code for Wrong Password
                        disconnect();
                    }
                }
                else if (device.verifiedUntil < DateTime.Now && AuthentificationManager.Instance.usesPassword)
                {
                    Console.WriteLine("Client Authentification Expired, requesting new authentication");
                    unprocessedReceiveUntilAuthentication = recv;
                    send("AUTH");
                    return;
                }

                if (ClientListener.knownDevices.ContainsKey(device.ID))
                {
                    ClientListener.knownDevices[device.ID] = device;
                }
                else
                {
                    ClientListener.knownDevices.Add(device.ID, device);
                }
                ClientListener.saveDevices();
                MainWindow.Instance.NotifyDeviceDatasetChanged();

                if (recv.Equals("GETAUDIOSESSIONS"))
                {
                    int sessionsSent = sendAllAudioSessions();
                    Console.WriteLine("\tSent " + sessionsSent + "Sessions");
                }
                else if (recv.StartsWith("EDIT"))
                {
                    recv = recv.Substring(4);
                    AudioSession session = JSONManager.deserialize<AudioSession>(recv).toSessionId();
                    Main.Instance.audioManager.updateChangedSession(session);
                }
                else if (recv.StartsWith("TRACK"))
                {
                    recv = recv.Substring(5);
                    sendVolumeBlacklist.Add(AudioSession.getSessionId(recv));
                }
                else if (recv.StartsWith("ENDTRACK"))
                {
                    recv = recv.Substring(8);
                    sendVolumeBlacklist.Remove(AudioSession.getSessionId(recv));
                    AudioSession session = Main.Instance.audioManager.getAudioSessionWithID(AudioSession.getSessionId(recv));

                }
                else
                {
                    Console.WriteLine("\nReceived unknown command: " + recv);
                }

            }
        }

        public string decryptMessage(string encryptedString)
        {
            byte[] EncryptedData = Convert.FromBase64String(encryptedString);
            byte[] DecryptedData = VCCryptography.RSADecrypt(EncryptedData);
            return Encoding.UTF8.GetString(DecryptedData ?? new byte[0]);       
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
