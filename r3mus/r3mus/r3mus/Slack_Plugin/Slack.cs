using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slack_Plugin
{
    public class Slack
    {
        public static void SendToRoom(string message, string roomname, string token)
        {
            try
            {
                SlackClient client = new SlackClient(token);
                client.PostMessage(message, "R3mus Bot", string.Concat("#", roomname));
            }
            catch (Exception ex)
            {
            }
        }

        public static void SendPM(string message, string username, string token)
        {
            try
            {
                SlackClient client = new SlackClient(token);
                client.PostMessage(message, "R3mus Bot", string.Concat("@", username));
            }
            catch (Exception ex)
            {
            }
        }
    }
}
