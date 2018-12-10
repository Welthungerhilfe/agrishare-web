/* Title: Gloo Framework
 * Author: Bradley Searle (C2 Digital)
 * Source: www.c2.co.zw
 * License: CC BY 4.0 (https://creativecommons.org/licenses/by/4.0/legalcode)
 */

agrishareApp.controller('ListViewController', function ($attrs, $controller, $http, $location, $q, $scope, $timeout, App, Utils) {

    $controller('ViewController', { $scope: $scope, $attrs: $attrs });
    
    var list = this;
    list.apiUrl = App.apiUrl + $attrs.agApiRoute;
    list.pageIndex = 0;
    list.pageSize = 20;
    list.loading = true;
    list.busy = false;
    list.more = true;
    list.aborter = null;
    list.data = [];
    list.recordCount = 0;
    list.error = '';
    list.searching = false;
    list.query = '';
    list.filter = $scope.filter;

    $scope.list = list;

    list.refresh = function () {
        if (list.aborter)
            list.aborter.resolve();
        list.loading = true;
        App.status = "Loading...";
        list.error = '';
        list.data = [];
        list.busy = false;
        list.more = true;
        list.pageIndex = 0;
        list.next();
    };

    list.next = function () {

        if (list.busy || !list.more)
            return;
        list.busy = true;

        var url = list.apiUrl + 'list?PageIndex=' + list.pageIndex + '&PageSize=' + list.pageSize;

        if (list.query)
            url += '&Query=' + encodeURIComponent(list.query);

        for (var item in list.filter)
            url += '&' + item + '=' + encodeURIComponent(list.filter[item]);

        list.aborter = $q.defer();
        $http({
            timeout: list.aborter.promise,
            url: url,
            headers: App.authorizationHeader()
        }).then(function (response) {

            list.busy = false;
            list.loading = false;

            var json = response.data;
            App.title = json.Title;
            list.recordCount = parseInt(json.Count);
            
            var items = json.List;
            for (var i = 0; i < items.length; i++)
                list.data.push(items[i]);

            if (json.List.length > 0 && json.List.length < list.recordCount) {
                list.pageIndex += 1;
                list.more = true;
            }
            else
                list.more = false;

            list.updateStatus();

            }, function (response) {
                list.busy = false;
                list.loading = false;
                App.status = 'Error loading list';
                list.error = response.data && response.data.Message ? response.data.Message : 'An unknown error occurred';
            
        });

    };

    list.updateStatus = function () {
        var list = this;
        if (list.data.length === 0)
            App.status = 'No results';
        else if (list.data.length === 1)
            App.status = '1 result';
        else if (list.recordCount > list.data.length)
            App.status = 'Showing ' + list.data.length + ' of ' + list.recordCount + ' results';
        else
            App.status = 'Showing ' + list.data.length + ' results';
    };

    list.showSearch = function () {
        list.searching = true;
        Utils.focus('titleBarSearch');
    };

    list.hideSearch = function () {
        list.searching = false;
        if (list.query !== '') {
            list.query = '';
            list.refresh();
        }
    };

    list.delete = function (index, url) {
        var list = this;
        var item = list.data[index];
        Utils.confirm('Confirm Delete', 'Are you sure you want to delete <strong>' + item.Title + '</strong>? This action can not be undone.', function () {
            list.data.splice(index, 1);
            $http({
                url: url,
                headers: App.authorizationHeader()
            }).then(function (response) {
                var feedback = response.data.Feedback || item.Title + ' was deleted';
                Utils.toast.success(feedback);
                list.recordCount -= 1;
                list.updateStatus();
            }, function (response) {
                var feedback = response.data.Message;
                Utils.toast.error(feedback);
            });
        });
    };

    list.async = function (index, url) {
        var list = this;
        $http({
            url: url,
            headers: App.authorizationHeader()
        }).then(function (response) {
            Utils.toast.success(response.data.Feedback);
            list.data[index] = response.data.Entity;
        }, function (response) {
            var feedback = response.data.Message;
            Utils.toast.error(feedback);
        });
    };

    list.refresh();

});