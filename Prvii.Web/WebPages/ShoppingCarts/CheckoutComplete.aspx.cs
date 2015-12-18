using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Prvii.Business;
using Prvii.Entities.Enumerations;
using Prvii.Web.AppCode;
using System.Text;
using System.Data;
using Prvii.Entities;

namespace Prvii.Web.WebPages.ShoppingCarts
{
    public partial class CheckoutComplete : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
               // this.CompleteCheckout();
                PayPalManager paypal = new PayPalManager();
                string retMsg = "";
                string token = "";
                string payerId = "";
                string ShoppingCartID = "";
                token = Session["token"].ToString();
                //string cartID = Session["ShoppingCartID"].ToString();
                NVPCodec nvpResponse = new NVPCodec();
                bool result = paypal.GetCheckoutDetails(token, ref payerId, ref nvpResponse, ref retMsg, ref ShoppingCartID);
                if (result)
                {
                    Session["payerId"] = payerId;
                    Session["ShoppingCartID"] = ShoppingCartID;
                    string cartID = Session["ShoppingCartID"].ToString();
                    paypal.DoCheckoutPaymentNew(cartID, token, payerId, ref nvpResponse, ref retMsg);
                    CreateRecurringPaymentsProfile();
                    List<Channel> channel = ShoppingCartManager.GetItem(Convert.ToInt64(cartID));
                    if (channel != null)
                    {
                        long channelID = channel.Select(s => s.ID).FirstOrDefault();
                        Session["ShoppingCartID"] = "";
                        Response.Redirect("~/WebPages/Channels/ChannelDetailsView.aspx?ID=" + channelID, true);
                    }
                    else
                    {
                        Response.Redirect("~/ErrorPage.aspx", true);
                    }
                }
                
            }
        }

        protected void CreateRecurringPaymentsProfile()
        {

            string cartID = Session["ShoppingCartID"].ToString();

            List<Channel> channel = ShoppingCartManager.GetItem(Convert.ToInt64(cartID));

            string token = Session["token"] as string;
            string PayerId = Session["PayerID"] as string;

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
                    if (isSuccess)
                    {
                        
                        ShoppingCartManager.UpdatePaymentNew(Convert.ToInt64(shoppingcartID), Convert.ToInt64(item.ID), item.Price, PayerId, ProfileID, item.BillingCycleID, Convert.ToInt16(BillingFrequenc), 0, ProfileStatus,retMsg,(short)PaymentStatus.Payment_Completed);
                    }
                    else
                    {
                        ShoppingCartManager.UpdatePaymentNew(Convert.ToInt64(shoppingcartID), Convert.ToInt64(item.ID), item.Price, PayerId, ProfileID, item.BillingCycleID, Convert.ToInt16(BillingFrequenc), 0, ProfileStatus, retMsg, (short)PaymentStatus.Payment_Cancelled);
                    }
                }
            }
        }

        private void CompleteCheckout()
        {
            if (Session["ShoppingCartID"] != null)
            {
                long cartID = Convert.ToInt64(Session["ShoppingCartID"]);
                ShoppingCartManager.UpdatePaymentStatus(cartID, PaymentStatus.Awaiting_Payment_Confirmation);
                Session["ShoppingCartID"] = null;
            }
        }
    }
}