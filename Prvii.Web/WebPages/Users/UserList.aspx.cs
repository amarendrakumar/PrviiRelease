using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Prvii.Web.AppCode;
using Prvii.Business;
using Prvii.Entities.Enumerations;
using System.Collections;
using Prvii.Entities;
using Prvii.Entities.DataEntities;

namespace Prvii.Web.WebPages.Users
{
    public partial class UserList : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                this.btnBack.PostBackUrl = "~/WebPages/Channels/ChannelDetailsView.aspx?ID=" + this.ChannelID;
                this.BindUsers();

                this.BindSearchColumns();
            }
        }

        private void BindSearchColumns()
        {
            List<ListItem> searchList = new List<ListItem>();
            searchList.Add(new ListItem("Name", "1"));
            searchList.Add(new ListItem("Mobile", "2"));
            searchList.Add(new ListItem("Email", "3"));
            searchList.Add(new ListItem("Zip Code", "4"));
            if (this.UserSesssion.UserRole == Role.Administrator)
            {
                searchList.Add(new ListItem("Role", "5"));
                searchList.Add(new ListItem("Active", "6"));
            }
            

            this.BindDropDown(this.ddlFilters, searchList, true,"-Select Column-");
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            this.BindUsers();
        }

        private List<UserProfileData> ApplyFilter(List<UserProfileData> userList, string sortExpression)
        {
            string columnIndex = this.ddlFilters.SelectedValue;
            string filterValue = this.tbFilterBy.Text.ToLower();
            string filterStatusvalue = this.ddlStatus.SelectedValue.ToLower();

            switch(columnIndex)
            {
                case "1":
                    userList = userList.Where(x => x.Name.ToLower().Contains(filterValue)).ToList();
                    break;
                case "2":
                    userList = userList.Where(x => x.Mobile.ToLower().Contains(filterValue)).ToList();
                    break;
                case "3":
                    userList = userList.Where(x => x.Email.ToLower().Contains(filterValue)).ToList();
                    break;
                case "4":
                    userList = userList.Where(x => x.ZipCode.ToLower().Contains(filterValue)).ToList();
                    break;
                case "5":
                    userList = userList.Where(x => x.RoleName.ToLower().Contains(filterValue)).ToList();
                    break;
                case "6":
                    userList = userList.Where(x => x.IsActive.ToString().ToLower().Contains(filterStatusvalue)).ToList();
                    break;
            }

            SortDirection sortDirection = this.GetSortDirection() == "ASC" ? SortDirection.Ascending : SortDirection.Descending;

            switch(sortExpression)
            {
                case "Name":
                    userList = sortDirection == SortDirection.Ascending ? userList.OrderBy(o => o.Name).ToList() : userList.OrderByDescending(o => o.Name).ToList();
                    break;
                case "Mobile":
                    userList = sortDirection == SortDirection.Ascending ? userList.OrderBy(o => o.Mobile).ToList() : userList.OrderByDescending(o => o.Mobile).ToList();
                    break;
                case "Email":
                    userList = sortDirection == SortDirection.Ascending ? userList.OrderBy(o => o.Email).ToList() : userList.OrderByDescending(o => o.Email).ToList();
                    break;
                case "ZipCode":
                    userList = sortDirection == SortDirection.Ascending ? userList.OrderBy(o => o.ZipCode).ToList() : userList.OrderByDescending(o => o.ZipCode).ToList();
                    break;
                case "RoleName":
                    userList = sortDirection == SortDirection.Ascending ? userList.OrderBy(o => o.RoleName).ToList() : userList.OrderByDescending(o => o.RoleName).ToList();
                    break;
                case "IsActive":
                    userList = sortDirection == SortDirection.Ascending ? userList.OrderBy(o => o.IsActive).ToList() : userList.OrderByDescending(o => o.IsActive).ToList();
                    break;
            }

            return userList;
        }

        private void BindUsers(string sortExpression = "")
        {
            long channelID = this.ChannelID;

            if (channelID == 0 && this.UserSesssion.UserRole == Role.Administrator)
            {
                this.lblPageHeader.Text = "User for  " + ChannelManager.GetCelebrityName(this.ChannelID);
                this.gvUserList.DataSource = this.ApplyFilter(UserProfileManager.GetUsersNew(), sortExpression);
                this.gvUserList.DataBind();
            }
            else if(this.UserSesssion.UserRole != Role.Subscriber)
            {
                this.lblPageHeader.Text = "Subscribers for  " + ChannelManager.GetCelebrityName(this.ChannelID);
                this.gvUserList.DataSource = this.ApplyFilter(ChannelSubscribersManager.GetSubscribers(channelID), sortExpression);
                this.gvUserList.DataBind();

                this.btnShowStatistics.Visible = true;
                this.btnBack.Visible = true;
                this.btnShowStatistics.PostBackUrl = "~/WebPages/ChannelSubscribers/ChannelSubscriberStatistics.aspx?ID=" + channelID;

                this.btnCreateNew.Visible = false;
                this.gvUserList.Columns[4].Visible = false;
                this.gvUserList.Columns[5].Visible = false;
            }
        }

        protected void gvUserList_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.gvUserList.PageIndex = e.NewPageIndex;
            this.BindUsers();
        }

        private long ChannelID
        {
            get
            {
                return Convert.ToInt64(Request["ID"]);
            }
        }

        protected void gvUserList_Sorting(object sender, GridViewSortEventArgs e)
        {
            this.BindUsers(e.SortExpression);

            int colIndex = this.GetGridViewColumnIndex(e.SortExpression, this.gvUserList);

            Image sortImage = new Image();
            sortImage.ImageUrl = this.GetCurrentSortDirection() == SortDirection.Ascending ? "~/Images/asc.png" : "~/Images/desc.png";

            this.gvUserList.HeaderRow.Cells[colIndex].Controls.Add(sortImage);
        }

        protected SortDirection GetCurrentSortDirection()
        {
            string sortDirection = "ASC";

            if (ViewState["SortDirection"] != null)
                sortDirection = ViewState["SortDirection"].ToString();

            return sortDirection == "ASC" ? SortDirection.Ascending : SortDirection.Descending;
        }

        protected string GetSortDirection()
        {
            if (ViewState["SortDirection"] == null)
            {
                ViewState["SortDirection"] = "ASC";
                return "ASC";
            }
            else if (ViewState["SortDirection"].ToString() == "ASC")
            {
                ViewState["SortDirection"] = "DESC";
                return "DESC";
            }
            else if (ViewState["SortDirection"].ToString() == "DESC")
            {
                ViewState["SortDirection"] = "ASC";
                return "ASC";
            }

            return "ASC";
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            this.BindSearchColumns();
            this.tbFilterBy.Text = string.Empty;
            this.BindUsers();
        }

        protected void ddlFilters_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.ddlFilters.SelectedValue == "6")
            {
                this.ddlStatus.Visible = true;
                this.tbFilterBy.Visible = false;
                this.rfvFilterBy.Visible = false;
                this.rfvFilterBy.Enabled = false;
            }
            else
            {
                this.ddlStatus.Visible = false;
                this.tbFilterBy.Visible = true;
                this.rfvFilterBy.Visible = true;
                this.rfvFilterBy.Enabled = true;
            }
        } 
    }

}