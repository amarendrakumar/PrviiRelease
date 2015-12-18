using Prvii.Entities;
using Prvii.Entities.DataEntities;
using Prvii.Entities.Enumerations;
using Prvii.Web.AppCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Prvii.Web.MasterPages
{
    public partial class Site : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var loggedUser = (this.Page as BasePage).UserSesssion;

            if (!Page.IsPostBack && loggedUser != null)
            {
                this.lblUsername.Text = loggedUser.Name;
                this.hypLoginAccount.NavigateUrl = "~/WebPages/Users/UserDetails.aspx?ID=" + loggedUser.ID;
                this.hypLoginAccount.Text = " Account Setting ";
                this.HypRegistration.Visible = false;
                this.HypLogout.Visible = true;
            }
            else
            {
                this.hypLoginAccount.NavigateUrl = "~/WebPages/Login.aspx";
                this.hypLoginAccount.Text = " Login ";

                this.HypRegistration.Visible = true;
                this.HypLogout.Visible = false;
            }

            //if (this.divLogin.Visible)
            //    this.divLogin.Visible = loggedUser == null;

            this.divLoggedUser.Visible = loggedUser != null;

            this.ShowHideMainMenu(loggedUser);
        }

        private void ShowHideMainMenu(UserProfileData loggedUser)
        {
            var celebrityMainMenu = Celebrities; // this.mnuSite.FindItem("Celebrities");
            var cartMainMenu = Cart; // this.mnuSite.FindItem("Cart");
            var reportsMainMenu = Reports; // this.mnuSite.FindItem("Reports");
            var manageMainMenu = Manage;  // this.mnuSite.FindItem("Manage");
            var manageRevenueReport = RevenueReport;


            bool showCelebrityMainMenu = true;
            bool showCartMainMenu = true;
            bool showReportsMainMenu = true;
            bool showManageMainMenu = true;
            bool showManageRevenueReport = true;

            if (loggedUser == null)
            {
                showCartMainMenu = false;
                showReportsMainMenu = false;
                showManageMainMenu = false;
                showManageRevenueReport = false;
            }
            else
            {
                Role userRole = loggedUser.UserRole;

                if (userRole == Role.Administrator)
                {
                    showCelebrityMainMenu = false;
                    showCartMainMenu = false;
                    showManageRevenueReport = false;
                }
                else
                {
                    showReportsMainMenu = false;
                    showManageMainMenu = false;
                  

                    if (userRole == Role.Group || userRole == Role.Celebrity)
                    {
                        showCartMainMenu = false;                        
                    }
                     if (userRole == Role.Subscriber)
                    {
                        showManageRevenueReport = false;                      
                    }
                    

                    if(userRole == Role.Celebrity)
                    {
                        this.CelebritiesLink.Text = "My Page";
                    }
                }
            }

            if (!showCelebrityMainMenu && celebrityMainMenu != null)
                celebrityMainMenu.Visible=false;

            if (!showCartMainMenu && cartMainMenu != null)
               cartMainMenu.Visible = false;

            if (!showReportsMainMenu && reportsMainMenu != null)
                reportsMainMenu.Visible = false;

            if (!showManageMainMenu && manageMainMenu != null)
                manageMainMenu.Visible = false;
            if (!showManageRevenueReport && showManageRevenueReport != null)
                manageRevenueReport.Visible = false;
        }

      
        public string SetBodyBackgroundImage
        {
            set
            {
                this.pageBody.Style["background-image"] = value;
            }
        }

        public void HideBannerItems()
        {
           // this.mnuSite.Visible = false;
           // this.divLogin.Visible = false;
        }

      
    }
}