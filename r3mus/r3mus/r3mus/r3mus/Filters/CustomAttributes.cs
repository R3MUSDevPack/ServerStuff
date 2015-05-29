using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace r3mus.Filters
{
#region "Filters"

    public class ApiKeyAttribute : ValidationAttribute
    {
        private const string NullValueMessage = "You MUST supply an API Key";
        private const string InvalidMessage = "You appear to have supplied an invald API Key";

        private const string NumericRegex = "[0-9]";

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