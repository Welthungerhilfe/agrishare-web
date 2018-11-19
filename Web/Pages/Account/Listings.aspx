<%@ MasterType TypeName="Agrishare.Web.Pages.Default" %>
<%@ Page Title="Listings" Language="C#" MasterPageFile="~/Pages/Default.Master" AutoEventWireup="true" CodeBehind="Listings.aspx.cs" Inherits="Agrishare.Web.Pages.Account.Listings" %>

<asp:Content ContentPlaceHolderID="Content" runat="server">
    
    <h1>Listings</h1>

    <web:PagedRepeater runat="server" ID="List" OnItemDataBound="BindListing">
        <HeaderTemplate>
            <div class="listings-list">
        </HeaderTemplate>
        <ItemTemplate>
                <asp:HyperLink runat="server" ID="Link">
                    <span>
                        <asp:Image runat="server" ID="Photo" />
                    </span>
                    <span>
                        <strong><asp:Literal runat="server" ID="Title" /></strong>
                        <span><asp:Literal runat="server" ID="Description" /></span>
                    </span>
                </asp:HyperLink>
        </ItemTemplate>
        <FooterTemplate>
            </div>
        </FooterTemplate>
    </web:PagedRepeater>

</asp:Content>
