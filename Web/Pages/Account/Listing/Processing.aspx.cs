using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Agrishare.Web.Pages.Account.Listing
{
    public partial class Processing : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Master.RequiresAuthentication = true;
            Master.Body.Attributes["class"] += " account ";

            if (!Page.IsPostBack)
            {
                Service.Items.Add(new ListItem { Text = "Select...", Value = "" });
                var services = Core.Entities.Category.List(ParentId: Core.Entities.Category.ProcessingId);
                foreach (var service in services)
                    Service.Items.Add(new ListItem { Text = service.Title, Value = $"{service.Id}" });
            
                var id = Convert.ToInt32(Request.QueryString["id"]);
                var listing = Core.Entities.Listing.Find(Id: id);
                if (listing.Id > 0)
                {
                    EquipmentTitle.Text = listing.Title;
                    Description.Text = listing.Description;
                    Brand.Text = listing.Brand;
                    Year.Text = listing.Year?.ToString();
                    GroupHire.Checked = listing.GroupServices;
                    Location.Latitude = listing.Latitude;
                    Location.Longitude = listing.Longitude;
                    Gallery.Photos = listing.Photos;

                    if (listing.Services.Count > 0)
                    {
                        Service.SelectedValue = listing.Services.First().CategoryId.ToString();
                        TimePerQuantityUnit.Text = listing.Services.First().TimePerQuantityUnit.ToString();
                        PricePerQuantityUnit.Text = listing.Services.First().PricePerQuantityUnit.ToString();
                        MinimumQuantity.Text = listing.Services.First().MinimumQuantity.ToString("0");
                        Mobile.Checked = listing.Services.First().Mobile;
                        PricePerDistanceUnit.Text = listing.Services.First().PricePerDistanceUnit.ToString();
                        MaximumDistance.Text = listing.Services.First().MaximumDistance.ToString();
                    }
                }
                else
                {
                    PricePerDistanceUnit.Text = "0";
                    MaximumDistance.Text = "0";
                }

            }
        }

        public void Save(object s, EventArgs e)
        {
            var id = Convert.ToInt32(Request.QueryString["id"]);
            var listing = Core.Entities.Listing.Find(Id: id);

            if (listing.Id == 0)
            {
                listing.User = Master.CurrentUser;
                listing.CategoryId = Core.Entities.Category.ProcessingId;
                listing.Services = new List<Core.Entities.Service>
                {
                    new Core.Entities.Service()
                };
            }

            listing.Title = EquipmentTitle.Text;
            listing.Description = Description.Text;
            listing.Brand = Brand.Text;
            listing.HorsePower = 0;
            listing.Year = Convert.ToInt32(Year.Text);
            listing.GroupServices = GroupHire.Checked;
            listing.Latitude = Location.Latitude;
            listing.Longitude = Location.Longitude;
            listing.AvailableWithFuel = true;
            listing.AvailableWithoutFuel = true;

            listing.Services.First().DistanceUnitId = Core.Entities.DistanceUnit.Km;
            listing.Services.First().FuelPerQuantityUnit = 0;
            listing.Services.First().MaximumDistance = Convert.ToDecimal(MaximumDistance.Text);
            listing.Services.First().MinimumQuantity = Convert.ToInt32(MinimumQuantity.Text);
            listing.Services.First().Mobile = Mobile.Checked;
            listing.Services.First().PricePerDistanceUnit = Convert.ToDecimal(PricePerDistanceUnit.Text);
            listing.Services.First().PricePerQuantityUnit = Convert.ToDecimal(PricePerQuantityUnit.Text);
            listing.Services.First().QuantityUnitId = Core.Entities.QuantityUnit.Bags;
            listing.Services.First().CategoryId = Convert.ToInt32(Service.SelectedValue);
            listing.Services.First().TimePerQuantityUnit = Convert.ToDecimal(TimePerQuantityUnit.Text);
            listing.Services.First().TimeUnitId = Core.Entities.TimeUnit.BagsPerHour;
            listing.Services.First().TotalVolume = 0;

            if (Gallery.Photos.Count > 0)
                listing.PhotoPaths = string.Join(",", Gallery.Photos.Select(o => o.Filename).ToArray());

            if (listing.Save())
            {
                Master.Feedback = "Listing updated";
                Response.Redirect($"/account/listings?cid={Core.Entities.Category.ProcessingId}");
            }
            else
                Master.Feedback = "An unknown error ocurred";

        }
    }
}