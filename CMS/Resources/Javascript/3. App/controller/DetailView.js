/* Title: Gloo Framework
 * Author: Bradley Searle (C2 Digital)
 * Source: www.c2.co.zw
 * License: CC BY 4.0 (https://creativecommons.org/licenses/by/4.0/legalcode)
 */

agrishareApp.controller('DetailViewController', function ($attrs, $controller, $http, $scope, App, Utils) {

    $controller('ViewController', { $scope: $scope, $attrs: $attrs });

    var detail = this;
    detail.apiUrl = App.apiUrl + $attrs.agApiRoute;
    detail.data = {};
    detail.entity = {};
    detail.busy = true;
    detail.error = '';
    detail.loading = true;

    detail.fetch = function (id) {

        App.status = "Loading...";

        detail.busy = true;

        var url = detail.apiUrl + '/find';
        if (id)
            url += '?Id=' + id;

        $http({
            url: url,
            headers: App.authorizationHeader()
        })
        .then(function (response) {
            detail.data = response.data;
            detail.entity = response.data.Entity;
            detail.busy = false;
            detail.loading = false;
            App.status = "Done";
        }, function (response) {
            detail.error = response.data.Message || 'An unknown error occurred';
            detail.busy = false;
            detail.loading = false;
            App.status = "Error";
        });
    };

    detail.async = function (url) {
        $http({
            url: App.apiUrl + url,
            headers: App.authorizationHeader()
        })
            .then(function (response) {
                detail.entity = response.data.Entity;
                detail.data = response.data;
            }, function (response) {
                Utils.toast.error(response.data.Message || 'An unknown error occurred');
            });
    };

    detail.delete = function (item, url, returnUrl) {
        var list = this;
        Utils.confirm('Confirm Delete', 'Are you sure you want to delete <strong>' + item.Title + '</strong>? This action can not be undone.', function () {
            list.data.splice(index, 1);
            $http({
                url: url,
                headers: App.authorizationHeader()
            }).then(function (response) {
                var feedback = response.data.Feedback || item.Title + ' was deleted';
                Utils.toast.success(feedback);
                if (App.returnUrl)
                    App.go(App.returnUrl);
            }, function (response) {
                var feedback = response.data.Message;
                Utils.toast.error(feedback);
            });
        });
    };

    detail.addNote = function (url, note, notes) {
        $http({
            method: 'POST',
            url: App.apiUrl + Utils.qs.add(url, 'id=' + $scope.id),
            data: note,
            headers: App.authorizationHeader()
        })
            .then(function (response) {
                if (notes == null)
                    notes = [];
                notes.push(response.data);
                note.Notes = '';
            }, function (response) {
                Utils.toast.error(response.data.Message || 'An unknown error occurred');
            });
    };
    
    detail.fetch($scope.entityId);

    $scope.detail = detail;
});