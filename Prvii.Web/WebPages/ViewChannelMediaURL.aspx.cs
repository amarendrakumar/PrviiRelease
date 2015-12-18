using Prvii.Business;
using Prvii.Web.AppCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Prvii.Web.WebPages
{
    public partial class ViewChannelMediaURL : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                if (!ChannelManager.CanViewMedia(this.UserSesssion, this.MediaID))
                    this.UnAuthorizeAccess();

                var mediaMimeType = ChannelManager.GetMediaMimeType(this.MediaID);

                if (mediaMimeType.Contains("image"))
                {
                    this.imgMedia.Visible = true;
                    this.imgMedia.ImageUrl = "~/WebPages/GetChannelMedia.aspx?ID=" + this.MediaID;
                }
                else
                {
                    this.vdoMedia.Visible = true;
                    this.vdoMedia.HRef = "~/WebPages/GetChannelMedia.aspx?ID=" + this.MediaID;
                }

                this.btnBack.Visible = this.UserSesssion.UserRole == Entities.Enumerations.Role.Administrator;
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            this.NavigateBack();
        }

        private long MediaID
        {
            get
            {
                return Convert.ToInt64(Request["ID"]);
            }
        }
    }
}