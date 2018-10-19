/* Title: Gloo Framework
 * Author: Bradley Searle (C2 Digital)
 * Source: www.c2.co.zw
 * License: CC BY 4.0 (https://creativecommons.org/licenses/by/4.0/legalcode) */

agrishareApp.directive('glRating', function () {
    return {
        restrict: 'E',
        replace: true,

        scope: {
            stars: '=glStars'
        },

        template: function (element, attrs) {
            return '' +
                '<ul class="gl-rating">' +
                '<li ng-repeat="i in getNumber(stars) track by $index" class="material-icons md-16 full">star</li>' +
                '<li ng-repeat="i in getNumber(5 - stars) track by $index" class="material-icons md-16 empty">star_border</li>' +
                '</ul>';
        },

        link: function (scope, element, attrs) {
            scope.getNumber = function (num) {
                return new Array(num);
            };
        }
    };
});