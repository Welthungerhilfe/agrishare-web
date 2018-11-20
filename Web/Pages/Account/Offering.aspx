﻿<%@ MasterType TypeName="Agrishare.Web.Pages.Default" %>
<%@ Page Title="Dashboard - Offering Equipment" Language="C#" MasterPageFile="~/Pages/Default.Master" AutoEventWireup="true" CodeBehind="Offering.aspx.cs" Inherits="Agrishare.Web.Pages.Account.Offering.Default" %>

<asp:Content ContentPlaceHolderID="Content" runat="server">
    
    <h1>Offering</h1>
    
    <h2>My Equipment</h2>

    <ul>
        <li><a href="/account/listings?cid=1">Tractors</a></li>
        <li><a href="/account/listings?cid=2">Lorries</a></li>
        <li><a href="/account/listings?cid=3">Processing</a></li>
    </ul>

    <p>
        <a href="/account/listing/edit" class="button">Add listing</a>
    </p>

    <h2>Recent Notifications <a href="/account/notifications/offering">View All</a></h2>

    <web:PagedRepeater runat="server" ID="Notifications" OnItemDataBound="BindNotification">
        <HeaderTemplate>
            <div class="notifications-list">
        </HeaderTemplate>
        <ItemTemplate>
                <asp:HyperLink runat="server" ID="Link">
                    <span>
                        <asp:Image runat="server" ID="Photo" />
                    </span>
                    <span>
                        <small><asp:Literal runat="server" ID="Date" /> &bull;<asp:Literal runat="server" ID="Title" /></small>
                        <span><asp:Literal runat="server" ID="Message" /> <em><asp:Literal runat="server" ID="TimeAgo" /></em></span>
                        <asp:HyperLink runat="server" ID="Action" CssClass="button" />
                    </span>
                </asp:HyperLink>
        </ItemTemplate>
        <FooterTemplate>
            </div>
        </FooterTemplate>
    </web:PagedRepeater>

    <h2>My Bookings <a href="/account/bookings/offering">View All</a></h2>

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

    <web:PagedRepeater runat="server" ID="Bookings" OnItemDataBound="BindBooking">
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
