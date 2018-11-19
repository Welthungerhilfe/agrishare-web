using Agrishare.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Agrishare.Web.Pages.Account.Offering
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Notifications.DataSource = Core.Entities.Notification.List(PageSize: 10, UserId: Master.CurrentUser.Id, GroupId: Core.Entities.NotificationGroup.Offering);
            Notifications.DataBind();

            Bookings.DataSource = Core.Entities.Booking.List(PageSize: 10, SupplierId: Master.CurrentUser.Id);
            Bookings.DataBind();

            var startDate = DateTime.Today.StartOfDay().AddDays(-(DateTime.Today.Day - 1));
            MonthSummary.Text = "$" + Core.Entities.Booking.OfferingSummary(Master.CurrentUser.Id, startDate);
            AllTimeSummary.Text = "$" + Core.Entities.Booking.OfferingSummary(Master.CurrentUser.Id);
        }

        public void BindNotification(object s, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var notification = (Core.Entities.Notification)e.Item.DataItem;
                var actionText = "";
                var actionLink = "";
                switch (notification.TypeId)
                {
                    case Core.Entities.NotificationType.BookingCancelled:
                        break;
                    case Core.Entities.NotificationType.BookingConfirmed:
                        break;
                    case Core.Entities.NotificationType.NewBooking:
                        actionText = "View";
                        actionLink = $"/account/booking/details?id={notification.BookingId}#confirm";
                        break;
                    case Core.Entities.NotificationType.NewReview:
                        actionText = "View";
                        actionLink = $"/account/booking/details?id={notification.BookingId}#review";
                        break;
                    case Core.Entities.NotificationType.PaymentReceived:
                        break;
                    case Core.Entities.NotificationType.ServiceComplete:
                        break;
                    case Core.Entities.NotificationType.ServiceIncomplete:
                        break;
                }

                ((HyperLink)e.Item.FindControl("Link")).NavigateUrl = "";
                ((Image)e.Item.FindControl("Photo")).ImageUrl = notification.Booking.Listing.Photos.Count > 0 ? $"{Core.Entities.Config.CDNURL}{notification.Booking.Listing.Photos.FirstOrDefault().ThumbName}" : "";
                ((Literal)e.Item.FindControl("Date")).Text = notification.Booking.StartDate.ToString("d MMMM yyyy");
                ((Literal)e.Item.FindControl("Title")).Text = HttpUtility.HtmlEncode(notification.Booking.Listing.Title);
                ((Literal)e.Item.FindControl("Message")).Text = HttpUtility.HtmlEncode(notification.Message);
                ((Literal)e.Item.FindControl("TimeAgo")).Text = notification.DateCreated.TimeAgo();
                ((HyperLink)e.Item.FindControl("Action")).Text = actionText;
                ((HyperLink)e.Item.FindControl("Action")).NavigateUrl = actionLink;
                ((HyperLink)e.Item.FindControl("Action")).Visible = !actionLink.IsEmpty();
            }
        }

        public void BindBooking(object s, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var booking = (Core.Entities.Booking)e.Item.DataItem;
                ((HyperLink)e.Item.FindControl("Link")).NavigateUrl = $"/account/booking/details?id={booking.Id}";
                ((Image)e.Item.FindControl("Photo")).ImageUrl = booking.Listing.Photos.Count > 0 ? $"{Core.Entities.Config.CDNURL}{booking.Listing.Photos.FirstOrDefault().ThumbName}" : "";
                ((Literal)e.Item.FindControl("Date")).Text = booking.StartDate.ToString("d MMMM yyyy");
                ((Literal)e.Item.FindControl("Title")).Text = HttpUtility.HtmlEncode(booking.Listing.Title);
                ((Literal)e.Item.FindControl("Price")).Text = "$" + booking.Price.ToString("N2");
            }
        }
    }
}