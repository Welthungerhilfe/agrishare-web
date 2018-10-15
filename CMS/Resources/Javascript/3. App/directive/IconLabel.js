/* Title: Gloo Framework
 * Author: Bradley Searle (C2 Digital)
 * Source: www.c2.co.zw
 * License: CC BY 4.0 (https://creativecommons.org/licenses/by/4.0/legalcode) */

agrishareApp.directive('glIconLabel', function () {
    return {
        restrict: 'E,A',
        transclude: true,
        scope: {
            icon: '@glIcon',
            size: '@glSize'
        },
        template: '<i class="material-icons {{size}}">{{icon}}</i><ng-transclude></ng-transclude>',
        link: function (scope, element, attr) {
            element.addClass('gl-icon-label');
        }
    };
});