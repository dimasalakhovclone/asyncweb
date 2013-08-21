using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Routing;

namespace Web.Extensions
{
    public static class UrlExtensions
    {
        public static string Action<TController>(this UrlHelper helper, Expression<Action<TController>> action) where TController : Controller
        {
            var valuesFromExpression = Microsoft.Web.Mvc.Internal.ExpressionHelper.GetRouteValuesFromExpression<TController>(action);

            var _controller = valuesFromExpression["Controller"] as string;
            var _action = valuesFromExpression["Action"] as string;

            return UrlHelper.GenerateUrl(null, _action, _controller, valuesFromExpression, helper.RouteCollection, helper.RequestContext, true);
        }
    }
}