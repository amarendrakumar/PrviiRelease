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
using Prvii.Entities.DataEntities;

namespace Prvii.Web.WebPages.Users
{
    public partial class UserDetails : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.BindMasters();
                if (this.UserID != 0)
                    this.BindUser();
                else
                {
                    var listItem = this.ddlCountry.Items.FindByText("United States");

                    if (listItem != null)
                    {
                        this.ddlCountry.ClearSelection();
                        listItem.Selected = true;
                    }
                    this.BindState();
                }

                this.CheckAuthorization();
            }
            else
            {
                this.tbPassword.Attributes["value"] = this.tbPassword.Text;
                this.tbConfirmPassword.Attributes["value"] = this.tbConfirmPassword.Text;
            }
        }

        private void CheckAuthorization()
        {
            if (this.UserSesssion == null || this.UserSesssion.UserRole != Role.Administrator)
            {
                this.lblRolesLabel.Visible = false;
                this.cblRoles.Visible = false;
                this.lblChannelLabel.Visible = false;
                this.lblChannelLabel.Visible = false;
                this.ddlChannels.Visible = false;
                this.ddlGroups.Visible = false;
                this.trIsActive.Visible = false;
                this.trAddress.Visible = false;
                
            }
        }

        private void BindMasters()
        {
            this.cblDeliveryMechanisms.DataSource = this.GetEnumList<DeliveryMechanism>();
            this.cblDeliveryMechanisms.DataBind();

            this.rblDeviceTypes.DataSource = this.GetEnumList<DeviceType>();
            this.rblDeviceTypes.DataBind();

            this.cblRoles.DataSource = this.GetEnumList<Role>();
            this.cblRoles.DataBind();

            this.BindDropDown(this.ddlTimezones, TimeZoneInfo.GetSystemTimeZones().Select(x => new { ID = x.Id, Name = x.DisplayName }).ToList());

            this.BindDropDown(this.ddlCountry, MasterManager.GetAllCountry());
            this.BindState();
        }

        private void BindState()
        {
            this.BindDropDown(this.ddlStates, MasterManager.GetState(Convert.ToInt64(this.ddlCountry.SelectedValue)));
        }

        private void ShowGroupList()
        {
            this.BindDropDown(this.ddlGroups, GroupManager.GetAll());
            this.ddlGroups.Visible = true;
        }

        private void ShowChannelList()
        {
            this.BindDropDown(this.ddlChannels, ChannelManager.GetCelebrityList());
            this.ddlChannels.Visible = true;
        }

        private void BindUser()
        {
            UserProfile user = UserProfileManager.GetByID(this.UserID);

            if (user.ID != this.UserSesssion.ID && this.UserSesssion.UserRole != Role.Administrator)
            {
                this.tblrowPassword.Visible = false;
                this.btnSave.Visible = false;
            }
            
            this.tbFirstName.Text = user.Firstname;
            this.tbLastName.Text = user.Lastname;
            this.tbNickName.Text = user.NickName;
            this.tbAddress1.Text = user.Address1;
            this.tbAddress2.Text = user.Address2;
            this.ddlCountry.SelectedValue = user.CountryID.ToString();
            this.BindState();
            this.ddlStates.SelectedValue = user.StateID.ToString();
            this.tbCity.Text = Convert.ToString(user.City);
            this.tbZipCode.Text = user.ZipCode;
            this.ddlTimezones.SelectedValue = user.TimeZoneID;

            this.tbEmail.Text = user.Email;
            this.tbPassword.Attributes["value"] = user.Password;
            this.tbConfirmPassword.Attributes["value"] = user.Password;

            this.tbMobile.Text = user.Mobile;
            this.tbTelephone.Text = user.Telephone;
            this.cbxIsActive.Checked = user.IsActive;

            var deliveryMethodList = user.DeliveryMechanisms.Split(',').ToList();

            foreach (var item in deliveryMethodList)
            {
                var listItem = this.cblDeliveryMechanisms.Items.FindByValue(item);

                if (listItem != null)
                    listItem.Selected = true;
            }

            this.rblDeviceTypes.SelectedValue = ((DeviceType)user.DeviceTypeID).ToString();

            var userRoles = UserProfileManager.GetRoles(user.ID);

            foreach (var item in userRoles)
            {
                var listItem = this.cblRoles.Items.FindByValue(((Role)item.RoleID).ToString());

                if (listItem != null)
                    listItem.Selected = true;
            }

            foreach (var item in userRoles)
            {
                var listItem = this.cblRoles.Items.FindByValue(((Role)item.RoleID).ToString());

                if (listItem != null)
                    listItem.Selected = true;
            }

            this.cblRoles_SelectedIndexChanged(null, null);

            if (user.GroupID.HasValue)
            {
                this.ddlGroups.SelectedValue = user.GroupID.Value.ToString();
            }

            if (user.ChannelID.HasValue)
            {
                this.ddlChannels.SelectedValue = user.ChannelID.Value.ToString();
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            if (!this.ValidateFields())
                return;

            List<UserRole> userRoles = new List<UserRole>();

            UserProfile userProfile = this.PrepareToSave(userRoles);

            UserProfileManager.Save(userProfile, userRoles);

            if (this.Request["ChannelID"] != null)
            {
                this.UserSesssion = new UserProfileData { ID = userProfile.ID, Firstname = userProfile.Firstname, Lastname = userProfile.Lastname, Email = userProfile.Email, ChannelID = userProfile.ChannelID, GroupID = userProfile.GroupID, UserRole = Role.Subscriber };

                Response.Redirect("~/WebPages/ShoppingCarts/ShoppingCartList.aspx?ChannelID=" + this.Request["ChannelID"]);
            }
            else
                this.NavigateBack();
        }

        private bool ValidateFields()
        {
            if (!this.ValidateEmailUniqueness())
                return false;

            if (!this.ValidateDeliveryMechannism())
                return false;

            if (!this.ValidateDeviceTypeSelection())
                return false;

            if (this.trIsActive.Visible && this.UserID == this.UserSesssion.ID && !this.cbxIsActive.Checked)
            {
                this.DisplayMessageBox("You cannot disable yourself!", this.upPage);
                return false;
            }

            if (!this.ValidateRoles())
                return false;

            return true;
        }

        private bool ValidateEmailUniqueness()
        {
            if (UserProfileManager.Exists(this.tbEmail.Text, this.UserID))
            {
                this.DisplayMessageBox("Email exists. Please use another email.", this.upPage, this.tbEmail);
                return false;
            }
            this.tbNickName.Focus();
            return true;
        }

        private bool ValidateDeliveryMechannism()
        {
            bool selected = false;

            foreach (ListItem item in this.cblDeliveryMechanisms.Items)
            {
                if (item.Selected)
                {
                    selected = true;
                    return true;
                }
            }

            if (!selected)
            {
                this.DisplayMessageBox("Please select Delivery Mechanism.", this.upPage);
                return false;
            }

            return true;
        }

        private bool ValidateDeviceTypeSelection()
        {
            bool isDeliveryMechanismText = false;
            ListItem textItem = null;

            foreach (ListItem item in this.cblDeliveryMechanisms.Items)
            {
                if (item.Selected && item.Text == DeliveryMechanism.Text.ToString())
                {
                    isDeliveryMechanismText = true;
                    textItem = item;
                    break;
                }
            }

            if (isDeliveryMechanismText)
            {
                if (this.rblDeviceTypes.SelectedItem.Text == DeviceType.Other.ToString())
                {
                    textItem.Selected = false;
                    this.DisplayMessageBox("Text messages are not currently available if you don’t use either an Apple or an Android device.", this.upPage);
                    return false;
                }
            }

            return true;
        }

        private bool ValidateRoles()
        {
            if (this.UserID == 0 && this.UserSesssion == null)
            {
                foreach (ListItem item in this.cblRoles.Items)
                {
                    if (item.Text == Role.Subscriber.ToString())
                    {
                        item.Selected = true;
                        break;
                    }
                }
                return true;
            }

            bool selected = false;

            foreach (ListItem item in this.cblRoles.Items)
            {
                if (item.Selected)
                    selected = true;

                if (item.Text == Role.Group.ToString())
                {
                    if (item.Selected && this.ddlGroups.SelectedValue == "0")
                    {
                        this.DisplayMessageBox("Please select group.", this.upPage);
                        return false;
                    }
                }
                else if (item.Text == Role.Celebrity.ToString())
                {
                    if (item.Selected && this.ddlChannels.SelectedValue == "0")
                    {
                        this.DisplayMessageBox("Please select celebrity.", this.upPage);
                        return false;
                    }
                }
            }

            if (!selected)
            {
                this.DisplayMessageBox("Please select at least one role.", this.upPage);
                return false;
            }

            return true;
        }

        private UserProfile PrepareToSave(List<UserRole> userRoles)
        {
            UserProfile user = this.UserID == 0 ? new UserProfile() : UserProfileManager.GetByID(this.UserID);

            user.Firstname = this.tbFirstName.Text;
            user.Lastname = this.tbLastName.Text;
            user.NickName = this.tbNickName.Text;
            user.Address1 = this.tbAddress1.Text;
            user.Address2 = this.tbAddress2.Text;
            user.CountryID = Convert.ToInt64(this.ddlCountry.SelectedValue);

            if (this.ddlStates.SelectedValue != "0")
                user.StateID = Convert.ToInt64(this.ddlStates.SelectedValue);

            user.City = this.tbCity.Text;

            user.ZipCode = this.tbZipCode.Text;
            user.TimeZoneID = this.ddlTimezones.SelectedValue;

            user.Email = this.tbEmail.Text;
            user.Password = this.tbPassword.Text;

            user.Mobile = this.tbMobile.Text;
            user.Telephone = this.tbTelephone.Text;

            if (this.trIsActive.Visible)
                user.IsActive = this.cbxIsActive.Checked;
            else if (this.UserSesssion == null && user.ID == 0)
                user.IsActive = true;

            user.DeliveryMechanisms = this.GetSelectedValues(this.cblDeliveryMechanisms.Items);
            user.DeviceTypeID = Convert.ToInt16(Enum.Parse(typeof(DeviceType), this.rblDeviceTypes.SelectedItem.Value));

            foreach (ListItem item in this.cblRoles.Items)
            {
                if (item.Selected)
                    userRoles.Add(new UserRole { RoleID = Convert.ToInt16(Enum.Parse(typeof(Role), item.Value)) });
            }

            user.GroupID = this.ddlGroups.SelectedValue != "0" && this.ddlGroups.SelectedValue != "" ? (long?)Convert.ToInt64(this.ddlGroups.SelectedValue) : null;
            user.ChannelID = this.ddlChannels.SelectedValue != "0" && this.ddlChannels.SelectedValue != "" ? (long?)Convert.ToInt64(this.ddlChannels.SelectedValue) : null;

            return user;
        }

        private void SetDeliveryMechanisms(UserProfile user)
        {
            foreach (ListItem item in this.cblDeliveryMechanisms.Items)
            {
                if (item.Selected)
                {
                    if (user.DeliveryMechanisms == string.Empty)
                        user.DeliveryMechanisms = item.Value;
                    else
                        user.DeliveryMechanisms += "," + item.Value;
                }

            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            this.NavigateBack();
        }

        protected void cblRoles_SelectedIndexChanged(object sender, EventArgs e)
        {
            var celebItem = this.cblRoles.Items.FindByText(Role.Celebrity.ToString());
            var groupItem = this.cblRoles.Items.FindByText(Role.Group.ToString());

            foreach (ListItem item in this.cblRoles.Items)
            {
                if (item.Text == Role.Group.ToString())
                {
                    if (item.Selected)
                    {
                        this.ShowGroupList();
                        this.ddlChannels.Visible = false;
                        celebItem.Selected = false;
                        celebItem.Enabled = false;
                        this.lblGroupLabel.Visible = true;
                        this.lblChannelLabel.Visible = false;
                    }
                    else
                    {
                        this.ddlGroups.SelectedValue = "0";
                        this.ddlGroups.Visible = false;
                        this.lblGroupLabel.Visible = false;
                        celebItem.Enabled = true;
                    }

                }
                else if (item.Text == Role.Celebrity.ToString())
                {
                    if (item.Selected)
                    {
                        this.ShowChannelList();
                        this.ddlGroups.Visible = false;

                        groupItem.Selected = false;
                        groupItem.Enabled = false;
                        this.lblChannelLabel.Visible = true;
                        this.lblGroupLabel.Visible = false;
                    }
                    else
                    {
                        this.ddlChannels.SelectedValue = "0";
                        this.ddlChannels.Visible = false;
                        groupItem.Enabled = true;
                        this.lblChannelLabel.Visible = false;
                    }

                }
            }
        }

        protected void ddlCountry_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.BindState();
        }

        protected void tbEmail_TextChanged(object sender, EventArgs e)
        {
            this.ValidateEmailUniqueness();
        }

        protected void rblDeviceTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.ValidateDeviceTypeSelection();
        }

        private long UserID
        {
            get
            {
                return Convert.ToInt64(Request["ID"]);
            }
        }


    }
}