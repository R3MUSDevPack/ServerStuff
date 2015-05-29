using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace r3mus.Filters
{
#region "Filters"

    public class ApiKeyAttribute : ValidationAttribute
    {
        private const string NullValueMessage = "You MUST supply an API Key";
        private const string InvalidMessage = "You appear to have supplied an invald API Key";

        private const string NumericRegex = "[0-9]";

        [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
        public class MultipleButtonAttribute : ActionNameSelectorAttribute
        {
            public string Name { get; set; }
            public string Argument { get; set; }

            public override bool IsValidName(ControllerContext controllerContext, string actionName, MethodInfo methodInfo)
            {
                var isValidName = false;
                var keyValue = string.Format("{0}:{1}", Name, Argument);
                var value = controllerContext.Controller.ValueProvider.GetValue(keyValue);

                if (value != null)
                {
                    controllerContext.Controller.ControllerContext.RouteData.Values[Name] = Argument;
                    isValidName = true;
                }

                return isValidName;
            }
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            Regex regex;
            Match match;

            if (value != null)
            {
                regex = new Regex(NumericRegex);
                match = regex.Match(value.ToString());

                if ((value.ToString().Length == 7) && (match.Success))
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult(InvalidMessage);
                }
            }
            else
            {
                return new ValidationResult(NullValueMessage);
            }
        }
    }     

#endregion
}