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
    public partial class ChannelSubscriberStatistics : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                this.BindMasters();

                if (this.ChannelID == 0)
                {
                    this.tblChannelFilter.Visible = true;
                    this.btnBack.Visible = false;
                }
                else
                {
                    this.ddlChannels.SelectedValue = this.ChannelID.ToString();
                    this.litCelebrityName.Text = ChannelManager.GetCelebrityName(this.ChannelID);
                }
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            this.BindSubscribers();
        }

        private void BindSubscribers()
        {
            DateTime? startDate = null;
            DateTime? endDate = null;
            bool compareWithETD = false;
            bool activeOnly = false;
            int? selectYear = null;
            bool NAvalue = false;
            
            string periodTypes = this.ddlPeriodTypes.SelectedValue;

            if (this.ddlSubscriber.SelectedValue == "1")
            {
                activeOnly = true;
            }
            else if (this.ddlSubscriber.SelectedValue == "2")
            {
                compareWithETD = true;
                 selectYear = Convert.ToInt32(this.ddlYears.SelectedValue);
                if (periodTypes == "1") //month
                {
                    int selectedMonth = Convert.ToInt32(this.ddlPeriods.SelectedValue);
                    if ((DateTime.UtcNow.Month <= selectedMonth) && (selectYear == DateTime.UtcNow.Year))
                    {
                        startDate = null;
                        endDate = null;
                        NAvalue = true;
                    }
                    else
                    {
                        //startDate = new DateTime(DateTime.UtcNow.Year, selectedMonth, 1, 23, 59, 59);
                        startDate = new DateTime(selectYear.Value, selectedMonth, 1, 00, 00, 01);
                        endDate = startDate.Value.AddMonths(1).AddDays(-1);
                        endDate = new DateTime(endDate.Value.Year, endDate.Value.Month, endDate.Value.Day, 23, 59, 59);
                    }
                   
                }
                else if (periodTypes == "2") //quater
                {

                    int selectedQuater = Convert.ToInt32(this.ddlPeriods.SelectedValue);
                    int startMonth = 1 + 3 * (selectedQuater - 1);
                    int endMonth = startMonth + 2;
                    int iQuarter =DateTime.UtcNow.GetQuarter();
                    //startDate = new DateTime(DateTime.UtcNow.Year, startMonth, 1, 23, 59, 59);
                    //endDate = new DateTime(DateTime.UtcNow.Year, endMonth, 1, 23, 59, 59).AddMonths(1).AddDays(-1);
                    if ((iQuarter <= selectedQuater) && (selectYear == DateTime.UtcNow.Year))
                    {
                        startDate = null;
                        endDate = null;
                        NAvalue = true;
                    }
                    else
                    {
                        startDate = new DateTime(selectYear.Value, startMonth, 1, 00, 00, 01);
                        endDate = new DateTime(selectYear.Value, endMonth, 1, 23, 59, 59).AddMonths(1).AddDays(-1);
                    }
                }
            }
            else if (this.ddlSubscriber.SelectedValue == "3")
            {
                if (periodTypes == "1") //today
                {
                    startDate = endDate = DateTime.UtcNow;
                    endDate = new DateTime(endDate.Value.Year, endDate.Value.Month, endDate.Value.Day, 23, 59, 59);
                    startDate = new DateTime(startDate.Value.Year, startDate.Value.Month, startDate.Value.Day, 00, 00, 01);
                }
                else if (periodTypes == "2") //last week
                {
                    DayOfWeek weekStart = DayOfWeek.Sunday;
                    endDate = DateTime.UtcNow;

                   
                    while (endDate.Value.DayOfWeek != weekStart)
                        endDate = endDate.Value.AddDays(-1);

                    endDate = endDate.Value.AddDays(-1);

                    startDate = endDate.Value.AddDays(-6);
                    endDate = new DateTime(endDate.Value.Year, endDate.Value.Month, endDate.Value.Day, 23, 59, 59);
                    startDate = new DateTime(startDate.Value.Year, startDate.Value.Month, startDate.Value.Day, 00, 00, 01);

                }
                else if (periodTypes == "3") //last month
                {
                    startDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1).AddMonths(-1);
                    endDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1).AddDays(-1);
                    endDate = new DateTime(endDate.Value.Year, endDate.Value.Month, endDate.Value.Day, 23, 59, 59);
                    startDate = new DateTime(startDate.Value.Year, startDate.Value.Month, startDate.Value.Day, 00, 00, 01);
                }
                else if (periodTypes == "4") //this month
                {
                    startDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
                    endDate = startDate.Value.AddMonths(1).AddSeconds(-1);
                    endDate = new DateTime(endDate.Value.Year, endDate.Value.Month, endDate.Value.Day, 23, 59, 59);
                    startDate = new DateTime(startDate.Value.Year, startDate.Value.Month, startDate.Value.Day, 00, 00, 01);
                }
                else if (periodTypes == "5") //this week
                {
                    DayOfWeek weekStart = DayOfWeek.Sunday;
                    endDate = DateTime.UtcNow;

                    var WeekstartDate = DateTime.UtcNow;

                    while (WeekstartDate.DayOfWeek != weekStart)
                        WeekstartDate = WeekstartDate.AddDays(-1);

                    startDate = WeekstartDate;
                    endDate = new DateTime(endDate.Value.Year, endDate.Value.Month, endDate.Value.Day, 23, 59, 59);
                    startDate = new DateTime(startDate.Value.Year, startDate.Value.Month, startDate.Value.Day, 00, 00, 01);
                   
                }
                else if (periodTypes == "6") //month
                {
                    
                    selectYear = Convert.ToInt32(this.ddlYears.SelectedValue);
                    int selectedMonth = Convert.ToInt32(this.ddlPeriods.SelectedValue);
                    startDate = new DateTime(selectYear.Value, selectedMonth, 1, 23, 59, 59);
                    endDate = startDate.Value.AddMonths(1).AddDays(-1);
                    endDate = new DateTime(endDate.Value.Year, endDate.Value.Month, endDate.Value.Day, 23, 59, 59);
                    startDate = new DateTime(startDate.Value.Year, startDate.Value.Month, startDate.Value.Day, 00, 00, 01);
                }
                else if (periodTypes == "7") //quater
                {
                    selectYear = Convert.ToInt32(this.ddlYears.SelectedValue);
                    int selectedQuater = Convert.ToInt32(this.ddlPeriods.SelectedValue);
                    int startMonth = 1 + 3 * (selectedQuater - 1);
                    int endMonth = startMonth + 2;
                    int iQuarter = DateTime.UtcNow.GetQuarter();
                    startDate = new DateTime(selectYear.Value, startMonth, 1, 00, 00, 01);
                    endDate = new DateTime(selectYear.Value, endMonth, 1, 23, 59, 59).AddMonths(1).AddDays(-1);
                }
               
              
            }

          

            long channelID = Convert.ToInt64(this.ddlChannels.SelectedValue);
            if(NAvalue)
            {                
                this.gvUserList.DataSource = null;
                this.gvUserList.DataBind();
                this.lblSubscriberCount.Text = "0";
            }
           else
            {
                var list = ChannelSubscribersManager.GetSubscriberStatistics(channelID, startDate, endDate, compareWithETD, activeOnly);
                this.gvUserList.DataSource = list;
                this.gvUserList.DataBind();

                this.lblSubscriberCount.Text = list.Count.ToString();
            }
        }

     
        private void BindMasters()
        {
            this.BindDropDown(this.ddlChannels, ChannelManager.GetForDropdown(), false, "-Select Celebrity-");
            this.BindPeriodTypes();
            this.BindPeriods();
        }
       

        private void BindPeriodTypes()
        {
            List<ListItem> list = new List<ListItem>();

            if (this.ddlSubscriber.SelectedValue == "2")
            {
                list.Add(new ListItem("Month", "1"));
                list.Add(new ListItem("Quarter", "2"));
            }
            else if (this.ddlSubscriber.SelectedValue == "3")
            {
                list.Add(new ListItem("Today", "1"));
                list.Add(new ListItem("This Week", "5"));
                list.Add(new ListItem("Last Week", "2"));
                list.Add(new ListItem("Last Month", "3"));
                list.Add(new ListItem("This Month", "4"));
                list.Add(new ListItem("Monthly", "6"));
                list.Add(new ListItem("Quarterly", "7"));

            }

            this.BindDropDown(this.ddlPeriodTypes, list, true, "-Period-");
        }

        private void BindPeriods()
        {
            List<ListItem> list = new List<ListItem>();

            if (this.ddlSubscriber.SelectedValue == "2")
            {
                if (this.ddlPeriodTypes.SelectedValue == "1")
                {
                    list.Add(new ListItem("January", "1"));
                    list.Add(new ListItem("February", "2"));
                    list.Add(new ListItem("March", "3"));
                    list.Add(new ListItem("April", "4"));
                    list.Add(new ListItem("May", "5"));
                    list.Add(new ListItem("June", "6"));
                    list.Add(new ListItem("July", "7"));
                    list.Add(new ListItem("August", "8"));
                    list.Add(new ListItem("September", "9"));
                    list.Add(new ListItem("October", "10"));
                    list.Add(new ListItem("November", "11"));
                    list.Add(new ListItem("December", "12"));
                }
                else if (this.ddlSubscriber.SelectedValue == "2")
                {
                    list.Add(new ListItem("1st", "1"));
                    list.Add(new ListItem("2nd", "2"));
                    list.Add(new ListItem("3rd", "3"));
                    list.Add(new ListItem("4th", "4"));
                }
            }
            else if (this.ddlSubscriber.SelectedValue == "3")
            {
                if (this.ddlPeriodTypes.SelectedValue == "6")
                {
                    list.Add(new ListItem("January", "1"));
                    list.Add(new ListItem("February", "2"));
                    list.Add(new ListItem("March", "3"));
                    list.Add(new ListItem("April", "4"));
                    list.Add(new ListItem("May", "5"));
                    list.Add(new ListItem("June", "6"));
                    list.Add(new ListItem("July", "7"));
                    list.Add(new ListItem("August", "8"));
                    list.Add(new ListItem("September", "9"));
                    list.Add(new ListItem("October", "10"));
                    list.Add(new ListItem("November", "11"));
                    list.Add(new ListItem("December", "12"));
                }
                else if (this.ddlPeriodTypes.SelectedValue == "7")
                {
                    list.Add(new ListItem("1st", "1"));
                    list.Add(new ListItem("2nd", "2"));
                    list.Add(new ListItem("3rd", "3"));
                    list.Add(new ListItem("4th", "4"));
                }

            }

           

            this.BindDropDown(this.ddlPeriods, list, true);
        }

        private void BindYears()
        {
            List<ListItem> list = new List<ListItem>();

            for (int y = DateTime.UtcNow.Year; y >= 2014; y--)
            {
                list.Add(new ListItem(y.ToString(), y.ToString()));
            }
               

            this.BindDropDown(this.ddlYears, list, true,"-Year-");
        }

        protected void ddlSubscriber_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.ddlPeriodTypes.ClearSelection();
            this.ddlPeriods.ClearSelection();
            this.ddlYears.ClearSelection();

            if(this.ddlSubscriber.SelectedValue == "1")
            {
                this.ddlYears.Enabled = false;
                this.ddlPeriodTypes.Enabled = false;
                this.ddlPeriods.Enabled = false;
                this.rfvPeriodTypes.Visible = false;
                this.rfvPeriods.Visible = false;
                this.rfvYears.Visible = false;
            }
            else if (this.ddlSubscriber.SelectedValue == "2")
            {
                this.BindYears();
                this.BindPeriodTypes();
                this.ddlPeriodTypes.Enabled = true;
                this.ddlPeriods.Enabled = true;
                this.rfvPeriodTypes.Visible = true;
                this.rfvPeriods.Visible = true;
                this.ddlYears.Enabled = true;
                this.rfvYears.Visible = true;
            }
            else if (this.ddlSubscriber.SelectedValue == "3")
            {
                this.BindPeriodTypes();
                this.BindYears();
                this.ddlPeriodTypes.Enabled = true;
                this.ddlPeriods.Enabled = false;
                this.rfvPeriodTypes.Visible = true;
                this.rfvPeriods.Visible = false;
                this.ddlYears.Enabled = false;
                this.rfvYears.Visible = false;
            }
        }

        protected void ddlPeriodTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
           // this.ddlPeriodTypes.ClearSelection();
            this.ddlPeriods.ClearSelection();
           // this.ddlYears.ClearSelection();


            if (this.ddlSubscriber.SelectedValue == "2")
                this.BindPeriods();
            else if (this.ddlSubscriber.SelectedValue == "3")
            {
                if ((this.ddlPeriodTypes.SelectedValue == "6") || (this.ddlPeriodTypes.SelectedValue == "7"))
                {
                    //this.BindYears();
                    this.BindPeriods();
                    this.ddlPeriodTypes.Enabled = true;
                    this.ddlPeriods.Enabled = true;
                    this.rfvPeriodTypes.Visible = true;
                    this.rfvPeriods.Visible = true;
                    this.ddlYears.Enabled = true;
                    this.rfvYears.Visible = true;
                }
                else
                {
                   
                    this.ddlPeriodTypes.Enabled = true;
                    this.ddlPeriods.Enabled = false;
                    this.rfvPeriodTypes.Visible = true;
                    this.rfvPeriods.Visible = false;
                    this.ddlYears.Enabled = false;
                    this.rfvYears.Visible = false;
                }
            }

        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            this.NavigateBack();
        }

        private long ChannelID
        {
            get
            {
                return Convert.ToInt64(Request["ID"]);
            }
        }

        protected void btn_DownLoadAll_Click(object sender, EventArgs e)
        {
           // DownloadToExcel.GetAllSubstciberDetails();
            long channelID = Convert.ToInt64(this.ddlChannels.SelectedValue);
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
    }
}