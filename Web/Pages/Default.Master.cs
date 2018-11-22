using Agrishare.Core;
using Agrishare.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Agrishare.Web.Pages
{
    public partial class Default : System.Web.UI.MasterPage
    {
        public User CurrentUser
        {
            get
            {
                if ((currentUser == null || currentUser.Id == 0) && Request.Cookies[User.AuthCookieName] != null)
                {
                    var authToken = Request.Cookies[User.AuthCookieName].Value;
                    if (!authToken.IsEmpty())
                        currentUser = User.Find(AuthToken: authToken);
                }
                return currentUser ?? new User();
            }
            set
            {
                currentUser = value;
            }
        }
        private User currentUser;

        public string Feedback
        {
            get
            {
                return Session["Feedback"]?.ToString() ?? string.Empty;                    
            }
            set
            {
                Session.Add("Feedback", value);
            }
        }

        public bool RequiresAuthentication { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            var analyticsId = Config.Find(Key: "Google Analytics Tracking ID").Value;
            var domainName = Config.Find(Key: "Domain Name").Value;
            if (!analyticsId.IsEmpty())
                Page.Header.Controls.Add(new Literal
                {
                    Text = $@"<script>" +
                            $@"(function(i,s,o,g,r,a,m){{i['GoogleAnalyticsObject']=r;i[r]=i[r]||function(){{" +
                            $@"(i[r].q=i[r].q||[]).push(arguments)}},i[r].l=1*new Date();a=s.createElement(o)," +
                            $@"m=s.getElementsByTagName(o)[0];a.async=1;a.src=g;m.parentNode.insertBefore(a,m)" +
                            $@"}})(window,document,'script','//www.google-analytics.com/analytics.js','ga');" +
                            $@"ga('create', '{analyticsId}', '{domainName}');" +
                            $@"ga('send', 'pageview');" +
                            $@"</script>"
                });

            var version = Config.Find(Key: "Resource Version")?.Value ?? "1";
            Page.Header.Controls.Add(new Literal { Text = $@"<link rel=""stylesheet"" href=""/styles-{version}.css"" />" });
            Page.Header.Controls.Add(new Literal { Text = $@"<script type=""text/javascript"" src=""/script-{version}.js""></script>" });
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (!Feedback.IsEmpty())
            {
                FeedbackPrompt.Visible = true;
                FeedbackMessage.Text = Feedback;
                Feedback = string.Empty;
            }

            if (RequiresAuthentication && !CurrentUser.Roles.Contains(Role.User))
                Response.Redirect("/account/get-started?r=" + HttpUtility.UrlEncode(Request.Path));

            GuestMenu.Visible = !CurrentUser.Roles.Contains(Role.User);
            UserMenu.Visible = CurrentUser.Roles.Contains(Role.User);
            UserName.Text = HttpUtility.HtmlEncode(CurrentUser.FirstName);

            Page.Title = $"{Page.Title} - {Config.ApplicationName}";

            base.OnPreRender(e);
        }

        public void DropCookie(string AuthToken)
        {
            var cookie = new HttpCookie(User.AuthCookieName)
            {
                Value = AuthToken,
                Expires = DateTime.Now.AddDays(30),
                Path = "/"
            };
            HttpContext.Current.Response.Cookies.Add(cookie);
        }
    }
}