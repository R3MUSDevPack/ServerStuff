using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EveAI.Live;
using EveAI.Live.Account;
using EveAI.Live.Character;
using EveAI.Live.Corporation;

using EveAI.Product;
using Hipchat_Plugin;
using Slack_Plugin;

namespace ContractNotifyBot
{
    class Program
    {
        static void Main(string[] args)
        {
            CheckContracts();
        }

        private static void CheckContracts()
        {
            EveApi api;
            List<EveAI.Live.Utility.Contract> Contracts;    
            Dictionary<long, string> Names = new Dictionary<long, string>();
            List<long> IDs = new List<long>();

            try
            {
                api = new EveAI.Live.EveApi("Clyde en Marland's Contract Notifier", (long)Properties.Settings.Default.CorpAPI, Properties.Settings.Default.VCode);
                Contracts = api.GetCorporationContracts();
                Contracts = api.GetCharacterContracts();

                foreach(EveAI.Live.Utility.Contract Contract in Contracts)
                {
                    IDs.Add(Contract.IssuerID);
                }
                Names = api.ConvertIDsToNames(IDs);
                foreach (EveAI.Live.Utility.Contract Contract in Contracts)
                {
                    if ((Contract.Type == EveAI.Live.Utility.Contract.ContractType.Courier) && (Contract.Status == EveAI.Live.Utility.Contract.ContractStatus.Outstanding))
                    {
                        SendMessage(FormatMessage(Contract, Names[Contract.IssuerID]));
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        private static string FormatMessage(EveAI.Live.Utility.Contract contract, string IssuerName)
        {
            string type;
            List<string> messageLines = new List<string>();
            string message = string.Empty;

            messageLines.Add(string.Format(Properties.Settings.Default.MessageFormatLine1, contract.DateIssued.ToString("yyyy-MM-dd hh:mm:ss")));
            messageLines.Add(string.Format(Properties.Settings.Default.MessageFormatLine2, IssuerName));
            messageLines.Add(string.Format(Properties.Settings.Default.MessageFormatLine3, contract.Reward.ToString()));
            messageLines.Add(string.Format(Properties.Settings.Default.MessageFormatLine4, contract.StartStation.Name, contract.EndStation.Name));

            message = String.Join("\n", messageLines.ToArray());
            return message;
        }

        private static void SendMessage(string message)
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
