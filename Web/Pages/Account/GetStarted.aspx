<%@ MasterType TypeName="Agrishare.Web.Pages.Default" %>
<%@ Page Title="Get Started" Language="C#" MasterPageFile="~/Pages/Default.Master" AutoEventWireup="true" CodeBehind="GetStarted.aspx.cs" Inherits="Agrishare.Web.Pages.Account.GetStarted" %>

<asp:Content ContentPlaceHolderID="Content" runat="server">
    
    <h1>Get Started</h1>

    <p>Lorem ipsum dolores sit amet.</p>

    <div class="cols">

        <div>

            <asp:PlaceHolder runat="server" ID="RegisterForm" Visible="false">

                <h2>Register Now</h2>

                <div class="form-cols">

                    <div class="form-row">
                        <asp:Label runat="server" AssociatedControlID="FirstName" Text="First Name *" />
                        <asp:TextBox runat="server" ID="FirstName" MaxLength="64" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="FirstName" Text="First name is required" Display="Dynamic" ValidationGroup="Register" />
                    </div>

                    <div class="form-row">
                        <asp:Label runat="server" AssociatedControlID="LastName" Text="Last Name *" />
                        <asp:TextBox runat="server" ID="LastName" MaxLength="64" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="LastName" Text="Last name is required" Display="Dynamic" ValidationGroup="Register" />
                    </div>

                </div>

                <div class="form-row">
                    <asp:Label runat="server" AssociatedControlID="EmailAddress" Text="Email Address *" />
                    <asp:TextBox runat="server" ID="EmailAddress" MaxLength="1024" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="EmailAddress" Text="Email address is required" Display="Dynamic" ValidationGroup="Register" />
                </div>

                <div class="form-cols">

                    <div class="form-row">
                        <asp:Label runat="server" AssociatedControlID="Telephone" Text="Telephone *" />
                        <asp:TextBox runat="server" ID="Telephone" MaxLength="16" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="Telephone" Text="Telephone is required" Display="Dynamic" ValidationGroup="Register" />
                    </div>

                    <div class="form-row">
                        <asp:Label runat="server" AssociatedControlID="PIN" Text="PIN *" />
                        <asp:TextBox runat="server" ID="PIN" MaxLength="4" TextMode="Password" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="PIN" Text="PIN is required" Display="Dynamic" ValidationGroup="Register" />
                    </div>

                </div>

                <div class="form-cols">

                    <div class="form-row">
                        <asp:Label runat="server" AssociatedControlID="Gender" Text="Gender *" />
                        <asp:DropDownList runat="server" ID="Gender" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="Gender" Text="Gender is required" Display="Dynamic" ValidationGroup="Register" />
                    </div>

                    <div class="form-row">
                        <asp:Label runat="server" AssociatedControlID="DateOfBirth" Text="Date of birth *" />
                        <asp:TextBox runat="server" ID="DateOfBirth" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="DateOfBirth" Text="Date of birth is required" Display="Dynamic" ValidationGroup="Register" />
                    </div>

                </div>

                <p>
                    <asp:Button runat="server" Text="Register" CssClass="button" OnClick="RegisterUser" ValidationGroup="Register" />
                </p>

            </asp:PlaceHolder>

            <asp:PlaceHolder runat="server" ID="ForgotForm" Visible="false">

                <h2>Forgot PIN</h2>

                <div class="form-row">
                    <asp:Label runat="server" AssociatedControlID="ForgotTelephone" Text="Telephone *" />
                    <asp:TextBox runat="server" ID="ForgotTelephone" MaxLength="10" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="ForgotTelephone" Text="Telephone is required" Display="Dynamic" ValidationGroup="Forgot" />
                </div>

                <p>
                    <asp:Button runat="server" Text="Send Code" CssClass="button" OnClick="SendResetCode" ValidationGroup="Forgot" />
                </p>

            </asp:PlaceHolder>

            <asp:PlaceHolder runat="server" ID="ResetForm" Visible="false">

                <h2>Reset Password</h2>

                <div class="form-row">
                    <asp:Label runat="server" AssociatedControlID="ResetTelephone" Text="Telephone *" />
                    <asp:TextBox runat="server" ID="ResetTelephone" MaxLength="10" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="ResetTelephone" Text="Telephone is required" Display="Dynamic" ValidationGroup="Reset" />
                </div>

                <div class="form-row">
                    <asp:Label runat="server" AssociatedControlID="ResetCode" Text="Verification Code *" />
                    <asp:TextBox runat="server" ID="ResetCode" MaxLength="4" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="ResetCode" Text="Verification code is required" Display="Dynamic" ValidationGroup="Reset" />
                </div>

                <div class="form-row">
                    <asp:Label runat="server" AssociatedControlID="ResetPIN" Text="New PIN *" />
                    <asp:TextBox runat="server" ID="ResetPIN" MaxLength="4" TextMode="Password" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="ResetPIN" Text="New PIN is required" Display="Dynamic" ValidationGroup="Reset" />
                </div>

                <p>
                    <asp:Button runat="server" Text="Reset" CssClass="button" OnClick="UpdatePassword" ValidationGroup="Reset" />
                </p>

            </asp:PlaceHolder>

        </div>

        <div>

            <asp:PlaceHolder runat="server" ID="LoginForm" Visible="false">

                <h2>Existing Users</h2>

                <div class="form-row">
                    <asp:Label runat="server" AssociatedControlID="LoginTelephone" Text="Telephone *" />
                    <asp:TextBox runat="server" ID="LoginTelephone" MaxLength="10" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="LoginTelephone" Text="Telephone is required" Display="Dynamic" ValidationGroup="Login" />
                </div>

                <div class="form-row">
                    <asp:Label runat="server" AssociatedControlID="LoginPIN" Text="PIN *" />
                    <asp:TextBox runat="server" ID="LoginPIN" MaxLength="4" TextMode="Password" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="LoginPIN" Text="PIN is required" Display="Dynamic" ValidationGroup="Login" />
                </div>

                <p>
                    <asp:Button runat="server" Text="Login" CssClass="button" OnClick="AuthenticateUser" ValidationGroup="Login" />
                </p>

                <p>
                    <strong>Forgot your PIN?</strong>
                    Click <a href="/account/get-started?view=forgot">here</a> to reset your PIN.
                </p>

            </asp:PlaceHolder>

        </div>

    </div>

</asp:Content>
