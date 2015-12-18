using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Prvii.Web.AppCode;
using Prvii.Business;
using System.Text;
using Prvii.Entities;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Microsoft.Reporting.WebForms;
using Prvii.Entities.DataEntities;
using Prvii.Entities.Enumerations;

namespace Prvii.Web.WebPages.ChannelSubscribers
{
    public partial class RevenueReporting : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
            if (!Page.IsPostBack)
            {
                this.BindMasters();
              
            }
        }
        private void BindMasters()
        {
            if ( this.UserSesssion.UserRole == Role.Administrator)
            {
                this.BindCheckBoxList(this.chkblChannels, ChannelManager.GetForDropdown(), false, " select ");
                this.chkAll.Visible = true;
                this.chkblChannels.Enabled = true;
            }
            else if (this.UserSesssion.UserRole == Role.Group)
            {
                this.BindCheckBoxList(this.chkblChannels, ChannelManager.GetForDropdown(this.UserSesssion.GroupID.Value), false, " select ");
                this.chkAll.Visible = true;
                this.chkblChannels.Enabled = true;
            }
            else if (this.UserSesssion.UserRole == Role.Celebrity)
            {

                this.BindCheckBoxList(this.chkblChannels, ChannelManager.GetForDropdownByChannel(this.UserSesssion.ChannelID.Value), false, " select ");
                this.chkAll.Visible = false;
                foreach (ListItem item in this.chkblChannels.Items)
                {
                    item.Selected = true;
                }

                this.chkblChannels.Enabled = false;
               
            }
        }

        protected void btnAdminReport_Click(object sender, EventArgs e)
        {
            //divProgress.Visible = true;
            //Warning[] warnings;
            //string[] streamIds;
            string mimeType;
            string encoding;
            string extension;
            BindReport(out mimeType, out encoding, out extension);

            
        }

        private void BindReport(out string mimeType, out string encoding, out string extension)
        {


            mimeType = string.Empty;
            encoding = string.Empty;
            extension = string.Empty;

            ReportViewer1.ProcessingMode = ProcessingMode.Local;
            ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/WebPages/ChannelSubscribers/CelebrityRevenue.rdlc");


            IEnumerable<string> CheckedItems = this.chkblChannels.Items.Cast<ListItem>()
                                   .Where(i => i.Selected)
                                   .Select(i => i.Value);

            string Celebrities = string.Empty;

            for (int i = 0; i < chkblChannels.Items.Count; i++)
            {
                if (chkblChannels.Items[i].Selected)
                {
                    Celebrities += chkblChannels.Items[i].Value + ",";
                }
            }
            if (Celebrities.Length > 0)
            {
                Celebrities = Celebrities.Substring(0, Celebrities.Length - 1);
            }




            ChannelSubscribersManager manage = new Business.ChannelSubscribersManager();
            List<CelebrityRevenueReport> list = manage.GetCelebrityRevenue(Celebrities, Convert.ToDateTime(txtStartDate.Text.Trim()), Convert.ToDateTime(txtEndDate.Text.Trim()));
            ReportDataSource datasource = new ReportDataSource("DataSet1", list);
            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.LocalReport.DataSources.Add(datasource);
        }

        protected void tbnExcelReport_Click(object sender, ImageClickEventArgs e)
        {
            Warning[] warnings;
            string[] streamIds;
            string mimeType;
            string encoding;
            string extension;
            BindReport(out mimeType, out encoding, out extension);


            try
            {
                byte[] bytes = ReportViewer1.LocalReport.Render("Excel", null, out mimeType, out encoding, out extension, out streamIds, out warnings);


                // byte[] bytes = viewer.LocalReport.Render("Excel", null, out mimeType, out encoding, out extension, out streamIds, out warnings);
                // Now that you have all the bytes representing the PDF report, buffer it and send it to the client.          
                // System.Web.HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Buffer = true;
                Response.Clear();
                Response.ContentType = mimeType;
                Response.AddHeader("content-disposition", "attachment; filename= filename" + "." + extension);
                Response.OutputStream.Write(bytes, 0, bytes.Length); // create the file  
               // Response.Flush(); // send it to the client to download  
                Response.End();
            }
            catch (Exception ex)
            {
                string ss = ex.Message;
            }
        }

        protected void btnPDFReport_Click(object sender, ImageClickEventArgs e)
        {
            Warning[] warnings;
            string[] streamIds;
            string mimeType;
            string encoding;
            string extension;
            BindReport(out mimeType, out encoding, out extension);

            try
            {
                byte[] bytes = ReportViewer1.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);


                // byte[] bytes = viewer.LocalReport.Render("Excel", null, out mimeType, out encoding, out extension, out streamIds, out warnings);
                // Now that you have all the bytes representing the PDF report, buffer it and send it to the client.          
                // System.Web.HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Buffer = true;
                Response.Clear();
                Response.ContentType = mimeType;
                Response.AddHeader("content-disposition", "attachment; filename= filename" + "." + extension);
                Response.OutputStream.Write(bytes, 0, bytes.Length); // create the file  
                Response.Flush(); // send it to the client to download  
                Response.End();
            }
            catch (Exception ex)
            {
                string ss = ex.Message;
            }
        }

       
    }
}