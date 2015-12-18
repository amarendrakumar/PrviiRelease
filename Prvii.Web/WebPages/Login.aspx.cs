using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Prvii.Web.AppCode;
using Prvii.Entities;
using Prvii.Business;
using Prvii.Entities.Enumerations;
using System.Drawing;
using Prvii.Entities.DataEntities;
using System.Web.Configuration;
using Prvii.ExceptionHandling;

namespace Prvii.Web.WebPages
{
    public partial class Login : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
                this.tbEmail.Focus();
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            UserProfile userProfile = UserProfileManager.Authenticate(tbEmail.Text.Trim(), tbPassword.Text.Trim());

            if (userProfile == null)
            {
                this.lblMessage.Text = "Email ID or Password is not valid";
                this.lblMessage.ForeColor = Color.Red;
                return;
            }

            this.UserID = userProfile.ID;
            
            var userRoles = UserProfileManager.GetRoleIDList(userProfile.ID);

            if (userRoles.Count == 1)
                this.LoginSuccessful((short)userRoles.First());

            this.BindRoles(userRoles);

            this.lblPageHeader.Text = "Select Role";
            this.tblLogin.Visible = false;
            this.tblSelectRole.Visible = true;
        }

        private void BindRoles(List<short> userRoles)
        {
            var itemList = userRoles.Select(x => new
                {
                    ID = x,
                    Name = Enum.GetName(typeof(Role), x)
                }).OrderBy(o=>o.Name).ToList();

            this.BindDropDown(this.ddlRoles, itemList);
        }

        protected void btnContinue_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            this.LoginSuccessful(Convert.ToInt16(this.ddlRoles.SelectedValue));
        }

        private void LoginSuccessful(short roleID)
        {
            var userProfile = UserProfileManager.GetByID(this.UserID);
            if (roleID == (short)Role.Celebrity)
            {
                if (userProfile.ChannelID.HasValue)
                {
                    var channel = ChannelManager.GetByID(userProfile.ChannelID.Value);
                    if (!channel.IsActive)
                    {
                        this.lblMessage.Text = channel.Firstname.Trim() + " " + channel.Lastname.Trim() + " has suspended her VIP club for now. We will let you know when it is available again.";
                        this.lblMessage.ForeColor = Color.Red;
                        return;
                    }
                }
            }

            this.UserSesssion = new UserProfileData { ID = userProfile.ID, Firstname = userProfile.Firstname, Lastname = userProfile.Lastname, Email = userProfile.Email, ChannelID = userProfile.ChannelID, GroupID = userProfile.GroupID, UserRole = (Role)roleID, TimeZoneID = userProfile.TimeZoneID };

            string returnURL = Request["ReturnURL"] != null && !Request["ReturnURL"].ToString().Contains("ErrorPage.aspx") ? Server.UrlDecode(Request["ReturnURL"].ToString()) : "~/WebPages/Channels/ChannelList.aspx";
            
            Response.Redirect(returnURL,true);
        }

        protected void lbtnForgotPassword_Click(object sender, EventArgs e)
        {
            this.tbForgotPasswordEmail.Focus();
            this.tblLogin.Visible = false;
            this.tblForgotPassword.Visible = true;
            this.lblPageHeader.Text = "Forgot Password";
        }

        protected void btnForgotPasswordCancel_Click(object sender, EventArgs e)
        {
            this.tblLogin.Visible = true;
            this.tblForgotPassword.Visible = false;
            this.lblPageHeader.Text = "Log in";
        }

        protected void btnForgotPasswordSendEmail_Click(object sender, EventArgs e)
        {
            try
            {
                var userProfile = UserProfileManager.GetByEmail(this.tbForgotPasswordEmail.Text);

                if (userProfile != null)
                {
                    UserProfileManager.ForgotPassword(userProfile);
                    this.tblForgotPassword.Visible = true;
                    // this.tblLogin.Visible = true;
                    this.lblMessageForgetpassword.Text = "An email has been sent to your Email ID (" + this.tbForgotPasswordEmail.Text + "), Please Check your email to reset password.";
                    this.lblMessageForgetpassword.ForeColor = System.Drawing.Color.Gray;
                }
                else
                {
                    this.lblMessageForgetpassword.Text = "The email id, you have entered, is not registered with Prvii Celebrity Services. Kindly, use a registered email id and try again.";
                    this.lblMessageForgetpassword.ForeColor = System.Drawing.Color.Red;
                }
            }
            catch (Exception ex)
            {
                this.LogMessage(" Web page login  Email Error msg : " + ex.Message + " - " + ex.StackTrace);
            }
            
        }

        private void LogMessage(string message)
        {
            string logfilePath = WebConfigurationManager.AppSettings["LogFilePathweb"].ToString();
            ExceptionHandler.LogMessage(message, true, 10240, logfilePath);
        }


        private long UserID
        {
            get
            {
                return Convert.ToInt64(ViewState["UserID"]);
            }
            set
            {
                ViewState["UserID"] = value;
            }
        }



    }
}