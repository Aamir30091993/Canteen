using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using GCC_Canteen.Models;
using System.IO;

namespace GCC_Canteen.App_Code
{
    public class GCC_CanteenAuthorizeAttribute : AuthorizeAttribute
    {
        public GCC_CanteenAuthorizeAttribute(int _PageID)
        {
            this.PageID = _PageID;
        }
        int PageID;
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (System.Web.HttpContext.Current.Request.QueryString["Model"] != null)
            {
                if (HttpContext.Current.Session["UserInfo"] != null)
                {
                    //Check Access Rights
                    UserInfo user = HttpContext.Current.Session["UserInfo"] as UserInfo;

                }
            }
            //bool authorized = base.AuthorizeCore(httpContext);
            bool RequestAuthorized = false;
            if (httpContext.Session["UserInfo"] != null && httpContext.Request.Headers["Cookie"] != null && httpContext.Request.Headers["Cookie"].ToString().IndexOf("ASP.NET_SessionId") >= 0)
            {
                RequestAuthorized = true;
            }
            else
            {
                HttpContext.Current.Response.Redirect("~/Dashboard//Show?RedirectUrl=" + HttpUtility.UrlEncode(HttpContext.Current.Request.Url.AbsoluteUri), true);
            }
            bool canAccess = false;
            if (RequestAuthorized)
            {
                var routeData = httpContext.Request.RequestContext.RouteData;
                var controller = routeData.GetRequiredString("controller");
                var action = routeData.GetRequiredString("action");

                if (HttpContext.Current.Session["UserInfo"] != null)
                {
                    //Check Access Rights
                    UserInfo user = HttpContext.Current.Session["UserInfo"] as UserInfo;
                    if (user.UserAccess.Where(m => m.PageID == PageID).ToList().Count > 0)
                    {
                        canAccess = true;
                    }
                }
                else
                {
                    return false;
                }
                if (!canAccess)
                {
                    UserInfo user = HttpContext.Current.Session["UserInfo"] as UserInfo;
                    httpContext.Items["Error"] = true;
                    return false;
                }
                return true;
            }
            else
            {
                httpContext.Items["AuthorizationFailed"] = true;
                return RequestAuthorized;

            }
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            bool Error = filterContext.HttpContext.Items["Error"] != null && Convert.ToBoolean(filterContext.HttpContext.Items["Error"].ToString());
            if (Error)
            {
                //change the controller name and action name accordingly as needed.
                filterContext.Result = new RedirectToRouteResult(new
                                        RouteValueDictionary(new { controller = "UnauthorisedAccess", action = "Show", area = "" }));
            }
            bool AuthorizationFailed = filterContext.HttpContext.Items["AuthorizationFailed"] != null && Convert.ToBoolean(filterContext.HttpContext.Items["AuthorizationFailed"].ToString());
            if (AuthorizationFailed)
            {
                if (new HttpRequestWrapper(System.Web.HttpContext.Current.Request).IsAjaxRequest())
                {
                    filterContext.HttpContext.Items["AjaxPermissionDenied"] = true;
                }
                base.HandleUnauthorizedRequest(filterContext);
            }

        }
    }
}