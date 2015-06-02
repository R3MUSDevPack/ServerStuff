using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Configuration;
using System.Reflection;

using EveAI.Product;
using EveAI.Live;
using EveAI.Live.Account;
using EveAI.Live.Character;
using EveAI.Live.Corporation;

using eZet.EveLib.ZKillboardModule;
using eZet.EveLib.ZKillboardModule.Models;

using Hipchat_Plugin;
using Slack_Plugin;
using System.Text.RegularExpressions;
using JKON.EveWho;

namespace Killbot
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                CorporationSheet corpSheet = GetCorpDetails();
                if (Properties.Settings.Default.Debug)
                {
                    SendPM(string.Format("Corpsheet for {0} obtained.", corpSheet.Ticker));
                }

                UpdateRunTime(CheckKills(corpSheet.Ticker, corpSheet.CorporationID));
            }
            catch(Exception ex)
            {
                SendPM(ex.Message);
            }
        }

        private static DateTime CheckKills(string corpName, long corpId)
        {
            CorporationSheet Corp = GetCorpDetails();

            DateTime Latest = Convert.ToDateTime(ConfigurationSettings.AppSettings["StartDate"]);

            ZkbResponse Kills; 
            ZKillboard kb = new ZKillboard();
            ZKillboardOptions Options = new ZKillboardOptions();
            IEnumerable<ZkbResponse.ZkbKill> OrderedKills;
            Options.CorporationId.Add(Corp.CorporationID);
            if (Latest > DateTime.MinValue)
            {
                Options.StartTime = Latest;
            }
            if (Properties.Settings.Default.Debug)
            {
                SendPM(string.Format("Using StartTime {0}.", Latest.ToString("yyyy-MM-dd HH:mm:ss")));
            }

            try
            {
                Kills = kb.GetKills(Options);
                OrderedKills = Kills.OrderBy(Kill => Kill.KillTime);
                Latest = OrderedKills.Last().KillTime;

                foreach (ZkbResponse.ZkbKill Kill in OrderedKills)
                {
                    if ((Options.StartTime == null) || (Options.StartTime < Kill.KillTime))
                    {
                        SendMessage(FormatKillMessage(Kill, corpName, corpId));
                    }
                }
            }
            catch (Exception Ex)
            {
                if (Properties.Settings.Default.Debug)
                {
                    SendPM(Ex.Message);
                }
            }
            try
            {
                Kills = kb.GetLosses(Options);
                OrderedKills = Kills.OrderBy(Kill => Kill.KillTime);
                if (OrderedKills.Last().KillTime > Latest)
                {
                    Latest = OrderedKills.Last().KillTime;
                }

                foreach (ZkbResponse.ZkbKill Kill in OrderedKills)
                {
                    if ((Options.StartTime == null) || (Options.StartTime < Kill.KillTime))
                    {
                        SendMessage(FormatKillMessage(Kill, corpName, corpId));
                    }
                }
            }
            catch (Exception Ex)
            {
                if (Properties.Settings.Default.Debug)
                {
                    SendPM(Ex.Message);
                }
            }

            return Latest;
        }

        private static void SendMessage(string message)
        {
            if (Properties.Settings.Default.Plugin.ToUpper() == "HIPCHAT")
            {
                Hipchat.SendToRoom(message, Properties.Settings.Default.RoomName, Properties.Settings.Default.HipchatToken);
            }
            else if (Properties.Settings.Default.Plugin.ToUpper() == "SLACK")
            {
                //message = Linkify(message);
                Slack.SendToRoom(message, Properties.Settings.Default.RoomName, Properties.Settings.Default.SlackWebhook);
            }
        }

        private static string Linkify(string SearchText)
        {
            Regex regx = new Regex(@"\b(((\S+)?)(@|mailto\:|(news|(ht|f)tp(s?))\://)\S+)\b", RegexOptions.IgnoreCase);
            SearchText = SearchText.Replace("&nbsp;", " ");
            MatchCollection matches = regx.Matches(SearchText);

            foreach (Match match in matches)
            {
                if (match.Value.StartsWith("http"))
                { // if it starts with anything else then dont linkify -- may already be linked!
                    SearchText = SearchText.Replace(match.Value, "<a href='" + match.Value + "'>" + match.Value + "</a>");
                }
            }

            return SearchText;
        }

        private static void SendPM(string message)
        {
            if (Properties.Settings.Default.Plugin.ToUpper() == "HIPCHAT")
            {
                Hipchat.SendPM(message, "Clyde en Marland", Properties.Settings.Default.HipchatToken);
            }
            else if (Properties.Settings.Default.Plugin.ToUpper() == "SLACK")
            {
                Slack.SendPM(message, "ClydeenMarland", Properties.Settings.Default.SlackWebhook);
            }
        }

        private static string FormatKillMessage(ZkbResponse.ZkbKill kill, string corpName, long corpId)
        {
            string type;
            List<string> messageLines = new List<string>();
            string message = string.Empty;

            if (kill.Victim.CorporationId == corpId)
            {
                type = "LOSS";
            }
            else
            {
                type = "KILL";
            }
            EveAI.Map.SolarSystem system = GetSolarSystem(kill.SolarSystemId);

            ZkbResponse.ZkbStats stats = kill.Stats;

            messageLines.Add(string.Format(Properties.Settings.Default.MessageFormatLine1, corpName, type, kill.KillTime.ToString()));
            messageLines.Add(string.Format(Properties.Settings.Default.MessageFormatLine2, kill.Victim.CharacterName, GetProductType(kill.Victim.ShipTypeId).Name, system.Name, system.Region.Name));

            foreach (ZkbResponse.ZkbAttacker Attacker in kill.Attackers)
            {
                if (Attacker.FinalBlow == "1")
                {
                    messageLines.Add(string.Format(Properties.Settings.Default.MessageFormatLine3, Attacker.CharacterName, GetProductType(Attacker.ShipTypeId).Name));
                }
            }
            foreach (ZkbResponse.ZkbAttacker Attacker in kill.Attackers)
            {
                if (Attacker.CorporationId == corpId)
                {
                    messageLines.Add(string.Format(Properties.Settings.Default.MessageFormatLine4, Attacker.CharacterName, GetProductType(Attacker.ShipTypeId).Name));
                }
            }
            if (stats != null)
            {
                messageLines.Add(string.Format(Properties.Settings.Default.MessageFormatLine5, stats.TotalValue.ToString("N2")));
            }
            messageLines.Add(string.Format(Properties.Settings.Default.MessageFormatLine6, kill.KillId.ToString()));

            message = String.Join("\n", messageLines.ToArray());
            return message;
        }

        private static EveAI.Map.SolarSystem GetSolarSystem(int systemId)
        {
            EveApi API;
            APIKeyInfo KeyInfo;
            EveAI.Map.SolarSystem system;

            try
            {
                API = new EveApi("Clyde en Marland's API Checker", Properties.Settings.Default.CorpAPI, Properties.Settings.Default.VCode);
                KeyInfo = API.GetApiKeyInfo();

                system = EveApi.EveApiCore.FindSolarSystem(systemId);
            }
            catch (Exception Ex)
            {
                system = new EveAI.Map.SolarSystem();
                system.Name = string.Concat("Error querying API server; ", Ex.Message);
            }

            return system;
        }

        private static CorporationSheet GetCorpDetails()
        {
            EveApi API = new EveApi("Clyde en Marland's KillBot", Properties.Settings.Default.CorpAPI, Properties.Settings.Default.VCode);

            return API.GetCorporationSheet();
        }

        private static ProductType GetProductType(int shipTypeId)
        {
            EveApi API;
            APIKeyInfo KeyInfo;
            ProductType PType;

            try
            {
                API = new EveApi("Clyde en Marland's API Checker", Properties.Settings.Default.CorpAPI, Properties.Settings.Default.VCode);
                KeyInfo = API.GetApiKeyInfo();

                PType = EveApi.EveApiCore.FindProductType(shipTypeId);
            }
            catch (Exception Ex)
            {
                PType = new ProductType();
                PType.Name = "Error querying API server";
                PType.Description = Ex.Message;
            }

            if (PType == null)
            {
                PType = new ProductType();
                PType.Name = LookupShipName(shipTypeId);
            }

            return PType;

        }
        
        private static string LookupShipName(int shipTypeId)
        {
            string result = string.Empty;

            try
            {
                result = ConfigurationManager.AppSettings[shipTypeId.ToString()];
            }
            catch (Exception ex)
            {
            }
            if ((result == string.Empty) || result == null)
            {
                result = string.Concat("unknown ship ID: ", shipTypeId.ToString());
            }

            return result;
        }

        private static void UpdateRunTime(DateTime writeThis)
        {

            if (Properties.Settings.Default.Debug)
            {
                SendPM(string.Format("Updating run time: {0}.", writeThis.ToString("yyyy-MM-dd HH:mm:ss")));
            }

            Configuration config = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);
            config.AppSettings.Settings.Remove("StartDate");
            config.AppSettings.Settings.Add("StartDate", writeThis.ToString("yyyy-MM-dd HH:mm:ss"));
            config.Save();
        }
    }
}
