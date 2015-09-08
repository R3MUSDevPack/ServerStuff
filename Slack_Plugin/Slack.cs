using JKON.Slack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slack_Plugin
{
    public class Slack
    {
        public static void SendToRoom(string message, string roomname, string token, string username = "R3mus Bot")
        {
            try
            {
                SlackClient client = new SlackClient(token);
                client.PostMessage(message, username, string.Concat("#", roomname));

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static void SendToRoom(MessagePayload payload, string roomname, string token, string username = "R3mus Bot")
        {
            try
            {
                payload.Channel = string.Concat("#", roomname);
                payload.Username = username;

                SlackClient client = new SlackClient(token);
                client.PostMessage(payload);
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
        public static void SendPM(MessagePayload payload, string username, string token)
        {
            try
            {
                payload.Channel = string.Concat("@", username);
                payload.Username = "R3mus Bot";

                SlackClient client = new SlackClient(token);
                client.PostMessage(payload);
            }
            catch (Exception ex)
            {
            }
        }
    }
}
