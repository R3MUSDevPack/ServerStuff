using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using r3mus.Models;

namespace r3mus.Infrastructure
{
    public interface IApplicationUser
    {
        IList<ApiInfo> ApiKeys {get;set;}
        string MemberType { get; set; }
        void AddApiInfo(string api, string vcode);
        bool IsValid();
        long GetCorpOrAllianceId(r3mus.Models.ApplicationUser.IDType type, int apiKey, string vcode);
    }
}
