/* Title: Gloo Framework
 * Author: Bradley Searle (C2 Digital)
 * Source: www.c2.co.zw
 * License: CC BY 4.0 (https://creativecommons.org/licenses/by/4.0/legalcode) */

agrishareApp.directive('glCheckbox', function () {
    return {
        restrict: 'E',
        replace: true,

        scope: {
            label: '@glLabel',
            ngModel: '='
        },

        template: function (element, attrs) {

            var required = attrs.glRequired || false;
            var label = attrs.glLabel || '';
            var name = attrs.glRequired ? attrs.glName || label : '';
            if (required && label)
                label += ' *';

            return '' +
                '<div class="row gl-checkbox" name="' + name + '">' +
                '<div>' +
                '<div ng-click="ngModel=!ngModel">' +
                '<i class="material-icons" ng-if="!ngModel">check_box_outline_blank</i>' +
                '<i class="material-icons" ng-if="ngModel">check_box</i>' +
                '<span>' + label + '</span>' +
                '</div>' +
                '</div>' +
                '</div>';
        },

        require: ['ngModel'],

        link: function (scope, element, attrs, ctrl) {

            if (attrs.glRequired)
                ctrl[0].$setValidity('required', false);

            scope.$watch('ngModel', function (newValue) {
                if (newValue)
                    ctrl[0].$setValidity('required', true);
                else if (attrs.glRequired)
                    ctrl[0].$setValidity('required', false);
            }, true);

            scope.validity = function (count) {
                ctrl[0].$setValidity('required', count > 0);
            };

        }
    };
});