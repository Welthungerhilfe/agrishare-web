<%@ MasterType TypeName="Agrishare.Web.Pages.Default" %>
<%@ Page Title="Home" Language="C#" MasterPageFile="~/Pages/Default.Master" AutoEventWireup="true" CodeBehind="Homepage.aspx.cs" Inherits="Agrishare.Web.Pages.Homepage" %>

<asp:Content ContentPlaceHolderID="Content" runat="server">

    <div class="background">
        <div></div>
    </div>
    
    <div class="slideshow" ng-controller="SlideshowController">
        <div ng-class="{'visible': index==0 }">
            <h2>Linking farmers in Africa to agricultural equipment</h2>
            <p>AgriShare is an app to hire or rent out agricultural equipment between farmers and equipment manufacturers securely and with ease.</p>
            <p><a href="/account/get-started">Get Started</a></p>
        </div>
        <div ng-class="{'visible': index==1 }">
            <h2>Share Agricultural resources</h2>
            <p>Find farming equipment in your area and hire your equipment out to other farmers.</p>
            <p><a href="/account/get-started">Get Started</a></p>
        </div>
    </div>

    <div class="touts">
        <div>
            <img src="/Resources/Images/Tout-Seeking.svg" />
            <h2>Seeking</h2>
            <p>Are you looking for some agricultural equipment to hire? Look through the listings to find something in your area and check if it is available.</p>
            <p><a href="/account/get-started" class="button">Get Started</a></p>
        </div>        
        <div>
            <img src="/Resources/Images/Tout-Offering.svg" />
            <h2>Offering</h2>
            <p>Would you like to hire out your farming equipment to other farmers for extra money? Register on the app or the site, and add your equipment to hire it out.</p>
            <p><a href="/account/get-started" class="button">Get Started</a></p>
        </div>
    </div>

    <div class="apps">
        <div>
            <img src="/Resources/Images/Screenshot.png" />
        </div>
        <div>
            <h2>Download the app</h2>
            <p>Use the AgriShare app to advertise the farming equipment that you would like to hire out to other farmers. Get the easy to use AgriShare app to find out which farming equipment is available for hire or to buy. </p>
            <p>
                <a href="https://play.google.com/store/apps/details?id=app.agrishare"><img src="/Resources/Images/Google-Play.png" alt="Download on Google Play" /></a>
            </p>
        </div>
    </div>

    <div class="testimony">
        <p><span>Phinneas Chikombo was able to reap his tobacco on time thanks to AgriShare</span></p>
    </div>

</asp:Content>
