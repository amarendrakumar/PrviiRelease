using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Prvii.Web.AppCode;
using Prvii.Business;

namespace Prvii.Web.WebPages.Groups
{
    public partial class GroupList : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.gvGroupList.DataSource = GroupManager.GetAll();
            this.gvGroupList.DataBind();
        }
    }
}