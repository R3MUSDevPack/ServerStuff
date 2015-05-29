using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TentacleSoftware.TeamSpeakQuery;
using TentacleSoftware.TeamSpeakQuery.ServerQueryResult;

namespace Teamspeak_Plugin
{
    public class Teamspeak
    {
        public enum AddSuccess
        {
            Success,
            Fail
        }

        ServerQueryClient client;
        ClientListResult clientList;
        ClientInfoResult selected;
        public string Message;
        int DBID;
        int SGID;
        public AddSuccess SuccessType { get; set; }

        public async Task<bool> AddClient(string name, string groupName, string TSURL, string password)
        {
            client = new ServerQueryClient(TSURL, 10011, TimeSpan.FromSeconds(3));
            try
            {
                ServerQueryBaseResult connected = client.Initialize().Result;
                if (connected.Success)
                {
                    ServerQueryBaseResult login = client.Login("serveradmin", password).Result;
                    if (login.Success)
                    {
                        ServerQueryBaseResult use = client.Use(UseServerBy.Port, 9987).Result;

                        if (use.Success)
                        {
                            client.KeepAlive(TimeSpan.FromMinutes(2));
                            clientList = client.ClientList().Result;

                            ClientInfoResult ClientInfo = clientList.Values.Where(m => m.ClientNickname == name).FirstOrDefault();

                            if (ClientInfo != null)
                            {
                                await GetDBID(ClientInfo.ClientUniqueIdentifier);
                                await GetServerGroup(groupName);
                                await AddClient();
                                Message = string.Format("User {0} successfully created on TS Server {1}.", name, TSURL);
                            }
                            else
                            {
                                SuccessType = AddSuccess.Fail;
                                Message = string.Format("Could not find a valid user on {0}. Please make sure your nickname is {1} and try again.", TSURL, name);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SuccessType = AddSuccess.Fail;
            }
            finally
            {
                ServerQueryBaseResult unregister = client.ServerNotifyUnregister().Result;
                ServerQueryBaseResult logout = client.Logout().Result;
                ServerQueryBaseResult quit = client.Quit().Result;
            }
            return true;
        }

        private async Task GetDBID(string ID)
        {
            Task<TextResult> result = client.SendCommandAsync(string.Concat("clientgetdbidfromuid cluid=", ID));
            TextResult resultText = await result;
            DBID = Convert.ToInt32(resultText.Response.Substring((resultText.Response.IndexOf("cldbid=") + 7), (resultText.Response.Length - (resultText.Response.IndexOf("cldbid=") + 7))));
        }

        private async Task GetServerGroup(string groupName)
        {
            Task<TextResult> result = client.SendCommandAsync("servergrouplist");
            TextResult resultText = await result;
            string group = resultText.Response.Split(new char[]{'|'}).ToList<string>().Where(sg => sg.Contains(string.Format("name={0} ", groupName))).FirstOrDefault();
            if (group != string.Empty)
            {
                SGID = Convert.ToInt32(group.Split(new char[] { ' ' })[0].Substring(5));
            }
        }

        private async Task AddClient()
        {
            Task<TextResult> result = client.SendCommandAsync(string.Format("servergroupaddclient sgid={0} cldbid={1}", SGID.ToString(), DBID.ToString()));
            TextResult resultText = await result;
            if (resultText.Response.Contains("errorid=0"))
            {
                SuccessType = AddSuccess.Success;
                Message = "Teamspeak Registration Successful!";
            }
            else
            {
                SuccessType = AddSuccess.Fail;
                Message = string.Concat("Teamspeak Registration Unsuccessful: ", resultText.Response);
            }
        }
    }
}
