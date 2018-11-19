<%@ MasterType TypeName="Agrishare.Web.Pages.Default" %>
<%@ Page Title="Home" Language="C#" MasterPageFile="~/Pages/Default.Master" AutoEventWireup="true" CodeBehind="Homepage.aspx.cs" Inherits="Agrishare.Web.Pages.Homepage" %>

<asp:Content ContentPlaceHolderID="Content" runat="server">
    
    <div class="slideshow">
        <div>
            <h2>Lorem ipsum dolor sit amet sit</h2>
            <p>Lorem ipsum dolor sit amet, consectetuer adipiscing elit, sed diam nonummy nibh euismod tincidunt ut laoreet.</p>
            <p><a href="/">Learn more</a></p>
        </div>
        <div>
            <h2>Lorem ipsum dolor sit amet sit</h2>
            <p>Lorem ipsum dolor sit amet, consectetuer adipiscing elit, sed diam nonummy nibh euismod tincidunt ut laoreet.</p>
            <p><a href="/">Learn more</a></p>
        </div>
        <div>
            <h2>Lorem ipsum dolor sit amet sit</h2>
            <p>Lorem ipsum dolor sit amet, consectetuer adipiscing elit, sed diam nonummy nibh euismod tincidunt ut laoreet.</p>
            <p><a href="/">Learn more</a></p>
        </div>
    </div>

    <div class="touts">
        <div>
            <img src="/Resources/Images/Tout-Seeking.svg" />
            <h2>Seeking</h2>
            <p>Lorem ipsum dolor sit amet, consectetuer adipiscing elit, sed diam nonummy nibh euismod tincidunt ut laoreet.</p>
            <p><a href="/about/seeking" class="button">Register Now</a></p>
        </div>        
        <div>
            <img src="/Resources/Images/Tout-Offering.svg" />
            <h2>Offering</h2>
            <p>Lorem ipsum dolor sit amet, consectetuer adipiscing elit, sed diam nonummy nibh euismod tincidunt ut laoreet.</p>
            <p><a href="/about/offering" class="button">Register Now</a></p>
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
                <a href="/"><img src="/Resources/Images/Button-Play.svg" alt="Download on Google Play" /></a>
            </p>
        </div>
    </div>

    <div class="testimony">
        <p>Phinneas Chikombo was able to reap his tobacco on time thanks to AgriShare</p>
    </div>

</asp:Content>
