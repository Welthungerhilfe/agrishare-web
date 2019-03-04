using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Agrishare.Web.Pages.About
{
    public partial class Contact : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public void SendMessage(object s, EventArgs e)
        {
            var template = Core.Entities.Template.Find(Title: "Contact");
            template.Replace("Name", Name.Text);
            template.Replace("Email Address", EmailAddress.Text);
            template.Replace("Telephone", Telephone.Text);
            template.Replace("Message", Message.Text);

            new Core.Entities.Email
            {
                Message = template.EmailHtml(),
                RecipientEmail = Core.Entities.Config.ApplicationEmailAddress,
                SenderEmail = EmailAddress.Text,
                Subject = "Contact from website"
            }.Send();

            Master.Feedback = "Your message has been sent";
        }
    }
}