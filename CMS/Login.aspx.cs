using Agrishare.Core;
using Agrishare.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Agrishare.CMS
{
    public partial class Login : System.Web.UI.Page
    {
        public DateTime Today = DateTime.Today;

        protected void Page_Load(object sender, EventArgs e)
        {
            //var brad = Core.Entities.User.Find(Id: 10000);
            //brad.ClearPassword = "1234";
            //brad.Roles = new List<Role>
            //{
            //    Role.Administrator,
            //    Role.User
            //};
            //brad.Save();
        }

        public void Authenticate(object s, EventArgs e)
        {
            if (LoginMobileNumber.Text.IsEmpty() || LoginPin.Text.IsEmpty())
            {
                Feedback.InnerText = "Invalid mobile number or PIN.";
                Feedback.Visible = true;
                return;
            }

            var user = Core.Entities.User.Find(Telephone: LoginMobileNumber.Text);
            if (user.Id == 0 || !user.ValidatePassword(LoginPin.Text))
            {
                if (user.Id > 0)
                {
                    user.FailedLoginAttempts += 1;
                    user.Save();
                }

                Feedback.InnerText = "Invalid mobile number or PIN";
                Feedback.Visible = true;
                return;
            }

            user.FailedLoginAttempts = 0;
            if (user.AuthToken.IsEmpty())
                user.AuthToken = Guid.NewGuid().ToString();
            user.Save();

            DropCookie(user.AuthToken);
            Response.Redirect("/#/dashboard");

        }

        public void SendCode(object s, EventArgs e)
        {
            var user = Core.Entities.User.Find(Telephone: ForgotMobileNumber.Text);
            if (user.Id > 0)
                user.SendVerificationCode();

            ResetForm.Visible = true;
            LoginForm.Visible = false;
            ForgotForm.Visible = false;
        }

        public void ResetPin(object s, EventArgs e)
        {
            var user = Core.Entities.User.Find(Telephone: ResetMobileNumber.Text);

            if (user.Id > 0 && user.VerificationCode == ResetSmsCode.Text && user.VerificationCodeExpiry <= DateTime.Now)
            {
                user.ClearPassword = ResetNewPin.Text;
                user.FailedLoginAttempts = 0;
                user.AuthToken = Guid.NewGuid().ToString();
                user.Save();

                DropCookie(user.AuthToken);
                Response.Redirect("/#/dashboard");
            }

            Feedback.InnerText = "Invalid SMS code - please try again";
            Feedback.Visible = true;
        }

        private void DropCookie(string AuthToken)
        {
            var cookie = new HttpCookie(Core.Entities.User.AuthCookieName)
            {
                Value = Core.Utils.Encryption.EncryptWithRC4(AuthToken, Config.EncryptionSalt),
                Expires = DateTime.Now.AddDays(30),
                Path = "/",
                Domain = Config.DomainName
            };
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

    }
}