<%@ MasterType TypeName="Agrishare.Web.Pages.Default" %>
<%@ Page Title="Booking Details" Language="C#" MasterPageFile="~/Pages/Default.Master" AutoEventWireup="true" CodeBehind="Details.aspx.cs" Inherits="Agrishare.Web.Pages.Account.Booking.Details" %>

<asp:Content ContentPlaceHolderID="Content" runat="server">
    
    <h1><asp:Literal runat="server" ID="BookingTitle" /></h1>

    <asp:Repeater runat="server" ID="Gallery" OnItemDataBound="BindPhoto">
        <HeaderTemplate>
            <ul>
        </HeaderTemplate>
        <ItemTemplate>
                <li>
                    <asp:HyperLink runat="server" ID="Thumb" />
                </li>
        </ItemTemplate>
        <FooterTemplate>
            </ul>
        </FooterTemplate>
    </asp:Repeater>

    <p><asp:Literal runat="server" ID="Description" /></p>

    <ul>
        <li><strong>Brand</strong><span><asp:Literal runat="server" ID="Brand" /></span></li>
        <li><strong>Horse Power</strong><span><asp:Literal runat="server" ID="HorsePower" /></span></li>
        <li><strong>Year</strong><span><asp:Literal runat="server" ID="Year" /></span></li>
    </ul>

    <div class="booking">

        <ul>
            <li>
                <strong>Dates</strong>
                <span><asp:Literal runat="server" ID="Dates" /></span>
            </li>
            <li>
                <strong>Transport Cost</strong>
                <span><asp:Literal runat="server" ID="TransportDistance" /></span>
                <span><asp:Literal runat="server" ID="TransportCost" /></span>
            </li>
            <li>
                <strong>Hire Cost</strong>
                <span><asp:Literal runat="server" ID="HireSize" /></span>
                <span><asp:Literal runat="server" ID="HireCost" /></span>
            </li>
            <li>
                <strong>Fuel Cost</strong>
                <span><asp:Literal runat="server" ID="FuelSize" /></span>
                <span><asp:Literal runat="server" ID="FuelCost" /></span>
            </li>
            <li runat="server" id="CommissionRow">
                <strong>Extras</strong>
                <span>AgriShare Commission</span>
                <span><asp:Literal runat="server" ID="Commission" /></span>
            </li>
            
            <li>
                <strong>Total</strong>
                <span><asp:Literal runat="server" ID="Total" /></span>
            </li>
        </ul>

    </div>

    <div class="action">
        //TODO
    </div>

</asp:Content>
