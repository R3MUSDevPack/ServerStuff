using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

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
        [XmlAttribute("recordID")]
        public long RecordID { get; set; }
        [DataMember]
        [XmlAttribute("corporationID")]
        public long CorporationID { get; set; }
        [DataMember]
        [XmlAttribute("corporationName")]
        public string CorporationName { get; set; }
        [DataMember]
        [XmlAttribute("startDate")]
        public string StartDate { get; set; }
    }
}
