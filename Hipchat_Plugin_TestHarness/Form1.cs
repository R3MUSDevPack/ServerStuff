using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Hipchat_Plugin;
using eZet.EveLib.ZKillboardModule;
using eZet.EveLib.ZKillboardModule.Models;

using EveAI.Product;

using EveAI.Live;
using EveAI.Live.Account;
using EveAI.Live.Character;
using EveAI.Live.Corporation;

namespace Hipchat_Plugin_TestHarness
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //string thing = "bzuowPX0H4xq75y1yBGibMLB0VJkIpqUhMYIIEhZhMFpVbP5tK7F8805qNSlWhZW";
            //MessageBox.Show(thing.Length.ToString());

            List<string> AllRooms = Hipchat.GetAllRooms("token");
            List<string> AllUsers = Hipchat.GetAllUsers("token");

            comboBox1.DataSource = AllRooms;
            comboBox2.DataSource = AllUsers;

            CheckKills();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != string.Empty)
            {
                Hipchat.SendToRoom(textBox1.Text, comboBox1.Text, "token");
            }
            textBox1.Text = string.Empty;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != string.Empty)
            {
                Hipchat.SendPM(textBox1.Text, comboBox2.Text, "token");
            }
            textBox1.Text = string.Empty;
        }

        private void CheckKills()
        {

            ZKillboard kb = new ZKillboard();
            ZKillboardOptions Options = new ZKillboardOptions();
            Options.CorporationId.Add(98389365);
            Options.StartTime = new DateTime(2015, 04, 01);

            ZkbResponse Kills;  // = kb.GetKills(Options);
            ZkbResponse Losses; // = kb.GetLosses(Options);

            string thing = Properties.Settings.Default.TestThis;

            try
            {
                Kills = kb.GetKills(Options);
                Losses = kb.GetLosses(Options);

                foreach (ZkbResponse.ZkbKill Kill in Kills)
                {
                    textBox2.Text = string.Concat(textBox2.Text, "R3MUS KILL at  ", Kill.KillTime.ToString(), Environment.NewLine);
                    textBox2.Text = string.Concat(textBox2.Text, "Victim: ", Kill.Victim.CharacterName, " lost a ", GetProductType(Kill.Victim.ShipTypeId).Name, Environment.NewLine);
                    foreach(ZkbResponse.ZkbAttacker Attacker in Kill.Attackers)
                    {
                        if (Attacker.FinalBlow == "1")
                        {
                            textBox2.Text = string.Concat(textBox2.Text, "Last Blow: ", Attacker.CharacterName, Environment.NewLine);
                        }
                        if (Attacker.CorporationId == 98389365)
                        {
                            textBox2.Text = string.Concat(textBox2.Text, "Wolf ", Attacker.CharacterName, " got on the board in a ", GetProductType(Attacker.ShipTypeId).Name, "!", Environment.NewLine);
                        }
                    }
                    if (Kill.Stats != null)
                    {
                        textBox2.Text = string.Concat(textBox2.Text, "Loss: ", Kill.Stats.TotalValue.ToString(), Environment.NewLine);
                    }
                    else
                    {
                        textBox2.Text = string.Concat(textBox2.Text, "Loss value not obtainable.", Environment.NewLine);
                    }
                }

            }
            catch (Exception ex)
            {
            }
        }

        private ProductType GetProductType(int shipTypeId)
        {
            long KeyID = 4086267;
            string VCode = "HUkqOyewFYvuSkWgDoCOH1L2juXnaxh3gCLDT1qwAfrpYt381CkT6XeZ8qe2rpH0";

            KeyID = 3704919;
            VCode = "JXcmbP0t9NCR6rld6qxOvbDFomx076ZERIs84p5XoBn0IYzVJJX1Jj6bIEYhBKlk";

            EveApi API;
            APIKeyInfo KeyInfo;
            List<AccountCharacter> Characters;
            List<CorporationMemberTrackingEntry> CorpMembers;
            ProductType PType;

            try
            {
                API = new EveApi("Clyde en Marland's API Checker", KeyID, VCode);
                KeyInfo = API.GetApiKeyInfo();

                PType = EveApi.EveApiCore.FindProductType(shipTypeId);
            }
            catch (Exception Ex)
            {
                PType = new ProductType();
                PType.Name = "Error querying API server";
                PType.Description = Ex.Message;
            }

            return PType;
        }
    }
}
