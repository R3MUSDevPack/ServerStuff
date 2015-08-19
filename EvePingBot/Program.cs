using Slack_Plugin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace EvePingBot
{
    class Program
    {
        static void Main(string[] args)
        {
            var messages = GetPings();
            DateTime lastDate;

            if (File.Exists(Path.Combine(Environment.CurrentDirectory, "lastmessage.txt")))
            {
                lastDate = Convert.ToDateTime(File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "lastmessage.txt")));
            }
            else
            {
                lastDate = DateTime.Now.AddMinutes(-60);
            }
            foreach(var msg in messages)
            {
                if (msg.Time > lastDate)
                {
                    Slack.SendToRoom(msg.Text, "fleets", "https://hooks.slack.com/services/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", string.Format("{0} via EvePing", msg.Sender));
                }
            }
            File.WriteAllText(Path.Combine(Environment.CurrentDirectory, "lastmessage.txt"), messages[0].Time.ToString("yyyy-MM-ddTHH:mm:ss"));
        }

        private static List<Message> GetPings()
        {
            var messages = new List<Message>();

            System.Net.ServicePointManager.ServerCertificateValidationCallback =
                ((sender, certificate, chain, sslPolicyErrors) => true);

            var client = new WebClient();
            client.Headers.Add(HttpRequestHeader.Cookie, "__cfduid=df516327f15b5c05817849856f96d994b1438259709; PHPSESSID=k4u2rs8egfr1j9lvm762m9gd86");
            var htmlString = client.DownloadString("https://www.eveping.com/ping.php");

            var name = string.Empty;
            var time = string.Empty;
            var getTimeNext = false;
            var message = new List<string>();

            try
            {

                var reader = new XmlTextReader(new StringReader(htmlString));
                while (reader.Read())
                {
                    if (message.Count > 0)
                    {
                        var buildMsgText = string.Join("\n", message.ToArray());
                        messages.Add(new Message() { Sender = name, Text = string.Format("{0}\n{1}", Convert.ToDateTime(time).ToString("yyyy-MM-dd HH:mm:ss"), buildMsgText), Time = Convert.ToDateTime(time) });
                        name = string.Empty;
                        time = string.Empty;
                        message = new List<string>();
                    }
                    if ((reader.NodeType == XmlNodeType.Element) && (reader.Name == "tr"))
                    {
                        var trString = string.Format("{0}{1}{2}", "<table><tr>", reader.ReadInnerXml(), "</tr></table>");
                        var subReader = new XmlTextReader(new StringReader(trString));
                        while (subReader.Read())
                        {
                            if (subReader.NodeType == XmlNodeType.Text)
                            {
                                if (name == string.Empty)
                                {
                                    name = subReader.Value;
                                }
                                else if (getTimeNext == true)
                                {
                                    getTimeNext = false;
                                    time = subReader.Value;
                                }
                                else if ((time != string.Empty) && (subReader.Value != "<br/>"))
                                {
                                    message.Add(subReader.Value);
                                }
                                Console.WriteLine(subReader.Value);
                            }
                            else if ((subReader.Name == "time") && (time == string.Empty))
                            {
                                getTimeNext = true;
                            };
                        }
                    }
                }
            }
            catch(Exception ex)
            {

            }
            return messages;
        }

        private class Message
        {
            public string Sender { get; set; }
            public DateTime Time { get; set; }
            public string Text { get; set; }
        }
    }
}
