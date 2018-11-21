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
    public class Map : WebControl
    {
        public string ApiKey => Core.Entities.Config.Find(Key: "Google Maps API Key").Value;

        private decimal latitude = -17.824858M;
        public decimal Latitude
        {
            get
            {
                if (Page.Request.Form[$"{ClientID}_Latitude"] != null)
                {
                    try { return decimal.Parse(Page.Request.Form[$"{ClientID}_Latitude"]); }
                    catch { }
                }
                return latitude;
            }
            set
            {
                latitude = value;
            }
        }

        public decimal longitude = 31.053028M;
        public decimal Longitude
        {
            get
            {
                if (Page.Request.Form[$"{ClientID}_Longitude"] != null)
                {
                    try { return decimal.Parse(Page.Request.Form[$"{ClientID}_Longitude"]); }
                    catch { }
                }
                return longitude;
            }
            set
            {
                longitude = value;
            }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            base.AddAttributesToRender(writer);

            writer.WriteLine($@"<input type=""hidden"" id=""{ClientID}_Latitude"" value=""{Latitude}"" />");
            writer.WriteLine($@"<input type=""hidden"" id=""{ClientID}_Longitude"" value=""{Longitude}"" />");

            writer.AddAttribute(HtmlTextWriterAttribute.Class, "google-map");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            writer.RenderEndTag();

            var script = @"
                var gmaps = [];
                function initMaps() {
                    for (var i = 0; i < gmaps.length; i++) {
                        var obj = gmaps[i];
                        map = new google.maps.Map(document.getElementById(obj.id), {
                            center: {lat: obj.lat, lng: obj.lng},
                            zoom: 8,
                            zoomControl: true,
                            mapTypeControl: true,
                            scaleControl: true,
                            streetViewControl: false,
                            rotateControl: false,
                            fullscreenControl: false
                        });
                        map.addListener('dragend', function() {
                            document.getElementById(obj.id + '_Latitude').value = this.center.lat();
                            document.getElementById(obj.id + '_Longitude').value = map.center.lng();
                        });
                    }
                }";

            Page.ClientScript.RegisterStartupScript(GetType(), $"GoogleMaps", script, true);
            Page.ClientScript.RegisterStartupScript(GetType(), $"GoogleMap{ID}", $@"gmaps.push({{ id: '{ClientID}', lat: {Latitude}, lng: {Longitude} }});", true);
            Page.ClientScript.RegisterStartupScript(GetType(), "GoogleMapsSrc", $@"<script async defer src=""https://maps.googleapis.com/maps/api/js?key={ApiKey}&callback=initMaps""></script>");
        }
    }
}
