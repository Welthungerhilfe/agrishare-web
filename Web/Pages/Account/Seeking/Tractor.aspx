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
    </div>

    <div id="StepTractorDetails">
        <p>Enter the size of the field</p>
        <asp:TextBox runat="server" ID="FieldSize" />
    </div>

    <div id="StepDate">
        <p>When do you require the service to be performed?</p>
        <asp:TextBox runat="server" ID="StartDate" />
    </div>

    <div id="StepLocation">
        <p>Where do you need the service to be performed?</p>
        <web:Map runat="server" Id="Location" />
    </div>

    <div id="StepFuel">
        <p>Should the supplier provide the fuel?</p>
        <asp:CheckBoxList runat="server" ID="Fuel">
            <asp:ListItem Text="Yes, fuel must be supplied" Value="1" />
            <asp:ListItem Text="No, I will supply my own fuel" Value="0" />
        </asp:CheckBoxList>
    </div>

    <p>
        <asp:Button runat="server" OnClick="FindListings" Text="Next" ID="SearchButton" />
    </p>

</asp:Content>
