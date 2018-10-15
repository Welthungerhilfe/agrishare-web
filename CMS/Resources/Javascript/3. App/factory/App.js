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
                title: "Admin", icon: "settings", submenu: [
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
        }
        
    };

    return App;
});