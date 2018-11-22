using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Agrishare.Web.Pages.Account.Seeking
{
    public partial class Processing : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Master.RequiresAuthentication = true;

            if (!Page.IsPostBack)
            {
                For.Items.Add(new ListItem { Text = "Me", Value = ((int)Core.Entities.BookingFor.Me).ToString() });
                For.Items.Add(new ListItem { Text = "A friend", Value = ((int)Core.Entities.BookingFor.Friend).ToString() });
                For.Items.Add(new ListItem { Text = "A group", Value = ((int)Core.Entities.BookingFor.Group).ToString() });

                var services = Core.Entities.Category.List(ParentId: Core.Entities.Category.ProcessingId);
                foreach (var service in services)
                    Services.Items.Add(new ListItem { Text = service.Title, Value = service.Id.ToString() });
            }
        }

        public void FindListings(object s, EventArgs e)
        {
            var url = $"/account/seeking/results?cid={Core.Entities.Category.ProcessingId}&" +
                $"sid={Services.SelectedValue}&" +
                $"lat={Location.Latitude}&" +
                $"lng={Location.Longitude}&" +
                $"std={StartDate.Text}&" +
                $"qty={NumberOfBags.Text}&" +
                $"fue=1&" +
                $"mob={Mobile.SelectedValue}&" +
                $"for={For.SelectedValue}&" +
                $"dla=0&" +
                $"dlo=0&" +
                $"vol=0&" +
                $"des=";

            Response.Redirect(url);
        }
    }
}