using JKON.EveWho.EveCharacter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace JKON.EveWho.Models
{
    [DataContract]
    public class EveCharacter
    {
        [DataMember]
        [XmlElement("currentTime")]
        public string _currentTime { get; set; }

        [XmlIgnore]
        public DateTime currentTime 
        {
            get
            {
                return Convert.ToDateTime(_currentTime);
            }
        }

        [DataMember]
        [XmlElement("cachedUntil")]
        public string _cachedUntil { get; set; }

        [XmlIgnore]
        public DateTime cachedUntil
        {
            get
            {
                return Convert.ToDateTime(_cachedUntil);
            }
        }

        [DataMember]
        public result result { get; set; }
    }

    [DataContract]
    public class EveCharacterID
    {
        [DataMember]
        [XmlElement("currentTime")]
        public string _currentTime { get; set; }

        [XmlIgnore]
        public DateTime currentTime
        {
            get
            {
                return Convert.ToDateTime(_currentTime);
            }
        }

        [DataMember]
        [XmlElement("cachedUntil")]
        public string _cachedUntil { get; set; }

        [XmlIgnore]
        public DateTime cachedUntil
        {
            get
            {
                return Convert.ToDateTime(_cachedUntil);
            }
        }

        [DataMember]
        public JKON.EveWho.EveCharacterID.Models.result result { get; set; }
    }
}
