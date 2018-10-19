/* Title: Gloo Framework
 * Author: Bradley Searle (C2 Digital)
 * Source: www.c2.co.zw
 * License: CC BY 4.0 (https://creativecommons.org/licenses/by/4.0/legalcode) */

agrishareApp.directive('glCheckboxList', function ($http, App) {
    return {
        restrict: 'E',
        replace: true,

        scope: {
            label: '@glLabel',
            source: '=?glSource',
            ngModel: '='
        },

        template: function (element, attrs) {

            var required = attrs.glRequired || false;
            var label = attrs.glLabel || '';
            var name = attrs.glRequired ? attrs.glName || label : '';
            if (required && label)
                label += ' *';
            var requiredAttr = required ? 'required' : '';

            return '' +
                '<div class="row gl-checkbox-list" name="' + name + '">' +
                '<label>' + label + '</label>' +
                '<div>' +
                '<div ng-repeat="item in source" ng-click="toggle(item)">' +
                '<i class="material-icons" ng-if="exists(item)">check_box</i>' +
                '<i class="material-icons" ng-if="!exists(item)">check_box_outline_blank</i>' +
                '<span>{{item.Title}}</span>' +
                '</div>' +
                '</div>' +
                '</div>';
        },

        require: ['ngModel'],

        link: function (scope, element, attrs, ctrl) {

            if (attrs.glRequired)
                ctrl[0].$setValidity('required', false);
            else
                ctrl[0].$setValidity('required', true);

            scope.$watch('ngModel', function (newValue) {
                if (!attrs.glRequired)
                    ctrl[0].$setValidity('required', true);
                else if (newValue && newValue.length > 0)
                    ctrl[0].$setValidity('required', true);
                else
                    ctrl[0].$setValidity('required', false);
            }, true);

            scope.exists = function (item) {
                if (scope.ngModel) {
                    if (attrs.glSelection)
                        return scope.ngModel.some(e => (e === item[attrs.glSelection]));
                    return scope.ngModel.some(e => (e.Id === item.Id));
                }
                return false;
            };

            scope.toggle = function (item) {
                if (!scope.ngModel)
                    scope.ngModel = [];

                var index;
                if (attrs.glSelection) {
                    index = scope.ngModel.findIndex(e => e === item[attrs.glSelection]);
                    if (index > -1)
                        scope.ngModel.splice(index, 1);
                    else
                        scope.ngModel.push(item[attrs.glSelection]);
                }
                else {
                    index = scope.ngModel.findIndex(e => (e.Id === item.Id));
                    if (index > -1)
                        scope.ngModel.splice(index, 1);
                    else
                        scope.ngModel.push(item);
                }
            };

            scope.validity = function (count) {
                if (attrs.glRequired)
                    ctrl[0].$setValidity('required', count > 0);
                else {
                    ctrl[0].$setValidity('required', true);
                }
            };


            scope.$watch(function () {
                return attrs.glSourceUrl;
            }, function (value) {

                if (value) {
                    var req = {
                        url: App.apiUrl + attrs.glSourceUrl,
                        headers: App.authorizationHeader()
                    };

                    $http(req).then(function (response) {
                        scope.source = response.data;
                    }, function () { });
                }

            });

        }
    };
});