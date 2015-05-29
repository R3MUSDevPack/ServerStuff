using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CookComputing.XmlRpc;

namespace r3mus.Infrastructure
{
    public interface IMoodle : IXmlRpcProxy
    {
        [XmlRpcMethod("moodle_user_get_users_by_id")]
        object[] GetUserById(object[] id);
        [XmlRpcMethod("moodle_course_get_courses")]
        object[] GetCourses();
        [XmlRpcMethod("system/listMethods")]
        string[] ListMethods();
        [XmlRpcMethod("auth/email/auth.php/user_login")]
        bool Login(string user, string password);
    }
}