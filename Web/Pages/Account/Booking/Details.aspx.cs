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
            HorsePower.Text = HttpUtility.HtmlEncode(SelectedBooking.Listing.HorsePower);
            Year.Text = HttpUtility.HtmlEncode(SelectedBooking.Listing.Year);

            if (SelectedBooking.StartDate.Date == SelectedBooking.EndDate.Date)
                Dates.Text = SelectedBooking.StartDate.ToString("d MMMM yyyy");
            else if (SelectedBooking.StartDate.ToString("MM/yy") == SelectedBooking.EndDate.ToString("MM/yy"))
                Dates.Text = SelectedBooking.StartDate.Day + " - " + SelectedBooking.EndDate.ToString("d MMMM yyyy");
            else
                Dates.Text = SelectedBooking.StartDate.ToString("d MMMM yyyy") + " - " + SelectedBooking.EndDate.ToString("d MMMM yyyy");

            TransportDistance.Text = SelectedBooking.TransportDistance.ToString("N2") + "km";
            TransportCost.Text = "$" + SelectedBooking.TransportCost.ToString("N2");
            HireSize.Text = SelectedBooking.Quantity + SelectedBooking.Service.QuantityUnit;
            HireCost.Text = "$" + SelectedBooking.HireCost.ToString("N2");
            FuelSize.Text = SelectedBooking.TransportDistance.ToString("N2") + "km";
            FuelCost.Text = "$" + SelectedBooking.FuelCost.ToString("N2");

            CommissionRow.Visible = SelectedBooking.Listing.UserId == Master.CurrentUser.Id;
            Commission.Text = "$" + SelectedBooking.AgriShareCommission.ToString("N2");

            Total.Text = "$" + SelectedBooking.Price.ToString("N2");

        }

        public void BindPhoto(object s, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var photo = (Core.Entities.File)e.Item.DataItem;
                ((HyperLink)e.Item.FindControl("Thumb")).Style.Add("background-image", $"url({Core.Entities.Config.CDNURL}{photo.ZoomName})");
            }
        }
    }
}