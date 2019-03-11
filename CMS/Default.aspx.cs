using Agrishare.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Agrishare.CMS
{
    public partial class Default : System.Web.UI.Page
    {
        public DateTime Today = DateTime.Today;

        public User CurrentUser
        {
            get
            {
                if (currentUser == null)
                {
                    try
                    {
                        string encryptedToken = Request.Cookies[Core.Entities.User.AuthCookieName].Value;
                        string authToken = Core.Utils.Encryption.DecryptWithRC4(encryptedToken, Config.EncryptionSalt);
                        currentUser = Core.Entities.User.Find(AuthToken: authToken);
                    }
                    catch
                    {
                        currentUser = new User();
                    }

                    if (currentUser == null)
                        currentUser = new User();
                }

                return currentUser;
            }
        }
        private User currentUser;

        protected override void OnPreRender(EventArgs e)
        {
            var version = Config.Find(Key: "Resource Version")?.Value ?? "1";

            Page.Header.Controls.Add(new Literal { Text = $@"<link rel=""stylesheet"" href=""/styles-{version}.css"" />" });
            Page.Header.Controls.Add(new Literal { Text = $@"<script type=""text/javascript"" src=""/script-{version}.js""></script>" });

            Body.Attributes.Add("ag-api-url", Config.APIURL);
            Body.Attributes.Add("ng-init", $"app.user={CurrentUser.CmsJsonString()}");

            base.OnPreRender(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            var userRoles = CurrentUser.Roles ?? new List<Role>();

            if (Request.QueryString["logout"] == "true")
            {
                CurrentUser.AuthToken = string.Empty;
                CurrentUser.Save();

                currentUser = null;

                var cookie = HttpContext.Current.Request.Cookies[Core.Entities.User.AuthCookieName];
                cookie.Value = string.Empty;
                cookie.Domain = Config.DomainName;
                HttpContext.Current.Response.Cookies.Add(cookie);

                Response.Redirect("/login.aspx");
            }
            else if (!userRoles.Contains(Role.Administrator) && !userRoles.Contains(Role.Dashboard))
                Response.Redirect("/login.aspx");
        }

    }
}