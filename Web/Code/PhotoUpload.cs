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
    public class PhotoUpload : WebControl
    {
        private List<Core.Entities.File> _photos = new List<Core.Entities.File>();
        public List<Core.Entities.File> Photos
        {
            get
            {
                if (Page.Request.Form[$"{ClientID}-files"] != null)
                {
                    try
                    {
                        _photos = JsonConvert.DeserializeObject<List<Core.Entities.File>>(Page.Request.Form[$"{ClientID}-files"]);
                    }
                    catch { }
                }
                return _photos ?? new List<Core.Entities.File>();
            }
            set
            {
                _photos = value;
            }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            base.AddAttributesToRender(writer);

            writer.AddAttribute(HtmlTextWriterAttribute.Class, "uploader");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            writer.WriteLine($@"<script>
                $(function() {{
                    $('#{ClientID}-file-upload').uploadifive({{
                        'queueID': '{ClientID}-upload-queue',
                        'uploadScript': '{Core.Entities.Config.APIURL}/upload/photo',
                        'removeCompleted': true,
                        'multi': true,
                        'onInit': function () {{
                            var str = $('#{ClientID}-upload-files').val();
                            var files = str !== '' ? JSON.parse(str) : [];
                            for (var i = 0; i < files.length; i++)
                                $('#{ClientID}-upload-preview').append('<div><div style=""background-image:url({Core.Entities.Config.CDNURL}' + files[i].ThumbName + ')""></div></div>');
                        }},
                        'onUploadComplete': function (file, data) {{
                            var str = $('#{ClientID}-upload-files').val();
                            var files = str !== '' ? JSON.parse(str) : [];
                            data = JSON.parse(data);
                            for (var i = 0; i < data.length; i++) {{
                                $('#{ClientID}-upload-preview').append('<div><div style=""background-image:url({Core.Entities.Config.CDNURL}' + data[i].ThumbName + ')""></div></div>');
                                files.push(data[i]);
                            }}
                            $('#{ClientID}-upload-files').val(JSON.stringify(files));
                        }}
                    }});
                }});
            </script>
            <input type=""hidden"" id=""{ClientID}-upload-files"" />
            <div id=""{ClientID}-upload-preview"" class=""uploadifive-preview""></div>
            <input id=""{ClientID}-file-upload"" type=""file"" name=""file_upload"" />   
            <div id=""{ClientID}-upload-queue""></div>");

            writer.RenderEndTag();
        }
    }
}
