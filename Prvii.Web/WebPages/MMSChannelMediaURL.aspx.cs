using Prvii.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Prvii.Web.WebPages
{
    public partial class MMSChannelMediaURL : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                

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

               
            }
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