<%@ MasterType TypeName="Agrishare.Web.Pages.Default" %>
<%@ Page Title="Booking Details" Language="C#" MasterPageFile="~/Pages/Default.Master" AutoEventWireup="true" CodeBehind="Reviews.aspx.cs" Inherits="Agrishare.Web.Pages.Account.Listing.Reviews" %>

<asp:Content ContentPlaceHolderID="Content" runat="server">
    
    <h1><asp:Literal runat="server" ID="ListingTitle" /></h1>

    <Web:PagedRepeater runat="server" ID="List" OnItemDataBound="BindReview">
        <HeaderTemplate>
            <div class="review-list">
        </HeaderTemplate>
        <ItemTemplate>
                <div>
                    <strong runat="server" id="Stars"><asp:Literal runat="server" ID="User" /></strong>
                    <p><asp:Literal runat="server" ID="Comments" /></p>
                    <small><asp:Literal runat="server" ID="Date" /></small>                    
                </div>
        </ItemTemplate>
        <FooterTemplate>
            </div>
        </FooterTemplate>
    </Web:PagedRepeater>

</asp:Content>
