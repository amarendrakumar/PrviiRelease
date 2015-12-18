using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Prvii.ExceptionHandling;
using Prvii.Web.AppCode;

namespace Prvii.Web
{
    public partial class ErrorPage : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            int errorID = Convert.ToInt32(Request["ErrorID"]);

            if(errorID == 1)
            {
                FriendlyErrorMsg.Text = "Access Error! You do not have permission to access this page.";
                return;
            }
            else if (errorID == 2)
            {
                FriendlyErrorMsg.Text = "Invalid URL! Page not found.";
                return;
            }

            // Create safe error messages.
            string generalErrorMsg = "A problem has occurred on this web site. Please try again. " +
                "If this error continues, please contact support.";

            // Display safe error message.
            FriendlyErrorMsg.Text = generalErrorMsg;
         }
    }
}