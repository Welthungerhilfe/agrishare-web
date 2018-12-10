/* Title: Gloo Framework
 * Author: Bradley Searle (C2 Digital)
 * Source: www.c2.co.zw
 * License: CC BY 4.0 (https://creativecommons.org/licenses/by/4.0/legalcode)
 */

agrishareApp.controller('DashboardViewController', function ($attrs, $controller, $http, $scope, App, Utils) {

    $controller('ViewController', { $scope: $scope, $attrs: $attrs });

    var dashboard = this;
    dashboard.apiUrl = App.apiUrl + $attrs.agApiRoute;
    dashboard.data = {};
    dashboard.busy = true;
    dashboard.error = '';
    dashboard.loading = true;

    dashboard.activity = {
        filter: {
            type: 'User',
            timespan: 7,
            category: 0
        }
    };

    dashboard.fetch = function () {

        App.status = "Loading...";
        dashboard.busy = true;

        var url = dashboard.apiUrl + '?type=' + dashboard.activity.filter.type + '&timespan=' + dashboard.activity.filter.timespan + '&category=' + dashboard.activity.filter.category;

        $http({
            url: url,
            headers: App.authorizationHeader()
        })
        .then(function (response) {
            dashboard.data = response.data;
            dashboard.busy = false;
            dashboard.loading = false;
            App.status = "Done";
        }, function (response) {
            dashboard.error = response.data.Message || 'An unknown error occurred';
            dashboard.busy = false;
            dashboard.loading = false;
            App.status = "Error";
        });
    };
    
    dashboard.fetch();

    $scope.dashboard = dashboard;
});