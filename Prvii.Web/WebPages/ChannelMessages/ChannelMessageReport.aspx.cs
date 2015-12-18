using Prvii.Business;
using Prvii.Web.AppCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Prvii.Web.WebPages.ChannelMessages
{
    public partial class ChannelMessageReport : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                this.BindDropDown(this.ddlChannels, ChannelManager.GetForDropdown(), false, "-Select Celebrity-");
            }
        }

        private void BindMessages()
        {
            long channelID = Convert.ToInt64(this.ddlChannels.SelectedValue);
            DateTime? startDate = this.tbStartDate.Text != string.Empty ? (DateTime?) Convert.ToDateTime(this.tbStartDate.Text) : null;
            DateTime? endDate = this.tbEndDate.Text != string.Empty ? (DateTime?)Convert.ToDateTime(this.tbEndDate.Text).AddHours(23).AddMinutes(59) : null;

            if (!this.ValidateFields())
                return;

            this.litCelebrityName.Text = ChannelManager.GetCelebrityName(channelID);

            //string timeZoneID = UserProfileManager.GetSubscriberTimeZone(this.UserID);

            var list = ChannelMessageManager.GetMessageReport(channelID, startDate, endDate);

            if(this.tbFilterBy.Text != string.Empty)
            {
                list = list.Where(x => x.Subject.ToLower().Contains(this.tbFilterBy.Text.ToLower())
                    || x.Message.ToLower().Contains(this.tbFilterBy.Text.ToLower())
                    || x.ChannelName.ToLower().Contains(this.tbFilterBy.Text.ToLower())).ToList();
            }

            this.gvMessageList.DataSource = list;
            this.gvMessageList.DataBind();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            this.BindMessages();
        }

        protected void gvMessageList_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.gvMessageList.PageIndex = e.NewPageIndex;
            this.BindMessages();
        }

        private bool ValidateFields()
        {

            if (this.tbStartDate.Text != string.Empty)
            {
                DateTime startDate = (DateTime)Convert.ToDateTime(this.tbStartDate.Text);

                if (this.tbEndDate.Text != string.Empty)
                {
                    DateTime endDate = (DateTime)Convert.ToDateTime(this.tbEndDate.Text).AddHours(23).AddMinutes(59);
                    if (startDate >= endDate)
                    {
                        this.DisplayMessageBox("End date should be greater than or equal to Start date.", this.upPage, this.tbEndDate);
                        return false;
                    }
                }
            }


            return true;
        }

        private long UserID
        {
            get
            {
                return Convert.ToInt64(this.UserSesssion.ID);
            }
        }

    }
}