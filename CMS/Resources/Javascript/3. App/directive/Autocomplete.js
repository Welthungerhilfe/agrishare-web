/* Title: Gloo Framework
 * Author: Bradley Searle (C2 Digital)
 * Source: www.c2.co.zw
 * License: CC BY 4.0 (https://creativecommons.org/licenses/by/4.0/legalcode) */

agrishareApp.directive('glAutocomplete', function ($http, $q, $timeout, $document, $rootScope, App, Utils) {
    return {
        restrict: 'E',
        replace: true,

        scope: {
            label: '@glLabel',
            icon: '@glIcon',
            glAddNew: '@glAddNew',
            glAutoAdd: '@glAutoAdd',
            glEmptyClick: '&',
            glOnSet: '&',
            glRequired: '=',
            ngModel: '='
        },

        template: function (element, attrs) {

            var required = attrs.glRequired || false;
            var label = attrs.glLabel || '';
            var name = attrs.glRequired ? attrs.glName || label : '';
            if (required && label)
                label += ' *';
            var placeholder = attrs.glPlaceholder || label;
            var requiredAttr = required ? 'required' : '';
            var empty = attrs.glEmpty || "No results";

            getItemTemplateHtml = function () { return element.find('gl-item-template').html() || '{{item.Title}}' };
            return '' +
                '<div class="row autocomplete-row" name="' + name + '" ng-class="{\'searching\':searching}">' +
                '<label ng-show="label">' + label + '</label>' +
                '<div ng-class="{\'icon\':icon}">' +
                '<i class="material-icons" ng-if="icon">{{icon}}</i>' +
                '<input type="text" placeholder="' + placeholder + '" maxlength="256" ng-model="query" />' +
                '<span class="busy" ng-show="busy"><span class="spinner"></span></span>' +
                '<span class="done" ng-show="ngModel"><i class="material-icons md-16">check_circle</i></span>' +
                '</div>' +
                '<div class="autocomplete-results">' +
                '<ul>' +
                '<li ng-repeat="item in data" tabindex="0" class="item" ng-click="set(item)">' + getItemTemplateHtml() + '</li>' +
                '<li ng-show="query != \'\' && (!busy || fetched) && data.length == 0 && !glAddNew && !glAutoAdd" ng-click="glEmptyClick();set(null)"><i>' + empty + '</i></li>' +
                '<li ng-show="query != \'\' && (!busy || fetched) && data.length == 0 && glAddNew && !glAutoAdd" ng-click="setNew()"><i>Add</i></li>' +
                '</ul>' +
                '</div>' +
                '</div>';
        },

        require: ['ngModel'],

        link: function (scope, element, attrs, ctrl) {

            var setValid = function (bool) {
                $timeout(function () {
                    ctrl[0].$setValidity('required', bool);
                });
            };

            var label = element.find('label').css({ opacity: 0 });
            scope.fetched = false;

            scope.$watch('query', function (newValue) {
                if (newValue)
                    label.stop(true, true).animate({ opacity: 1 });
                else
                    label.stop(true, true).animate({ opacity: 0 });
            });

            //

            var doc = angular.element(document);
            var body = angular.element(document.body);
            var input = angular.element(element[0].getElementsByTagName("input")[0]);
            var list = angular.element(element[0].getElementsByClassName("autocomplete-results")[0]);
            var clearbutton = angular.element(element[0].getElementsByTagName("a")[0]);
            var source = angular.element(element);

            if (scope.glRequired)
                setValid(false);
            else
                setValid(true);

            var hiding = null;
            var aborter = null;
            var selectedQuery = null;

            scope.$watch('ngModel', function (newValue) {
                if (newValue) {
                    scope.query = (typeof newValue) == 'object' ? newValue.Title || '' : newValue;
                    selectedQuery = scope.query;
                    setValid(true);
                }
                if (attrs.glAutoAdd) {
                    selectedQuery = scope.query;
                    setValid(true);
                }
                if (!newValue && scope.glRequired) {
                    setValid(false);
                }
                if (!scope.glRequired) {
                    setValid(true);
                }
            });

            scope.$watch('glRequired', function (newValue) {
                if (!newValue || (scope.ngModel && scope.ngModel.Title)) {
                    setValid(true);
                }
                else if (!scope.ngModel) {
                    setValid(false);
                }
            });

            scope.data = [];
            scope.busy = false;
            scope.searching = false;
            scope.listIndex = 0;

            var setPosition = function () {

                list.css({
                    top: source.offset().top + source.outerHeight(),
                    left: source.offset().left,
                    width: source.outerWidth(),
                    position: 'absolute',
                    display: 'none'
                });

                if (list.offset().top + list.outerHeight() > doc.innerHeight())
                    list.css({
                        top: input.offset().top - list.outerHeight()
                    }).addClass('top');
                else
                    list.removeClass('top');

                list.show();
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

                scope.searching = true;
                scope.busy = true;

                if (search.aborter != null)
                    search.aborter.resolve();

                if (scope.query && scope.query.length > 0) {
                    showSearch();
                }
                else {
                    hideSearch();
                    return;
                }

                var url = Utils.qs.add(attrs.glApiUrl, "Query=" + encodeURIComponent(scope.query) + "&Count=5");
                this.aborter = $q.defer();
                var req = {
                    timeout: search.aborter.promise,
                    url: App.apiUrl + url,
                    headers: App.authorizationHeader()
                };

                $http(req).then(function (response) {
                    scope.busy = false;
                    scope.data = response.data;
                    scope.listIndex = 0;
                    scope.fetched = true;
                    $timeout(setPosition);
                }, function (response) {
                    if (response.status === -1)
                        return;
                    scope.busy = false;
                    scope.data = [];
                    scope.listIndex = 0;
                    $timeout(setPosition);
                });
            };

            input.on('focus', function () {
                search();
                ctrl[0].$setTouched();
            });

            clearbutton.on('click', function (e) {
                scope.query = '';
                scope.data = [];
                scope.searching = false;
                scope.$apply();
            });

            input.on('keyup', function (e) {

                if (attrs.glAutoAdd) {
                    selectedQuery = scope.query;
                    scope.ngModel = selectedQuery;
                }
                else if (scope.query != selectedQuery) {
                    scope.ngModel = null;
                    if (attrs.glRequired)
                        setValid(false);
                }

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

            scope.setNew = function () {
                scope.ngModel = {};
                scope.ngModel[attrs.glAddNew] = scope.query;
                scope.searching = false;
                hideSearch();
                setValid(true);
            };

            scope.set = function (x) {
                if (x) {
                    scope.query = x.Title;
                    selectedQuery = scope.query;
                    if (attrs.glSelectionModel)
                        scope.ngModel = x[attrs.glSelectionModel];
                    else
                        scope.ngModel = x;
                    if (scope.glOnSet)
                        scope.glOnSet({ 'selected': x });
                }
                scope.searching = false;
                hideSearch();
                setValid(true);
            };

        }
    };
});