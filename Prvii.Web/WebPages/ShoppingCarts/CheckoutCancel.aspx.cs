using Prvii.Web.AppCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Prvii.Entities;
using Prvii.Business;
using Prvii.Entities.Enumerations;


namespace Prvii.Web.WebPages.ShoppingCarts
{
    public partial class CheckoutCancel : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
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

                        ShoppingCartManager.UpdatePaymentNew(Convert.ToInt64(shoppingcartID), Convert.ToInt64(item.ID), item.Price, PayerId, ProfileID, item.BillingCycleID, Convert.ToInt16(BillingFrequenc), 0, ProfileStatus, retMsg, (short)PaymentStatus.Payment_Cancelled);
                    }
                }
            }
        }
    }
}