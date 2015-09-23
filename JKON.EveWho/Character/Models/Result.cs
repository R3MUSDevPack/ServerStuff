﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace JKON.EveWho.EveCharacter.Models
{
    [DataContract]
    public class result
    {
        [DataMember]
        public long characterID { get; set; }
        [DataMember]
        public string characterName { get; set; }
        [DataMember]
        public string race { get; set; }
        [DataMember]
        public string bloodline { get; set; }
        [DataMember]
        public long corporationID { get; set; }
        [DataMember]
        public string corporation { get; set; }

        [DataMember]
        [XmlElement("corporationDate")]
        public string _corporationDate { get; set; }

        [XmlIgnore]
        public DateTime corporationDate
        {
            get
            {
                return Convert.ToDateTime(_corporationDate);
            }
        }

        [DataMember]
        public double securityStatus { get; set; }

        [DataMember]
        [XmlElement("rowset")]
        public rowset employmentHistory { get; set; }
    }

    public class rowset
    {
        [XmlElement("row")]
        public List<row> employmentRecords { get; set; }
    }
}

namespace JKON.EveWho.EveCharacterID.Models
{
    [DataContract]
    public class result
    {
        [DataMember]
        public List<row> rowset { get; set; }

    }

    
    [DataContract]
    public class row
    {
        [DataMember]
        [System.Xml.Serialization.XmlAttribute("characterID")]
        public long ID { get; set; }
        [DataMember]
        [System.Xml.Serialization.XmlAttribute("name")]
        public string Name { get; set; }
    }
}