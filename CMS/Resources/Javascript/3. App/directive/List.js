/* Title: Gloo Framework
 * Author: Bradley Searle (C2 Digital)
 * Source: www.c2.co.zw
 * License: CC BY 4.0 (https://creativecommons.org/licenses/by/4.0/legalcode) */

agrishareApp.directive('glList', function ($timeout, $parse) {
    return {
        restrict: 'E',
        transclude: true,

        scope: {
            list: '=glController'
        },

        template: '' +
            '<div ng-show="!list.busy && controller.data.length==0" class="feedback"><i class="material-icons md-48">sentiment_dissatisfied</i><span>No results</span></div>' +
            '<div ng-show="list.data.length>0"><ng-transclude></ng-transclude></div>',

        link: function (scope, element, attrs) {

            if (attrs.glInfinite) {

                var tick;
                var callback = $parse(attrs.glInfinite);
                var body = angular.element(document.body);

                var check = function () {
                    $timeout.cancel(tick);

                    var bottom = element.offset().top + element.outerHeight();

                    if (body.outerHeight() > bottom && !scope.list.error) {                        
                        callback(scope, {});
                        tick = $timeout(check, 1000);
                    }
                    else
                        tick = $timeout(check, 1);
                };
                check();

                scope.$on('$destroy', function () {
                    $timeout.cancel(tick);
                });
            }

        }
    };
});