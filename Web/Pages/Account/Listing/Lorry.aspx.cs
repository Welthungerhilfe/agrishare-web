using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

namespace Agrishare.Web.Pages.Account.Listing
{
    public partial class Lorry : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Master.RequiresAuthentication = true;
            Master.Body.Attributes["class"] += " account ";

            if (!Page.IsPostBack)
            {
                var id = Convert.ToInt32(Request.QueryString["id"]);
                var listing = Core.Entities.Listing.Find(Id: id);
                if (listing.Id > 0)
                {
                    EquipmentTitle.Text = listing.Title;
                    Description.Text = listing.Description;
                    Brand.Text = listing.Brand;
                    Horsepower.Text = listing.HorsePower?.ToString();
                    Year.Text = listing.Year?.ToString();
                    GroupHire.Checked = listing.GroupServices;
                    Location.Latitude = listing.Latitude;
                    Location.Longitude = listing.Longitude;
                    Gallery.Photos = listing.Photos;

                    if (listing.Services.Count > 0)
                    {
                        TotalVolume.Text = listing.Services.First().TotalVolume.ToString();
                        TimePerQuantityUnit.Text = listing.Services.First().TimePerQuantityUnit.ToString();
                        PricePerQuantityUnit.Text = listing.Services.First().PricePerQuantityUnit.ToString();
                        DistanceCharge.Text = listing.Services.First().PricePerDistanceUnit.ToString();
                        MaximumDistance.Text = listing.Services.First().MaximumDistance.ToString();
                    }
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
                listing.CategoryId = Core.Entities.Category.LorriesId;
                listing.Services = new List<Core.Entities.Service>
                {
                    new Core.Entities.Service()
                };
            }

            listing.Title = EquipmentTitle.Text;
            listing.Description = Description.Text;
            listing.Brand = Brand.Text;
            listing.HorsePower = Convert.ToInt32(Horsepower.Text);
            listing.Year = Convert.ToInt32(Year.Text);
            listing.GroupServices = GroupHire.Checked;
            listing.Latitude = Location.Latitude;
            listing.Longitude = Location.Longitude;
            listing.AvailableWithFuel = true;
            listing.AvailableWithoutFuel = true;

            var service = listing.Services.First();
            service.DistanceUnitId = Core.Entities.DistanceUnit.Km;
            service.FuelPerQuantityUnit = 0;
            service.MaximumDistance = Convert.ToDecimal(MaximumDistance.Text);
            service.MinimumQuantity = 0;
            service.Mobile = true;
            service.PricePerDistanceUnit = Convert.ToDecimal(DistanceCharge.Text);
            service.PricePerQuantityUnit = Convert.ToDecimal(PricePerQuantityUnit.Text);
            service.QuantityUnitId = Core.Entities.QuantityUnit.None;
            service.CategoryId = Core.Entities.Category.LorriesServiceId;
            service.TimePerQuantityUnit = Convert.ToDecimal(TimePerQuantityUnit.Text);
            service.TimeUnitId = Core.Entities.TimeUnit.HoursPer100Kms;
            service.TotalVolume = Convert.ToDecimal(TotalVolume.Text);

            if (Gallery.Photos.Count > 0)
                listing.PhotoPaths = string.Join(",", Gallery.Photos.Select(o => o.Filename).ToArray());

            if (listing.Save())
            {
                Master.Feedback = "Listing updated";
                Response.Redirect($"/account/listings?cid={Core.Entities.Category.LorriesId}");
            }
            else
                Master.Feedback = "An unknown error ocurred";

        }
    }
}