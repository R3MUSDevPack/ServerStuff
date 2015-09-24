using EveAI.Live;
using JKON.EveWho.Models;
using r3mus.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace r3mus.Controllers
{
    [Authorize]
    public class LogisticsController : Controller
    {
        public ActionResult ContractStatus()
        {
            EveApi api;
            List<EveAI.Live.Utility.Contract> Contracts = new List<EveAI.Live.Utility.Contract>();
            Dictionary<long, EveCharacter> Names = new Dictionary<long, EveCharacter>();
            List<long> IDs = new List<long>();

            DateTime backDate = DateTime.Now.AddDays(-7).Date;

            try
            {
                api = new EveAI.Live.EveApi("Clyde en Marland's Contract Notifier", (long)Properties.Settings.Default.LogisticsCorpAPI, Properties.Settings.Default.LogisticsVCode);
                Contracts = api.GetCorporationContracts().ToList().Where(contract =>
                    (contract.Type == EveAI.Live.Utility.Contract.ContractType.Courier)
                    &&
                    (contract.Status != EveAI.Live.Utility.Contract.ContractStatus.Deleted)
                    &&
                    ((contract.DateIssued >= backDate)
                    ||
                    (contract.Status == EveAI.Live.Utility.Contract.ContractStatus.Outstanding)
                    ||
                    (contract.Status == EveAI.Live.Utility.Contract.ContractStatus.InProgress))).ToList();

                IDs = Contracts.Select(contract => contract.IssuerID).ToList();

                Contracts.Select(contract => contract.IssuerID).ToList().ForEach(id =>
                        {
                            if (!Names.ContainsKey(id))
                            {
                                Names.Add(id, JKON.EveWho.Api.GetCharacter(id));
                            }
                        });
                Contracts.Select(contract => contract.AcceptorID).ToList().ForEach(id =>
                {
                    if (!Names.ContainsKey(id))
                    {
                        Names.Add(id, JKON.EveWho.Api.GetCharacter(id));
                    }
                });
                
            }
            catch(Exception ex)
            {

            }

            return View(new LogisticsContractsViewModel() { DisplayContracts = Contracts, CharacterInfos = Names });
        }
	}
}