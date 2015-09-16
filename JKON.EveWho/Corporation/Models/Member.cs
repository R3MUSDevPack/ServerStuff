using EveAI.Live;
using System;
using System.Collections.Generic;
using System.IO;
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
        public string currentTime { get; set; }

        [DataMember]
        public result result { get; set; }

        [DataMember]
        public string cachedUntil { get; set; }
    }
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

        [DataMember]
        [System.Xml.Serialization.XmlAttribute("startDateTime")]
        public string StartDateTime { get; set; }

        [DataMember]
        [System.Xml.Serialization.XmlAttribute("baseID")]
        public int BaseID { get; set; }

        [DataMember]
        [System.Xml.Serialization.XmlAttribute("base")]
        public string Base { get; set; }

        [DataMember]
        [System.Xml.Serialization.XmlAttribute("title")]
        public string Title { get; set; }

        [DataMember]
        [System.Xml.Serialization.XmlAttribute("logonDateTime")]
        public string LastLogonDateTime { get; set; }

        [DataMember]
        [System.Xml.Serialization.XmlAttribute("logoffDateTime")]
        public string LastLogoffDateTime { get; set; }

        [DataMember]
        [System.Xml.Serialization.XmlAttribute("locationID")]
        public long LocationID { get; set; }

        [DataMember]
        [System.Xml.Serialization.XmlAttribute("location")]
        public string Location { get; set; }

        [DataMember]
        [System.Xml.Serialization.XmlAttribute("shipTypeID")]
        public long ShipTypeID { get; set; }

        [DataMember]
        [System.Xml.Serialization.XmlAttribute("shipType")]
        public string ShipType { get; set; }

        [DataMember]
        [System.Xml.Serialization.XmlAttribute("roles")]
        public string Roles { get; set; }

        [DataMember]
        [System.Xml.Serialization.XmlAttribute("grantableRoles")]
        public string GrantableRoles { get; set; }
    }

    public class Member
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public DateTime MemberSince { get; set; }
        public string LastLogonDateTime { get; set; }
        public string Location { get; set; }
        public string ShipType { get; set; }
        public string Roles { get; set; }
        public string GrantableRoles { get; set; }

        public string Avatar
        {
            get
            {
                return GetAvatar();
            }
            set
            {

            }
        }

        public string GetAvatar()
        {
            string result = string.Empty;
            try
            {
                var tempAvatar = ImageServer.DownloadCharacterImage(ID, ImageServer.ImageSize.Size128px);
                using (var stream = new MemoryStream())
                {
                    tempAvatar.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                    result = string.Format("data:image/png;base64,{0}", Convert.ToBase64String(stream.ToArray()));
                }
                tempAvatar.Dispose();
            }
            catch(Exception ex)
            {

            }
            return result;
        }
    }
}
