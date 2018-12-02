using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Agrishare.Web.Pages.Account.Seeking
{
    public partial class Lorry : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Master.RequiresAuthentication = true;
            Master.Body.Attributes["class"] += " account ";
            Master.SelectedUrl = "/account/seeking";

            if (!Page.IsPostBack)
            {
                For.Items.Add(new ListItem { Text = "Me", Value = ((int)Core.Entities.BookingFor.Me).ToString() });
                For.Items.Add(new ListItem { Text = "A friend", Value = ((int)Core.Entities.BookingFor.Friend).ToString() });
                For.Items.Add(new ListItem { Text = "A group", Value = ((int)Core.Entities.BookingFor.Group).ToString() });
            }
        }

        public void FindListings(object s, EventArgs e)
        {
            var url = $"/account/seeking/results?cid={Core.Entities.Category.LorriesId}&" +
                $"sid={Core.Entities.Category.LorriesServiceId}&" +
                $"lat={PickupLocation.Latitude}&" +
                $"lng={PickupLocation.Longitude}&" +
                $"std={StartDate.Text}&" +
                $"qty=0&" +
                $"fue=1&" +
                $"mob=1&" +
                $"for={For.SelectedValue}&" +
                $"dla={DropoffLocation.Latitude}&" +
                $"dlo={DropoffLocation.Longitude}&" +
                $"vol={LoadWeight.Text}&" +
                $"des={HttpUtility.UrlEncode(LoadDescription.Text)}";

            Response.Redirect(url);
        }
    }
}