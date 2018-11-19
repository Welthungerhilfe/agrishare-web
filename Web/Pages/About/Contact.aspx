<%@ MasterType TypeName="Agrishare.Web.Pages.Default" %>
<%@ Page Title="Contact Us" Language="C#" MasterPageFile="~/Pages/Default.Master" AutoEventWireup="true" CodeBehind="Contact.aspx.cs" Inherits="Agrishare.Web.Pages.About.Contact" %>

<asp:Content ContentPlaceHolderID="Content" runat="server">
    
    <h1>Contact Us</h1>

    <p>Lorem ipsum dolores sit amet</p>

    <div class="form-row">
        <asp:Label runat="server" AssociatedControlID="Name" Text="Name *" />
        <asp:TextBox runat="server" ID="Name" />
        <asp:RequiredFieldValidator runat="server" ControlToValidate="Name" Text="Name is required" Display="Dynamic" />
    </div>

    <div class="form-row">
        <asp:Label runat="server" AssociatedControlID="EmailAddress" Text="Email address *" />
        <asp:TextBox runat="server" ID="EmailAddress" />
        <asp:RequiredFieldValidator runat="server" ControlToValidate="EmailAddress" Text="Email address is required" Display="Dynamic" />
    </div>

    <div class="form-row">
        <asp:Label runat="server" AssociatedControlID="Telephone" Text="Telephone" />
        <asp:TextBox runat="server" ID="Telephone" />
    </div>

    <div class="form-row">
        <asp:Label runat="server" AssociatedControlID="Message" Text="Message *" />
        <asp:TextBox runat="server" ID="Message" TextMode="MultiLine" />
        <asp:RequiredFieldValidator runat="server" ControlToValidate="Message" Text="Message is required" Display="Dynamic" />
    </div>

    <p>
        <asp:Button runat="server" Text="Send" CssClass="button" OnClick="SendMessage" />
    </p>

    <h2>Faqs</h2>

    <p>Lorem ipsum dolores sit amet.</p>

    <p><a href="/about/faqs">Read the FAQs</a></p>


</asp:Content>
