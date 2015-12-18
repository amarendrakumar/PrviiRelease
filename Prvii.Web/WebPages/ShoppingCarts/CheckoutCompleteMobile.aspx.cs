using Prvii.Business;
using Prvii.Entities;
using Prvii.Entities.Enumerations;
using Prvii.ExceptionHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Prvii.Web.WebPages.ShoppingCarts
{
    public partial class CheckoutCompleteMobile : System.Web.UI.Page
    {
        private void LogMessage(string message)
        {
            string logfilePath = WebConfigurationManager.AppSettings["PayPal_IPNLogFilePath"].ToString();
            ExceptionHandler.LogMessage(message, true, 10240, logfilePath);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.LogMessage("Paypal : log values : ");
                // this.CompleteCheckout();
                PayPalManager paypal = new PayPalManager();
                string retMsg = "";
                string token = "";
                string payerId = "";
                string ShoppingCartID = "";
               // token = Session["token"].ToString();
                if (Request["token"] != null)
                    token = Request["token"].ToString();
               
                NVPCodec nvpResponse = new NVPCodec();
                bool result = paypal.GetCheckoutDetails(token, ref payerId, ref nvpResponse, ref retMsg, ref ShoppingCartID);

                this.LogMessage("Paypal : GetCheckoutDetails values : " + result.ToString());

                if (result)
                {
                    string cartID = ShoppingCartID;
                    paypal.DoCheckoutPaymentNew(cartID, token, payerId, ref nvpResponse, ref retMsg);
                    this.LogMessage("Paypal : DoCheckoutPaymentNew values  cartID : " + cartID.ToString() + " token :- " + token + "  payerId:- " + payerId);

                    CreateRecurringPaymentsProfile(cartID, token, payerId);
                    //List<Channel> channel = ShoppingCartManager.GetItem(Convert.ToInt64(cartID));
                    //if (channel != null)
                    //{
                    //    long channelID = channel.Select(s => s.ID).FirstOrDefault();
                    //    Session["ShoppingCartID"] = "";
                    //    Response.Redirect("~/WebPages/ShoppingCarts/ChannelDetailsView.aspx?ID=" + channelID, true);
                    //}
                    //else
                    //{
                    //    Response.Redirect("~/ErrorPage.aspx", true);
                    //}
                }
                else
                    Response.Redirect("~/WebPages/ShoppingCarts/CheckoutCancelMobile.aspx", true); 
              
            }
        }

        protected void CreateRecurringPaymentsProfile(string cartID,string token,string PayerId)
        {
            List<Channel> channel = ShoppingCartManager.GetItem(Convert.ToInt64(cartID));

            PayPalManager paypal = new PayPalManager();
            if (channel != null)
            {
                foreach (var item in channel)
                {
                    string retMsg = "", ProfileStatus = "", ProfileID = "";
                    string shoppingcartID = cartID;
                    string BillingFrequenc = "1";

                    if (item.BillingCycleID == (short)BillingCycleType.Quarterly)
                        BillingFrequenc = "3";
                   
                    bool isSuccess = paypal.CreateRecurringPaymentsProfile(token, ref PayerId, ref ProfileID, ref retMsg, ref ProfileStatus, item);
                    this.LogMessage("Paypal : CreateRecurringPaymentsProfile values : " + isSuccess.ToString());
                    if (isSuccess)
                    {                      
                      
                        ShoppingCartManager.UpdatePaymentNew(Convert.ToInt64(shoppingcartID), Convert.ToInt64(item.ID), item.Price, PayerId, ProfileID, item.BillingCycleID, Convert.ToInt16(BillingFrequenc), 0, ProfileStatus, retMsg, (short)PaymentStatus.Payment_Completed);
                    }
                    else
                    {                       
                        ShoppingCartManager.UpdatePaymentNew(Convert.ToInt64(shoppingcartID), Convert.ToInt64(item.ID), item.Price, PayerId, ProfileID, item.BillingCycleID, Convert.ToInt16(BillingFrequenc), 0, ProfileStatus, retMsg, (short)PaymentStatus.Payment_Cancelled);
                    }
                }
            }
        }

      
       
    }
}