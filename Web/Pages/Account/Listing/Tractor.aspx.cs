using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Agrishare.Web.Pages.Account.Listing
{
    public partial class Tractor : System.Web.UI.Page
    {
        Core.Entities.Listing SelectedListing;

        protected void Page_Load(object sender, EventArgs e)
        {
            Master.RequiresAuthentication = true;
            Master.Body.Attributes["class"] += " account ";

            var id = Convert.ToInt32(Request.QueryString["id"]);
            SelectedListing = Core.Entities.Listing.Find(Id: id);

            if (!Page.IsPostBack)
            {
                Services.DataSource = Core.Entities.Category.List(ParentId: Core.Entities.Category.TractorsId);
                Services.DataBind();

                var listing = SelectedListing;
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
                    AvailableWithFuel.Checked = listing.AvailableWithFuel;
                    AvailableWithoutFuel.Checked = listing.AvailableWithoutFuel;
                }

            }
        }

        public void BindService(object s, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var category = (Core.Entities.Category)e.Item.DataItem;
                ((CheckBox)e.Item.FindControl("Title")).Text = category.Title;

                ((HiddenField)e.Item.FindControl("CategoryId")).Value = category.Id.ToString();
                ((HiddenField)e.Item.FindControl("ServiceId")).Value = "0";

                foreach (var service in SelectedListing.Services)
                {
                    if (service.CategoryId == category.Id)
                    {
                        ((CheckBox)e.Item.FindControl("Title")).Checked = true;

                        ((HiddenField)e.Item.FindControl("ServiceId")).Value = service.Id.ToString();

                        ((TextBox)e.Item.FindControl("TimePerQuantityUnit")).Text = service.TimePerQuantityUnit.ToString();
                        ((TextBox)e.Item.FindControl("PricePerQuantityUnit")).Text = service.PricePerQuantityUnit.ToString();
                        ((TextBox)e.Item.FindControl("FuelPerQuantityUnit")).Text = service.FuelPerQuantityUnit.ToString();
                        ((TextBox)e.Item.FindControl("MinimumQuantity")).Text = service.MinimumQuantity.ToString();

                        MaximumDistance.Text = service.MaximumDistance.ToString();
                        PricePerDistanceUnit.Text = service.PricePerDistanceUnit.ToString();
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
            }

            listing.Title = EquipmentTitle.Text;
            listing.Description = Description.Text;
            listing.Brand = Brand.Text;
            listing.HorsePower = Convert.ToInt32(Horsepower.Text);
            listing.Year = Convert.ToInt32(Year.Text);
            listing.GroupServices = GroupHire.Checked;
            listing.Latitude = Location.Latitude;
            listing.Longitude = Location.Longitude;
            listing.AvailableWithFuel = AvailableWithFuel.Checked;
            listing.AvailableWithoutFuel = AvailableWithoutFuel.Checked;

            var services = listing.Services;
            listing.Services = new List<Core.Entities.Service>();
            foreach (RepeaterItem item in Services.Items)
            {
                var enabled = ((CheckBox)item.FindControl("Title")).Checked;
                if (!enabled)
                    continue;

                var categoryId = Convert.ToInt32(((HiddenField)item.FindControl("CategoryId")).Value);
                var serviceId = Convert.ToInt32(((HiddenField)item.FindControl("ServiceId")).Value);
                var service = services.FirstOrDefault(o => o.Id == serviceId);

                if (service == null)
                    service = new Core.Entities.Service();

                service.CategoryId = categoryId;
                service.TimePerQuantityUnit = Convert.ToDecimal(((TextBox)item.FindControl("TimePerQuantityUnit")).Text);
                service.PricePerQuantityUnit = Convert.ToDecimal(((TextBox)item.FindControl("PricePerQuantityUnit")).Text);
                service.FuelPerQuantityUnit = Convert.ToDecimal(((TextBox)item.FindControl("FuelPerQuantityUnit")).Text);
                service.MinimumQuantity = Convert.ToDecimal(((TextBox)item.FindControl("MinimumQuantity")).Text);
                listing.Services.Add(service);
            }

            if (Gallery.Photos.Count > 0)
                listing.PhotoPaths = string.Join(",", Gallery.Photos.Select(o => o.Filename).ToArray());

            if (listing.Save())
            {
                Master.Feedback = "Listing updated";
                Response.Redirect($"/account/listings?cid={Core.Entities.Category.TractorsId}");
            }
            else
                Master.Feedback = "An unknown error ocurred";

        }

        protected void ValidateServiceField(object source, ServerValidateEventArgs args)
        {
            //args.IsValid = false;
        }
    }
}