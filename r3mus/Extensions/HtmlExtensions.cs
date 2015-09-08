using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace r3mus.Extensions
{
    public static class HtmlExtensions
    {
        public static MvcHtmlString DisplayForEnum<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum>> expression)
        {
            return new MvcHtmlString(ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData).Model.ToString());
        }
    }
}