using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace JKON.EveApi.Corporation.Models
{
    [DataContract]
    public class MemberQuery
    {
        [DataMember]
        public DateTime currentTime { get; set; }

        [DataMember]
        public result result { get; set; }

        [DataMember]
        public DateTime cachedUntil { get; set; }
    }

    [DataContract]
    public class Member
    {
        [DataMember]
        [System.Xml.Serialization.XmlElement("characterID")]
        public long ID { get; set; }

        [DataMember]
        [System.Xml.Serialization.XmlElement("name")]
        public string Name { get; set; }

        [DataMember]
        [System.Xml.Serialization.XmlElement("startDateTime")]
        public DateTime StartDateTime { get; set; }

        [DataMember]
        [System.Xml.Serialization.XmlElement("baseID")]
        public int BaseID { get; set; }

        [DataMember]
        [System.Xml.Serialization.XmlElement("base")]
        public string Base { get; set; }

        [DataMember]
        [System.Xml.Serialization.XmlElement("title")]
        public string Title { get; set; }
    }
}
