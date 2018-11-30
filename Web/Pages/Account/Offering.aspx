<%@ MasterType TypeName="Agrishare.Web.Pages.Default" %>
<%@ Page Title="Dashboard - Offering Equipment" Language="C#" MasterPageFile="~/Pages/Default.Master" AutoEventWireup="true" CodeBehind="Offering.aspx.cs" Inherits="Agrishare.Web.Pages.Account.Offering.Default" %>

<asp:Content ContentPlaceHolderID="Content" runat="server">
    
    <h1>Offering</h1>

    <h3>My Equipment</h3>

    <ul class="search">
        <li>
            <a href="/account/listings?cid=1">
                <img src="/Resources/Images/Tractor.svg" />
                <span>Tractors</span>
            </a>
        </li>
        <li>
            <a href="/account/listings?cid=2">
                <img src="/Resources/Images/Lorry.svg" />
                <span>Lorries</span>
            </a>
        </li>
        <li>
            <a href="/account/listings?cid=3">
                <img src="/Resources/Images/Processing.svg" />
                <span>Processing</span>
            </a>
        </li>
    </ul>

    <br />

    <div class="cols-rev">

        <div>

            <h3>Recent Notifications <a href="/account/notifications/offering">View All</a></h3>

            <web:PagedRepeater runat="server" ID="Notifications" OnItemDataBound="BindNotification">
                <HeaderTemplate>
                    <div class="notifications-summary-list">
                </HeaderTemplate>
                <ItemTemplate>
                        <asp:HyperLink runat="server" ID="Link">
                            <strong><asp:Literal runat="server" ID="Title" /> <em><asp:Literal runat="server" ID="TimeAgo" /></em></strong>
                            <span><asp:Literal runat="server" ID="Message" /></span>
                            <small><asp:Literal runat="server" ID="Date" /> &bull; <asp:Literal runat="server" ID="Listing" /></small>
                            <strong runat="server" ID="Action" class="button" />
                        </asp:HyperLink>
                </ItemTemplate>
                <FooterTemplate>
                    </div>
                </FooterTemplate>
            </web:PagedRepeater>

        </div>

        <div>

            <h3>My Bookings <a href="/account/bookings/offering">View All</a></h3>

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
                            <span runat="server" ID="Photo" />
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

        </div>

    </div>    

</asp:Content>
