using System;
using System.Windows;
using DylanTwitch.Util;
using TwitchLib;
using TwitchLib.Events.Client;

namespace DylanTwitch.Forms
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            ChatBot.Client.OnMessageReceived += OnChatMessage;
            ChatBot.Client.OnUserJoined += OnUserJoin;
            ChatBot.Client.OnUserLeft += OnUserLeft;

            // Add all users currently in chat
            TwitchAPI.Undocumented.GetChatters(ChatBot.Client.JoinedChannels[0].Channel).ContinueWith(task =>
            {
                Dispatcher.Invoke(() =>
                {
                    task.Result.ForEach(a =>
                    {
                        if(!listBox.Items.Contains(a.Username)) listBox.Items.Add(a.Username);
                    });
                });
            });
        }

        private void OnUserJoin(object sender, OnUserJoinedArgs e)
        {
            Dispatcher.Invoke(() => listBox.Items.Add(e.Username));
        }

        private void OnUserLeft(object sender, OnUserLeftArgs e)
        {
            Dispatcher.Invoke(() => listBox.Items.Remove(e.Username));
        }

        public void OnChatMessage(object sender, OnMessageReceivedArgs arg)
        {
            Dispatcher.Invoke(() => AddChatMessage(arg.ChatMessage.Username, arg.ChatMessage.Message));
        }

        private void AddChatMessage(string username, string message)
        {
            richTextBox.AppendText($"[{DateTime.Now:T}]\t{username}: {message}\n");
        }

        private void textBox_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                ChatBot.Client.SendMessage(textBox.Text);
                textBox.Text = "";
            }
        }
    }
}
