using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Agrishare.Web.Pages.Account.Listing
{
    public partial class Reviews : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Master.RequiresAuthentication = true;

            try
            {
                var listing = Core.Entities.Listing.Find(Id: Convert.ToInt32(Request.QueryString["lid"]));
                if (listing.Id == 0)
                    throw new Exception("Listing not found");

                ListingTitle.Text = HttpUtility.HtmlEncode(listing.Title);

                List.RecordCount = Core.Entities.Rating.Count(ListingId: listing.Id);
                List.DataSource = Core.Entities.Rating.List(PageIndex: List.CurrentPageIndex, PageSize: List.PageSize, Sort: "ID DESC", ListingId: listing.Id);
                List.DataBind();
            }
            catch
            {
                Master.Feedback = "Listing not found";
                Response.Redirect("/account/seeking");
            }
        }

        public void BindReview(object s, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var review = (Core.Entities.Rating)e.Item.DataItem;
                ((HtmlControl)e.Item.FindControl("Stars")).Attributes.Add("class", $"stars-{(int)review.Stars}");
                ((Literal)e.Item.FindControl("User")).Text = HttpUtility.HtmlEncode(review.User.FirstName);
                ((Literal)e.Item.FindControl("Comments")).Text = HttpUtility.HtmlEncode(review.Comments);
                ((Literal)e.Item.FindControl("Date")).Text = review.DateCreated.ToString("d MMMM yyyy");
            }
        }
    }
}