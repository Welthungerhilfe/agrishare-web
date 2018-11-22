using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Agrishare.Web.Pages.Account.Booking
{
    public partial class Details : System.Web.UI.Page
    {
        private Core.Entities.Booking SelectedBooking;

        protected void Page_Load(object sender, EventArgs e)
        {
            Master.RequiresAuthentication = true;

            try { SelectedBooking = Core.Entities.Booking.Find(Id: Convert.ToInt32(Request.QueryString["id"])); }
            catch { SelectedBooking = null; }

            if (SelectedBooking == null || SelectedBooking.UserId != Master.CurrentUser.Id || SelectedBooking.Listing.UserId != Master.CurrentUser.Id)
            {
                Master.Feedback = "Booking not found";
                Response.Redirect("/account/bookings");
            }

            BookingTitle.Text = HttpUtility.HtmlEncode(SelectedBooking.Listing.Title);
            Gallery.DataSource = SelectedBooking.Listing.Photos;
            Gallery.DataBind();
            Description.Text = HttpUtility.HtmlEncode(SelectedBooking.Listing.Description);
            Brand.Text = HttpUtility.HtmlEncode(SelectedBooking.Listing.Brand);
            Year.Text = HttpUtility.HtmlEncode(SelectedBooking.Listing.Year);

            if (SelectedBooking.StartDate.Date == SelectedBooking.EndDate.Date)
                Dates.Text = SelectedBooking.StartDate.ToString("d MMMM yyyy");
            else if (SelectedBooking.StartDate.ToString("MM/yy") == SelectedBooking.EndDate.ToString("MM/yy"))
                Dates.Text = SelectedBooking.StartDate.Day + " - " + SelectedBooking.EndDate.ToString("d MMMM yyyy");
            else
                Dates.Text = SelectedBooking.StartDate.ToString("d MMMM yyyy") + " - " + SelectedBooking.EndDate.ToString("d MMMM yyyy");

            //TODO finish detail screen
        }
    }
}