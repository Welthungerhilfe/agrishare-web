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
        public string Path => HttpContext.Current.Request.RawUrl;

        public int CurrentPageIndex
        {
            get
            {
                var index = 0;
                try { index = int.Parse(HttpContext.Current.Request.QueryString["pg"]); }
                catch { index = 0; }
                return index;// Math.Min(PageCount - 1, Math.Max(1, index));
            }
        }

        public int PageSize { get; set; } = 25;

        public int RecordCount { get; set; } = 0;

        public int PageCount => Math.Max(1, int.Parse((Math.Ceiling(double.Parse(RecordCount.ToString()) / double.Parse(PageSize.ToString()))).ToString()));

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
