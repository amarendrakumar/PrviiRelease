using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Prvii.Web.AppCode;
using Prvii.Business;

namespace Prvii.Web.WebPages.Users
{
    public partial class UserPasswordReset : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnResetPassword_Click(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
                return;

            long id = 0;

            Int64.TryParse(UtilityManager.Decrypt(Request["ID"].ToString()), out id);

            if (id == 0)
                this.UnAuthorizeAccess();

            UserProfileManager.PasswordReset(id, this.tbPassword.Text);
            this.tblForgotPassword.Visible = false;
            this.lblMessage.Text = "Password successfully reset!";
        }
    }
}