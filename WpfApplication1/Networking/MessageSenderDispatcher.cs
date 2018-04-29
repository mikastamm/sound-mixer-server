using SoundMixerServer.Networking.MessageSenders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundMixerServer.Networking
{
    class MessageSenderDispatcher
    {
        private Connection connection;
        private ClientLogic logic;
        public MessageSenderDispatcher(Connection connection, ClientLogic logic)
        {
            this.connection = connection;
            this.logic = logic;
            Main.Instance.audioManager.OnAudioSessionAdded += Form1_OnAudioSessionAdded;
            Main.Instance.audioManager.OnAudioSessionEdited += Form1_OnAudioSessionEdited;
            Main.Instance.audioManager.OnAudioSessionIconChanged += Form1_OnAudioSessionIconChanged;
            Main.Instance.audioManager.OnAudioSessionRemoved += Form1_OnAudioSessionRemoved;
        }

        private void Form1_OnAudioSessionRemoved(AudioSession session)
        {
            new AudioSessionRemoveMessageSender(connection).send(session);
        }

        private void Form1_OnAudioSessionIconChanged(AudioSession session)
        {
            new AudioSessionImageSender(connection).send(Main.Instance.audioManager.getSessionIcon(session.id));
        }

        private void Form1_OnAudioSessionEdited(AudioSession session)
        {
            new AudioSessionEditedMessageSender(connection, logic).send(session);
         
        }

        private void Form1_OnAudioSessionAdded(AudioSession session)
        {
            Console.WriteLine("Audio session added " + session.title + ":" + session.id);
            AudioSession.registerSessionID(session.id);
            ApplicationIcon icon = Main.Instance.audioManager.getSessionIcon(session.id);
            new AudioSessionSender(connection).send("ADD", session);
            new AudioSessionImageSender(connection).send(icon);
        }
    }
}
