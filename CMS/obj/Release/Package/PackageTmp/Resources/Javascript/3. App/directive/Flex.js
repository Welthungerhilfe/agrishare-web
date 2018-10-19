/* Title: Gloo Framework
 * Author: Bradley Searle (C2 Digital)
 * Source: www.c2.co.zw
 * License: CC BY 4.0 (https://creativecommons.org/licenses/by/4.0/legalcode) */

agrishareApp.directive('flex', function () {
    return {
        link: function (scope, element, attrs) {
            if (parseInt(attrs.flex))
                element.css({ flex: attrs.flex });
            else
                element.css({ display: 'flex', flexDirection: attrs.flex }).addClass('flex');
        }
    };
});