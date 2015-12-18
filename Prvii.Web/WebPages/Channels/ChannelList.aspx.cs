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
using Prvii.Entities.DataEntities;
using System.Web.UI.HtmlControls;

namespace Prvii.Web.WebPages.Channels
{
    public partial class ChannelList : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                if (this.UserSesssion != null && this.UserSesssion.UserRole == Role.Celebrity)
                    Response.Redirect("~/WebPages/Channels/ChannelDetailsView.aspx?ID="  + this.UserSesssion.ChannelID, true);
                if (this.UserSesssion != null && this.UserSesssion.UserRole == Role.Subscriber)
                {
                    if (Request["val"] != null)
                    {
                        string val = Convert.ToString(Request["val"]);
                        var listItem = this.ddlFilters.Items.FindByValue(val);

                        if (listItem != null)
                        {
                            this.ddlFilters.ClearSelection();
                            listItem.Selected = true;
                        }
                    }
                    else
                    {
                        var listItem = this.ddlFilters.Items.FindByValue("1");

                        if (listItem != null)
                        {
                            this.ddlFilters.ClearSelection();
                            listItem.Selected = true;
                        }
                    }
                }

                this.BindChannels();
                this.ShowHideGridColumns();
            }
        }

        private void BindChannels()
        {
            List<ChannelData> channelList = null;
            if (this.UserSesssion == null)
                channelList = ChannelManager.GetCelebrityListUser();
           else if (this.UserSesssion.UserRole == Role.Administrator)
                channelList = ChannelManager.GetCelebrityList();
            else if(this.UserSesssion.UserRole == Role.Group)
                channelList = ChannelManager.GetForGroupUser(this.UserSesssion.ID);
            else if(this.UserSesssion.UserRole == Role.Subscriber)
                channelList = ChannelManager.GetForSubscriberUser(this.UserSesssion.ID);

            var filterValue = this.ddlFilters.SelectedValue;

            if (filterValue == "1")
                channelList = channelList.Where(x => x.IsSubscribed).ToList();
            else if (filterValue == "2")
                channelList = channelList.Where(x => !x.IsSubscribed).ToList();

            
            this.DataList1.DataSource = channelList;
            this.DataList1.DataBind();
        }

        private void ShowHideGridColumns()
        {

            Role userRole = Role.Subscriber;
            var groupID = Convert.ToInt64(Request["GroupID"]);

            if (this.UserSesssion != null)
            { 
                userRole = this.UserSesssion.UserRole;

                if (userRole == Role.Subscriber)
                { 
                    this.tblFilter.Visible = true;
                }
            }

            
        }

        protected void btnSaveGroupChannels_Click(object sender, EventArgs e)
        {
            if(!this.IsAnyChannelSelected())
            {
                this.DisplayMessageBox("Please select at least one celebrity!", this.upPage);
                return;
            }

            GroupManager.SaveChannels(this.GroupID, this.GetSelectedChannels());
            this.NavigateBack();
        }

        private bool IsAnyChannelSelected()
        {
            bool selected = false;
            
            foreach (DataListItem dli in DataList1.Items)
            {
                CheckBox cbxSelect = dli.FindControl("cbxSelect") as CheckBox;
                if (cbxSelect.Checked)
                {
                    selected = true;
                    break;
                }
            }

            return selected;
        }

        private List<long> GetSelectedChannels()
        {
            List<long> channelIds = new List<long>();

            foreach(DataListItem row in DataList1.Items)
            {
                CheckBox cbxSelect = row.FindControl("cbxSelect") as CheckBox;
                HiddenField hfID = row.FindControl("hfID") as HiddenField;

                if (cbxSelect.Checked)
                    channelIds.Add(Convert.ToInt64(hfID.Value));
            }

            return channelIds;
        }

       
        List<GroupChannel> groupChannels = null;
        private bool CheckChannel(long id)
        {
            if (groupChannels == null)
                groupChannels = GroupManager.GetChannelsByGroup(this.GroupID) ?? new List<GroupChannel>();

            return groupChannels.Any(x=>x.ChannelID == id);
        }

        protected void ddlFilters_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.BindChannels();
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            if (this.GroupID != 0)
                this.Response.Redirect("~/WebPages/Groups/GroupList.aspx", true);
            else
                this.NavigateBack();
        }

       

        private long GroupID
        {
            get
            {
                return Convert.ToInt64(Request["GroupID"]);
            }
        }

        protected void DataList1_ItemDataBound(object sender, DataListItemEventArgs e)
        {
            Role userRole;
            if (this.UserSesssion == null)
            {
                userRole = Role.Subscriber;
            }
            else
            {
                userRole = this.UserSesssion.UserRole;
            }
           
           
            if (e.Item.ItemType == ListItemType.Header)
            {
                if (userRole == Role.Subscriber)
                {
                    Control DivMobileHeader = e.Item.FindControl("DivMobileHeader");
                    DivMobileHeader.Visible = false;
                    Control DivEmailHeader = e.Item.FindControl("DivEmailHeader");
                    DivEmailHeader.Visible = false;
                    Control DivActiveHeader = e.Item.FindControl("DivActiveHeader");
                    DivActiveHeader.Visible = false;
                    Control DivEditHeader = e.Item.FindControl("DivEditHeader");
                    DivEditHeader.Visible = false;


                    HtmlGenericControl DivPhotoHeader = (HtmlGenericControl)e.Item.FindControl("DivPhotoHeader");
                    DivPhotoHeader.Attributes.Remove("class");
                    DivPhotoHeader.Attributes["class"] = "col-md-4";
                    HtmlGenericControl DivNameHeader = (HtmlGenericControl)e.Item.FindControl("DivNameHeader");
                    DivNameHeader.Attributes.Remove("class");
                    DivNameHeader.Attributes["class"] = "col-md-4";
                    HtmlGenericControl DivPriceHeader = (HtmlGenericControl)e.Item.FindControl("DivPriceHeader");
                    DivPriceHeader.Attributes.Remove("class");
                    DivPriceHeader.Attributes["class"] = "col-md-2";
                    HtmlGenericControl DivSubscribeHeader = (HtmlGenericControl)e.Item.FindControl("DivSubscribeHeader");
                    DivSubscribeHeader.Attributes.Remove("class");
                    DivSubscribeHeader.Attributes["class"] = "col-md-2";


                }
                else if (userRole == Role.Administrator)
                {
                    Control DivSubscribeHeader = e.Item.FindControl("DivSubscribeHeader");
                    DivSubscribeHeader.Visible = false;

                    this.tblAddNewCelebrity.Visible = true;
                    var groupID = Convert.ToInt64(Request["GroupID"]);
                    if (groupID != 0)
                    {
                       
                        Control DivEditHeader = e.Item.FindControl("DivEditHeader");
                        DivEditHeader.Visible = false;

                        Control DivSelectHeader = e.Item.FindControl("DivSelectHeader");
                        DivSelectHeader.Visible = true;

                     

                        //
                        this.tblGroupName.Visible = true;
                        this.tblSaveGroupChannels.Visible = true;
                        this.lblGroupName.Text = GroupManager.GetName(this.GroupID);
                    }
                }
                else if (userRole == Role.Group)
                {

                    Control DivMobileHeader = e.Item.FindControl("DivMobileHeader");
                    DivMobileHeader.Visible = false;
                    Control DivEmailHeader = e.Item.FindControl("DivEmailHeader");
                    DivEmailHeader.Visible = false;
                    Control DivActiveHeader = e.Item.FindControl("DivActiveHeader");
                    DivActiveHeader.Visible = false;
                    Control DivSubscribeHeader = e.Item.FindControl("DivSubscribeHeader");
                    DivSubscribeHeader.Visible = false;
                    Control DivEditHeader = e.Item.FindControl("DivEditHeader");
                    DivEditHeader.Visible = false;

                    HtmlGenericControl DivPhotoHeader = (HtmlGenericControl)e.Item.FindControl("DivPhotoHeader");
                    DivPhotoHeader.Attributes.Remove("class");
                    DivPhotoHeader.Attributes["class"] = "col-md-4";
                    HtmlGenericControl DivNameHeader = (HtmlGenericControl)e.Item.FindControl("DivNameHeader");
                    DivNameHeader.Attributes.Remove("class");
                    DivNameHeader.Attributes["class"] = "col-md-4";
                    HtmlGenericControl DivPriceHeader = (HtmlGenericControl)e.Item.FindControl("DivPriceHeader");
                    DivPriceHeader.Attributes.Remove("class");
                    DivPriceHeader.Attributes["class"] = "col-md-4";
                   

                }


               
            }

            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                if (userRole == Role.Subscriber)
                {

                    Control DivMobileItem = e.Item.FindControl("DivMobileItem");
                    DivMobileItem.Visible = false;
                    Control DivEmailItem = e.Item.FindControl("DivEmailItem");
                    DivEmailItem.Visible = false;
                    Control DivActiveItem = e.Item.FindControl("DivActiveItem");
                    DivActiveItem.Visible = false;
                    Control DivEditItem = e.Item.FindControl("DivEditItem");
                    DivEditItem.Visible = false;


                    HtmlGenericControl DivPhotoItem = (HtmlGenericControl)e.Item.FindControl("DivPhotoItem");
                    DivPhotoItem.Attributes.Remove("class");
                    DivPhotoItem.Attributes["class"] = "col-md-4";
                    HtmlGenericControl DivNameItem = (HtmlGenericControl)e.Item.FindControl("DivNameItem");
                    DivNameItem.Attributes.Remove("class");
                    DivNameItem.Attributes["class"] = "col-md-4";
                    HtmlGenericControl DivPriceItem = (HtmlGenericControl)e.Item.FindControl("DivPriceItem");
                    DivPriceItem.Attributes.Remove("class");
                    DivPriceItem.Attributes["class"] = "col-md-2";
                    HtmlGenericControl DivSubscribeItem = (HtmlGenericControl)e.Item.FindControl("DivSubscribeItem");
                    DivSubscribeItem.Attributes.Remove("class");
                    DivSubscribeItem.Attributes["class"] = "col-md-2";



                }
                else if (userRole == Role.Administrator)
                {
                    Control DivSubscribeItem = e.Item.FindControl("DivSubscribeItem");
                    DivSubscribeItem.Visible = false;

                    this.tblAddNewCelebrity.Visible = true;
                    var groupID = Convert.ToInt64(Request["GroupID"]);
                    if (groupID != 0)
                    {

                        Control DivSelectItem = e.Item.FindControl("DivSelectItem");
                        DivSelectItem.Visible = true;

                        Control DivEditItem = e.Item.FindControl("DivEditItem");
                        DivEditItem.Visible = false;

                    }
                }
                else if (userRole == Role.Group)
                {
                    Control DivMobileItem = e.Item.FindControl("DivMobileItem");
                    DivMobileItem.Visible = false;
                    Control DivEmailItem = e.Item.FindControl("DivEmailItem");
                    DivEmailItem.Visible = false;
                    Control DivActiveItem = e.Item.FindControl("DivActiveItem");
                    DivActiveItem.Visible = false;
                    Control DivSubscribeItem = e.Item.FindControl("DivSubscribeItem");
                    DivSubscribeItem.Visible = false;
                    Control DivEditItem = e.Item.FindControl("DivEditItem");
                    DivEditItem.Visible = false;

                    HtmlGenericControl DivPhotoItem = (HtmlGenericControl)e.Item.FindControl("DivPhotoItem");
                    DivPhotoItem.Attributes.Remove("class");
                    DivPhotoItem.Attributes["class"] = "col-md-4";
                    HtmlGenericControl DivNameItem = (HtmlGenericControl)e.Item.FindControl("DivNameItem");
                    DivNameItem.Attributes.Remove("class");
                    DivNameItem.Attributes["class"] = "col-md-4";
                    HtmlGenericControl DivPriceItem = (HtmlGenericControl)e.Item.FindControl("DivPriceItem");
                    DivPriceItem.Attributes.Remove("class");
                    DivPriceItem.Attributes["class"] = "col-md-4";
                   

                }


              

                if (this.GroupID != 0)
                {
                    HiddenField hfID = e.Item.FindControl("hfID") as HiddenField;
                    CheckBox cbxSelect = e.Item.FindControl("cbxSelect") as CheckBox;
                    cbxSelect.Checked = this.CheckChannel(Convert.ToInt64(hfID.Value));
                }
                else if (this.UserSesssion != null && this.UserSesssion.UserRole == Role.Subscriber)
                {
                    HiddenField hfID = e.Item.FindControl("hfID") as HiddenField;
                    HiddenField hfIsSubscribed = e.Item.FindControl("hfIsSubscribed") as HiddenField;
                    Button btnAddToCart = e.Item.FindControl("btnAddToCart") as Button;
                    LinkButton lbtnUnSubscribe = e.Item.FindControl("lbtnUnSubscribe") as LinkButton;

                    if (Convert.ToInt64(hfID.Value) == this.UserSesssion.ChannelID)
                    {
                        btnAddToCart.Visible = false;
                        lbtnUnSubscribe.Visible = false;
                    }
                    else
                    {
                        bool isSubscribed = Convert.ToBoolean(hfIsSubscribed.Value);

                        btnAddToCart.Visible = !isSubscribed;
                        lbtnUnSubscribe.Visible = isSubscribed;
                    }

                    
                }
               
            }
        }

        protected void DataList1_ItemCommand(object source, DataListCommandEventArgs e)
        {
            if (e.CommandName == "AddToCart")
            {
                Response.Redirect("~/WebPages/ShoppingCarts/ShoppingCartList.aspx?ChannelID=" + e.CommandArgument, true);
            }
            else if (e.CommandName == "Unsubscribe")
            {
                if (ShoppingCartManager.IsSubscriptionCancellationInProgress(Convert.ToInt64(e.CommandArgument), this.UserSesssion.ID))
                {
                    this.DisplayMessageBox("Subscription cancellation in progress!", this.upPage);
                    return;
                }

                ChannelManager.ChannelUnsubscribe(Convert.ToInt64(e.CommandArgument), this.UserSesssion.ID);
                this.BindChannels();
            }
        }

    }
}