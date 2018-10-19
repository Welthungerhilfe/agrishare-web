/* Title: Gloo Framework
 * Author: Bradley Searle (C2 Digital)
 * Source: www.c2.co.zw
 * License: CC BY 4.0 (https://creativecommons.org/licenses/by/4.0/legalcode)
 */

agrishareApp.controller('FormViewController', function ($attrs, $controller, $http, $location, $rootScope, $scope, App, Utils) {

    $controller('ViewController', { $scope: $scope, $attrs: $attrs });

    var form = this;
    form.apiUrl = App.apiUrl + $attrs.agApiRoute;
    form.data = {};
    form.entity = null;
    form.loading = true;
    form.busy = false;
    form.error = '';
    form.qs = $scope.qs;

    form.fetch = function () {
        form.loading = true;
        App.status = "Loading...";

        var url = form.apiUrl + 'find';
        if ($scope.entityId)
            url += '?Id=' + $scope.entityId;

        $http({
            url: url,
            headers: App.authorizationHeader()
        })
            .then(function (response) {
                form.data = response.data;
                form.entity = response.data.Entity;
                form.loading = false;
                App.status = "Done";
            }, function (response) {
                form.error = response.data.Message || 'An unknown error occurred';
                App.status = "Error";
                form.loading = false;
            });
    };

    form.save = function (callback) {
        form.busy = true;
        $http({
            method: 'POST',
            data: form.entity,
            url: form.apiUrl + 'save',
            headers: App.authorizationHeader()
        })
            .then(function (response) {
                if (response.data.DownloadUrl)
                    location.href = response.data.DownloadUrl;
                var feedback = response.data.Feedback ? response.data.Feedback : "Details saved";
                Utils.toast.success(feedback);
                if (callback)
                    eval(callback);
                $rootScope.$emit('form:saveComplete');
            }, function (response) {
                Utils.alert('Error', response.data.Message || 'An unknown error occurred');
                form.busy = false;
            });
    };

    form.fetch();

    $scope.app = App;
    $scope.form = form;

});