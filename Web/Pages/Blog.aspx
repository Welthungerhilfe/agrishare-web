<%@ MasterType TypeName="Agrishare.Web.Pages.Default" %>
<%@ Page Title="Blog" Language="C#" MasterPageFile="~/Pages/Default.Master" AutoEventWireup="true" CodeBehind="Blog.aspx.cs" Inherits="Agrishare.Web.Pages.Blog" %>

<asp:Content ContentPlaceHolderID="Content" runat="server">
    
    <h1><asp:Literal runat="server" Id="PageTitle" Text="Blog" /></h1>
    <p runat="server" id="Introduction">Catch up on the latest news and updates from the AgriShare team.</p>
    
    <div class="cols">

        <div>

            <web:PagedRepeater runat="server" ID="BlogList" OnItemDataBound="BindBlog" EmptyMessage="There are no blog posts yet!" Visible="false">
                <HeaderTemplate>
                    <div class="blog-list">
                </HeaderTemplate>
                <ItemTemplate>
                        <asp:HyperLink runat="server" id="Link">
                            <span>
                                <strong><asp:Literal runat="server" ID="Title" /></strong>
                                <small><asp:Literal runat="server" ID="Date" /></small>
                            </span>
                        </asp:HyperLink>
                </ItemTemplate>
                <FooterTemplate>
                    </div>
                </FooterTemplate>
            </web:PagedRepeater>

            <asp:PlaceHolder runat="server" ID="BlogDetail" Visible="false">
                <p><small><asp:Literal runat="server" ID="BlogDate" /></small></p>
                <p><asp:Image runat="server" ID="BlogPhoto" CssClass="hero" /></p>
                <asp:Literal runat="server" ID="BlogContent" />
            </asp:PlaceHolder>

        </div>

        <div>

            <asp:Repeater runat="server" ID="BlogLinks" OnItemDataBound="BindLink">
                <HeaderTemplate>
                    <h3>Recent Blog Posts</h3>
                    <ul class="blog-links">
                </HeaderTemplate>
                <ItemTemplate>
                        <li><asp:HyperLink runat="server" id="Link" /></li>
                </ItemTemplate>
                <FooterTemplate>
                    </ul>
                </FooterTemplate>
            </asp:Repeater>

            <h3>Get in touch</h3>

            <p>We'd love to hear from you. Fill out this simple form and we'll get in touch with you.</p>

            <p><a href="/about/contact" class="button">Contact us</a></p>

        </div>

    </div>

</asp:Content>
