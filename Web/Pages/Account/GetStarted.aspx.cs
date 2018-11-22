using Agrishare.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Agrishare.Web.Pages.Account
{
    public partial class GetStarted : System.Web.UI.Page
    {
        private string DashboardUrl = "/account/seeking";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["view"]?.Equals("forgot") ?? false)
            {
                ForgotForm.Visible = true;
            }
            else if (!Page.IsPostBack)
            {
                RegisterForm.Visible = true;
                LoginForm.Visible = true;

                Gender.Items.Add(new ListItem { Text = $"{Core.Entities.Gender.Male}", Value = Core.Entities.Gender.Male.ToString() });
                Gender.Items.Add(new ListItem { Text = $"{Core.Entities.Gender.Female}", Value = Core.Entities.Gender.Female.ToString() });
            }
        }

        public void RegisterUser(object s, EventArgs e)
        {
            if (Page.IsValid)
            {
                var user = new Core.Entities.User
                {
                    FirstName = FirstName.Text,
                    LastName = LastName.Text,
                    EmailAddress = EmailAddress.Text,
                    Telephone = Telephone.Text,
                    ClearPassword = PIN.Text,
                    GenderId = (Core.Entities.Gender)Enum.ToObject(typeof(Core.Entities.Gender), int.Parse(Gender.SelectedValue)),
                    DateOfBirth = DateTime.Parse(DateOfBirth.Text),
                    Roles = new List<Core.Entities.Role> { Core.Entities.Role.User },
                    AuthToken = Guid.NewGuid().ToString(),
                    StatusId = Core.Entities.UserStatus.Verified
                };
                user.Save();
                Master.DropCookie(user.AuthToken);
                Master.Feedback = "Your account has been created and you are now logged in.";
                Response.Redirect(Request.QueryString["r"] ?? DashboardUrl);
            }
        }

        public void AuthenticateUser(object s, EventArgs e)
        {
            var maxFailedLoginAttempts = 5;
            if (Page.IsValid)
            {
                var user = Core.Entities.User.Find(Telephone: LoginTelephone.Text);
                if (user.Id > 0)
                {
                    if (user.FailedLoginAttempts > maxFailedLoginAttempts)
                    {
                        Master.Feedback = "Your account has been locked - please reset your password";
                    }
                    else if (user.ValidatePassword(LoginPIN.Text))
                    {
                        user.FailedLoginAttempts = 0;
                        user.AuthToken = Guid.NewGuid().ToString();
                        user.Save();
                        Master.DropCookie(user.AuthToken);
                        Response.Redirect(Request.QueryString["r"] ?? DashboardUrl);
                    }
                    else
                    {
                        user.FailedLoginAttempts += 1;
                        user.Save();
                        Master.Feedback = "Telephone or PIN not recognised";
                    }
                }
                else
                {
                    Master.Feedback = "Telephone or PIN not recognised";
                }
            }
        }

        public void SendResetCode(object s, EventArgs e)
        {
            if (Page.IsValid)
            {
                var user = Core.Entities.User.Find(Telephone: ForgotTelephone.Text);
                if (user.Id > 0)
                    user.SendVerificationCode();
                ResetForm.Visible = true;
                ForgotForm.Visible = false;
            }
        }

        public void UpdatePassword(object s, EventArgs e)
        {
            if (Page.IsValid)
            {
                var user = Core.Entities.User.Find(Telephone: ResetTelephone.Text);
                if(user.Id > 0 && !user.VerificationCode.IsEmpty() && user.VerificationCode == ResetCode.Text)
                {
                    user.ClearPassword = ResetPIN.Text;
                    user.FailedLoginAttempts = 0;
                    user.AuthToken = Guid.NewGuid().ToString();
                    user.VerificationCode = string.Empty;
                    user.Save();
                    Master.DropCookie(user.AuthToken);
                    Response.Redirect(Request.QueryString["r"] ?? DashboardUrl);
                }
                else
                    Master.Feedback = "The verification code has expired - please reset your PIN again";
            }
        }
    }
}