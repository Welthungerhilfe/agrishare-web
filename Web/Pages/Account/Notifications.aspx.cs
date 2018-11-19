using Agrishare.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Agrishare.Web.Pages.Account
{
    public partial class Notifications : System.Web.UI.Page
    {
        public Core.Entities.NotificationGroup Group => (Core.Entities.NotificationGroup)Enum.Parse(typeof(Core.Entities.NotificationGroup), Request.QueryString["view"], true);

        protected void Page_Load(object sender, EventArgs e)
        {
            List.RecordCount = Core.Entities.Notification.Count(UserId: Master.CurrentUser.Id, GroupId: Group);
            List.DataSource = Core.Entities.Notification.List(PageIndex: List.CurrentPageIndex, PageSize: List.PageSize, UserId: Master.CurrentUser.Id, GroupId: Group);
            List.DataBind();
        }

        public void BindNotification(object s, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var notification = (Core.Entities.Notification)e.Item.DataItem;
                var actionText = "";
                var actionLink = "";

                if (Group == Core.Entities.NotificationGroup.Offering)
                {
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
                }
                else
                {
                    switch (notification.TypeId)
                    {
                        case Core.Entities.NotificationType.BookingCancelled:
                            actionText = "View";
                            actionLink = $"/account/bookings/details?id={notification.BookingId}";
                            break;
                        case Core.Entities.NotificationType.BookingConfirmed:
                            actionText = "Pay Now";
                            actionLink = $"/account/bookings/details?id={notification.BookingId}#payment";
                            break;
                        case Core.Entities.NotificationType.NewBooking:
                            break;
                        case Core.Entities.NotificationType.NewReview:
                            break;
                        case Core.Entities.NotificationType.PaymentReceived:
                            break;
                        case Core.Entities.NotificationType.ServiceComplete:
                            actionText = "View";
                            actionLink = $"/account/bookings/details?id={notification.BookingId}#rating";
                            break;
                        case Core.Entities.NotificationType.ServiceIncomplete:
                            break;
                    }
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
    }
}