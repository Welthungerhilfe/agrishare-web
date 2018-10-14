/* Title: Gloo Framework
 * Author: Bradley Searle (C2 Digital)
 * Source: www.c2.co.zw
 * License: CC BY 4.0 (https://creativecommons.org/licenses/by/4.0/legalcode) */

agrishareApp.directive('glPopupMenu', function ($timeout) {
    return {
        restrict: 'E',
        transclude: true,
        scope: {
            glIcon: '@glIcon'
        },
        template: '<i class="material-icons">{{glIcon || \'more_vert\'}}</i><div class="popup-menu"><ng-transclude></ng-transclude></div>',
        link: function (scope, element, attrs) {

            var menu = element.find('>div');
            menu.addClass(attrs.glPosition);

            var body = angular.element(document.body);
            
            var button = element.find('>i');
            button.on('click', function () {

                body.append(menu);

                var top = button.offset().top;
                if (attrs.glPosition.match(/bottom/gi))
                    top = button.offset().top + button.outerHeight() - menu.outerHeight();
                if (attrs.glPosition.match(/top-offset/gi))
                    top = button.offset().top + button.outerHeight();

                var left = button.offset().left;
                if (attrs.glPosition.match(/right/gi))
                    left = button.offset().left + button.outerWidth() - menu.outerWidth();

                menu.css({ top: top, left: left, display: 'block' });
                body.on('mouseup', hideMenu);
            });

            var hideMenu = function (e) {
                if (e) {
                    var el = e.target;
                    while (el != null) {
                        if (el == button.get(0))
                            return;
                        el = el.parentNode;
                    }
                }
                body.off('mouseup', hideMenu);
                $timeout(function () {
                    menu.css({ display: 'none' });
                    element.append(menu);
                }, 1);
                
            };

        }
    };
});

agrishareApp.directive('glMenuItem', function () {
    return {
        restrict: 'E',
        replace: true,
        transclude: true,
        scope: {
            icon: '@glIcon',
            labelOptional: '@glLabelOptional'
        },
        template: '<a ng-class="{\'label-optional\': labelOptional}"><i class="material-icons">{{icon}}</i> <ng-transclude /></a>'
    };
});