agrishareApp.directive('agStatusBar', function (App) {
    return {
        restrict: 'E',

        transclude: true,

        template: '<footer><i class="material-icons">info</i><span>{{status}}</span><ng-transclude></ng-transclude></footer>',

        link: function ($scope) {
            $scope.$watch(function () {
                return App.status;
            }, function () {
                $scope.status = App.status || 'Done';
            });
        }
    };
});