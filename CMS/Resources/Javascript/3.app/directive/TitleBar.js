agrishareApp.directive('agTitleBar', function (App) {
    return {
        restrict: 'E',

        transclude: true,

        template: '<header><a class="material-icons" ng-if="returnUrl" ng-click="go(returnUrl)">chevron_left</a><strong>{{title}}</strong><ng-transclude></ng-transclude></header>',

        link: function ($scope, element, attrs) {

            $scope.$watch('App.title', function () {
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