/* Title: Gloo Framework
 * Author: Bradley Searle (C2 Digital)
 * Source: www.c2.co.zw
 * License: CC BY 4.0 (https://creativecommons.org/licenses/by/4.0/legalcode)
 */

agrishareApp.controller('DashboardViewController', function ($attrs, $controller, $document, $http, $scope, $timeout, App, Utils) {

    $controller('ViewController', { $scope: $scope, $attrs: $attrs });

    var dashboard = this;
    dashboard.apiUrl = App.apiUrl + $attrs.agApiRoute;
    dashboard.data = {};
    dashboard.busy = true;
    dashboard.error = '';
    dashboard.loading = true;
    dashboard.map = null;
    dashboard.heatmap = null;

    dashboard.activity = {
        filter: {
            type: 'User',
            timespan: 7,
            category: 0,
            startDate: new moment().add(-7, 'days').toDate(),
            endDate: new Date()
        }
    };

    dashboard.fetch = function () {

        App.status = "Loading...";
        dashboard.busy = true;

        var startDate = new moment(dashboard.activity.filter.startDate);
        if (!startDate.isValid()) startDate = new moment(0);
        var endDate = new moment(dashboard.activity.filter.endDate);
        if (!endDate.isValid()) endDate = new moment();    

        var url = dashboard.apiUrl + '?type=' + dashboard.activity.filter.type + '&startDate=' + startDate.format('YYYY-MM-DDTHH:mm:ss') + '&endDate=' + endDate.format('YYYY-MM-DDTHH:mm:ss') + '&category=' + dashboard.activity.filter.category;

        $http({
            url: url,
            headers: App.authorizationHeader()
        })
        .then(function (response) {
            dashboard.data = response.data;
            dashboard.setHeatmap(response.data.locations);
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
    
    $scope.$watch("dashboard.activity.filter", dashboard.fetch, true);

    dashboard.setHeatmap = function (locations) {

        var el = $document.find('#heatmap').get(0);

        if (!dashboard.map)
            dashboard.map = new google.maps.Map(el, {
                zoom: 6,
                center: { lat: -19.0955342, lng: 29.5124486 }
            });

        var gradient = [
            'rgba(0, 255, 255, 0)',
            'rgba(0, 255, 255, 1)',
            'rgba(0, 191, 255, 1)',
            'rgba(0, 127, 255, 1)',
            'rgba(0, 63, 255, 1)',
            'rgba(0, 0, 255, 1)',
            'rgba(0, 0, 223, 1)',
            'rgba(0, 0, 191, 1)',
            'rgba(0, 0, 159, 1)',
            'rgba(0, 0, 127, 1)',
            'rgba(63, 0, 91, 1)',
            'rgba(127, 0, 63, 1)',
            'rgba(191, 0, 31, 1)',
            'rgba(255, 0, 0, 1)'
        ];

        if (dashboard.heatmap) {
            dashboard.heatmap.setMap(null);
            dashboard.heatmap = null;
        }

        var data = [];
        for (var i = 0; i < locations.length; i++)
            data.push(new google.maps.LatLng(locations[i].Latitude, locations[i].Longitude));

        dashboard.heatmap = new google.maps.visualization.HeatmapLayer({
            map: dashboard.map,
            gradient: gradient,
            radius: 20,
            opacity: 0.5,
            data: data
        });

    };


    $scope.dashboard = dashboard;
});