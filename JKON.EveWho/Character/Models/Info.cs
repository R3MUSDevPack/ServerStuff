using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace JKON.EveWho.Character.Models
{
    [DataContract]
    public class info
    {
        [DataMember]
        public long character_id { get; set; }
        [DataMember]
        public long corporation_id { get; set; }
        [DataMember]
        public long alliance_id { get; set; }
        [DataMember]
        public long faction_id { get; set; }
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public double sec_status { get; set; }

        [DataMember]
        public List<history> history { get; set; }
    }
}
