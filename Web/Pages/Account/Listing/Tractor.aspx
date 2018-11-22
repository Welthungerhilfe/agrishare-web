<%@ MasterType TypeName="Agrishare.Web.Pages.Default" %>
<%@ Page Title="Tractor" Language="C#" MasterPageFile="~/Pages/Default.Master" AutoEventWireup="true" CodeBehind="Tractor.aspx.cs" Inherits="Agrishare.Web.Pages.Account.Listing.Tractor" %>

<asp:Content ContentPlaceHolderID="Content" runat="server">
    
    <h1>Tractor</h1>

    <p>Enter the details for our equipment.</p>

    <div class="form-cols">

        <div>

            <h2>Listing</h2>

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
                <asp:TextBox runat="server" ID="Brand" MaxLength="64" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="Brand" Text="Brand is required" Display="Dynamic" />
            </div>

            <div class="form-row">
                <asp:Label runat="server" AssociatedControlID="Horsepower" Text="Horsepower *" />
                <asp:TextBox runat="server" ID="Horsepower" MaxLength="16" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="Horsepower" Text="Horsepower is required" Display="Dynamic" />
            </div>

            <div class="form-row">
                <asp:Label runat="server" AssociatedControlID="Year" Text="Year *" />
                <asp:TextBox runat="server" ID="Year" MaxLength="4" TextMode="Number" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="Year" Text="Year is required" Display="Dynamic" />
            </div>

            <div class="checkbox-row">
                <asp:CheckBox runat="server" ID="AvailableWithFuel" Text="Available with fuel"  />        
            </div>

            <div class="checkbox-row">
                <asp:CheckBox runat="server" ID="AvailableWithoutFuel" Text="Available without fuel"  />        
            </div>

            <div class="checkbox-row">
                <asp:CheckBox runat="server" ID="GroupHire" Text="Allow group hire" />
            </div>

            <h2>Services</h2>

            <asp:Repeater runat="server" ID="Services" OnItemDataBound="BindService">
                <ItemTemplate>

                    <h3 class="checkbox-row service-checkbox"><asp:CheckBox runat="server" ID="Title" /></h3>

                    <div class="service-row">

                        <div class="form-row">
                            <asp:Label runat="server" AssociatedControlID="TimePerQuantityUnit" Text="Hours required per hectare *" />
                            <div><asp:TextBox runat="server" ID="TimePerQuantityUnit" MaxLength="8" /><span>HRS</span></div>
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="TimePerQuantityUnit" Text="Hours required per hectare is required" Display="Dynamic" />
                        <asp:RegularExpressionValidator runat="server" ControlToValidate="TimePerQuantityUnit" Text="Hours required per hectare is invalid" ValidationExpression="^[\d]+(.[\d]+)?$" Display="Dynamic" />
                        </div>

                        <div class="form-row">
                            <asp:Label runat="server" AssociatedControlID="PricePerQuantityUnit" Text="Hire Cost *" />
                            <div><asp:TextBox runat="server" ID="PricePerQuantityUnit" MaxLength="8" /><span>$/HA</span></div>
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="PricePerQuantityUnit" Text="Hire cost is required" Display="Dynamic" />
                            <asp:RegularExpressionValidator runat="server" ControlToValidate="PricePerQuantityUnit" Text="Hire cost  is invalid" ValidationExpression="^[\d]+(.[\d]+)?$" Display="Dynamic" />
                        </div>

                        <div class="form-row">
                            <asp:Label runat="server" AssociatedControlID="FuelPerQuantityUnit" Text="Fuel Cost *" />
                            <div><asp:TextBox runat="server" ID="FuelPerQuantityUnit" MaxLength="8" /><span>$/HA</span></div>
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="FuelPerQuantityUnit" Text="Fuel cost is required" Display="Dynamic" />
                            <asp:RegularExpressionValidator runat="server" ControlToValidate="FuelPerQuantityUnit" Text="Fuel cost is invalid" ValidationExpression="^[\d]+(.[\d]+)?$" Display="Dynamic" />
                        </div>

                        <div class="form-row">
                            <asp:Label runat="server" AssociatedControlID="MinimumQuantity" Text="Minimum field size *" />
                            <div><asp:TextBox runat="server" ID="MinimumQuantity" MaxLength="8" /><span>HA</span></div>
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="MinimumQuantity" Text="Minimum field size is required" Display="Dynamic" />
                            <asp:RegularExpressionValidator runat="server" ControlToValidate="MinimumQuantity" Text="Minimum field size is invalid" ValidationExpression="^[\d]+(.[\d]+)?$" Display="Dynamic" />
                        </div>

                    </div>

                </ItemTemplate>
            </asp:Repeater>

        </div>
        <div>

            <h2>Location</h2>

            <div class="form-row">
                <label>Location</label>
                <web:Map runat="server" Id="Location" />
            </div>

            <div class="form-row">
                <asp:Label runat="server" AssociatedControlID="PricePerDistanceUnit" Text="Distance charge *" />
                <div><asp:TextBox runat="server" ID="PricePerDistanceUnit" MaxLength="8" /><span>$/KM</span></div>
                <asp:RequiredFieldValidator runat="server" ControlToValidate="PricePerDistanceUnit" Text="Distance charge is required" Display="Dynamic" />
                <asp:RegularExpressionValidator runat="server" ControlToValidate="PricePerDistanceUnit" Text="Distance charge is invalid" ValidationExpression="^[\d]+(.[\d]+)?$" Display="Dynamic" />
            </div>

            <div class="form-row">
                <asp:Label runat="server" AssociatedControlID="MaximumDistance" Text="Maximum distance *" />
                <div><asp:TextBox runat="server" ID="MaximumDistance" MaxLength="8" /><span>KM</span></div>
                <asp:RequiredFieldValidator runat="server" ControlToValidate="MaximumDistance" Text="Maximum distance is required" Display="Dynamic" />
                        <asp:RegularExpressionValidator runat="server" ControlToValidate="MaximumDistance" Text="Maximum distance is invalid" ValidationExpression="^[\d]+(.[\d]+)?$" Display="Dynamic" />
            </div>

            <h2>Photos</h2>
            <web:PhotoUpload runat="server" Id="Photos" />

        </div>

    </div>

    <p>
        <asp:Button runat="server" Text="Save" CssClass="button" OnClick="Save" />
    </p>

    <script>
        $('.service-checkbox input').change(function () {
            if ($(this).is(':checked'))
                $(this).parent().next('div').show();
            else                
                $(this).parent().next('div').hide();
        }).change();

    </script>

</asp:Content>
