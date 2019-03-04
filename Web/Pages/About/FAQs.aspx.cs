using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Agrishare.Web.Pages.About
{
    public partial class FAQs : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            List.DataSource = Core.Entities.Faq.List();
            List.DataBind();
        }

        public void BindFaq(object s, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var faq = (Core.Entities.Faq)e.Item.DataItem;
                ((Literal)e.Item.FindControl("Question")).Text = HttpUtility.HtmlEncode(faq.Question);
                ((Literal)e.Item.FindControl("Answer")).Text = HttpUtility.HtmlEncode(faq.Answer);
            }
        }
    }
}