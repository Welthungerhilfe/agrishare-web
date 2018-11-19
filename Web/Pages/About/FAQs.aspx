<%@ MasterType TypeName="Agrishare.Web.Pages.Default" %>
<%@ Page Title="FAQs" Language="C#" MasterPageFile="~/Pages/Default.Master" AutoEventWireup="true" CodeBehind="FAQs.aspx.cs" Inherits="Agrishare.Web.Pages.About.FAQs" %>

<asp:Content ContentPlaceHolderID="Content" runat="server">
    
    <h1>FAQs</h1>

    <p>Lorem ipsum dolores sit amet</p>

    <asp:Repeater runat="server" ID="List" OnItemDataBound="BindFaq">
        <HeaderTemplate>
            <div class="faq-list">
        </HeaderTemplate>
        <ItemTemplate>
                <div>
                    <p class="q"><asp:Literal runat="server" ID="Question" /></p>
                    <p class="a"><asp:Literal runat="server" ID="Answer" /></p>
                </div>
        </ItemTemplate>
        <FooterTemplate>
            </div>
        </FooterTemplate>
    </asp:Repeater>

    <h2>Get in touch</h2>

    <p>Lorem ipsum dolores sit amet.</p>

    <p><a href="/about/contact">Contact us</a></p>

</asp:Content>
