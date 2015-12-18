using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Prvii.Business;
using Prvii.Entities.Enumerations;
using Prvii.Entities;
using Prvii.Web.AppCode;

namespace Prvii.Web.WebPages
{
    public partial class GetChannelMedia : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.GetMedia();
        }

        private void GetMedia()
        {
            long id = Convert.ToInt64(Request["ID"]);
            short mediaTypeID = Convert.ToInt16(Request["MediaTypeID"]);
            long channelID = Convert.ToInt64(Request["ChannelID"]);

            ChannelMedia media = null;

            if (mediaTypeID == 0)
                mediaTypeID = (short)ChannelMediaType.Image;

            if(id != 0)
                media = ChannelManager.GetMediaByID(id);
            else
                media = ChannelManager.GetMedia(channelID, (ChannelMediaType)mediaTypeID);

            Response.Clear();

            if (media != null)
            {
                Response.ContentType = media.MimeType;

                if (media.MediaTypeID == (short)ChannelMediaType.Welcome_Media || media.MediaTypeID == (short)ChannelMediaType.Media_URL)
                    Response.AddHeader("content-disposition", "attachment; filename=" + media.Name);

                Response.BinaryWrite(media.Content);
            }

            Response.End();
        }
    }
}