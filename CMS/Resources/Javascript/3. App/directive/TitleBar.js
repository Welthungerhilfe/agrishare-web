agrishareApp.directive('agTitleBar', function (App) {
    return {
        restrict: 'E',

        transclude: true,

        template: '<header>' +
            '<a class="material-icons" ng-if="returnUrl" ng-click="go(returnUrl)">chevron_left</a>' +
            '<strong>{{ title }}</strong>' +
            '<ng-transclude></ng-transclude>' +
            '</header> ',

        link: function ($scope, element, attrs) {

            $scope.$watch(function () {
                return App.title;
            }, function () {
                $scope.title = App.title;
            });

            $scope.$watch('App.returnUrl', function () {
                $scope.returnUrl = App.returnUrl;
            });

            $scope.go = function (url) {
                location.hash = url;
            };

        }
    };
});

agrishareApp.directive('agTitleBarSearch', function () {
    return {
        restrict: 'E',

        scope: {
            list: '=agController'
        },

        template: '<div class="search" ng-show="list.searching">' +
                    '<i class="material-icons">search</i>' +
                    '<input type="search" id="titleBarSearch" ng-model="list.query" gl-enter-keypress="list.refresh()" placeholder="Search"  />' +
                    '<a class="material-icons" ng-click="list.hideSearch()">close</a>' +
                    '</div>'
    };
});