<%@ MasterType TypeName="Agrishare.Web.Pages.Default" %>
<%@ Page Title="Seeking a processor" Language="C#" MasterPageFile="~/Pages/Default.Master" AutoEventWireup="true" CodeBehind="Processing.aspx.cs" Inherits="Agrishare.Web.Pages.Account.Seeking.Processing" %>

<asp:Content ContentPlaceHolderID="Content" runat="server">
    
    <h1>Seeking a processor</h1>

    <div id="StepFor">
        <p>Who is this booking for?</p>
        <asp:CheckBoxList runat="server" ID="For" />
    </div>

    <div id="StepService">
        <p>What type of service do you require?</p>            
        <asp:DropDownList runat="server" ID="Services" />
    </div>

    <div id="StepProcessingDetails">
        <p>Enter the number of bags</p>
        <asp:TextBox runat="server" ID="NumberOfBags" />
    </div>

    <div id="StepDate">
        <p>When do you require the service to be performed?</p>
        <asp:TextBox runat="server" ID="StartDate" />
    </div>

    <div id="StepMobile">
        <p>Should the equipment be mobile</p>
        <asp:CheckBoxList runat="server" ID="Mobile">
            <asp:ListItem Text="Yes, it should be mobile" Value="1" />
            <asp:ListItem Text="No, I can deliver my bags" Value="0" />
        </asp:CheckBoxList>
    </div>

    <div id="StepLocation">
        <p>Where do you need the service to be performed?</p>
        <web:Map runat="server" Id="Location" />
    </div>

    <p>
        <asp:Button runat="server" OnClick="FindListings" Text="Next" ID="SearchButton" />
    </p>

</asp:Content>
