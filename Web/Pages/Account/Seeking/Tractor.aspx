<%@ MasterType TypeName="Agrishare.Web.Pages.Default" %>
<%@ Page Title="Seeking a tractor" Language="C#" MasterPageFile="~/Pages/Default.Master" AutoEventWireup="true" CodeBehind="Tractor.aspx.cs" Inherits="Agrishare.Web.Pages.Account.Seeking.Tractor" %>

<asp:Content ContentPlaceHolderID="Content" runat="server">
    
    <h1>Seeking a tractor</h1>

    <div id="StepFor">
        <p>Who is this booking for?</p>
        <asp:CheckBoxList runat="server" ID="For" />
    </div>

    <div id="StepService">
        <p>What type of service do you require?</p>            
        <asp:DropDownList runat="server" ID="Services" />
        <asp:RequiredFieldValidator runat="server" ControlToValidate="Services" Text="This is a required field" Display="Dynamic" />
    </div>

    <div id="StepTractorDetails">
        <p>Enter the size of the field</p>
        <asp:TextBox runat="server" ID="FieldSize" TextMode="Number" />
        <asp:RequiredFieldValidator runat="server" ControlToValidate="FieldSize" Text="This is a required field" Display="Dynamic" />
    </div>

    <div id="StepDate">
        <p>When do you require the service to be performed?</p>
        <web:Date runat="server" ID="StartDate" />
        <asp:RequiredFieldValidator runat="server" ControlToValidate="StartDate" Text="This is a required field" Display="Dynamic" />
        <asp:RegularExpressionValidator runat="server" ControlToValidate="StartDate" Text="Please enter a valid date in the format dd/MM/yyyy" ValidationExpression="^[\d]{2}/[\d]{2}/[\d]{4}?$" Display="Dynamic" />
    </div>

    <div id="StepLocation">
        <p>Where do you need the service to be performed?</p>
        <web:Map runat="server" Id="Location" />
    </div>

    <div id="StepFuel">
        <p>Should the supplier provide the fuel?</p>
        <asp:RadioButtonList runat="server" ID="Fuel">
            <asp:ListItem Text="Yes, fuel must be supplied" Value="1" />
            <asp:ListItem Text="No, I will supply my own fuel" Value="0" />
        </asp:RadioButtonList>
    </div>

    <p>
        <asp:Button runat="server" OnClick="FindListings" Text="Next" ID="SearchButton" />
    </p>

</asp:Content>
