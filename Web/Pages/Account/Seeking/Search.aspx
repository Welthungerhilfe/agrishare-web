<%@ MasterType TypeName="Agrishare.Web.Pages.Default" %>
<%@ Page Title="Search Results" Language="C#" MasterPageFile="~/Pages/Default.Master" AutoEventWireup="true" CodeBehind="Search.aspx.cs" Inherits="Agrishare.Web.Pages.Account.Seeking.Search" %>

<asp:Content ContentPlaceHolderID="Content" runat="server">
    
    <h1>Search</h1>

    <asp:PlaceHolder runat="server" ID="SearchForm">

        <div id="StepFor">
            <p>Who is this booking for?</p>
            <asp:CheckBoxList runat="server" ID="For" />
        </div>

        <div id="StepService">
            <p>What type of service do you require?</p>            
            <asp:CheckBoxList runat="server" ID="Services" />
        </div>

        <div id="StepTractorDetails">
            <p>Enter the size of the field</p>
            <asp:TextBox runat="server" ID="FieldSize" />
        </div>

        <div id="StepLorryDetails">
            <p>Enter the details of the load to be transported</p>
            <asp:TextBox runat="server" ID="LoadWeight" placeholder="Weight (tonnes)" />
            <asp:TextBox runat="server" ID="LoadDescription" TextMode="MultiLine" placeholder="Description" />
        </div>

        <div id="StepProcessingDetails">
            <p>Enter the number of bags</p>
            <asp:TextBox runat="server" ID="NumberOfBags" />
        </div>

        <div id="StepDate">
            <p>When do you require the service to be performed?</p>
            <asp:TextBox runat="server" ID="StartDate" />
        </div>

        <div id="StepMobile">
            <p>Should the equipment be mobile</p>
            <asp:CheckBoxList runat="server" ID="Mobile">
                <asp:ListItem Text="Yes, it should be mobile" Value="True" />
                <asp:ListItem Text="No, I can deliver my bags" Value="False" />
            </asp:CheckBoxList>
        </div>

        <div id="StepLocation">
            <p>Where do you need the service to be performed?</p>
            //TODO map
        </div>

        <div id="StepLorryPickUp">
            <p>Where is the pick-up location?</p>
            //TODO map
        </div>

        <div id="StepLorryDropOfff">
            <p>Where is the drop-off location?</p>
            //TODO map
        </div>

        <asp:HiddenField runat="server" ID="Latitude" />
        <asp:HiddenField runat="server" ID="Longitude" />
        <asp:HiddenField runat="server" ID="DestinationLatitude" />
        <asp:HiddenField runat="server" ID="DestinationLongitude" />

        <div id="StepFuel">
            <p>Should the supplier provide the fuel?</p>
            <asp:CheckBoxList runat="server" ID="Fuel">
                <asp:ListItem Text="Yes, fuel must be supplied" Value="True" />
                <asp:ListItem Text="No, I will supply my own fuel" Value="False" />
            </asp:CheckBoxList>
        </div>

        <p>
            <asp:Button runat="server" OnClick="FindListings" Text="Next" ID="SearchButton" />
        </p>

    </asp:PlaceHolder>

    <web:PagedRepeater runat="server" ID="SearchResults" OnItemDataBound="BindSearchResult">
        <HeaderTemplate>
            <div class="results-list">
        </HeaderTemplate>
        <ItemTemplate>
                <asp:LinkButton runat="server" ID="Link">
                    <span>
                        <asp:Image runat="server" ID="Photo" />
                    </span>
                    <span>
                        <small><asp:Literal runat="server" ID="Distance" /></small>
                        <strong><asp:Literal runat="server" ID="Title" /></strong>
                        <span>Year: <asp:Literal runat="server" ID="Year" /></span>
                        <b><asp:Literal runat="server" ID="Price" /></b>
                        <em><asp:Literal runat="server" ID="Status" /></em>
                    </span>
                </asp:LinkButton>
        </ItemTemplate>
        <FooterTemplate>
            </div>
        </FooterTemplate>
    </web:PagedRepeater>

</asp:Content>
