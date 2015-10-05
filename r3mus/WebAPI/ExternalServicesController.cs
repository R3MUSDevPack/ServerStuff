using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace r3mus.WebAPI
{
    public class ExternalServicesController : ApiController
    {
        [HttpGet]
        public IHttpActionResult RegisterForSlack(string group, string emailAddress, string token)
        {
            string result = string.Empty;
            string URI = string.Format(Properties.Settings.Default.SlackInviteURL, emailAddress, token, group);
            using (WebClient client = new WebClient())
            {
                byte[] response = client.DownloadData(URI);
                result = System.Text.Encoding.UTF8.GetString(response);
            }

            return Ok(result);
        }
    }
}
