using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Agrishare.Web.Pages.Account.Seeking
{
    public partial class Search : System.Web.UI.Page
    {
        public Core.Entities.Category Category;

        protected void Page_Load(object sender, EventArgs e)
        {
            try { Category = Core.Entities.Category.Find(Id: int.Parse(Request.QueryString["cid"])); }
            catch { Category = null; }

            if (!Page.IsPostBack)
            {
                For.Items.Add(new ListItem { Text = "Me", Value = ((int)Core.Entities.BookingFor.Me).ToString() });
                For.Items.Add(new ListItem { Text = "A friend", Value = ((int)Core.Entities.BookingFor.Friend).ToString() });
                For.Items.Add(new ListItem { Text = "A group", Value = ((int)Core.Entities.BookingFor.Group).ToString() });

                var services = Core.Entities.Category.List(ParentId: Category.Id);
                foreach (var service in services)
                    Services.Items.Add(new ListItem { Text = service.Title, Value = service.Id.ToString() });

                SearchForm.Visible = true;
                SearchResults.Visible = false;
            }
        }

        public void FindListings(object s, EventArgs e)
        {
            SearchForm.Visible = true;
            SearchResults.Visible = false;

            Core.Entities.Counter.Hit(Master.CurrentUser.Id, Core.Entities.Counters.Search, int.Parse(Services.SelectedValue));

            var categoryId = Category.Id;
            var serviceId = Convert.ToInt32(Services.SelectedValue);
            var latitude = Convert.ToDecimal(Latitude.Value);
            var longitude = Convert.ToDecimal(Longitude.Value);
            var startDate = Convert.ToDateTime(StartDate.Text);
            var size = 0;
            switch (Category.Id)
            {
                case Core.Entities.Category.TractorsId:
                    size = Convert.ToInt32(FieldSize.Text);
                    break;
                case Core.Entities.Category.ProcessingId:
                    size = Convert.ToInt32(NumberOfBags.Text);
                    break;
            }
            var includeFuel = Fuel.SelectedValue.Equals("True");
            var mobile = true;
            if (Category.Id == Core.Entities.Category.ProcessingId)
                mobile = Mobile.SelectedValue.Equals("True");
            var bookingFor = (Core.Entities.BookingFor)Enum.ToObject(typeof(Core.Entities.BookingFor), Convert.ToInt32(For.SelectedValue));
            var destinationLatitude = Convert.ToDecimal(DestinationLatitude.Value);
            var destinationLongitude = Convert.ToDecimal(DestinationLongitude.Value);
            var totalVolume = Convert.ToDecimal(LoadWeight.Text);

            SearchResults.RecordCount = Core.Entities.ListingSearchResult.Count(categoryId, serviceId, latitude, longitude, startDate, size, includeFuel, mobile, bookingFor, destinationLatitude, destinationLongitude, totalVolume);
            SearchResults.DataSource = Core.Entities.ListingSearchResult.List(SearchResults.CurrentPageIndex, SearchResults.PageSize, "Distance", categoryId, serviceId, latitude, longitude, startDate, size, includeFuel, mobile, bookingFor, destinationLatitude, destinationLongitude, totalVolume);
        }

        public void BindSearchResult(object s, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var result = (Core.Entities.ListingSearchResult)e.Item.DataItem;
                ((Image)e.Item.FindControl("Photo")).ImageUrl = result.Photos.Count > 0 ? $"{Core.Entities.Config.CDNURL}{result.Photos.FirstOrDefault().ThumbName}" : "";
                ((Literal)e.Item.FindControl("Distance")).Text = $"{result.Distance}kms away";
                ((Literal)e.Item.FindControl("Title")).Text = HttpUtility.HtmlEncode(result.Title);
                ((Literal)e.Item.FindControl("Year")).Text = HttpUtility.HtmlEncode(result.Year);
                ((Literal)e.Item.FindControl("Price")).Text = "$" + result.Price.ToString("N2");
                ((Literal)e.Item.FindControl("Status")).Text = result.Available ? "Available" : "Not available";
            }
        }
    }
}