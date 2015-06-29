using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKON.EveWho.Stations.Models
{
    public class Station
    {
        public long stationId { get; set; }

        public string stationName { get; set; }

        public long stationTypeID { get; set; }

        public long solarSystemID { get; set; }

        public long corporationID { get; set; }

        public string corporationName { get; set; }
    }
}
