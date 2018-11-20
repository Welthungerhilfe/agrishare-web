using Agrishare.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Agrishare.Web.Pages.Account
{
    public partial class Bookings : System.Web.UI.Page
    {
        private Core.Entities.NotificationGroup Group => (Core.Entities.NotificationGroup)Enum.Parse(typeof(Core.Entities.NotificationGroup), Request.QueryString["view"], true);
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Group == Core.Entities.NotificationGroup.Offering)
            {
                List.RecordCount = Core.Entities.Booking.Count(SupplierId: Master.CurrentUser.Id);
                List.DataSource = Core.Entities.Booking.List(PageSize: 10, SupplierId: Master.CurrentUser.Id);
            }
            else
            {
                List.RecordCount = Core.Entities.Booking.Count(UserId: Master.CurrentUser.Id);
                List.DataSource = Core.Entities.Booking.List(PageSize: 10, UserId: Master.CurrentUser.Id);
            }
            List.DataBind();

            var startDate = DateTime.Today.StartOfDay().AddDays(-(DateTime.Today.Day - 1));
            MonthSummary.Text = "$" + Core.Entities.Booking.OfferingSummary(Master.CurrentUser.Id, startDate);
            AllTimeSummary.Text = "$" + Core.Entities.Booking.OfferingSummary(Master.CurrentUser.Id);
        }

        public void BindBooking(object s, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var booking = (Core.Entities.Booking)e.Item.DataItem;
                ((HyperLink)e.Item.FindControl("Link")).NavigateUrl = $"/account/booking/details?id={booking.Id}";
                ((Image)e.Item.FindControl("Photo")).ImageUrl = (booking.Listing.Photos?.Count ?? 0) > 0 ? $"{Core.Entities.Config.CDNURL}{booking.Listing.Photos.FirstOrDefault().ThumbName}" : "";
                ((Literal)e.Item.FindControl("Date")).Text = booking.StartDate.ToString("d MMMM yyyy");
                ((Literal)e.Item.FindControl("Title")).Text = HttpUtility.HtmlEncode(booking.Listing.Title);
                ((Literal)e.Item.FindControl("Price")).Text = "$" + booking.Price.ToString("N2");
            }
        }
    }
}