<%@ MasterType TypeName="Agrishare.Web.Pages.Default" %>
<%@ Page Title="Search Results" Language="C#" MasterPageFile="~/Pages/Default.Master" AutoEventWireup="true" CodeBehind="Results.aspx.cs" Inherits="Agrishare.Web.Pages.Account.Seeking.Results" %>

<asp:Content ContentPlaceHolderID="Content" runat="server">
    
    <h1>Search Results</h1>

    <web:PagedRepeater runat="server" ID="SearchResults" OnItemDataBound="BindSearchResult" EmptyMessage="Your search did not return any results - please try again">
        <HeaderTemplate>
            <div class="results-list">
        </HeaderTemplate>
        <ItemTemplate>
                <asp:HyperLink runat="server" ID="Link">
                    <span runat="server" ID="Photo" />
                    <span>
                        <small><asp:Literal runat="server" ID="Distance" /></small>
                        <strong><asp:Literal runat="server" ID="Title" /></strong>
                        <span>Year: <asp:Literal runat="server" ID="Year" /></span>
                        <b><asp:Literal runat="server" ID="Price" /> <em class="status"><asp:Literal runat="server" ID="Status" /></em></b>
                        
                    </span>
                </asp:HyperLink>
        </ItemTemplate>
        <FooterTemplate>
            </div>
        </FooterTemplate>
    </web:PagedRepeater>

</asp:Content>
