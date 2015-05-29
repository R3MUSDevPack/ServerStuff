using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

using Hipchat_Plugin;
using Slack_Plugin;

namespace RandomQuoteBot
{
    class Program
    {
        static void Main(string[] args)
        {
            Random randomNoGen = new Random();
            int randNo1 = randomNoGen.Next(0, 10);

            if ((randNo1 % 3) == 0)
            {
                SendMessage(GenerateMessage(randNo1));
            }
        }

        private static string GenerateMessage(int seed)
        {
            System.Collections.Specialized.NameValueCollection AppSettings = ConfigurationManager.AppSettings;
            Random randomNoGen = new Random();
            string randQuote = AppSettings[randomNoGen.Next(1, AppSettings.Count).ToString()];
            int randNo1 = randomNoGen.Next(1, 10);
            int randNo2 = randomNoGen.Next(1, 10);
            bool showQuote = ((randNo1 % (randNo2 * AppSettings.Count)) == seed);

            if (showQuote == false)
            {
                randQuote = string.Empty;
            }

            return randQuote;
        }

        private static void SendMessage(string message)
        {
            if (message != string.Empty)
            {
                if (Properties.Settings.Default.Plugin.ToUpper() == "HIPCHAT")
                {
                    Hipchat.SendToRoom(message, Properties.Settings.Default.RoomName, Properties.Settings.Default.HipchatToken);
                }
                else if (Properties.Settings.Default.Plugin.ToUpper() == "SLACK")
                {
                    Slack.SendToRoom(message, Properties.Settings.Default.RoomName, Properties.Settings.Default.SlackWebhook);
                }
            }
        }

    }
}
