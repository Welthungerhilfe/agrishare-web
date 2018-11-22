using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Agrishare.Web.Pages.Account.Listing
{
    public partial class Tractor : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Master.RequiresAuthentication = true;

            if (!Page.IsPostBack)
            {
                Services.DataSource = Core.Entities.Category.List(ParentId: Core.Entities.Category.TractorsId);
                Services.DataBind();
            }
        }

        public void BindService (object s, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var service = (Core.Entities.Category)e.Item.DataItem;
                ((CheckBox)e.Item.FindControl("Title")).Text = service.Title;
            }
        }

        public void Save(object s, EventArgs e)
        {

        }
    }
}