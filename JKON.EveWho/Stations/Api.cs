using JKON.EveWho.Stations.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml.Linq;

namespace JKON.EveWho.Stations
{
    public class Api
    {
        private const string c_URL = "https://api.eveonline.com/eve/ConquerableStationList.xml.aspx";

        public static Station GetStation(long stationID)
        {
            return GetConquerableStations().Where(s => s.stationId == stationID).FirstOrDefault();
        }

        public static List<Station> GetConquerableStations()
        {
            string xml = GetResponse(c_URL);
            var stationList = new List<Station>();

            try
            {
                using (TextReader treader = new StringReader(xml))
                {
                    var xDoc = XDocument.Load(treader, LoadOptions.None);

                    xDoc.Element("eveapi").Elements("result").Elements("rowset").Elements("row").ToList().ForEach(row => stationList.Add(
                        new Station()
                        {
                            stationId = Convert.ToInt64(row.Attribute("stationID").Value),
                            stationName = row.Attribute("stationName").Value,
                            stationTypeID = Convert.ToInt64(row.Attribute("stationTypeID").Value),
                            solarSystemID = Convert.ToInt64(row.Attribute("solarSystemID").Value),
                            corporationID = Convert.ToInt64(row.Attribute("corporationID").Value),
                            corporationName = row.Attribute("corporationName").Value
                        }
                        ));
                }

            }
            catch (Exception ex)
            {
            }
            return stationList;
        }

        private static string GetResponse(string URI)
        {
            string data;

            using (WebClient client = new WebClient())
            {
                byte[] response = client.DownloadData(URI);
                data = System.Text.Encoding.UTF8.GetString(response);
            }

            return data;
        }
    }
}
