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
        public List<LastWeeksMailStat> MailersLastWeek { get; set; }
        public List<LastWeeksSubmissionStat> SubmittersLastWeek { get; set; }
        public List<LastMonthsMailStat> MailersLastMonth { get; set; }
        public List<LastMonthsSubmissionStat> SubmittersLastMonth { get; set; }
    }
}