using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace r3mus.Models
{
    public class MoodleUser
    {
        public string username { get; set; }
        public string password { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string email { get; set; }
    }

    public class MoodleException
    {
        public string exception { get; set; }
        public string errorcode { get; set; }
        public string message { get; set; }
        public string debuginfo { get; set; }
    }

    public class MoodleCreateUserResponse
    {
        public string id { get; set; }
        public string username { get; set; }
    }
}