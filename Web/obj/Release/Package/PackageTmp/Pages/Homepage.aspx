<%@ MasterType TypeName="Agrishare.Web.Pages.Default" %>
<%@ Page Title="Home" Language="C#" MasterPageFile="~/Pages/Default.Master" AutoEventWireup="true" CodeBehind="Homepage.aspx.cs" Inherits="Agrishare.Web.Pages.Homepage" %>

<asp:Content ContentPlaceHolderID="Content" runat="server">

    <div class="background">
        <div></div>
    </div>
    
    <div class="slideshow" ng-controller="SlideshowController">
        <div ng-class="{'visible': index==0 }">
            <h2>Lorem ipsum dolor sit amet sit</h2>
            <p>Lorem ipsum dolor sit amet, consectetuer adipiscing elit, sed diam nonummy nibh euismod tincidunt ut laoreet.</p>
            <p><a href="/about/offering">Learn more</a></p>
        </div>
        <div ng-class="{'visible': index==1 }">
            <h2>Ipsum dolor lorem sit amet sit</h2>
            <p>Lorem ipsum dolor sit amet, consectetuer adipiscing elit, sed diam nonummy nibh euismod tincidunt ut laoreet.</p>
            <p><a href="/about/seeking">Learn more</a></p>
        </div>
        <div ng-class="{'visible': index==2 }">
            <h2>Doloe ipsum dolor sit amet sit</h2>
            <p>Lorem ipsum dolor sit amet, consectetuer adipiscing elit, sed diam nonummy nibh euismod tincidunt ut laoreet.</p>
            <p><a href="/account/get-started">Register Now</a></p>
        </div>
    </div>

    <div class="touts">
        <div>
            <img src="/Resources/Images/Tout-Seeking.svg" />
            <h2>Seeking</h2>
            <p>Lorem ipsum dolor sit amet, consectetuer adipiscing elit, sed diam nonummy nibh euismod tincidunt ut laoreet.</p>
            <p><a href="/about/seeking" class="button">Learn more</a></p>
        </div>        
        <div>
            <img src="/Resources/Images/Tout-Offering.svg" />
            <h2>Offering</h2>
            <p>Lorem ipsum dolor sit amet, consectetuer adipiscing elit, sed diam nonummy nibh euismod tincidunt ut laoreet.</p>
            <p><a href="/about/offering" class="button">Learn more</a></p>
        </div>
    </div>

    <div class="apps">
        <div>
            <img src="/Resources/Images/Screenshot.png" />
        </div>
        <div>
            <h2>Download the app</h2>
            <p>Lorem ipsum dolor sit amet, consectetuer adipiscing elit, sed diam nonummy nibh euismod tincidunt ut laoreet dolore magna aliquam erat volutpat.</p>
            <p>
                <a href="https://play.google.com/store/apps/details?id=app.agrishare"><img src="/Resources/Images/Google-Play.png" alt="Download on Google Play" /></a>
            </p>
        </div>
    </div>

    <div class="testimony">
        <p><span>Phinneas Chikombo was able to reap his tobacco on time thanks to AgriShare</span></p>
    </div>

</asp:Content>
