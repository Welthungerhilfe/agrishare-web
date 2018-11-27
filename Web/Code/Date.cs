/* Title: Gloo Framework
 * Author: Bradley Searle (C2 Digital)
 * Source: www.c2.co.zw
 * License: CC BY 4.0 (https://creativecommons.org/licenses/by/4.0/legalcode) */

using Agrishare.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Agrishare.Web.Controls
{
    public class Date : TextBox
    {
        public DateTime? SelectedDate
        {
            get
            {
                try
                {
                    return DateTime.Parse(Text);
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                if (value.HasValue)
                    Text = value.Value.ToString("dd/MM/yyyy");

            }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            Page.ClientScript.RegisterStartupScript(GetType(), "DatePicker", "$('.date-picker').datepicker({'autoHide':true,'format':'dd/MM/yyyy'});", true);
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "date-picker");
            base.Render(writer);
        }
    }
}
