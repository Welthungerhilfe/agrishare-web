<%@ MasterType TypeName="Agrishare.Web.Pages.Default" %>
<%@ Page Title="Profile" Language="C#" MasterPageFile="~/Pages/Default.Master" AutoEventWireup="true" CodeBehind="Profile.aspx.cs" Inherits="Agrishare.Web.Pages.Account.Profile" %>

<asp:Content ContentPlaceHolderID="Content" runat="server">
    
    <h1>My Profile</h1>

    <p>
        <strong><asp:Literal runat="server" ID="DisplayName" /></strong><br />
        <em><asp:Literal runat="server" ID="DisplayTelephone" /></em><br />
        <em><asp:Literal runat="server" ID="DisplayEmailAddress" /></em>
    </p>

    <asp:PlaceHolder runat="server" ID="EditProfileForm" Visible="false">

        <h2>Edit Profile</h2>

        <div class="form-cols">

            <div class="form-row">
                <asp:Label runat="server" AssociatedControlID="FirstName" Text="First Name *" />
                <asp:TextBox runat="server" ID="FirstName" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="FirstName" Text="First name is required" Display="Dynamic" ValidationGroup="Edit" />
            </div>

            <div class="form-row">
                <asp:Label runat="server" AssociatedControlID="LastName" Text="Last Name *" />
                <asp:TextBox runat="server" ID="LastName" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="LastName" Text="Last name is required" Display="Dynamic" ValidationGroup="Edit" />
            </div>

        </div>

        <div class="form-cols">

            <div class="form-row">
                <asp:Label runat="server" AssociatedControlID="EmailAddress" Text="Email Address *" />
                <asp:TextBox runat="server" ID="EmailAddress" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="EmailAddress" Text="Email address is required" Display="Dynamic" ValidationGroup="Edit" />
            </div>

            <div class="form-row">
                <asp:Label runat="server" AssociatedControlID="Telephone" Text="Telephone *" />
                <asp:TextBox runat="server" ID="Telephone" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="Telephone" Text="Telephone is required" Display="Dynamic" ValidationGroup="Edit" />
            </div>

        </div>

        <div class="form-cols">

            <div class="form-row">
                <asp:Label runat="server" AssociatedControlID="Gender" Text="Gender *" />
                <asp:DropDownList runat="server" ID="Gender" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="Gender" Text="Gender is required" Display="Dynamic" ValidationGroup="Edit" />
            </div>

            <div class="form-row">
                <asp:Label runat="server" AssociatedControlID="DateOfBirth" Text="Date of birth *" />
                <asp:TextBox runat="server" ID="DateOfBirth" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="DateOfBirth" Text="Date of birth is required" Display="Dynamic" ValidationGroup="Edit" />
            </div>

        </div>

        <p>
            <asp:Button runat="server" Text="Update" CssClass="button" OnClick="UpdateUser" ValidationGroup="Edit" />
        </p>

    </asp:PlaceHolder>

    <asp:PlaceHolder runat="server" ID="NotificationPreferencesForm" Visible="false">

        <h2>Update Notification Preferences</h2>
        
        <div class="form-row">
            <asp:CheckBox runat="server" ID="SMS" Text="SMS" />
        </div>
        <div class="form-row">
            <asp:CheckBox runat="server" ID="PushNotifications" Text="Push Notifications" />
        </div>
        <div class="form-row">
            <asp:CheckBox runat="server" ID="Email" Text="Email" />
        </div>        

        <p>
            <asp:Button runat="server" Text="Update" CssClass="button" OnClick="UpdateNotificationPreferences" ValidationGroup="Notifications" />
        </p>

    </asp:PlaceHolder>

    <asp:PlaceHolder runat="server" ID="ResetForm" Visible="false">

        <h2>Reset Password</h2>

        <div class="form-row">
            <asp:Label runat="server" AssociatedControlID="ResetTelephone" Text="Telephone *" />
            <asp:TextBox runat="server" ID="ResetTelephone" />
            <asp:RequiredFieldValidator runat="server" ControlToValidate="ResetTelephone" Text="Telephone is required" Display="Dynamic" ValidationGroup="Reset" />
        </div>

        <div class="form-row">
            <asp:Label runat="server" AssociatedControlID="ResetCode" Text="Verification Code *" />
            <asp:TextBox runat="server" ID="ResetCode" />
            <asp:RequiredFieldValidator runat="server" ControlToValidate="ResetCode" Text="Verification code is required" Display="Dynamic" ValidationGroup="Reset" />
        </div>

        <div class="form-row">
            <asp:Label runat="server" AssociatedControlID="ResetPIN" Text="New PIN *" />
            <asp:TextBox runat="server" ID="ResetPIN" />
            <asp:RequiredFieldValidator runat="server" ControlToValidate="ResetPIN" Text="New PIN is required" Display="Dynamic" ValidationGroup="Reset" />
        </div>

        <p>
            <asp:Button runat="server" Text="Reset" CssClass="button" OnClick="UpdatePassword" ValidationGroup="Reset" />
        </p>

    </asp:PlaceHolder>

    <asp:PlaceHolder ID="DeleteForm" runat="server" Visible="false">

        <h2>Delete my account</h2>

        <asp:PlaceHolder ID="DeletePrompt" runat="server" Visible="true">
            <p>Are you sure you would like to delete your account?</p>
            <p>
                <asp:LinkButton runat="server" OnClick="DeleteAccount">Yes, delete my account</asp:LinkButton>
            </p>
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="DeleteWarning" runat="server" Visible="true">
            <p>You can not delete your account while you have active or pending bookings. Please cancel your bookings first.</p>
        </asp:PlaceHolder>

    </asp:PlaceHolder>

    <ul>
        <li><a href="/account/profile/edit">Edit Profile</a></li>
        <li><a href="/account/profile/notifications">Notification Preferences</a></li>
        <li><a href="/account/profile/resetpin">Reset PIN</a></li>
        <li><asp:LinkButton runat="server" OnClick="Logout">Logout</asp:LinkButton></li>
    </ul>

    <p>
        <a href="/account/profile/delete">Delete my account</a>
    </p>

</asp:Content>
