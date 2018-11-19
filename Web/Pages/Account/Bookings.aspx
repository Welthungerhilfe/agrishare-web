<%@ MasterType TypeName="Agrishare.Web.Pages.Default" %>
<%@ Page Title="Bookings" Language="C#" MasterPageFile="~/Pages/Default.Master" AutoEventWireup="true" CodeBehind="Bookings.aspx.cs" Inherits="Agrishare.Web.Pages.Account.Bookings" %>

<asp:Content ContentPlaceHolderID="Content" runat="server">
    
    <h1>Bookings</h1>

    <asp:Panel runat="server" ID="Summary" CssClass="summary">
        <div>
            <small>This Month</small>
            <span><asp:Literal runat="server" Id="MonthSummary" /></span>
        </div>
        <div>
            <small>All Time</small>
            <span><asp:Literal runat="server" Id="AllTimeSummary" /></span>
        </div>
    </asp:Panel>

    <web:PagedRepeater runat="server" ID="List" OnItemDataBound="BindBooking">
        <HeaderTemplate>
            <div class="bookings-list">
        </HeaderTemplate>
        <ItemTemplate>
                <asp:HyperLink runat="server" ID="Link">
                    <span>
                        <asp:Image runat="server" ID="Photo" />
                    </span>
                    <span>
                        <small><asp:Literal runat="server" ID="Date" /></small>
                        <span><asp:Literal runat="server" ID="Title" /></span>
                        <strong><asp:Literal runat="server" ID="Price" /></strong>
                    </span>
                </asp:HyperLink>
        </ItemTemplate>
        <FooterTemplate>
            </div>
        </FooterTemplate>
    </web:PagedRepeater>

</asp:Content>
