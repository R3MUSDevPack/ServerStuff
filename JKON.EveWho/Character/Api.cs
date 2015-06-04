using EveAI.Live;
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
        private const string c_URL = "https://api.eveonline.com/eve/CharacterInfo.xml.aspx";
        private const string c_CHARID = "characterID={0}";

        public static Models.EveCharacter GetCharacter(string characterName, long apikey, string vcode)
        {
            var api = new EveApi("R3MUS Recruitment", apikey, vcode);

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
                string reqURL = string.Concat(c_URL, "?", string.Format(c_CHARID, characterId));
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(reqURL);
                request.UserAgent = "JKON.EveWho-Clyde-en-Marland";
                WebResponse response = request.GetResponse();
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream);

                string data = reader.ReadToEnd();

                //JavaScriptSerializer cruncher = new JavaScriptSerializer();
                //Models.EveCharacter toon = cruncher.Deserialize<Models.EveCharacter>(data);

                //Models.EveCharacter toon = (Models.EveCharacter)cruncher.Deserialize(stream);

                //XmlDocument xmlDoc = new XmlDocument();
                //xmlDoc.LoadXml(data);
                Models.EveCharacter toon;
                //data = data.Replace(@"version=""2""", @"version=""2"" xmlns=""http://tempuri.org/eveapi/""");

                using (TextReader treader = new StringReader(data))
                {
                    XmlRootAttribute xRoot = new XmlRootAttribute();
                    xRoot.ElementName = "eveapi";
                    // xRoot.Namespace = "http://www.cpandl.com";
                    xRoot.IsNullable = true;
                    XmlSerializer cruncher = new XmlSerializer(typeof(Models.EveCharacter), xRoot);
                    toon = (Models.EveCharacter)cruncher.Deserialize(treader);
                }

                //reader.Close();
                response.Close();

                //return toon;
                return toon;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
