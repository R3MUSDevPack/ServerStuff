using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace JKON.EveWho.Character.Models
{
    [DataContract]
    public class history
    {
        [DataMember]
        public long corporation_id { get; set; }
        [DataMember]
        public DateTime start_date { get; set; }
        [DataMember]
        public DateTime? end_date { get; set; }
    }
}
