using r3mus.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace r3mus.ViewModels
{
    public class RecruitmentStatsViewModel
    {
        public int MailsToSend { get; set; }
        public int ApplicationsToProcess { get; set; }
        public List<LastWeeksMailStat> Mailers { get; set; }
        public List<LastWeeksSubmissionStat> Submitters { get; set; }
    }
}