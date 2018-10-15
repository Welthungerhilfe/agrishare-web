/* Title: Gloo Framework
 * Author: Bradley Searle (C2 Digital)
 * Source: www.c2.co.zw
 * License: CC BY 4.0 (https://creativecommons.org/licenses/by/4.0/legalcode) */

agrishareApp.directive('glDropdownList', function ($http, $q, App) {
    return {
        restrict: 'E',
        replace: true,

        scope: {
            label: '@glLabel',
            source: '=?glSource',
            icon: '@glIcon',
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

            getItemTemplateHtml = function () { return element.find('gl-item-template').html() || '{{item.Title}}' };
            return '' +
                '<div class="row gl-dropdown" name="' + name + '" tabindex="0">' +
                '<label ng-show="label">' + label + '</label>' +
                '<div ng-class="{\'icon\':icon}" class="input">' +
                '<i class="material-icons" ng-if="icon">{{icon}}</i>' +
                '<div ng-class="{\'placeholder\':!selection}">{{selection || \'' + placeholder + '\'}}</div>' +
                '<i class="material-icons">keyboard_arrow_down</i>' +
                '</div>' +
                '<div class="gl-dropdown-list">' +
                (attrs.glSearch ? '<div><input type="text" placeholder="Search" maxlength="256" ng-model="query" /></div>' : '') +
                '<ul>' +
                '<li ng-repeat="item in filteredSource track by $index" tabindex="0" class="item" ng-click="set(item)">' + getItemTemplateHtml() + '</li>' +
                '</ul>' +
                '</div>' +
                '</div>';
        },

        require: ['ngModel'],

        link: function (scope, element, attrs, ctrl) {

            var label = element.find('label').css({ opacity: 0 });

            scope.$watch('ngModel', function (newValue) {
                if (newValue || newValue == 0) 
                    label.stop(true, true).animate({ opacity: 1 });
                else
                    label.stop(true, true).animate({ opacity: 0 });
            });

            //

            scope.set = function (val) {

                if (attrs.glSelection)
                    scope.ngModel = val[attrs.glSelection];
                else
                    scope.ngModel = val;

                if (attrs.glTitle)
                    scope.selection = val[attrs.glTitle];
                else
                    scope.selection = val.Title;

                hideList();
            };

            scope.$watch('source', function (newValue) {
                if (scope.source)
                    scope.filteredSource = scope.source.slice();
            });

            scope.$watch('query', function (newValue) {
                if (!scope.source)
                    return;
                if (newValue) {
                    scope.listIndex = -1;
                    scope.filteredSource = scope.source.filter(e => e.Title.toLowerCase().indexOf(newValue.toLowerCase()) > -1);
                }
                else
                    scope.filteredSource = scope.source.slice();
            });

            //

            if (attrs.glRequired)
                ctrl[0].$setValidity('required', false);

            scope.$watch('ngModel', function (newValue) {
                if (newValue || newValue == 0)
                    ctrl[0].$setValidity('required', true);
                else if (attrs.glRequired)
                    ctrl[0].$setValidity('required', false);

                updateSelection();

            }, true);

            var updateSelection = function () {
                if (scope.source && (scope.ngModel || scope.ngModel == 0)) {
                    var selectedValue = attrs.glSelection ? scope.ngModel : scope.ngModel.Id;
                    for (var i = 0; i < scope.source.length; i++) {
                        var item = scope.source[i];
                        var compareValue = attrs.glSelection ? item[attrs.glSelection] : item.Id;
                        var prop = attrs.glTitle || 'Title';
                        if (selectedValue === compareValue)
                            scope.selection = scope.source[i][prop];
                    }
                }
                else
                    scope.selection = '';
            };

            scope.validity = function (count) {
                ctrl[0].$setValidity('required', count > 0);
            };

            //

            var body = angular.element(document.body);
            var input = angular.element(element[0].getElementsByClassName("input")[0]);
            var list = angular.element(element[0].getElementsByClassName("gl-dropdown-list")[0]);
            var source = angular.element(element);
            var search = angular.element(element[0].getElementsByTagName("input")[0]);

            var setPosition = function () {
                list.css({
                    top: input.offset().top + input.outerHeight(),
                    left: input.offset().left,
                    width: input.outerWidth(),
                    position: 'absolute'
                });

                if (list.offset().top + list.outerHeight() > body.innerHeight())
                    list.css({
                        top: input.offset().top - list.outerHeight()
                    });
            };

            var hideList = function (e) {
                if (e) {
                    var el = e.target;
                    while (el != null) {
                        if (el == list.get(0) || el == input.get(0))
                            return;
                        el = el.parentNode;
                    }
                }
                body.off('mouseup', hideList);
                list.css({ display: 'none' });
                scope.busy = false;
                element.append(list);
                source.removeClass('expanded');
            };

            var showList = function () {
                scope.listIndex = -1;
                body.append(list);
                body.off('mouseup', hideList);
                body.on('mouseup', hideList);
                setPosition();
                list.css({ display: 'block' });
                source.addClass('expanded');
            };

            source.on('focus', function () {
                showList();
                if (search)
                    search.focus();
            });

            source.on('keydown', function (e) {
                handleKeyCode(e);
                if (e.keyCode == 9)
                    hideList();
                if (e.keyCode == 40 || e.keyCode == 38 || e.keyCode == 13)
                    e.preventDefault();
            });

            search.on('keydown', function (e) {
                handleKeyCode(e);
            });

            input.on('click', function () {
                showList();
                if (search)
                    search.focus();
            });

            scope.listIndex = -1;

            var handleKeyCode = function (e) {

                var items = list.find('ul').children();

                if (e.keyCode == 40) {
                    scope.listIndex += 1;
                    scope.listIndex = Math.min(scope.listIndex, items.length - 1);
                    items.removeClass('selected');
                    items.eq(scope.listIndex).addClass('selected');
                    return false;
                }

                if (e.keyCode == 38) {
                    items.removeClass('selected');
                    scope.listIndex -= 1;
                    scope.listIndex = Math.max(scope.listIndex, 0);
                    items.eq(scope.listIndex).addClass('selected');
                    return false;
                }

                if (e.keyCode == 13) {
                    list.find('ul').children().eq(scope.listIndex).triggerHandler('click');
                    input.blur();
                    return false;
                }

            };

            list.on('mouseenter', function () {
                list.find('ul').children().removeClass('selected');
                scope.listIndex = 0;
            });

            //

            scope.$watch(function () {
                return attrs.glSourceUrl;
            }, function (value) {

                if (value) {
                    var req = {
                        url: App.apiUrl + attrs.glSourceUrl,
                        headers: App.authorizationHeader()
                    };

                    $http(req).then(function (response) {
                        scope.source = response.data;    
                        updateSelection();
                    }, function () { });
                }

            });

        }
    };
});