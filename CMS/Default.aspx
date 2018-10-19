<%@ Page CodeBehind="Default.aspx.cs" Inherits="Agrishare.CMS.Default" %>

<!DOCTYPE html>
<html ng-app="agrishareApp">
<head runat="server">
    <meta charset="utf-8" />
    <base href="/" />
    <link rel="stylesheet" href="https://fonts.googleapis.com/css?family=Roboto:300,400,500,700,400italic" />
    <link href="https://fonts.googleapis.com/icon?family=Material+Icons" rel="stylesheet" />
    <title>AgriShare CMS</title>
    <meta content="IE=edge, chrome=1" http-equiv="X-UA-Compatible" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <meta name="SKYPE_TOOLBAR" content="SKYPE_TOOLBAR_PARSER_COMPATIBLE" />
</head>
<body ng-controller="AppController" id="Body" runat="server">
    
    <toast></toast>

    <div id="app">

        <nav>
            <h1>
                <span>AgriShare</span>            
                <a class="material-icons" ng-click="app.menuVisible=true" ng-if="!app.menuVisible">menu</a>
                <a class="material-icons" ng-click="app.menuVisible=false" ng-if="app.menuVisible">close</a>
            </h1>
            <ul ng-class="{'visible': app.menuVisible}">
                <li ng-repeat="item in app.menu" ng-class="{'selected': item.url==app.selectedUrl, 'active': item.active}">
                    <a ng-href="{{item.url}}" ng-click="item.active=!item.active">
                        <i class="material-icons">{{item.icon}}</i>
                        <span>{{item.title}}</span>
                        <i ng-if="item.submenu" class="material-icons">keyboard_arrow_down</i>
                    </a>
                    <ul ng-if="item.submenu">
                        <li ng-repeat="subitem in item.submenu" ng-class="{'selected': subitem.url==app.selectedUrl }">
                            <a ng-href="{{subitem.url}}">{{subitem.title}}</a>
                        </li>
                    </ul>
                </li>
            </ul>
            <p ng-class="{'visible': app.menuVisible}">
                <i class="material-icons">verified_user</i>
                <strong>{{app.user.FirstName || 'Guest'}}</strong>
                <a href="/default.aspx?logout=true"> 
                    <span>Logout</span>
                </a> 
            </p>
        </nav>

        <ui-view />

    </div>

    <div class="gl-lightbox" ng-if="app.slideshow.visible">
        <div class="titlebar">
            <div>
                Photo
            </div>
            <div>
                <gl-icon-button gl-icon="keyboard_arrow_left" ng-click="app.slideshow.previous()" ng-if="app.slideshow.photos.length>1"></gl-icon-button>
                <gl-icon-button gl-icon="keyboard_arrow_right" ng-click="app.slideshow.next()" ng-if="app.slideshow.photos.length>1"></gl-icon-button>
                <gl-icon-button gl-icon="cancel" ng-click="app.slideshow.hide()"></gl-icon-button>
            </div>
        </div>
        <div class="photo">
            <span class="spinner"></span>
            <div ng-repeat="photo in app.slideshow.photos" style="background-image:url({{photo.Zoom}})" ng-class="{'visible':$index==app.slideshow.currentIndex}"></div>                
        </div>
    </div>

</body>
</html>
