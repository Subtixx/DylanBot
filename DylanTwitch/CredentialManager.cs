using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using WebBrowser = System.Windows.Forms.WebBrowser;

// This can be used to get an oauth to for a user. This is NOT finished.
namespace DylanTwitch
{
    // TODO
    public class CredentialManager
    {
        private const string _scope =
            "channel_read+channel_check_subscription+channel_commercial+channel_editor+channel_subscriptions+chat_login+communities_moderate+user_blocks_edit+user_blocks_read+user_read+viewing_activity_read";

        private const string _clientId = "27yqxc53mrldhm1mwtobwuqbr7x85f6";

        public void AskPermission()
        {
            var frm = new Form
            {
                Width = 1280,
                Height = 720
            };

            var wb = new WebBrowser
            {
                Dock = DockStyle.Fill
            };
            wb.Navigated += (sender, args) =>
            {
                if (args.Url.Host == "localhost")
                {
                    frm.Hide();
                    var fragment = args.Url.Fragment;
                }
            };
            frm.Controls.Add(wb);
            frm.Show();

            wb.Navigate(
                new Uri(
                    $"https://api.twitch.tv/kraken/oauth2/authorize?response_type=token&client_id={_clientId}&redirect_uri=http://localhost&scope={_scope}&state=c3ab8aa609ea11e793ae92361f002671"));

            while (frm.Visible)
                System.Windows.Forms.Application.DoEvents();
        }
    }
}