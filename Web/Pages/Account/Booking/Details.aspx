<%@ MasterType TypeName="Agrishare.Web.Pages.Default" %>
<%@ Page Title="Booking Details" Language="C#" MasterPageFile="~/Pages/Default.Master" AutoEventWireup="true" CodeBehind="Details.aspx.cs" Inherits="Agrishare.Web.Pages.Account.Booking.Details" %>

<asp:Content ContentPlaceHolderID="Content" runat="server">
    
    <h1><asp:Literal runat="server" ID="BookingTitle" /></h1>
    <p><asp:HyperLink runat="server" ID="Reviews" /></p>

    <asp:Repeater runat="server" ID="Gallery" OnItemDataBound="BindPhoto">
        <HeaderTemplate>
            <ul>
        </HeaderTemplate>
        <ItemTemplate>
                <li>
                    <asp:HyperLink runat="server" ID="Thumb" />
                </li>
        </ItemTemplate>
        <FooterTemplate>
            </ul>
        </FooterTemplate>
    </asp:Repeater>

    <p><asp:Literal runat="server" ID="Description" /></p>

    <ul>
        <li><strong>Brand</strong><span><asp:Literal runat="server" ID="Brand" /></span></li>
        <li><strong>Horse Power</strong><span><asp:Literal runat="server" ID="HorsePower" /></span></li>
        <li><strong>Year</strong><span><asp:Literal runat="server" ID="Year" /></span></li>
    </ul>

    <div class="booking" ng-controller="AvailabilityController">

        <ul>
            <li>
                <strong>Dates</strong>
                <span><asp:Literal runat="server" ID="Dates" /> <small runat="server" id="Unavaiable">Not available</small></span>
                <em>This service will take <asp:Literal runat="server" ID="Days" />.</em>
                <asp:HyperLink runat="server" ID="Availability" ng-click="calendar.show()">View Availability</asp:HyperLink>
            </li>
            <li>
                <strong>Transport Cost</strong>
                <span><asp:Literal runat="server" ID="TransportDistance" /></span>
                <span><asp:Literal runat="server" ID="TransportCost" /></span>
            </li>
            <li>
                <strong>Hire Cost</strong>
                <span><asp:Literal runat="server" ID="HireSize" /></span>
                <span><asp:Literal runat="server" ID="HireCost" /></span>
            </li>
            <li>
                <strong>Fuel Cost</strong>
                <span><asp:Literal runat="server" ID="FuelSize" /></span>
                <span><asp:Literal runat="server" ID="FuelCost" /></span>
            </li>
            <li runat="server" id="CommissionRow">
                <strong>Extras</strong>
                <span>AgriShare Commission</span>
                <span><asp:Literal runat="server" ID="Commission" /></span>
            </li>
            
            <li>
                <strong>Total</strong>
                <span><asp:Literal runat="server" ID="Total" /></span>
            </li>
        </ul>
        
        <div ng-show="calendar.visible">

            <asp:TextBox runat="server" ID="AvailabilityDays" ng-model="calendar.days" CssClass="hidden" />
            <asp:TextBox runat="server" ID="ListingId" ng-model="calendar.listingId" CssClass="hidden" />
            <asp:TextBox runat="server" ID="StartDate" ng-model="calendar.startDate" CssClass="hidden" />

            <div>
                <a ng-click="calendar.previous()">Prev</a>
                <strong>{{calendar.month}}</strong>
                <a ng-click="calendar.next()">Next</a>
            </div>

            <ul ng-repeat="item in calendar.dates">
                <li ng-click="calendar.setStartDate(item.Available, item.Date)" ng-class="{'available' : item.Available}">{{item.Date | date : 'd MMMM yyyy'}}</li>
            </ul>

        </div>

    </div>

    <div class="action">
        
        <p runat="server" id="RequestPanel" visible="false">
            <asp:Button runat="server" ID="SubmitButton" OnClick="SendRequest" Text="Send Request" CssClass="button" />
        </p>

        <p runat="server" id="AwaitingConfirmPanel" visible="false">
            <strong>Your request has been sent.</strong> Waiting for feedback from supplier.
        </p>
        
        <p runat="server" id="PendingPanel" visible="false">
            <asp:Button runat="server" ID="ConfirmButton" OnClick="ConfirmBooking" Text="Confirm" CssClass="button" />
            <asp:Button runat="server" ID="DeclineButton" OnClick="DeclineBooking" Text="Decline" CssClass="button" />
        </p>
        
        <p runat="server" id="AwaitingPaymentPanel" visible="false">
            <strong>Booking has been confirmed.</strong> Waiting for payment from user.
        </p>

        <p runat="server" id="DeclinedPanel" visible="false">
            <strong>This booking has been declined.</strong>
        </p>

        <div runat="server" id="PaymentPanel" visible="false">

            <p><strong>Make payment</strong></p>

            <p>Please enter the EcoCash number below and then check phone for further instructions.</p>

            <div class="form-row">
                <asp:Label runat="server" AssociatedControlID="PayerName" Text="Name" />
                <asp:TextBox runat="server" ID="PayerName" MaxLength="64" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="PayerName" Text="Name is required" ValidationGroup="Payment" />
            </div>

            <div class="form-row">
                <asp:Label runat="server" AssociatedControlID="PayerMobileNumber" Text="EcoCash Number" />
                <asp:TextBox runat="server" ID="PayerMobileNumber" MaxLength="10" placeholder="07" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="PayerMobileNumber" Text="EcoCash Number is required" ValidationGroup="Payment" />
            </div>

            <p>
                <asp:Button runat="server" OnClick="InitiatePayment" Text="Submit" CssClass="button" ValidationGroup="Payment" />
            </p>

        </div>

        <div runat="server" id="GroupPaymentPanel" visible="false">
            
            <p><strong>Make payment</strong></p>

            <p>Please enter the group details using the form below.</p>

            <asp:TextBox runat="server" ID="BookingUsers" ng-model="group.users" CssClass="hidden" />

            <div ng-repeat="item in group.users" class="form-col">                
                <div class="form-row">                    
                    <input type="text" maxlength="64" ng-model="item.Name" placeholder="Name" ng-disabled="item.Id"/>
                </div>
                <div class="form-row">                    
                    <input type="text" maxlength="10" ng-model="item.Number" placeholder="EcoCash Number" ng-disabled="item.Id"/>
                </div>
                <div class="form-row">                    
                    <input type="text" maxlength="8" ng-model="item.Quantity" placeholder="Quantity" ng-disabled="item.Id"/>
                </div>
                <div>
                    <a ng-click="group.users.splice($index, 1)">Remove</a>
                </div>
            </div>

            <div>
                <a ng-click="group.users.push({})">Add user</a>
            </div>

            <p>
                <asp:Button runat="server" OnClick="InitiatePayment" Text="Submit" CssClass="button" />
            </p>

        </div>

        <div runat="server" id="PaymentProgressPanel" visible="false" ng-controller="PollController">

            <p><strong>Checking for payment update</strong></p>
            <p>Please check your phone for a payment prompt. If you do not see a payment prompt, please dial *151*2*4#.</p>
            <p>You will be redirected automatically when payment is complete.</p>

            <ul class="transactions">
                <li ng-repeat="item in transactions">
                    <strong>{{item.BookingUser.Name}}</strong>
                    <em>{{item.Status}}</em>
                </li>
            </ul>

        </div>
        
        <p runat="server" id="PaidPanel" visible="false">
            <strong>Payment received.</strong> Booking is confirmed and paid.
        </p>
        
        <div runat="server" id="InProgressPanel" visible="false">

            <p><strong>Service is in progress.</strong></p>

            <ul ng-hide="service.rate || service.incomplete">
                <li><a ng-click="service.rate=true">Has this service been completed? Please leave a review.</a></li>
                <li><a ng-click="service.incomplete=true">Was there an issue and the service was not completed? Please contact us.</a></li>
            </ul>            

            <div ng-show="service.rate">
                <div>
                    <asp:TextBox runat="server" ID="RatingStars" CssClass="hidden" ng-model="rating.stars" ng-init="rating.stars=3" />
                    <ul class="rating">
                        <li ng-click="rating.stars=1" ng-class="{'star':rating.stars<=1}"></li>
                        <li ng-click="rating.stars=2" ng-class="{'star':rating.stars<=2}"></li>
                        <li ng-click="rating.stars=3" ng-class="{'star':rating.stars<=3}"></li>
                        <li ng-click="rating.stars=4" ng-class="{'star':rating.stars<=4}"></li>
                        <li ng-click="rating.stars=5" ng-class="{'star':rating.stars<=5}"></li>
                    </ul>
                </div>
                <div class="form-row">
                    <asp:Label runat="server" AssociatedControlID="RatingComments" Text="Comments" />
                    <asp:TextBox runat="server" ID="RatingComments" TextMode="MultiLine" />
                </div>
                <p>
                    <asp:Button runat="server"  OnClick="SubmitRating" Text="Submit" CssClass="button" />
                </p>
            </div>

            <div ng-show="service.incomplete">
                <div class="form-row">
                    <asp:Label runat="server" AssociatedControlID="Complaint" Text="Comments" />
                    <asp:TextBox runat="server" ID="Complaint" TextMode="MultiLine" />
                </div>
                <p>
                    <asp:Button runat="server" OnClick="SubmitComplaint" Text="Submit" CssClass="button" />
                </p>
            </div>
            
        </div>
        
        <p runat="server" id="IncompletePanel" visible="false">
            <strong>Service not completed.</strong> The user has flagged this booking as not being completed. AgriShare will get in touch to resolve the issue.
        </p>
        
        <div runat="server" id="CompletePanel" visible="false">
            <p><strong>Service is complete.</strong></p>
            <div class="review-list" runat="server" id="Review" visible="false">
                <div>
                    <strong runat="server" id="ReviewStars"><asp:Literal runat="server" ID="ReviewUser" /></strong>
                    <p><asp:Literal runat="server" ID="ReviewComments" /></p>
                    <small><asp:Literal runat="server" ID="ReviewDate" /></small>                    
                </div>
            </div>
        </div>

    </div>
        
    

</asp:Content>
