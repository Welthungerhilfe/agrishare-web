/* Title: Gloo Framework
 * Author: Bradley Searle (C2 Digital)
 * Source: www.c2.co.zw
 * License: CC BY 4.0 (https://creativecommons.org/licenses/by/4.0/legalcode)
 */

agrishareApp.directive('glIconButton', function () {
    return {
        restrict: 'E',
        transclude: true,
        replace: true,
        scope: {
            icon: '@glIcon',
            ngDisabled: '='
        },
        template: '<button ng-class="{\'icon\': icon}"><i class="material-icons">{{icon}}</i><ng-transclude></ng-transclude></button>',
        link: function (scope, element, attrs) {
            angular.element(element).addClass('gl-icon-button');
            element.on('click', function () {
                return false;
            });
        }
    };
});