using JKON.EveWho.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace r3mus.ViewModels
{
    public class LogisticsContractsViewModel
    {
        public List<EveAI.Live.Utility.Contract> DisplayContracts { get; set; }
        public Dictionary<long, EveCharacter> CharacterInfos { get; set; }
    }
}