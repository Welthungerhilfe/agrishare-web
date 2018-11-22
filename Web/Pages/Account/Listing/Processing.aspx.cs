using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Agrishare.Web.Pages.Account.Listing
{
    public partial class Processing : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Master.RequiresAuthentication = true;

            if (!Page.IsPostBack)
            {
                Service.Items.Add(new ListItem { Text = "Select...", Value = "" });
                var services = Core.Entities.Category.List(ParentId: Core.Entities.Category.ProcessingId);
                foreach (var service in services)
                    Service.Items.Add(new ListItem { Text = service.Title, Value = $"{service.Id}" });
            }
        }

        public void Save(object s, EventArgs e)
        {

        }
    }
}