using Prvii.Entities;
using Prvii.Entities.DataEntities;
using Prvii.Entities.Enumerations;
using Prvii.ExceptionHandling;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Prvii.Web.AppCode
{
    public class BasePage : System.Web.UI.Page
    {
        protected override void OnLoad(EventArgs e)
        {
            string pageName = VirtualPathUtility.GetFileName(Request.Path);
            List<string> publicPageList = new List<string> { "ErrorPage.aspx", "Default.aspx", "Login.aspx", "UserDetails.aspx", "ChannelList.aspx", "ChannelDetailsView.aspx", "UserPasswordReset.aspx", "MMSChannelMediaURL.aspx" };
            if (pageName.Contains(".aspx") && !publicPageList.Contains(pageName))
                this.CheckSession();

            base.OnLoad(e);

            if (!this.IsPostBack)
            {
                this.ViewState["PrevPage"] = Convert.ToString(Request.UrlReferrer);
            }
        }

        void Page_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();

            // Log the exception.
            ExceptionHandler.HandleException(ex);

            // Clear the error from the server.
            Server.ClearError();

            Response.Redirect("~/ErrorPage.aspx", true);
        }

        protected void UnAuthorizeAccess()
        {
            Response.Redirect("~/ErrorPage.aspx?ErrorID=1", true);
        }

        protected void InvalidURL()
        {
            Response.Redirect("~/ErrorPage.aspx?ErrorID=2", true);
        }

        protected void NavigateBack()
        {
            if( this.ViewState["PrevPage"] != null)
            {
                Response.Redirect(this.ViewState["PrevPage"].ToString(),true);
            }
        }

        public string GetCacheKey()
        {
            return Session.SessionID;
        }

        public string GetCacheKey(string name)
        {
            return name + GetCacheKey();
        }

        public void StoreObject(string sKey, object objectToStore)
        {
            if (objectToStore != null)
            {
                Session[this.GetCacheKey(sKey)] = objectToStore;
            }
            else
            {
                Session.Remove(this.GetCacheKey(sKey));
            }
        }

        public object RetrieveObject(string key)
        {
            return Session[this.GetCacheKey(key)];
        }

        public void RemoveObject(string key)
        {
            Session.Remove(this.GetCacheKey(key));
        }

        public UserProfileData UserSesssion
        {
            get
            {
                return (UserProfileData)this.RetrieveObject("UserSesssion");
            }
            set
            {
                this.StoreObject("UserSesssion", value);
            }
        }

        public void CheckSession()
        {
            string loginPageURL = "~/WebPages/Login.aspx";

            if (!Request.RawUrl.Contains("ErrorPage.aspx"))
                loginPageURL += "?ReturnURL=" + Request.RawUrl;

            if (this.UserSesssion == null)
                Response.Redirect(loginPageURL, true);
        }

        public void BindDropDown(DropDownList ddl, IList list, bool isListItemType = false, string defaultText = "-Select-")
        {
            ddl.DataSource = list;
            if (isListItemType)
            {
                ddl.DataTextField = "Text";
                ddl.DataValueField = "Value";
            }
            else
            {
                ddl.DataTextField = "Name";
                ddl.DataValueField = "ID";
            }
            ddl.DataBind();
            ddl.Items.Insert(0, new ListItem(defaultText, "0"));
        }

        public void BindCheckBoxList(CheckBoxList ddl, IList list, bool isListItemType = false, string defaultText = "-Select-")
        {
            ddl.DataSource = list;
            if (isListItemType)
            {
                ddl.DataTextField = "Text";
                ddl.DataValueField = "Value";
            }
            else
            {
                ddl.DataTextField = "Name";
                ddl.DataValueField = "ID";
            }
            ddl.DataBind();
           // ddl.Items.Insert(0, new ListItem(defaultText, "0"));
        }

        public string GetSelectedValues(ListItemCollection listItems)
        {
            string selectedValue = string.Empty;

            foreach (ListItem item in listItems)
            {
                if (item.Selected)
                {
                    if (selectedValue == string.Empty)
                        selectedValue = item.Value;
                    else
                        selectedValue += "," + item.Value;
                }
            }

            return selectedValue;
        }

        protected List<ListItem> GetEnumList<T>()
        {
            List<ListItem> itemList = new List<ListItem>();

            Type type = typeof(T);

            // validate that T is in fact an enum
            if (!type.IsEnum)
            {
                throw new InvalidOperationException();
            }

            foreach (var value in Enum.GetValues(type))
            {
                short itemValue =  Convert.ToInt16(value);

                itemList.Add(new ListItem { Text = Enum.GetName(type, itemValue), Value = itemValue.ToString() });
            }

            return itemList;
        }

        protected void DisplayMessageBox(string message, UpdatePanel upPage)
        {
            ScriptManager.RegisterStartupScript(upPage, typeof(UpdatePanel), "RefreshParentScript", "alert('" + message + "');", true);
        }


        //protected bool DisplayMessageBox1(string message, UpdatePanel upPage)
        //{
        //    bool result = true;
        //    ScriptManager.RegisterStartupScript(upPage, typeof(UpdatePanel), "RefreshParentScript", "var ok = confirm('" + message + "');result=ok", true);
        //    return result;
        //}


        protected void DisplayMessageBox(string message, UpdatePanel upPage, Control field)
        {
            ScriptManager.RegisterStartupScript(upPage, typeof(UpdatePanel), "RefreshParentScript", "alert('" + message + "');", true);

            if (field != null)
                field.Focus();
        }

        protected int GetGridViewColumnIndex(string sortExpression, GridView gvGrid)
        {
            int i = 0;
            foreach (DataControlField c in gvGrid.Columns)
            {
                if (c.SortExpression == sortExpression)
                    break;
                i++;
            }
            return i;
        }

    }
}