using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Prvii.Web.AppCode;
using Prvii.Entities.Enumerations;
using Prvii.Business;
using System.Web.Configuration;

namespace Prvii.Web.WebPages.Channels
{
    public partial class ChannelDetailsView : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                long channelID = this.ChannelID;
                var channel = ChannelManager.GetByID(channelID);
                if(!channel.IsActive)
                {
                   if( this.UserSesssion == null)
                   {
                       tblChannelDetails.Visible = false;
                       tblError.Visible = true;
                       lblError.Text = channel.Firstname.Trim() + " " + channel.Lastname.Trim() + " has suspended her VIP club for now. We will let you know when it is available again.";
                       return;
                   }
                }

                string baseURL = WebConfigurationManager.AppSettings["ServerUrl"].ToString();

                this.Master.SetBodyBackgroundImage = baseURL + "/WebPages/GetChannelMedia.aspx?ChannelID=" + channelID + "&MediaTypeID=" + (short)ChannelMediaType.Background_Image;
                this.imgLeft.ImageUrl = "~/WebPages/GetChannelMedia.aspx?ChannelID=" + channelID + "&MediaTypeID=" + (short)ChannelMediaType.Left_Image;
                this.imgRight.ImageUrl = "~/WebPages/GetChannelMedia.aspx?ChannelID=" + channelID + "&MediaTypeID=" + (short)ChannelMediaType.Right_Image;
                this.imgDescription.ImageUrl = "~/WebPages/GetChannelMedia.aspx?ChannelID=" + channelID + "&MediaTypeID=" + (short)ChannelMediaType.Center_Image;

                this.btnMessageProcessing.PostBackUrl = "~/WebPages/ChannelMessages/ChannelMessageList.aspx?ID=" + channelID;
                this.btnSubscribers.PostBackUrl = "~/WebPages/Users/UserList.aspx?ID=" + channelID;
                this.btnEdit.PostBackUrl = "~/WebPages/Channels/ChannelDetailsEdit.aspx?ID=" + channelID;

                this.tblMenus.Visible = this.UserSesssion != null;
                this.btnSubscribers.Visible = this.UserSesssion != null && channelID != 0;
                
                if(this.IsPublicURL)
                    this.Master.HideBannerItems();
                
                
                var billingFrequency = ((BillingCycleType)channel.BillingCycleID).ToString();

                this.lblPricingInfo.Text = "**ALL FOR ONLY " + channel.Price.ToString("C") + " PER " + billingFrequency.Substring(0, billingFrequency.Length - 2).ToUpper() + "**";
                this.lblPricingInfo.ToolTip = "**ALL FOR ONLY " + channel.Price.ToString("C") + " PER " + billingFrequency.Substring(0, billingFrequency.Length - 2).ToUpper() + "**";
                this.lblURL.Text = baseURL + "/Celebrity/" + channel.Firstname.Trim() + "-" + channel.Lastname.Trim();
                this.CheckAuthorization();
            }
        }

        private void CheckAuthorization()
        {
            if (this.UserSesssion != null)
            {
                if (this.UserSesssion.UserRole == Role.Subscriber)
                {
                    this.trSubscribers.Visible = false;

                    bool isSubscribed = ChannelManager.isSubscribed(this.ChannelID, this.UserSesssion.ID);

                    //this.tblMenus.Visible = isSubscribed;
                    this.btnMessageProcessing.Visible = isSubscribed;
                    this.btnEventsAndCommitments.Visible = isSubscribed;
                    this.btnURLs.Visible = false;


                    this.lblPricingInfo.Visible = !isSubscribed;
                    //this.btnAddToCartNewUser.Visible = !isSubscribed;
                    this.btnAddToCartNewUser.Visible = false;
                    //this.btnAddToCartExistingUser.Visible = !isSubscribed;
                    this.btnAddToCartExistingUser.Visible = !isSubscribed;
                    
                }
                else if (this.UserSesssion.UserRole == Role.Administrator)
                {
                    this.btnAddToCartExistingUser.Visible = false;
                    this.btnAddToCartNewUser.Visible = false;
                }
                else if (this.UserSesssion.UserRole == Role.Celebrity)
                {
                    this.btnAddToCartExistingUser.Visible = false;
                    this.btnAddToCartNewUser.Visible = false;
                    this.btnEdit.Visible = this.UserSesssion.ChannelID == this.ChannelID;
                }
                else if (this.UserSesssion.UserRole == Role.Group)
                {
                    this.btnAddToCartExistingUser.Visible = false;
                    this.btnAddToCartNewUser.Visible = false;
                    this.btnEdit.Visible = true;
                }
            }
            else
            {
                this.btnAddToCartExistingUser.Visible = true;
                this.btnAddToCartNewUser.Visible = true;
                this.tblMenus.Visible = false;
            }
        }

        protected void btnAddToCartExistingUser_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/WebPages/ShoppingCarts/ShoppingCartList.aspx?ChannelID=" + this.ChannelID);
        }

        protected void btnAddToCartNewUser_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/WebPages/Users/UserDetails.aspx?ID=0&ChannelID=" + this.ChannelID);
        }

        private long ChannelID
        {
            get 
            {
                var id = this.ViewState["ID"];

                if(id == null)
                {
                    var queryStringID = Request["ID"];

                    if (queryStringID != null)
                        id = Convert.ToInt64(queryStringID);
                    else
                    {
                        var routeDataName = Page.RouteData.Values["name"];
                        string[] names = routeDataName.ToString().Split('-');
                        string firstName = names[0];
                        string lastName = names.Length > 0 ? names[1] : string.Empty;
                        if (routeDataName != null)
                        {
                            var idValue = ChannelManager.GetByNames(firstName,lastName);

                            if (idValue == 0)
                                this.InvalidURL();

                            id = idValue;
                        }
                    }

                    this.ViewState["ID"] = id;
                }

                return Convert.ToInt64(id);
            }
        }

        private bool IsPublicURL
        {
            get
            {
                return Page.RouteData.Values["name"] != null;
            }
        }


    }
}