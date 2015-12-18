using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Prvii.Business;
using Prvii.Entities;
using Prvii.Web.AppCode;

namespace Prvii.Web.WebPages.ShoppingCarts
{
    public partial class checkout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Request["CartID"] != null)
                {
                    Session["ShoppingCartID"] = Convert.ToString(Request["CartID"]);
                    List<Channel> channel = ShoppingCartManager.GetItem(Convert.ToInt64(Session["ShoppingCartID"])).ToList();
                  
                    string token = "", expressCheckoutURL = "";
                    PayPalManager payPal = new PayPalManager();
                    bool result = payPal.MarkExpressCheckoutMobile(ref token, ref expressCheckoutURL, channel, Convert.ToString(Request["CartID"]));
                    if (result)
                    {
                        Session["token"] = token;                       
                        Response.Redirect(expressCheckoutURL, true);
                    }
                }
                    
            }
        } 
    }
}