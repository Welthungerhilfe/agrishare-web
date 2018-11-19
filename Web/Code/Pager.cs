/* Title: Gloo Framework
 * Author: Bradley Searle (C2 Digital)
 * Source: www.c2.co.zw
 * License: CC BY 4.0 (https://creativecommons.org/licenses/by/4.0/legalcode) */

using Agrishare.Core;
using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Agrishare.Web.Controls
{
    public class Pager : WebControl
    {
        public string Path => HttpContext.Current.Request.Path;

        public int CurrentPageIndex
        {
            get
            {
                if (currentPageIndex == -1)
                {
                    try { currentPageIndex = int.Parse(HttpContext.Current.Request.QueryString["pg"]); }
                    catch { currentPageIndex = 0; }
                }
                return currentPageIndex < 0 ? 0 : currentPageIndex > PageCount - 1 ? PageCount - 1 : currentPageIndex;
            }
            set
            {
                currentPageIndex = value;
            }
        }
        private int currentPageIndex = -1;

        public int PageSize
        {
            get
            {
                if (pageSize == -1)
                {
                    try { pageSize = int.Parse(HttpContext.Current.Request.QueryString["sz"]); }
                    catch { pageSize = 50; }
                }
                return Math.Max(pageSize, 1);
            }
            set
            {
                pageSize = Math.Max(0, value);
            }
        }
        private int pageSize = 0;

        public int RecordCount
        {
            get { return recordCount; }
            set
            {
                if (value >= 0) recordCount = value;
                pageCount = int.Parse((Math.Ceiling(double.Parse(recordCount.ToString()) / double.Parse(PageSize.ToString()))).ToString());
            }
        }
        private int recordCount = 0;

        public int PageCount
        {
            get
            {
                return pageCount < 1 ? 1 : pageCount;
            }
        }
        private int pageCount = 0;

        protected override void Render(HtmlTextWriter writer)
        {
            // Base method
            base.AddAttributesToRender(writer);

            // Build links
            string previousUrl = CurrentPageIndex > 0 ? Path.AddQueryStringVariable($"pg={CurrentPageIndex - 1}") : "";
            string nextUrl = CurrentPageIndex < PageCount - 1 ? Path.AddQueryStringVariable($"pg={CurrentPageIndex + 1}") : "";
            
            // Build HTML
            // **********

            // Start DIV
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "paging");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            // Previous anchor
            if (previousUrl == "") writer.WriteLine("<a class=\"previous disabled\" title=\"No previous pages\">Previous</a>");
            else writer.WriteLine($"<a href=\"{previousUrl}\" class=\"previous\" title=\"Previous page\">Previous</a>");
            
            // Setup more links
            var _startIndex = CurrentPageIndex - 2;
            if (_startIndex < 0) _startIndex = 0;
            if (PageCount > 5 && PageCount - _startIndex < 5) _startIndex = PageCount - 5;

            writer.WriteLine("<div class=\"more\">");

            if (_startIndex > 0)
                writer.WriteLine($@"<a href=""{Path.AddQueryStringVariable("pg=0")}"">1</a>");

            for (var i = _startIndex; i < PageCount && i < _startIndex + 5; i++)
            {
                var selected = CurrentPageIndex == i ? "class=\"selected\"" : "";
                writer.WriteLine($@"<a href=""{Path.AddQueryStringVariable("pg="+i)}"" {selected}>{i + 1}</a>");
            }

            writer.WriteLine("</div>");

            // Next anchor
            if (nextUrl == "") writer.WriteLine("<a class=\"next disabled\" title=\"No next pages\">Next</a>");
            else writer.WriteLine($"<a href=\"{nextUrl}\" class=\"next\" title=\"Next page\">Next</a>");

            // Close tag         
            writer.RenderEndTag();
        }
    }
}
