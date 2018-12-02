<%@ MasterType TypeName="Agrishare.Web.Pages.Default" %>
<%@ Page Title="Seeking a processor" Language="C#" MasterPageFile="~/Pages/Default.Master" AutoEventWireup="true" CodeBehind="Processing.aspx.cs" Inherits="Agrishare.Web.Pages.Account.Seeking.Processing" %>

<asp:Content ContentPlaceHolderID="Content" runat="server">
    
    <h1>Seeking a processor</h1>

    <p>Lorem ipsum dolores sit amet.</p>

    <div ng-controller="SearchController">

        <div class="search-form">

            <div id="StepFor" ng-show="search.step===1">
                <p>Who is this booking for?</p>
                <asp:RadioButtonList runat="server" ID="For" RepeatLayout="UnorderedList" CssClass="checkbox-list" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="For" Text="This is a required field" Display="Dynamic" ValidationGroup="Step1" />
            </div>

            <div id="StepService" ng-show="search.step===2">
                <p>What type of service do you require?</p>            
                <asp:DropDownList runat="server" ID="Services" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="Services" Text="This is a required field" Display="Dynamic" ValidationGroup="Step2" />
            </div>

            <div id="StepProcessingDetails" ng-show="search.step===3">
                <p>Enter the number of bags</p>
                <asp:TextBox runat="server" ID="NumberOfBags" MaxLength="4" TextMode="Number" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="NumberOfBags" Text="This is a required field" Display="Dynamic" ValidationGroup="Step3" />
                <asp:RegularExpressionValidator runat="server" ControlToValidate="NumberOfBags" Text="Please enter a valid weight as a number" ValidationExpression="^[\d]+(.[\d]+)?$" Display="Dynamic" ValidationGroup="Step3" />
            </div>

            <div id="StepDate" ng-show="search.step===4">
                <p>When do you require the service to be performed?</p>
                <web:Date runat="server" ID="StartDate" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="StartDate" Text="This is a required field" Display="Dynamic"  ValidationGroup="Step4"/>
                <asp:RegularExpressionValidator runat="server" ControlToValidate="StartDate" Text="Please enter a valid date in the format dd/MM/yyyy" ValidationExpression="^[\d]{2}/[\d]{2}/[\d]{4}?$" Display="Dynamic"  ValidationGroup="Step4"/>
            </div>

            <div id="StepMobile" ng-show="search.step==5">
                <p>Should the equipment be mobile</p>
                <asp:RadioButtonList runat="server" ID="Mobile">
                    <asp:ListItem Text="Yes, it should be mobile" Value="1" />
                    <asp:ListItem Text="No, I can deliver my bags" Value="0" />
                </asp:RadioButtonList>
                <asp:RequiredFieldValidator runat="server" ControlToValidate="Mobile" Text="This is a required field" Display="Dynamic" ValidationGroup="Step5" />
            </div>

            <div id="StepLocation" ng-show="search.step==6">
                <p>Where do you need the service to be performed?<br /><small>Drag the map to the desired location.</small></p>
                <web:Map runat="server" Id="Location" />
            </div>

        </div>

        <div class="cols">
            <div>
                <a ng-click="search.previous()" class="button" ng-hide="search.step===1">Back</a>
            </div>
            <div style="text-align: right">
                <a ng-click="search.next()" class="button" ng-hide="search.step===6">Next</a>
                <asp:Button runat="server" OnClick="FindListings" Text="Search" ID="SearchButton" CssClass="button" ng-show="search.step===6" />
            </div>
        </div>

    </div>

</asp:Content>
