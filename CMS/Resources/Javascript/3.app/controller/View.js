/* Title: Gloo Framework
 * Author: Bradley Searle (C2 Digital)
 * Source: www.c2.co.zw
 * License: CC BY 4.0 (https://creativecommons.org/licenses/by/4.0/legalcode)
 */

agrishareApp.controller('ViewController', function ($attrs, $location, App) {

    App.returnUrl = $location.search().return;
    App.selectedUrl = $attrs.agSelectedUrl;
    App.title = $attrs.agTitle;

});