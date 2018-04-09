using System;
using System.Web.Mvc;

namespace Ada.Framework
{
    public static class HtmlHelperExtensions
    {
        public static string IsSelected(this HtmlHelper html, string area = null, string controller = null, string action = null, string cssClass = null)
        {

            if (String.IsNullOrEmpty(cssClass))
                cssClass = "active";
            string currentArea = (string)html.ViewContext.RouteData.Values["area"];
            string currentAction = (string)html.ViewContext.RouteData.Values["action"];
            string currentController = (string)html.ViewContext.RouteData.Values["controller"];
            if (String.IsNullOrEmpty(area))
                area = currentArea;
            if (String.IsNullOrEmpty(controller))
                controller = currentController;
            if (String.IsNullOrEmpty(action))
                action = currentAction;

            return area == currentArea && controller == currentController && action == currentAction ?
                cssClass : string.Empty;
        }
        public static string IsActive(this HtmlHelper html, string controller = null, string action = null, string cssClass = null)
        {

            if (String.IsNullOrEmpty(cssClass))
                cssClass = "active";
            string currentAction = html.ViewContext.RouteData.Values["action"].ToString().ToLower();
            string currentController = html.ViewContext.RouteData.Values["controller"].ToString().ToLower();
           
            if (String.IsNullOrEmpty(controller))
                controller = currentController;
            if (String.IsNullOrEmpty(action))
                action = currentAction;

            return  controller.ToLower() == currentController && action.ToLower() == currentAction ?
                cssClass : string.Empty;
        }
        public static string PageClass(this HtmlHelper html)
        {
            string currentAction = (string)html.ViewContext.RouteData.Values["action"];
            return currentAction;
        }


    }
}
