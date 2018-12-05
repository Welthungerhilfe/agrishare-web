/* Title: Gloo Framework
 * Author: Bradley Searle (C2 Digital)
 * Source: www.c2.co.zw
 * License: CC BY 4.0 (https://creativecommons.org/licenses/by/4.0/legalcode)
 */

agrishareApp.factory('App', function ($location, Utils) {

    var App = {
        apiUrl: '',
        user: {},        

        title: '',
        status: '',
        selectedUrl: '',
        returnUrl: '',

        menu: [
            { title: "Dashboard", icon: "assessment", url: "#/dashboard" },
            { title: "Listings", icon: "local_shipping", url: "#/listings/list" },
            { title: "Bookings", icon: "account_balance_wallet", url: "#/bookings/list" },
            { title: "Users", icon: "person", url: "#/users/list" },
            {
                title: "Deleted", icon: "delete_sweep", submenu: [
                    { title: "Listings", url: "#/deleted/listings/list" },
                    { title: "Users", url: "#/deleted/users/list" }
                ]
            },
            {
                title: "Admin", icon: "settings", submenu: [
                    { title: "Blog", url: "#/blogs/list" },
                    { title: "FAQs", url: "#/faqs/list" },
                    { title: "Log", url: "#/log/list" },
                    { title: "Settings", url: "#/settings/list" },
                    { title: "Templates", url: "#/templates/list" }
                ]
            }
        ],

        authorizationHeader: function () {
            return {
                "Authorization": App.user.AuthToken,
                'UTCOffset': moment().utcOffset()
            };
        },

        returnUrlQs: function () {
            return 'return=' + encodeURIComponent(location.hash);
        },

        go: function (url) {
            location.hash = url;
        },

        slideshow: {

            visibile: false,
            title: 'Slideshow',
            currentIndex: 0,
            photos: [],

            show: function (photos, index = 0) {
                var slideshow = this;

                if (photos.length)
                    slideshow.photos = photos;
                else
                    slideshow.photos = [photos];

                slideshow.currentIndex = index;
                slideshow.visible = true;
            },

            hide: function () {
                var slideshow = this;
                slideshow.visible = false;
            },

            previous: function () {
                var slideshow = this;
                slideshow.currentIndex += 1;
                if (slideshow.currentIndex >= slideshow.photos.length)
                    slideshow.currentIndex = 0;
            },

            next: function () {
                var slideshow = this;
                slideshow.currentIndex -= 1;
                if (slideshow.currentIndex < 0)
                    slideshow.currentIndex = slideshow.photos.length - 1;
            }

        }
        
    };

    return App;
});