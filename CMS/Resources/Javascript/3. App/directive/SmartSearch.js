/* Title: Gloo Framework
 * Author: Bradley Searle (C2 Digital)
 * Source: www.c2.co.zw
 * License: CC BY 4.0 (https://creativecommons.org/licenses/by/4.0/legalcode) */

agrishareApp.directive('glSmartSearch', function ($http, $q, $timeout, $document, $rootScope, App, Utils) {
    return {
        restrict: 'E',
        replace: true,

        scope: {
            icon: '@glIcon',
            glApiUrl: '='
        },

        template: function (element, attrs) {

            var placeholder = attrs.glPlaceholder;

            return '<div class="row autocomplete-row" name="' + name + '">' +
                '<div ng-class="{\'icon\':icon}">' +
                '<i class="material-icons" ng-if="icon">{{icon}}</i>' +
                '<input type="text" placeholder="' + placeholder + '" maxlength="256" ng-model="query" />' +
                '<span class="busy" ng-show="busy"><span class="spinner"></span></span>' +
                '<a class="done" ng-show="query"><i class="material-icons md-16">cancel</i></a>' +
                '</div>' +
                '<div class="autocomplete-results">' +
                '<ul>' +
                '<li ng-repeat="item in data" tabindex="0" class="item" ng-click="go(item.Url)">{{item.Title}} <small>{{item.Type}}</small></li>' +
                '<li ng-show="query != \'\' && !busy && data.length == 0"><i>No results</i></li>' +
                '</ul>' +
                '</div>' +
                '</div>';
        },

        link: function (scope, element, attrs) {

            var body = angular.element(document.body);
            var input = angular.element(element[0].getElementsByTagName("input")[0]);
            var list = angular.element(element[0].getElementsByClassName("autocomplete-results")[0]);
            var clearbutton = angular.element(element[0].getElementsByTagName("a")[0]);
            var source = angular.element(element);

            var hiding = null;
            var aborter = null;
            var selectedQuery = null;

            scope.data = [];
            scope.busy = false;
            scope.listIndex = 0;

            scope.go = function (url) {
                location.hash = url;
                scope.query = '';
                hideSearch();
            };

            var setPosition = function () {
                list.css({
                    top: source.offset().top + source.outerHeight(),
                    left: source.offset().left,
                    width: source.outerWidth(),
                    position: 'absolute'
                });

                if (list.offset().top + list.outerHeight() > body.innerHeight())
                    list.css({
                        top: source.offset().top - list.outerHeight()
                    });
            };

            var hideSearch = function (e) {    
                if (e) {
                    var el = e.target;
                    while (el != null) {
                        if (el == list.get(0) || el == input.get(0))
                            return;
                        el = el.parentNode;
                    }
                }
                body.off('mouseup', hideSearch);
                list.css({ display: 'none' });
                scope.busy = false;
                element.append(list);
            };

            var showSearch = function () {
                body.append(list);
                body.off('mouseup', hideSearch);
                body.on('mouseup', hideSearch);
                setPosition();
                list.css({ display: 'block' });
            };

            var search = function () {
                var search = this;

                scope.busy = true;

                if (search.aborter != null)
                    search.aborter.resolve();

                if (scope.query && scope.query.length > 0)
                    showSearch();
                else {
                    hideSearch();
                    return;
                }

                var url = Utils.qs.add(scope.glApiUrl, "Query=" + encodeURIComponent(scope.query) + "&Count=5");
                this.aborter = $q.defer();
                var req = {
                    timeout: search.aborter.promise,
                    url: url,
                    headers: App.authorizationHeader()
                };

                $http(req).then(function (response) {
                    scope.busy = false;
                    scope.data = response.data;
                    scope.listIndex = 0;
                }, function () { });
            };

            input.on('focus', function () {
                search();
            });

            clearbutton.on('click', function (e) {
                scope.query = '';
                scope.data = [];
                scope.$apply();
            });

            input.on('keyup', function (e) {
                if (e.keyCode == 38 || e.keyCode == 40)
                    return;
               search();
            });

            input.on('keydown', function (e) {

                var items = list.find('ul').children();

                if (e.keyCode == 40) {
                    if (!items.eq(scope.listIndex).hasClass('item')) return;

                    items.removeClass('selected');
                    items.eq(scope.listIndex).addClass('selected');
                    scope.listIndex++;

                    return false;
                }

                if (e.keyCode == 38) {
                    if (scope.listIndex == 0) return;

                    scope.listIndex--;
                    items.removeClass('selected');
                    items.eq(scope.listIndex - 1).addClass('selected');

                    return false;
                }

                if (e.keyCode == 13) {
                    list.find('ul').children().eq(scope.listIndex - 1).triggerHandler('click');
                    input.blur();
                    return false;
                }

            });

            list.on('mouseenter', function () {
                list.find('ul').children().removeClass('selected');
                scope.listIndex = 0;
            });

        }
    };
});