using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Agrishare.Web.Pages.Account
{
    public partial class Listings : System.Web.UI.Page
    {
        public Core.Entities.Category Category;

        protected void Page_Load(object sender, EventArgs e)
        {
            try { Category = Core.Entities.Category.Find(Id: int.Parse(Request.QueryString["cid"])); }
            catch { Category = null; }

            List.RecordCount = Core.Entities.Listing.Count(UserId: Master.CurrentUser.Id, CategoryId: Category?.Id ?? 0);
            List.DataSource = Core.Entities.Listing.List(PageIndex: List.CurrentPageIndex, PageSize: List.PageSize, UserId: Master.CurrentUser.Id, CategoryId: Category?.Id ?? 0);
            List.DataBind();
        }

        public void BindListing(object s, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var listing = (Core.Entities.Listing)e.Item.DataItem;

                ((HyperLink)e.Item.FindControl("Link")).NavigateUrl = $"/account/listing/edit?id={listing.Id}";
                ((Image)e.Item.FindControl("Photo")).ImageUrl = (listing.Photos?.Count ?? 0) > 0 ? $"{Core.Entities.Config.CDNURL}{listing.Photos.FirstOrDefault().ThumbName}" : "";
                ((Literal)e.Item.FindControl("Title")).Text = HttpUtility.HtmlEncode(listing.Title);
                ((Literal)e.Item.FindControl("Description")).Text = HttpUtility.HtmlEncode(listing.Description);
            }
        }
    }
}