<%@ MasterType TypeName="Agrishare.Web.Pages.Default" %>
<%@ Page Title="Listing" Language="C#" MasterPageFile="~/Pages/Default.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Agrishare.Web.Pages.Account.Listing.Edit" %>

<asp:Content ContentPlaceHolderID="Content" runat="server">
    
    <h1>Listing</h1>

    <p>Enter the details for our equipment.</p>

    <div class="form-row">
        <asp:Label runat="server" AssociatedControlID="Category" Text="Type of equipment *" />
        <asp:TextBox runat="server" ID="Category" />
        <asp:RequiredFieldValidator runat="server" ControlToValidate="Category" Text="Type of equipment is required" Display="Dynamic" />
    </div>

    <div class="form-row">
        <asp:Label runat="server" AssociatedControlID="Title" Text="Title *" />
        <asp:TextBox runat="server" ID="Title" />
        <asp:RequiredFieldValidator runat="server" ControlToValidate="Title" Text="Title is required" Display="Dynamic" />
    </div>

    <div class="form-row">
        <asp:Label runat="server" AssociatedControlID="Description" Text="Additional Information" />
        <asp:TextBox runat="server" ID="Description" TextMode="MultiLine" />
    </div>

    <div class="form-row">
        <asp:Label runat="server" AssociatedControlID="Brand" Text="Brand *" />
        <asp:TextBox runat="server" ID="Brand" />
        <asp:RequiredFieldValidator runat="server" ControlToValidate="Brand" Text="Brand is required" Display="Dynamic" />
    </div>

    <div class="form-row">
        <asp:Label runat="server" AssociatedControlID="Horsepower" Text="Horsepower *" />
        <asp:TextBox runat="server" ID="Horsepower" />
        <asp:RequiredFieldValidator runat="server" ControlToValidate="Horsepower" Text="Horsepower is required" Display="Dynamic" />
    </div>

    <div class="form-row">
        <asp:Label runat="server" AssociatedControlID="Year" Text="Year *" />
        <asp:TextBox runat="server" ID="Year" />
        <asp:RequiredFieldValidator runat="server" ControlToValidate="Year" Text="Year is required" Display="Dynamic" />
    </div>

    <div class="form-row">
        <asp:CheckBox runat="server" ID="AvailableWithoutFuel" Text="Available without fuel"  />
    </div>

    <div class="form-row">
        <asp:CheckBox runat="server" ID="GroupHire" Text="Allow group hire" />
    </div>

    <h2>Location</h2>

    <div class="form-row">
        <label>Location</label>
        //TODO map
    </div>

    <div class="form-row">
        <asp:Label runat="server" AssociatedControlID="Mobile" Text="Mobile" />
        <asp:CheckBox runat="server" ID="Equipment is mobile"  />
    </div>

    <div class="form-row mobile-row">
        <asp:Label runat="server" AssociatedControlID="DistanceCharge" Text="Distance charge *" />
        <asp:TextBox runat="server" ID="DistanceCharge" /><span>$/KM</span>
        <asp:RequiredFieldValidator runat="server" ControlToValidate="DistanceCharge" Text="Distance charge is required" Display="Dynamic" />
    </div>

    <div class="form-row mobile-row">
        <asp:Label runat="server" AssociatedControlID="MaximumDistance" Text="Maximum distance *" />
        <asp:TextBox runat="server" ID="MaximumDistance" /><span>KM</span>
        <asp:RequiredFieldValidator runat="server" ControlToValidate="MaximumDistance" Text="Maximum distance is required" Display="Dynamic" />
    </div>

    <h2>Services</h2>

    <asp:Panel runat="server" ID="TractorServices">

        <div class="form-row">
            <asp:Label runat="server" AssociatedControlID="TimePerQuantityUnit" Text="Hours required per hectare *" />
            <asp:TextBox runat="server" ID="TimePerQuantityUnit" /><span>HRS</span>
            <asp:RequiredFieldValidator runat="server" ControlToValidate="TimePerQuantityUnit" Text="Hours required per hectare is required" Display="Dynamic" />
        </div>

        <div class="form-row">
            <asp:Label runat="server" AssociatedControlID="PricePerQuantityUnit" Text="Hire Cost *" />
            <asp:TextBox runat="server" ID="PricePerQuantityUnit" /><span>$/HA</span>
            <asp:RequiredFieldValidator runat="server" ControlToValidate="PricePerQuantityUnit" Text="Hire cost is required" Display="Dynamic" />
        </div>

        <div class="form-row">
            <asp:Label runat="server" AssociatedControlID="FuelCost" Text="Fuel Cost *" />
            <asp:TextBox runat="server" ID="FuelCost" /><span>$/HA</span>
            <asp:RequiredFieldValidator runat="server" ControlToValidate="FuelCost" Text="Fuel cost is required" Display="Dynamic" />
        </div>

        <div class="form-row">
            <asp:Label runat="server" AssociatedControlID="MinimumSize" Text="Minimum field size *" />
            <asp:TextBox runat="server" ID="MinimumSize" /><span>HRS</span>
            <asp:RequiredFieldValidator runat="server" ControlToValidate="MinimumSize" Text="Minimum field size is required" Display="Dynamic" />
        </div>

    </asp:Panel>

    <asp:Panel runat="server" ID="ProcessingServices">

        <div class="form-row">
            <asp:Label runat="server" AssociatedControlID="BagsPerHour" Text="Bags per hour *" />
            <asp:TextBox runat="server" ID="BagsPerHour" /><span>BAGS/HRS</span>
            <asp:RequiredFieldValidator runat="server" ControlToValidate="BagsPerHour" Text="Bags per hour is required" Display="Dynamic" />
        </div>

        <div class="form-row">
            <asp:Label runat="server" AssociatedControlID="PricePerQuantityUnit" Text="Hire Cost *" />
            <asp:TextBox runat="server" ID="TextBox2" /><span>$/HA</span>
            <asp:RequiredFieldValidator runat="server" ControlToValidate="PricePerQuantityUnit" Text="Hire cost is required" Display="Dynamic" />
        </div>

        <div class="form-row">
            <asp:Label runat="server" AssociatedControlID="Hire" Text="Fuel Cost *" />
            <asp:TextBox runat="server" ID="TextBox3" /><span>$/HA</span>
            <asp:RequiredFieldValidator runat="server" ControlToValidate="FuelCost" Text="Fuel cost is required" Display="Dynamic" />
        </div>

        <div class="form-row">
            <asp:Label runat="server" AssociatedControlID="MinimumBags" Text="Minimum bags *" />
            <asp:TextBox runat="server" ID="MinimumBags" /><span>BAGS</span>
            <asp:RequiredFieldValidator runat="server" ControlToValidate="MinimumBags" Text="Minimum bags is required" Display="Dynamic" />
        </div>

    </asp:Panel>

    <h2>Photos</h2>

    //TODO photo upload

    <p>
        <asp:Button runat="server" Text="Save" CssClass="button" OnClick="Save" />
    </p>

</asp:Content>
