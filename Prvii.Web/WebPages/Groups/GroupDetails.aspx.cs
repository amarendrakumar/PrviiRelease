using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Prvii.Web.AppCode;
using Prvii.Business;
using Prvii.Entities;

namespace Prvii.Web.WebPages.Groups
{
    public partial class GroupDetails : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.BindGroupDetails();
                this.tbName.Focus();
            }
        }

        private void BindGroupDetails()
        {
            var group = GroupManager.GetByID(this.GroupID);

            if (group == null)
                return;

            this.tbName.Text = group.Name;
            this.tbAddress1.Text = group.Address1;
            this.tbAddress2.Text = group.Address2;
            this.tbZipCode.Text = group.ZipCode;
            this.tbEmail.Text = group.Email;
            this.tbTelephone.Text = group.Telephone;
            this.tbMobile.Text = group.Mobile;
            this.cbxIsActive.Checked = group.IsActive;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            GroupManager.Save(this.PrepareToSave());
            this.NavigateBack();
        }

        private Group PrepareToSave()
        {
            var group = this.GroupID == 0 ? new Group() : GroupManager.GetByID(this.GroupID);
            
            group.Name = this.tbName.Text;
            group.Address1 = this.tbAddress1.Text;
            group.Address2 = this.tbAddress2.Text;
            group.ZipCode = this.tbZipCode.Text;
            group.Email = this.tbEmail.Text;
            group.Telephone = this.tbTelephone.Text;
            group.Mobile = this.tbMobile.Text;
            group.IsActive = this.cbxIsActive.Checked;

            return group;
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            this.NavigateBack();
        }

        private long GroupID
        {
            get
            {
                return Convert.ToInt64(Request["ID"]);
            }
        }
    }
}