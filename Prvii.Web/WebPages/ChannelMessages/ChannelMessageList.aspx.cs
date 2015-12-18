using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Prvii.Web.AppCode;
using Prvii.Entities.Enumerations;
using Prvii.Business;

namespace Prvii.Web.WebPages.ChannelMessages
{
    public partial class ChannelMessageList : BasePage
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                long channelID = this.ChannelID;
                this.litCelebrityName.Text = ChannelManager.GetCelebrityName(this.ChannelID);
                this.btnAddNewMessage.PostBackUrl = "~/WebPages/ChannelMessages/ChannelMessageDetails.aspx?ID=0" + "&ChannelID=" + channelID;
                this.btnBack.PostBackUrl = "~/WebPages/Channels/ChannelDetailsView.aspx?ID=" + channelID;
                if (this.UserSesssion.UserRole == Role.Subscriber)
                {
                    if (ChannelMessageManager.GetBySubscriberID(channelID, this.UserSesssion.ID).Count > 0)
                    {
                        litMessageTypeHeader.Text = "Current Messages";
                        this.PastAllMessage = false;
                        this.tblChannelMessage.Visible = false;
                        this.gvChannelSubscriberMessageList.Visible = true;
                        this.tblChannelSubscriberMessageList.Visible = true;
                        this.gvChannelSubscriberMessageList.DataSource = ChannelMessageManager.GetBySubscriberID(channelID, this.UserSesssion.ID);
                        this.gvChannelSubscriberMessageList.DataBind();
                    }
                    else
                    {
                        litMessageTypeHeader.Text = "Past Messages";
                        this.PastAllMessage = true;
                        this.tblChannelMessage.Visible = false;
                        this.gvChannelSubscriberMessageList.Visible = true;
                        this.tblChannelSubscriberMessageList.Visible = true;
                        this.gvChannelSubscriberMessageList.DataSource = ChannelMessageManager.GetChannelSubscriberPastMessageByChannelID(channelID, this.UserSesssion.ID);
                        this.gvChannelSubscriberMessageList.DataBind();
                    }
                }
                else
                {
                    this.BindMessageForAdmin();
                }
            }
        }

        private void BindMessageForAdmin()
        {
            var list = ChannelMessageManager.GetByChannelID(this.ChannelID);
            string filterValue = this.ddlFilters.SelectedValue;

            if (filterValue == "Scheduled")
                list = list.Where(x => x.Status != (short)ChannelMessageStatus.Sent).ToList();
            else if (filterValue == "Past")
                list = list.Where(x => x.Status == (short)ChannelMessageStatus.Sent).ToList();

            this.gvChannelMessageList.DataSource = list;
            this.gvChannelMessageList.DataBind();
        }

        protected void ddlFilters_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.BindMessageForAdmin();
        }

        private long ChannelID
        {
            get
            {
                return Convert.ToInt64(Request["ID"]);
            }
        }

        private bool PastAllMessage { get; set; }

        protected void gvChannelMessageList_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.gvChannelMessageList.PageIndex = e.NewPageIndex;
            this.BindMessageForAdmin();
        }

        protected void btnPastAllMessage_Click(object sender, EventArgs e)
        {
            litMessageTypeHeader.Text = "Past Messages";
            this.PastAllMessage = true;
            long channelID = this.ChannelID;
            this.tblChannelMessage.Visible = false;
            this.gvChannelSubscriberMessageList.Visible = true;
            this.tblChannelSubscriberMessageList.Visible = true;
            this.gvChannelSubscriberMessageList.DataSource = ChannelMessageManager.GetChannelSubscriberPastMessageByChannelID(channelID, this.UserSesssion.ID);
            this.gvChannelSubscriberMessageList.DataBind();
        }

        protected void btnBackMessage_Click(object sender, EventArgs e)
        {
           
            litMessageTypeHeader.Text = "Current Messages";
             long channelID = this.ChannelID;
             //Response.Redirect("~/WebPages/ChannelMessages/ChannelMessageList.aspx?ID=" + channelID, true);
             this.PastAllMessage = false;
             this.tblChannelMessage.Visible = false;
             this.gvChannelSubscriberMessageList.Visible = true;
             this.tblChannelSubscriberMessageList.Visible = true;
             this.gvChannelSubscriberMessageList.DataSource = ChannelMessageManager.GetBySubscriberID(channelID, this.UserSesssion.ID);
             this.gvChannelSubscriberMessageList.DataBind();
        }

        protected void gvChannelSubscriberMessageList_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            long channelID = this.ChannelID;

            this.gvChannelSubscriberMessageList.PageIndex = e.NewPageIndex;
            if (this.PastAllMessage)
            {
                this.gvChannelSubscriberMessageList.DataSource = ChannelMessageManager.GetChannelSubscriberPastMessageByChannelID(channelID, this.UserSesssion.ID);
                this.gvChannelSubscriberMessageList.DataBind();
            }
            else
            {
                this.gvChannelSubscriberMessageList.DataSource = ChannelMessageManager.GetBySubscriberID(channelID, this.UserSesssion.ID);
                this.gvChannelSubscriberMessageList.DataBind();
            }
        }

        protected void gvChannelMessageList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                HyperLink link = e.Row.Cells[0].Controls[0] as HyperLink;
                if (link != null)
                {
                    if (Convert.ToBoolean(DataBinder.Eval(e.Row.DataItem, "IsEmail")))
                    {
                        link.Text = Convert.ToString(DataBinder.Eval(e.Row.DataItem, "Subject"));
                    }
                    else
                    {
                        string sms = Convert.ToString(DataBinder.Eval(e.Row.DataItem, "Message"));
                        if (sms.Length > 50)
                            link.Text = sms.Substring(0, 50);
                        else
                            link.Text = sms;                      
                            
                    }
                   
                }
            }
        }
    }
}