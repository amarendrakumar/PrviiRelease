using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Prvii.Web.AppCode;
using Prvii.Business;
using System.Text;


namespace Prvii.Web.WebPages.ChannelSubscribers
{
    public partial class DownloadSubsciber : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            lblSubscriberCount.Text = "";
            if (!Page.IsPostBack)
            {
                this.BindMasters();
            }
        }
        private void BindMasters()
        {
            this.BindDropDown(this.ddlChannels, ChannelManager.GetForDropdown(), false, " All ");
        }
       

        protected void btnAdminDownload_Click(object sender, EventArgs e)
        {
            lblSubscriberCount.Text = "";
            long channelID = Convert.ToInt64(this.ddlChannels.SelectedValue);
            try
            {
                string csvExportContents = DownloadToExcel.GenerateLogFile(channelID, false);
                byte[] data = ASCIIEncoding.ASCII.GetBytes(csvExportContents);
                Response.Clear();
                Response.ContentType = "APPLICATION/OCTET-STREAM";
                Response.AppendHeader("Content-Disposition", "attachment; filename=Export.csv");
                Response.OutputStream.Write(data, 0, data.Length);
                Response.End();

                // HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected void btnAdvertiserDownload_Click(object sender, EventArgs e)
        {
            lblSubscriberCount.Text = "";
           
            long channelID = Convert.ToInt64(this.ddlChannels.SelectedValue);
            if (!DownloadToExcel.GetPrecludeCelebrity(channelID))
            {
                try
                {
                    string csvExportContents = DownloadToExcel.GenerateLogFile(channelID, true);
                    byte[] data = ASCIIEncoding.ASCII.GetBytes(csvExportContents);
                    Response.Clear();
                    Response.ContentType = "APPLICATION/OCTET-STREAM";
                    Response.AppendHeader("Content-Disposition", "attachment; filename=Export.csv");
                    Response.OutputStream.Write(data, 0, data.Length);
                    Response.End();

                    // HttpContext.Current.ApplicationInstance.CompleteRequest();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                lblSubscriberCount.Text = "This celebrity has Precluded itself for providing its Subscribers' data.";
                lblSubscriberCount.ForeColor = System.Drawing.Color.Red;
            }
            
        }
    }
}