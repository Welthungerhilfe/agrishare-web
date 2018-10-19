agrishareApp.directive('agSection', function ($timeout, $parse) {
    return {
        restrict: 'E',
        transclude: true,

        scope: {
            controller: '=agController'
        },

        template: '' +
            '<div ng-show="controller.loading" class="feedback busy"><i class="material-icons md-48">hourglass_empty</i></div>' +
            '<div ng-show="controller.error" class="feedback"><i class="material-icons md-48">bug_report</i><span>{{controller.error}}</span></div>' +
            '<ng-transclude ng-show="!controller.error && !controller.loading"></ng-transclude>'
    };
});