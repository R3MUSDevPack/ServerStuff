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

        private const string c_CORPMEMBERSURL = "eve/CharacterInfo.xml.aspx";
        private const string c_CHARID = "characterID={0}";
        private const string c_API = "KeyID={0}&vCode={1}";

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
            Models.EveCharacter toon;
            try
            {
                string reqURL = string.Concat(c_URL, "?", string.Format(c_API, apiKey, vCode));
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
                    XmlSerializer cruncher = new XmlSerializer(typeof(Models.EveCharacter), xRoot);
                    toon = (Models.EveCharacter)cruncher.Deserialize(treader);
                }

                response.Close();
            }
            catch (Exception ex)
            {

            }
        }
    }
}
