/* Title: Gloo Framework
 * Author: Bradley Searle (C2 Digital)
 * Source: www.c2.co.zw
 * License: CC BY 4.0 (https://creativecommons.org/licenses/by/4.0/legalcode)
 */

var agrishareApp =
    angular
        .module('agrishareApp', ['ui.router', '720kb.tooltips', 'ngToast', 'ngAnimate', 'ngFileUpload', 'cp.ngConfirm'])
        .config(function ($sceDelegateProvider, ngToastProvider, $stateProvider, $locationProvider, $urlRouterProvider) {

            $sceDelegateProvider.resourceUrlWhitelist([
                'self'
            ]);

            ngToastProvider.configure({
                animation: 'slide',
                horizontalPosition: 'right',
                verticalPosition: 'bottom',
                combineDuplications: true
            });

            $stateProvider
                .state('Default', {
                    url: '/{path:[a-z\/]+}',
                    templateUrl: function ($stateParams) {
                        return '/Pages/' + $stateParams.path + '.html';
                    }
                })
                .state('Filter', {
                    url: '/{path:[a-z\/]+}/filter/{filter:.*}',
                    templateUrl: function ($stateParams) {
                        return '/Pages/' + $stateParams.path + '.html';
                    },
                    controller: function ($scope, $stateParams) {
                        $scope.filter = {};
                        var parts = $stateParams.filter.split('/');
                        for (var i = 0; i < parts.length; i += 2)
                            $scope.filter[parts[i]] = parts[i + 1];
                    }
                })
                .state('Detail', {
                    url: '/{path:[a-z\/]+}/{id:[\d]+}',
                    templateUrl: function ($stateParams) {
                        return '/Pages/' + $stateParams.path + '.html';
                    },
                    controller: function ($scope, $stateParams) {
                        $scope.entityId = $stateParams.id;
                    }
                });
            $urlRouterProvider.otherwise('/dashboard');

            // trust html from local resources
            $sceDelegateProvider.resourceUrlWhitelist([
                'self'
            ]);

        })
        .filter('trustAsHtml', function ($sce) {
            return $sce.trustAsHtml;
        })
        .filter('trustAsResourceUrl', ['$sce', function ($sce) {
            return function (val) {
                return $sce.trustAsResourceUrl(val);
            };
        }])
        .filter('bytes', function () {
            return function (bytes, precision) {
                if (bytes === 0 || isNaN(parseFloat(bytes)) || !isFinite(bytes)) return '-';
                if (typeof precision === 'undefined') precision = 1;
                var units = ['bytes', 'KB', 'MB', 'GB', 'TB', 'PB'],
                    number = Math.floor(Math.log(bytes) / Math.log(1024));
                return (bytes / Math.pow(1024, Math.floor(number))).toFixed(precision) + ' ' + units[number];
            }
        })
        .filter('percent', function () {
            return function (value) {
                if (isNaN(parseFloat(value)) || !isFinite(value)) return '-';
                return (value * 100) + '%';
            }
        })
        .directive('glEnterKeypress', function () {
            return function (scope, element, attrs) {
                element.bind("keydown keypress", function (event) {
                    if (event.which === 13) {
                        scope.$apply(function () {
                            scope.$eval(attrs.glEnterKeypress);
                        });
                        event.preventDefault();
                    }
                });
            };
        })
        .directive('glRightClick', function ($parse) {
            return function (scope, element, attrs) {
                var fn = $parse(attrs.ngRightClick);
                element.bind('contextmenu', function (event) {
                    scope.$apply(function () {
                        event.preventDefault();
                        fn(scope, { $event: event });
                    });
                });
            };
        })
        .config(["$httpProvider", function ($httpProvider) {

            var regexIso8601 = /^([\+-]?\d{4}(?!\d{2}\b))((-?)((0[1-9]|1[0-2])(\3([12]\d|0[1-9]|3[01]))?|W([0-4]\d|5[0-2])(-?[1-7])?|(00[1-9]|0[1-9]\d|[12]\d{2}|3([0-5]\d|6[1-6])))([T\s]((([01]\d|2[0-3])((:?)[0-5]\d)?|24\:?00)([\.,]\d+(?!:))?)?(\17[0-5]\d([\.,]\d+)?)?([zZ]|([\+-])([01]\d|2[0-3]):?([0-5]\d)?)?)?)?$/;

            var convertDateStringsToDates = function (input) {
                if (typeof input !== "object") return input;
                for (var key in input) {
                    if (!input.hasOwnProperty(key)) continue;
                    var value = input[key];
                    var match;
                    if (typeof value === "string" && (match = value.match(regexIso8601))) {
                        var milliseconds = Date.parse(match[0]);
                        if (!isNaN(milliseconds)) {
                            input[key] = new Date(milliseconds);
                        }
                    } else if (typeof value === "object") {
                        convertDateStringsToDates(value);
                    }
                }
            };

            $httpProvider.defaults.transformResponse.push(function (responseData) {
                convertDateStringsToDates(responseData);
                return responseData;
            });

        }]);

