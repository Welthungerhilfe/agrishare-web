using Agrishare.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Agrishare.Web.Pages.Account
{
    public partial class Profile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Master.RequiresAuthentication = true;

            DisplayName.Text = HttpUtility.HtmlEncode(Master.CurrentUser.FirstName + " " + Master.CurrentUser.LastName);
            DisplayTelephone.Text = HttpUtility.HtmlEncode(Master.CurrentUser.Telephone);
            DisplayEmailAddress.Text = HttpUtility.HtmlEncode(Master.CurrentUser.EmailAddress);

            if (!Page.IsPostBack)
            {
                Gender.Items.Add(new ListItem { Text = $"{Core.Entities.Gender.Male}", Value = ((int)Core.Entities.Gender.Male).ToString() });
                Gender.Items.Add(new ListItem { Text = $"{Core.Entities.Gender.Female}", Value = ((int)Core.Entities.Gender.Female).ToString() });
            }

            switch (Request.QueryString["view"])
            {
                case "edit":
                    EditProfileForm.Visible = true;
                    if (!Page.IsPostBack)
                    {
                        FirstName.Text = Master.CurrentUser.FirstName;
                        LastName.Text = Master.CurrentUser.LastName;
                        EmailAddress.Text = Master.CurrentUser.EmailAddress;
                        Telephone.Text = Master.CurrentUser.Telephone;
                        Gender.SelectedValue = ((int)Master.CurrentUser.GenderId).ToString();
                        DateOfBirth.Text = Master.CurrentUser.DateOfBirth?.ToString("yyyy-MM-dd") ?? "";
                    }
                    break;
                case "notifications":
                    NotificationPreferencesForm.Visible = true;
                    if (!Page.IsPostBack)
                    {
                        SMS.Checked = (Master.CurrentUser.NotificationPreferences & (int)Core.Entities.NotificationPreferences.SMS) > 0;
                        PushNotifications.Checked = (Master.CurrentUser.NotificationPreferences & (int)Core.Entities.NotificationPreferences.PushNotifications) > 0;
                        Email.Checked = (Master.CurrentUser.NotificationPreferences & (int)Core.Entities.NotificationPreferences.Email) > 0;
                    }
                    break;
                case "resetpin":
                    ResetForm.Visible = true;
                    if (!Page.IsPostBack)
                        Master.CurrentUser.SendVerificationCode();
                    break;
                case "delete":
                    DeleteForm.Visible = true;
                    var bookings = Core.Entities.Booking.Count(UserId: Master.CurrentUser.Id, StartDate: DateTime.Now, Upcoming: true);
                    if (bookings > 0)
                        DeleteWarning.Visible = true;
                    var listings = Core.Entities.Listing.List(UserId: Master.CurrentUser.Id);
                    foreach (var listing in listings)
                    {
                        bookings = Core.Entities.Booking.Count(ListingId: listing.Id, StartDate: DateTime.Now, Upcoming: true);
                        if (bookings > 0)
                            DeleteWarning.Visible = true;
                    }
                    DeletePrompt.Visible = !DeleteWarning.Visible;
                    break;
            }

        }

        public void UpdateUser(object s, EventArgs e)
        {
            Master.CurrentUser.FirstName = FirstName.Text;
            Master.CurrentUser.LastName = LastName.Text;
            Master.CurrentUser.EmailAddress = EmailAddress.Text;
            Master.CurrentUser.Telephone = Telephone.Text;
            Master.CurrentUser.GenderId = (Core.Entities.Gender)Enum.ToObject(typeof(Core.Entities.Gender), int.Parse(Gender.SelectedValue));
            Master.CurrentUser.DateOfBirth = DateTime.Parse(DateOfBirth.Text);

            if (!Master.CurrentUser.UniqueTelephone())
                Master.Feedback = "Telephone number has already been registered";
            else if (!Master.CurrentUser.UniqueEmailAddress())
                Master.Feedback = "Email address has already been registered";
            else if (Master.CurrentUser.Save())
            {
                Master.Feedback = "Your details have been updated";
                Response.Redirect("/account/profile");
            }
            else
                Master.Feedback = "An unknown error occured";
        }

        public void UpdateNotificationPreferences(object s, EventArgs e)
        {
            Master.CurrentUser.NotificationPreferences = 0;
            if (SMS.Checked)
                Master.CurrentUser.NotificationPreferences += (int)Core.Entities.NotificationPreferences.SMS;
            if (PushNotifications.Checked)
                Master.CurrentUser.NotificationPreferences += (int)Core.Entities.NotificationPreferences.PushNotifications;
            if (Email.Checked)
                Master.CurrentUser.NotificationPreferences += (int)Core.Entities.NotificationPreferences.Email;

            if (Master.CurrentUser.Save())
            {
                Master.Feedback = "Your notification preferences have been updated";
                Response.Redirect("/account/profile");
            }
            else
                Master.Feedback = "An unknown error occured";
        }

        public void UpdatePassword(object s, EventArgs e)
        {
            if (Page.IsValid)
            {
                var user = Core.Entities.User.Find(Telephone: ResetTelephone.Text);
                if (user.Id > 0 && !user.VerificationCode.IsEmpty() && user.VerificationCode == ResetCode.Text)
                {
                    user.ClearPassword = ResetPIN.Text;
                    user.FailedLoginAttempts = 0;
                    user.AuthToken = Guid.NewGuid().ToString();
                    user.VerificationCode = string.Empty;
                    if (user.Save())
                        Master.Feedback = "Your PIN has been updated";
                    else
                        Master.Feedback = "An unknown error occured";
                }
                else
                    Master.Feedback = "The verification code has expired - please reset your PIN again";
            }
        }

        public void Logout(object s, EventArgs e)
        {
            Master.CurrentUser.AuthToken = string.Empty;
            Master.CurrentUser.Save();
            Master.DropCookie(string.Empty);
            Response.Redirect("/");
        }

        public void DeleteAccount(object s, EventArgs e)
        {
            var bookings = Core.Entities.Booking.Count(UserId: Master.CurrentUser.Id, StartDate: DateTime.Now, Upcoming: true);
            if (bookings > 0)
            {
                Master.Feedback = "Can not delete account - you have upcoming bookings";
                return;
            }
            
            var listings = Core.Entities.Listing.List(UserId: Master.CurrentUser.Id);
            foreach (var listing in listings)
            {
                bookings = Core.Entities.Booking.Count(ListingId: listing.Id, StartDate: DateTime.Now, Upcoming: true);
                if (bookings > 0)
                {
                    Master.Feedback = "Can not delete account - you have upcoming bookings";
                    return;
                }
            }

            foreach (var listing in listings)
                listing.Delete();

            var devices = Core.Entities.Device.List(UserId: Master.CurrentUser.Id);
            foreach (var device in devices)
                device.Delete();

            Master.CurrentUser.AuthToken = string.Empty;
            Master.CurrentUser.Delete();
            Master.DropCookie(string.Empty);
            Response.Redirect("/");
        }
    }
}