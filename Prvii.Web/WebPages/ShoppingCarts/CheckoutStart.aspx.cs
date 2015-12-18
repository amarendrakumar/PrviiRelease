using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using Prvii.Business;
using Prvii.Entities.Enumerations;

namespace Prvii.Web.WebPages.ShoppingCarts
{
    public partial class CheckoutStart : System.Web.UI.Page
    {
        protected string payPalURL = string.Empty;
        protected string business = string.Empty;
        protected string item_name = string.Empty;
        protected string item_number = string.Empty;
        protected string currency_code = string.Empty;
        protected string a3 = string.Empty;
        protected string p3 = string.Empty;
        protected string t3 = string.Empty;
        protected string src = string.Empty;
        protected string srt = string.Empty;
        protected string no_shipping = string.Empty;
        protected string return_url = string.Empty;
        protected string rm = string.Empty;
        protected string notify_url = string.Empty;
        protected string cancel_url = string.Empty;
        protected string custom = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            string ServerUrl = WebConfigurationManager.AppSettings["ServerUrl"].ToString();

            string returnURL = ServerUrl + "/WebPages/ShoppingCarts/CheckoutComplete.aspx"; 
            string cancelURL = ServerUrl + "/WebPages/ShoppingCarts/CheckoutCancel.aspx"; 
            string ipnURL = ServerUrl + "/WebPages/ShoppingCarts/CheckoutIPNHandler.aspx";

            string cartID = Session["ShoppingCartID"].ToString();

            var channel = ShoppingCartManager.GetFirstItem(Convert.ToInt64(cartID));

            int billingPeriod = 1;
            string billingFrequency = string.Empty;

            if (channel.BillingCycleID == (short)BillingCycleType.Monthly)
                billingFrequency = "M";
            else if (channel.BillingCycleID == (short)BillingCycleType.Quarterly)
            {
                billingFrequency = "M";
                billingPeriod = 3;
            }
            else if (channel.BillingCycleID == (short)BillingCycleType.Yearly)
                billingFrequency = "Y";

            if(channel.NoOfBillingPeriod.HasValue && channel.NoOfBillingPeriod.Value > 0)
            {
                srt = "<input type=\"hidden\" name=\"srt\" value=\"" + channel.NoOfBillingPeriod.Value + "\">";
            }

            PayPalManager payPal = new PayPalManager();

            payPalURL = payPal.PAYPAL_URL;
            business = payPal.PAYPAL_BUSINESS_EMAIL;
            item_name = channel.Firstname + " " + channel.Lastname +" Subscription";
            item_number = cartID;
            currency_code = "USD";
            a3 = Session["payment_amt"].ToString();
            p3 = billingPeriod.ToString();
            t3 = billingFrequency;
            src = "1";
            no_shipping = "1";
            return_url = returnURL;
            rm = "1";
            notify_url = ipnURL;
            cancel_url = cancelURL;
            custom = cartID;
        }
    }
}