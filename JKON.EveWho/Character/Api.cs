using EveAI.Live;
using JKON.EveApi.Corporation.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace JKON.EveWho
{
    public class Api
    {
        private const string c_URL = "https://api.eveonline.com/";
        private const string c_CHARINFOURL = "eve/CharacterInfo.xml.aspx";
        private const string c_CHARIDFROMNAMEURL = "eve/CharacterID.xml.aspx";

        private const string c_CORPMEMBERSURL = "corp/MemberTracking.xml.aspx";
        private const string c_CHARID = "characterID={0}";
        private const string c_API = "KeyID={0}&vCode={1}";
        private const string c_EXT = "&extended=1";
        private const string c_CHARNAME = "names={0}";
        
        public static Models.EveCharacter GetCharacter(string characterName)
        {
            return GetCharacter(GetCharacterID(characterName));
        }
        public static long GetCharacterID(string characterName)
        {
            try
            {
                string reqURL = string.Concat(c_URL, c_CHARIDFROMNAMEURL, "?", string.Format(c_CHARNAME, characterName));
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(reqURL);
                request.UserAgent = "JKON.EveWho-Clyde-en-Marland";
                WebResponse response = request.GetResponse();
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream);

                string data = reader.ReadToEnd();

                Models.EveCharacterID toon;

                using (TextReader treader = new StringReader(data))
                {
                    XmlRootAttribute xRoot = new XmlRootAttribute();
                    xRoot.ElementName = "eveapi";
                    xRoot.IsNullable = true;
                    XmlSerializer cruncher = new XmlSerializer(typeof(Models.EveCharacterID), xRoot);
                    toon = (Models.EveCharacterID)cruncher.Deserialize(treader);
                }

                response.Close();

                return toon.result.rowset.FirstOrDefault().ID;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public static Models.EveCharacter GetCharacter(string characterName, long apikey, string vcode)
        {
            var api = new EveAI.Live.EveApi("R3MUS Recruitment", apikey, vcode);

            List<string> list = new List<String>();
            list.Add(characterName);
            Dictionary<string, long> dict = api.ConvertNamesToIDs(list);
            try
            {
                dict = api.ConvertNamesToIDs(list);
            }
            catch (Exception ex)
            {
                return new Models.EveCharacter()
                {
                    _currentTime = DateTime.Now.ToString(),
                    _cachedUntil = DateTime.Now.ToString(),
                    //,
                    //result = new EveCharacter.Models.result() { 
                                    //characterName = characterName, 
                                    //characterID = 0, 
                                    //corporationID = 0, 
                                    //corporation = ex.Message, 
                                    //bloodline = string.Empty,
                                    //securityStatus = 0.0,
                                    //corporationDate = new DateTime(), 
                                    //race = string.Empty, 
                                    //employmentHistory = null
                    //}
                };                
            }
            if ((dict[characterName] == null) || (dict[characterName] == 0))
            {
                return new Models.EveCharacter()
                {
                    _currentTime = DateTime.Now.ToString(),
                    _cachedUntil = DateTime.Now.ToString(),
                    //    ,
                    //result = new EveCharacter.Models.result()
                    //{
                    //    characterName = characterName,
                    //    characterID = 0,
                    //    corporationID = 0,
                    //    corporation = string.Empty,
                    //    bloodline = string.Empty,
                    //    securityStatus = 0.0,
                    //    corporationDate = new DateTime(),
                    //    race = string.Empty,
                    //    employmentHistory = null
                    //}
                };        
            }
            else
            {
                return GetCharacter(dict[characterName]);
            }
        }

        public static Models.EveCharacter GetCharacter(long characterId)
        {
            try
            {
                string reqURL = string.Concat(c_URL, c_CHARINFOURL, "?", string.Format(c_CHARID, characterId));
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(reqURL);
                request.UserAgent = "JKON.EveWho-Clyde-en-Marland";
                WebResponse response = request.GetResponse();
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream);

                string data = reader.ReadToEnd();

                Models.EveCharacter toon;

                using (TextReader treader = new StringReader(data))
                {
                    XmlRootAttribute xRoot = new XmlRootAttribute();
                    xRoot.ElementName = "eveapi";
                    xRoot.IsNullable = true;
                    XmlSerializer cruncher = new XmlSerializer(typeof(Models.EveCharacter), xRoot);
                    toon = (Models.EveCharacter)cruncher.Deserialize(treader);
                }

                response.Close();

                return toon;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<Member> GetCorpMembers(long apiKey, string vCode)
        {
            JKON.EveApi.Corporation.Models.MemberQuery members;
            var result = new List<Member>();

            try
            {
                string reqURL = string.Concat(c_URL, c_CORPMEMBERSURL, "?", string.Format(c_API, apiKey, vCode), c_EXT);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(reqURL);
                request.UserAgent = "JKON.EveWho-Clyde-en-Marland";
                WebResponse response = request.GetResponse();
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream);

                string data = reader.ReadToEnd();

                using (TextReader treader = new StringReader(data))
                {
                    XmlRootAttribute xRoot = new XmlRootAttribute();
                    xRoot.ElementName = "eveapi";
                    xRoot.IsNullable = true;
                    XmlSerializer cruncher = new XmlSerializer(typeof(JKON.EveApi.Corporation.Models.MemberQuery), xRoot);
                    members = (JKON.EveApi.Corporation.Models.MemberQuery)cruncher.Deserialize(treader);
                }

                response.Close();

                members.result.rowset.ForEach(member => result.Add(
                    new Member() { 
                        ID = member.ID,
                        Name = member.Name, 
                        Title = member.Title, 
                        MemberSince = Convert.ToDateTime(member.StartDateTime), 
                        LastLogonDateTime  = member.LastLogonDateTime, 
                        ShipType = member.ShipType, 
                        Location = member.Location, 
                        Roles = member.Roles, 
                        GrantableRoles = member.GrantableRoles
                    }));
            }
            catch (Exception ex)
            {
                if(ex.Message.Contains("404"))
                {
                    result.Add(new Member() { ID = 0, Name = "The API Server is offline; member information cannot be displayed." });
                }
            }
            return result;
        }
    }
}
