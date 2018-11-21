<%@ MasterType TypeName="Agrishare.Web.Pages.Default" %>
<%@ Page Title="Lorry" Language="C#" MasterPageFile="~/Pages/Default.Master" AutoEventWireup="true" CodeBehind="Lorry.aspx.cs" Inherits="Agrishare.Web.Pages.Account.Listing.Lorry" %>

<asp:Content ContentPlaceHolderID="Content" runat="server">
    
    <h1>Lorry</h1>

    <p>Please provide the following details for your lorry service.</p>

    <div class="form-cols">

        <div>

            <h2>Listing Details</h2>

            <div class="form-row">
                <asp:Label runat="server" AssociatedControlID="EquipmentTitle" Text="Title *" />
                <asp:TextBox runat="server" ID="EquipmentTitle" MaxLength="1024" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="EquipmentTitle" Text="Title is required" Display="Dynamic" />
            </div>

            <div class="form-row">
                <asp:Label runat="server" AssociatedControlID="Description" Text="Additional Information" />
                <asp:TextBox runat="server" ID="Description" TextMode="MultiLine" />
            </div>

            <div class="form-row">
                <asp:Label runat="server" AssociatedControlID="Brand" Text="Brand *" />
                <asp:TextBox runat="server" ID="Brand" MaxLength="256" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="Brand" Text="Brand is required" Display="Dynamic" />
            </div>

            <div class="form-row">
                <asp:Label runat="server" AssociatedControlID="Horsepower" Text="Horsepower *" />
                <asp:TextBox runat="server" ID="Horsepower" MaxLength="64" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="Horsepower" Text="Horsepower is required" Display="Dynamic" />
            </div>

            <div class="form-row">
                <asp:Label runat="server" AssociatedControlID="Year" Text="Year *" />
                <asp:TextBox runat="server" ID="Year" MaxLength="4" TextMode="Number" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="Year" Text="Year is required" Display="Dynamic" />
            </div>

            <div class="checkbox-row">
                <asp:CheckBox runat="server" ID="GroupHire" Text="Allow group hire" />
            </div>

            <h2>Booking Details</h2>
    
            <div class="form-row">
                <asp:Label runat="server" AssociatedControlID="TotalVolume" Text="Total volume *" />
                <div><asp:TextBox runat="server" ID="TotalVolume" MaxLength="8" TextMode="Number" /><span>TONNES</span></div>
                <asp:RequiredFieldValidator runat="server" ControlToValidate="TotalVolume" Text="Hours required per hectare is required" Display="Dynamic" />
            </div>

            <div class="form-row">
                <asp:Label runat="server" AssociatedControlID="TimePerQuantityUnit" Text="Hours required per 100KM *" />
                <div><asp:TextBox runat="server" ID="TimePerQuantityUnit" MaxLength="8" /><span>HRS/100KM</span></div>
                <asp:RequiredFieldValidator runat="server" ControlToValidate="TimePerQuantityUnit" Text="Hours required per 100KM is required" Display="Dynamic" />
                <asp:RegularExpressionValidator runat="server" ControlToValidate="TimePerQuantityUnit" Text="Hours required per 100KM is invalid" ValidationExpression="^[\d]+(.[\d]+)?$" Display="Dynamic" />
            </div>

            <div class="form-row">
                <asp:Label runat="server" AssociatedControlID="PricePerQuantityUnit" Text="Hire Cost *" />
                <div><asp:TextBox runat="server" ID="PricePerQuantityUnit" MaxLength="8" /><span>$/HA</span></div>
                <asp:RequiredFieldValidator runat="server" ControlToValidate="PricePerQuantityUnit" Text="Hire cost is required" Display="Dynamic" />
                <asp:RegularExpressionValidator runat="server" ControlToValidate="PricePerQuantityUnit" Text="Hire cost is invalid" ValidationExpression="^[\d]+(.[\d]+)?$" Display="Dynamic" />
            </div>

            <div class="form-row">
                <asp:Label runat="server" AssociatedControlID="FuelCost" Text="Fuel Cost *" />
                <div><asp:TextBox runat="server" ID="FuelCost" MaxLength="8" /><span>$/HA</span></div>
                <asp:RequiredFieldValidator runat="server" ControlToValidate="FuelCost" Text="Fuel cost is required" Display="Dynamic" />
                <asp:RegularExpressionValidator runat="server" ControlToValidate="FuelCost" Text="Fuel cost is invalid" ValidationExpression="^[\d]+(.[\d]+)?$" Display="Dynamic" />
            </div>

        </div>

        <div>

            <h2>Location</h2>

            <div class="form-row">
                <label>Location</label>
                <web:Map runat="server" Id="Location" />
            </div>

            <div class="form-row mobile-row">
                <asp:Label runat="server" AssociatedControlID="DistanceCharge" Text="Distance charge *" />
                <div><asp:TextBox runat="server" ID="DistanceCharge" MaxLength="8" /><span>$/KM</span></div>
                <asp:RequiredFieldValidator runat="server" ControlToValidate="DistanceCharge" Text="Distance charge is required" Display="Dynamic" />
                <asp:RegularExpressionValidator runat="server" ControlToValidate="DistanceCharge" Text="Distance charge is invalid" ValidationExpression="^[\d]+(.[\d]+)?$" Display="Dynamic" />
            </div>

            <div class="form-row mobile-row">
                <asp:Label runat="server" AssociatedControlID="MaximumDistance" Text="Maximum distance *" />
                <div><asp:TextBox runat="server" ID="MaximumDistance" MaxLength="8" TextMode="Number" /><span>KM</span></div>
                <asp:RequiredFieldValidator runat="server" ControlToValidate="MaximumDistance" Text="Maximum distance is required" Display="Dynamic" />
            </div>

            <h2>Photos</h2>

            <p>//TODO photo upload</p>

        </div>

    </div>

    <p>
        <asp:Button runat="server" Text="Save" CssClass="button" OnClick="Save" />
    </p>

</asp:Content>
