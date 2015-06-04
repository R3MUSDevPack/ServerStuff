using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace JKON.EveWho.EveCharacter.Models
{
    //public class rowset
    //{
    //    public List<row>
    //}

    [DataContract]
    public class row
    {
        [DataMember]
        public long recordID { get; set; }
        [DataMember]
        public long corporationID { get; set; }
        [DataMember]
        public string corporationName { get; set; }
        [DataMember]
        public DateTime startDate { get; set; }
    }
}
