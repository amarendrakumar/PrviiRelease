using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Prvii.Web.AppCode;
using Prvii.Business;
using Prvii.Entities.Enumerations;
using Prvii.Entities;

namespace Prvii.Web.WebPages.ShoppingCarts
{
    public partial class ShoppingCartList : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                if(this.UserSesssion.UserRole != Role.Subscriber)
                    this.UnAuthorizeAccess();

                var cart = ShoppingCartManager.GetCart(Session.SessionID, this.UserSesssion.ID);

                if (cart != null)
                    this.ShoppingCartID = cart.ID;

                this.AddToCart();
                this.BindCart();
            }
        }

        private void BindCart()
        {
            var cartItems = ShoppingCartManager.GetCartItems(this.ShoppingCartID);

            this.DataList1.DataSource = cartItems;
            this.DataList1.DataBind();
            this.lblTotalPrice.Text = (Convert.ToDecimal(ShoppingCartManager.GetCartTotalPrice(this.ShoppingCartID))).ToString("C2");

            if (cartItems.Count == 0)
                this.tblCheckoutStart.Visible = false;
        }

        private void AddToCart()
        {
            var channelID = Convert.ToInt64(Request.QueryString["ChannelID"]);

            if (channelID != 0)
            {
                if(channelID == this.UserSesssion.ChannelID)
                {
                    this.DisplayMessageBox("You cannot subscribe yourself!", this.upPage);
                    return;
                }
                bool isSubcribed = ShoppingCartManager.IsSubscribed(channelID, this.UserSesssion.ID);
                if (isSubcribed)
                {
                    this.DisplayMessageBox("Celebrity already subscribed!", this.upPage);
                    return;
                }

                bool result = ShoppingCartManager.IsSubscriptionInProgress(channelID, this.UserSesssion.ID);
                if (result)
                {
                    this.DisplayMessageBox("Celebrity already subscribed. Awaiting Payment Confirmation!", this.upPage);
                    return;
                }

                var cartID = this.ShoppingCartID;

                if (cartID != 0)
                    ShoppingCartManager.AddItem(cartID, channelID);
                else
                {
                    var cart = new ShoppingCart { UserID = this.UserSesssion.ID, SessionID = Session.SessionID, CreatedOn = DateTime.UtcNow };

                    ShoppingCartManager.AddItem(cart, channelID);
                    this.ShoppingCartID = cart.ID;
                }

            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            List<long> itemList = new List<long>();

            foreach (DataListItem row in this.DataList1.Items)
            {
                var id = Convert.ToInt64((row.FindControl("hfItemID") as HiddenField).Value);

                if (((CheckBox)row.FindControl("cbxChannel")).Checked)
                {
                    itemList.Add(id);
                }
            }

            if (itemList.Count != 0)
                ShoppingCartManager.RemoveItems(this.ShoppingCartID, itemList);

            this.BindCart();
        }

        protected void btnCheckout_Click(object sender, EventArgs e)
        {
            //if(this.gvShoppingCartItems.Rows.Count > 1)
            //{
            //    this.DisplayMessageBox("Multiple celebrity cannot be subscribed at a time!", this.upPage);
            //    return;
            //}

            this.tblCheckoutStart.Visible = false;
            this.tblReviewOrder.Visible = true;
            this.lblPageHeader.Text = "Review Order";
        }

        protected void btnCompleteOrder_Click(object sender, EventArgs e)
        {
            ShoppingCartManager.UpdatePaymentStatus(this.ShoppingCartID, PaymentStatus.Checkout_Complete);

            Session["ShoppingCartID"] = this.ShoppingCartID;
            Session["payment_amt"] = Convert.ToDecimal(this.lblTotalPrice.Text.Replace("$", "").Replace("£", "")).ToString("#.##");

           // Response.Redirect("~/WebPages/ShoppingCarts/CheckoutStart.aspx");

            List<Channel> channel = ShoppingCartManager.GetItem(Convert.ToInt64(this.ShoppingCartID)).ToList();

            string token = "", expressCheckoutURL = "";
            PayPalManager payPal = new PayPalManager();
            bool result = payPal.MarkExpressCheckoutNew(ref token, ref expressCheckoutURL, channel, Convert.ToString(this.ShoppingCartID));
            if (result)
            {
                Session["token"] = token;

                Response.Redirect(expressCheckoutURL, true);
            }
            else
            {
                Response.Redirect("~/ErrorPage.aspx?" + expressCheckoutURL);
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            this.tblCheckoutStart.Visible = true;
            this.tblReviewOrder.Visible = false;
            this.lblPageHeader.Text = "Shopping Cart";
        }

        private long ShoppingCartID
        {
            get
            {
                var id = ViewState["ShoppingCartID"];

                if (id != null)
                    return Convert.ToInt64(id);

                return 0;
            }
            set
            {
                ViewState["ShoppingCartID"] = value;
            }
        }


    }
}