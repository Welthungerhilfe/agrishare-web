<%@ MasterType TypeName="Agrishare.Web.Pages.Default" %>
<%@ Page Title="Notifications" Language="C#" MasterPageFile="~/Pages/Default.Master" AutoEventWireup="true" CodeBehind="Notifications.aspx.cs" Inherits="Agrishare.Web.Pages.Account.Notifications" %>

<asp:Content ContentPlaceHolderID="Content" runat="server">
    
    <h1>Notifications</h1>

    <web:PagedRepeater runat="server" ID="List" OnItemDataBound="BindNotification">
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

</asp:Content>
