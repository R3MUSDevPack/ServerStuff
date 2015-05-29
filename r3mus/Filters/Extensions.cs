using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using System.ComponentModel.DataAnnotations;

namespace r3mus.Filters
{
    public static class Extensions
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            try
            {
                return enumValue.GetType()
                                .GetMember(enumValue.ToString())
                                .First()
                                .GetCustomAttribute<DisplayAttribute>()
                                .GetName();
            }
            catch(Exception ex)
            {
                return enumValue.ToString();
            }
        }
    }
}