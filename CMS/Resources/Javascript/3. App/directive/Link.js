/* Title: Gloo Framework
 * Author: Bradley Searle (C2 Digital)
 * Source: www.c2.co.zw
 * License: CC BY 4.0 (https://creativecommons.org/licenses/by/4.0/legalcode) */

agrishareApp.directive('glAutolink', function () {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            icon: '@glIcon',
            size: '@glSize',
            type: '@glType',
            ngModel: '='
        },
        template: '<a class="gl-autolink"><i class="material-icons {{size}}" ng-if="icon">{{icon}}</i>{{model}}</a>',
        require: ['ngModel'],
        link: function (scope, element, attrs, ctrl) {
            scope.$watch('ngModel', function (newValue) {                
                if (newValue) {
                    scope.model = newValue;
                    switch (attrs.glType) {
                        case 'web':
                            var link = newValue;
                            if (!link.match(/http/gi))
                                link = 'http://' + link;
                            element.attr('href', link);
                            element.attr('target', '_blank');
                            break;
                        case 'tel':
                            var link = 'tel://' + newValue;
                            element.attr('href', link);
                            break;
                        case 'mail':
                            var link = 'mailto://' + newValue;
                            element.attr('href', link);
                            break;
                        case 'map':
                            var link = 'https://maps.google.com/?q=' + encodeURIComponent(newValue);
                            element.attr('href', link);
                            element.attr('target', '_blank');
                            break;
                    }
                }
                else
                    scope.model = '-';

            }, true);
        }
    };
});