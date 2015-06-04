using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace JKON.EveWho
{
    public class Killboard
    {
        private static string c_URL = "https://zkillboard.com/api/{0}/corporationID/{1}/startTime/{2}/";
        
        public static void GetKills(long corpId, DateTime startDate)
        {
            GetResponse(string.Format(c_URL, "Kills", corpId, startDate.ToString("yyyyMMddHHmmss")));
        }

        private static void GetResponse(string URI)
        {
            JavaScriptSerializer cruncher = new JavaScriptSerializer();
            string data;

            using (WebClient client = new WebClient())
            {
                byte[] response = client.DownloadData(URI);
                data = System.Text.Encoding.UTF8.GetString(response);
            }

            
        }
    }
}
