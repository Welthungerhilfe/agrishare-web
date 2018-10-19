/* Title: Gloo Framework
 * Author: Bradley Searle (C2 Digital)
 * Source: www.c2.co.zw
 * License: CC BY 4.0 (https://creativecommons.org/licenses/by/4.0/legalcode) */

agrishareApp.directive('glListPhoto', function () {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            thumb: '=glThumbPath',
            icon: '@glIcon'
        },
        template: '' +
            '<div style="background-image:url({{thumb}})" ng-class="{\'photo\': thumb, \'icon\': !thumb}">' +
            '<i class="material-icons">{{icon}}</i>' +
            '</div>'
    };
});