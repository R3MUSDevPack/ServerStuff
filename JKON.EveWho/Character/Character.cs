using JKON.EveWho.Character.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace JKON.EveWho.Models
{
    [DataContract]
    public class Character
    {
        [DataMember]
        public info info { get; set; }

        [DataMember]
        public List<history> history { get; set; }
    }
}
