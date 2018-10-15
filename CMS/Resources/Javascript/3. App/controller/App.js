/* Title: Gloo Framework
 * Author: Bradley Searle (C2 Digital)
 * Source: www.c2.co.zw
 * License: CC BY 4.0 (https://creativecommons.org/licenses/by/4.0/legalcode) */

agrishareApp.controller('AppController', function ($attrs, $rootScope, $scope, App, Utils) {

    $scope.app = App;
    $scope.app.apiUrl = $attrs.agApiUrl;

    $rootScope.$on('form:saveComplete', function () {
        App.go(App.returnUrl);
    });

});