/* Title: Gloo Framework
 * Author: Bradley Searle (C2 Digital)
 * Source: www.c2.co.zw
 * License: CC BY 4.0 (https://creativecommons.org/licenses/by/4.0/legalcode)
 */

agrishareApp.controller('FormViewController', function ($attrs, $controller, $location, $scope, App) {

    $controller('ViewController', { $scope: $scope, $attrs: $attrs });

    var form = this;
    $scope.form = form;

});