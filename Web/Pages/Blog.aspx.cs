using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Agrishare.Web.Pages
{
    public partial class Blog : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Master.SelectedUrl = "/blog";

            if (Regex.IsMatch(Request.RawUrl, @"^/blog/[\d]+/.+$"))
            {
                var blog = Core.Entities.Blog.Find(UrlPath: Request.RawUrl);
                if (blog == null)
                    Response.Redirect("/blog");

                Title = blog.Title;
                PageTitle.Text = HttpUtility.HtmlEncode(blog.Title);
                BlogDate.Text = blog.DateCreated.ToString("d MMMM yyyy");
                BlogPhoto.ImageUrl = $"{Core.Entities.Config.CDNURL}/{blog.Photo.ZoomName}";
                BlogContent.Text = blog.Content;

                BlogDetail.Visible = true;
                Introduction.Visible = false;

                BlogLinks.DataSource = Core.Entities.Blog.List(PageSize: 10);
                BlogLinks.DataBind();
            }
            else
            {
                BlogList.Visible = true;
                BlogLinks.Visible = false;

                BlogList.RecordCount = Core.Entities.Blog.Count();
                BlogList.DataSource = Core.Entities.Blog.List(PageIndex: BlogList.CurrentPageIndex, PageSize: BlogList.PageSize);
                BlogList.DataBind();
            }
        }

        public void BindBlog(object s, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var blog = (Core.Entities.Blog)e.Item.DataItem;
                ((HyperLink)e.Item.FindControl("Link")).NavigateUrl = blog.UrlPath;
                ((HyperLink)e.Item.FindControl("Link")).Style.Add("background-image", $"url({Core.Entities.Config.CDNURL}/{blog.Photo?.ZoomName})");
                ((Literal)e.Item.FindControl("Title")).Text = HttpUtility.HtmlEncode(blog.Title);
                ((Literal)e.Item.FindControl("Date")).Text = blog.DateCreated.ToString("d MMMM yyyy");
            }
        }

        public void BindLink(object s, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var blog = (Core.Entities.Blog)e.Item.DataItem;
                ((HyperLink)e.Item.FindControl("Link")).NavigateUrl = blog.UrlPath;
                ((HyperLink)e.Item.FindControl("Link")).Text = HttpUtility.HtmlEncode(blog.Title);
            }
        }
    }
}