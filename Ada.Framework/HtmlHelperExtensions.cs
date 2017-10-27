using System;
using System.Web.Mvc;

namespace Ada.Framework
{
  public static class HtmlHelperExtensions
    {
        public static string IsSelected(this HtmlHelper html, string controller = null, string action = null, string cssClass = null)
        {

            if (String.IsNullOrEmpty(cssClass))
                cssClass = "active";

            string currentAction = (string)html.ViewContext.RouteData.Values["action"];
            string currentController = (string)html.ViewContext.RouteData.Values["controller"];

            if (String.IsNullOrEmpty(controller))
                controller = currentController;

            if (String.IsNullOrEmpty(action))
                action = currentAction;

            return controller == currentController && action == currentAction ?
                cssClass : string.Empty;
        }

        public static string PageClass(this HtmlHelper html)
        {
            string currentAction = (string)html.ViewContext.RouteData.Values["action"];
            return currentAction;
        }




        private static Action<T> Fix<T>(Func<Action<T>, Action<T>> f)
        {
            return x => f(Fix(f))(x);
        }

        public static void Render<T>(this HtmlHelper helper, T model, Func<Action<T>, Action<T>> f)
        {
            Fix(f)(model);
        }
    }
}
