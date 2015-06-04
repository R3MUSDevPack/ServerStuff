using EveAI.Live;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace JKON.EveWho
{
    public class EveWho
    {
        private const string c_URL = "http://evewho.com/api.php";
        private const string c_TYPE = "type={0}";
        private const string c_ID = "id={0}";
        private const string c_PAGE = "page={0}";

        private enum Types
        {
            character,
            alliance,
            corporation
        }

        public static Models.Character GetCharacter(string characterName, long apikey, string vcode)
        {
            var api = new EveApi("R3MUS Recruitment", apikey, vcode);

            List<string> list = new List<String>();
            list.Add(characterName);
            Dictionary<string, long> dict = api.ConvertNamesToIDs(list);
            
            try
            {
                dict = api.ConvertNamesToIDs(list);
            }
            catch(Exception ex)
            {
                return new Models.Character() { info = new Character.Models.info() { name = characterName, character_id = 0, corporation_id = 0, alliance_id = 0, faction_id = 0, sec_status = 0.0 }, history = null };
            }
            if ((dict[characterName] == null) || (dict[characterName] == 0))
            {
                return new Models.Character() { info = new Character.Models.info() { name = characterName, character_id = 0, corporation_id = 0, alliance_id = 0, faction_id = 0, sec_status = 0.0 }, history = null };
            }
            else
            {
                return GetCharacter(dict[characterName]);
            }
        }

        public static Models.Character GetCharacter(long characterId)
        {
            try
            {
                string reqURL = string.Concat(c_URL, "?", string.Format(c_TYPE, Types.character.ToString()), "&", string.Format(c_ID, characterId.ToString()), "&", string.Format(c_PAGE, "0"));
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(reqURL);
                request.UserAgent = "JKON.EveWho-Clyde-en-Marland";
                WebResponse response = request.GetResponse();
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream);

                string data = reader.ReadToEnd();

                JavaScriptSerializer cruncher = new JavaScriptSerializer();
                Models.Character toon = cruncher.Deserialize<Models.Character>(data);

                reader.Close();
                response.Close();

                return toon;
            }
            catch(Exception ex)
            {
                return null;
            }
        }
    }
}
