<%@ MasterType TypeName="Agrishare.Web.Pages.Default" %>
<%@ Page Title="Seeking a lorry" Language="C#" MasterPageFile="~/Pages/Default.Master" AutoEventWireup="true" CodeBehind="Lorry.aspx.cs" Inherits="Agrishare.Web.Pages.Account.Seeking.Lorry" %>

<asp:Content ContentPlaceHolderID="Content" runat="server">
    
    <h1>Seeking a lorry</h1>

    <div id="StepFor">
        <p>Who is this booking for?</p>
        <asp:CheckBoxList runat="server" ID="For" />
    </div>

    <div id="StepLorryDetails">
        <p>Enter the details of the load to be transported</p>
        <asp:TextBox runat="server" ID="LoadWeight" placeholder="Weight (tonnes)" /><br />        
        <asp:RequiredFieldValidator runat="server" ControlToValidate="LoadWeight" Text="This is a required field" Display="Dynamic" />
        <asp:RegularExpressionValidator runat="server" ControlToValidate="LoadWeight" Text="Please enter a valid weight as a number" ValidationExpression="^[\d]+(.[\d]+)?$" Display="Dynamic" />
        <asp:TextBox runat="server" ID="LoadDescription" TextMode="MultiLine" placeholder="Description" />        
        <asp:RequiredFieldValidator runat="server" ControlToValidate="LoadDescription" Text="This is a required field" Display="Dynamic" />
    </div>

    <div id="StepDate">
        <p>When do you require the service to be performed?</p>
        <web:Date runat="server" ID="StartDate" />
        <asp:RequiredFieldValidator runat="server" ControlToValidate="StartDate" Text="This is a required field" Display="Dynamic" />
        <asp:RegularExpressionValidator runat="server" ControlToValidate="StartDate" Text="Please enter a valid date in the format dd/MM/yyyy" ValidationExpression="^[\d]{2}/[\d]{2}/[\d]{4}?$" Display="Dynamic" />
    </div>

    <div id="StepLorryPickUp">
        <p>Where is the pick-up location?</p>
        <web:Map runat="server" Id="PickupLocation" />
    </div>

    <div id="StepLorryDropOfff">
        <p>Where is the drop-off location?</p>
        <web:Map runat="server" Id="DropoffLocation" />
    </div>

    <p>
        <asp:Button runat="server" OnClick="FindListings" Text="Next" ID="SearchButton" />
    </p>

</asp:Content>
