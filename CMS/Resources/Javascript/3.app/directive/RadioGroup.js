/* Title: Gloo Framework
 * Author: Bradley Searle (C2 Digital)
 * Source: www.c2.co.zw
 * License: CC BY 4.0 (https://creativecommons.org/licenses/by/4.0/legalcode) */

agrishareApp.directive('glRadioGroup', function ($timeout) {
    return {
        restrict: 'E',
        replace: true,

        scope: {
            label: '@glLabel',
            row: '@glRow',
            source: '=glSource',
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
                '<div class="row gl-radio-group" name="' + name + '">' +
                '<label>' + label + '</label>' +
                '<div ng-class="{\'flex-row\':row}">' +
                '<div ng-repeat="item in source" ng-click="set(item)">' +
                '<i class="material-icons" ng-if="!checked(item)">radio_button_unchecked</i>' +
                '<i class="material-icons" ng-if="checked(item)">radio_button_checked</i>' +
                '<span>{{item.Title}}</span>' +
                '</div>' +
                '</div>' +
                '</div>';
        },

        require: ['ngModel'],

        link: function (scope, element, attrs, ctrl) {

            if (attrs.glRequired)
                ctrl[0].$setValidity('required', false);

            scope.$watch('ngModel', function (newValue) {
                $timeout(function () {
                    if (newValue)
                        ctrl[0].$setValidity('required', true);
                    else
                        ctrl[0].$setValidity('required', false);
                });
            }, true);

            scope.set = function (val) {
                if (attrs.glSelection)
                    scope.ngModel = val[attrs.glSelection];
                else
                    scope.ngModel = val;
            };

            scope.checked = function (val) {
                if (val && scope.ngModel)
                    if (attrs.glSelection)
                        return scope.ngModel == val[attrs.glSelection];
                    else
                        return scope.ngModel.Id == val.Id;
            };

            scope.validity = function (count) {
                ctrl[0].$setValidity('required', count > 0);
            };

        }
    };
});