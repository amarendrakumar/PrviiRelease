using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace Prvii.Web
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            RegisterRoutes(RouteTable.Routes);
        }

        void RegisterRoutes(RouteCollection routes)
        {
            routes.MapPageRoute("",
                "Celebrity/{name}/", "~/WebPages/Channels/ChannelDetailsView.aspx");
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            if (ConfigurationManager.AppSettings["MaintenanceMode"] == "true")
            {
                string allowedIPs = ConfigurationManager.AppSettings["allowedIPs"].ToString();
                if (!Request.IsLocal && !allowedIPs.Contains(Request.UserHostAddress))
                {
                    HttpContext.Current.RewritePath("maintenance.aspx");
                }
            }

        }
       

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}