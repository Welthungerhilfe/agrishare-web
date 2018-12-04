using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Agrishare.Web.Pages.Account
{
    public partial class Listings : System.Web.UI.Page
    {
        Core.Entities.Category Category;

        string Type
        {
            get
            {
                switch (Category.Id)
                {
                    case Core.Entities.Category.LorriesId:
                        return "lorry";
                    case Core.Entities.Category.TractorsId:
                        return "tractor";
                    case Core.Entities.Category.ProcessingId:
                        return "processing";
                }
                return string.Empty;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Master.RequiresAuthentication = true;
            Master.SelectedUrl = "/account/offering";
            Master.Body.Attributes["class"] += " account ";

            try { Category = Core.Entities.Category.Find(Id: int.Parse(Request.QueryString["cid"])); }
            catch { Category = Core.Entities.Category.Find(Id: Core.Entities.Category.LorriesId); }

            switch (Category.Id)
            {
                case Core.Entities.Category.LorriesId:
                    ListTitle.Text = "Lorries"; break;
                case Core.Entities.Category.TractorsId:
                    ListTitle.Text = "Tractors"; break;
                case Core.Entities.Category.ProcessingId:
                    ListTitle.Text = "Processing"; break;
            }

            List.RecordCount = Core.Entities.Listing.Count(UserId: Master.CurrentUser.Id, CategoryId: Category?.Id ?? 0);
            List.DataSource = Core.Entities.Listing.List(PageIndex: List.CurrentPageIndex, PageSize: List.PageSize, UserId: Master.CurrentUser.Id, CategoryId: Category?.Id ?? 0);
            List.DataBind();

            AddButton.Text = $"Add {Type}";
            AddButton.NavigateUrl = $"/account/listing/{Type}";
        }

        public void BindListing(object s, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var listing = (Core.Entities.Listing)e.Item.DataItem;
                ((HyperLink)e.Item.FindControl("Link")).NavigateUrl = $"/account/listing/{Type}?id={listing.Id}";
                if ((listing.Photos?.Count ?? 0) > 0)
                    ((HtmlContainerControl)e.Item.FindControl("Photo")).Style.Add("background-image", $"url({Core.Entities.Config.CDNURL}/{listing.Photos.FirstOrDefault().ThumbName})");
                ((Literal)e.Item.FindControl("Title")).Text = HttpUtility.HtmlEncode(listing.Title);
                ((Literal)e.Item.FindControl("Description")).Text = HttpUtility.HtmlEncode(listing.Description);
            }
        }
    }
}