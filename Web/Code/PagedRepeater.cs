/* Title: Gloo Framework
 * Author: Bradley Searle (C2 Digital)
 * Source: www.c2.co.zw
 * License: CC BY 4.0 (https://creativecommons.org/licenses/by/4.0/legalcode) */
 
using System;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Agrishare.Web.Controls
{
    /// <summary>
    /// Repeater with Pager controler in footer
    /// </summary>
    public class PagedRepeater : Repeater
    {
        Pager Pager = new Pager();

        /// <summary>
        /// Current page index
        /// </summary>
        public int CurrentPageIndex
        {
            get
            {
                return Pager.CurrentPageIndex;
            }
            set
            {
                Pager.CurrentPageIndex = value;
            }
        }

        /// <summary>
        /// Page size
        /// </summary>
        public int PageSize
        {
            get
            {
                return Pager.PageSize;
            }
            set
            {
                Pager.PageSize = value;
            }
        }

        /// <summary>
        /// Record count
        /// </summary>
        public int RecordCount
        {
            get
            {
                return Pager.RecordCount;
            }
            set
            {
                Pager.RecordCount = value;
            }
        }

        /// <summary>
        /// Page count
        /// </summary>
        public int PageCount
        {
            get
            {
                return Pager.PageCount;
            }
        }

        /// <summary>
        /// Message to display if the list is empty
        /// </summary>
        public string EmptyMessage
        {
            get
            {
                return emptyMessage;
            }
            set
            {
                emptyMessage = value;
            }
        }
        private string emptyMessage = "The list is empty!";

        /// <summary>
        /// On pre-render
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRender(EventArgs e)
        {
            if (PageCount == 1)
                Pager.Visible = false;

            if (RecordCount == 0 && !string.IsNullOrEmpty(EmptyMessage))
                Controls.AddAt(0, new Literal
                {
                    Text = $"<p class=\"empty\">{EmptyMessage}</p>"
                });

            base.OnPreRender(e);
        }

        /// <summary>
        /// On item data-bound
        /// </summary>
        /// <param name="e"></param>
        protected override void OnItemDataBound(RepeaterItemEventArgs e)
        {
            if (Pager.RecordCount == 0)
                e.Item.Visible = false;

            if (e.Item.ItemType == ListItemType.Footer)
                e.Item.Controls.Add(Pager);

            base.OnItemDataBound(e);
        }

        /// <summary>
        /// Convenience method to bind to the datasource
        /// </summary>
        /// <param name="DataSource"></param>
        public void Bind(object DataSource)
        {
            this.DataSource = DataSource;
            this.DataBind();
        }
    }
}
