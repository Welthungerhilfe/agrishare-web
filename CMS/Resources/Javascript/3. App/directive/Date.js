/* Title: Gloo Framework
 * Author: Bradley Searle (C2 Digital)
 * Source: www.c2.co.zw
 * License: CC BY 4.0 (https://creativecommons.org/licenses/by/4.0/legalcode) */

agrishareApp.directive('glDate', function ($timeout) {
    return {
        restrict: 'E',
        replace: true,

        scope: {
            label: '@glLabel',
            glooDateModel: '=ngModel',
            disabledBool: '=glDisabled'
        },

        template: function (element, attrs) {

            var required = attrs.glRequired || false;
            var label = attrs.glLabel || '';
            var name = attrs.glRequired ? attrs.glName || label : '';
            if (required && label)
                label += ' *';
            var placeholder = attrs.glPlaceholder || label;
            var requiredAttr = required ? 'required' : '';

            return '' +
                '<div class="row date-row">' +
                '<label ng-if="label">' + label + '</label>' +
                '<div class="icon">' +
                '<i class="material-icons">date_range</i>' +
                '<input type="text" placeholder="' + placeholder + '" ng-model="displayDate" ' + requiredAttr + ' name="' + name + '" ng-disabled="disabledBool" />' +
                '<a class="material-icons md-18" ng-click="clear()" ng-show="displayDate && !disabledBool">close</a>' +
                '</div>' +
                '<div class="date-dropdown">' +
                '<span><strong>{{month}}</strong><input type="number" value="2018" min="0" max="2999" ng-model="year" /><span></span><a class="material-icons month-nav">keyboard_arrow_left</a><a class="material-icons month-nav">keyboard_arrow_right</a></span>' +
                '<ul><li ng-repeat="day in days" ng-click="setDate(day.date)" ng-class="{\'selected\': day.selected, \'other\': day.other \}">{{day.date | date:\'dd\'}}</li></ul>' +
                '</div> ' +
                '</div>';
        },

        link: function (scope, element, attrs) {

            var label = element.find('label').css({ opacity: 0 });

            scope.$watch('glooDateModel', function (newValue) {              
                if (newValue)
                    label.stop(true, true).animate({ opacity: 1 });
                else
                    label.stop(true, true).animate({ opacity: 0 });
            });

            //

            var body = angular.element(document.body);
            var icon = element[0].getElementsByTagName("i")[0];
            var input = element[0].getElementsByTagName("input")[0];
            var popup = element[0].getElementsByClassName("date-dropdown")[0];
            var anchors = element[0].getElementsByClassName("month-nav");

            scope.calendarDate = null;
            scope.days = [];

            //

            popup.style.display = 'none';

            var showPopup = function () {
                setMonth();

                body.append(popup);
                body.on('mouseup', hidePopup);

                var source = angular.element(input).parent();
                var diag = angular.element(popup);

                var width = 220;   
                var top = source.offset().top + 5;
                var left = source.offset().left + 5;

                setTimeout(function () {
                    diag.css({
                        top: top,
                        left: left,
                        width: width,
                        display: 'block',
                        position: 'absolute'
                    });

                    if (diag.offset().top + diag.outerHeight() > body.innerHeight())
                        diag.css({
                            top: source.offset().top + source.outerHeight() - diag.outerHeight() - 5
                        });

                }, 1);
                
            };

            var hidePopup = function (e) {
                if (e) {
                    var el = e.target;
                    while (el != null) {
                        el = el.parentNode;
                        if (el == popup)
                            return;
                    }
                }
                body.off('mouseup', hidePopup);
                popup.style.display = 'none';
                element.append(popup);
            };

            var setMonth = function () {
                if (!(scope.glooDateModel instanceof Date)) {
                    $timeout(function () { scope.glooDateModel = new Date(); });
                    scope.calendarDate = moment(new Date());
                }

                var date = scope.calendarDate.clone();
                var month = date.month();
                scope.month = date.format('MMM');
                date.date(1);
                date.add(-date.day(), 'days');
                $timeout(function () {
                    scope.days = [];
                    var counter = 0;
                    while (date.month() == month || counter % 7 > 0 || counter == 0) {
                        scope.days.push({ other: date.month() != month, date: date.toDate(), selected: date.format('DDMMYY') == moment(scope.glooDateModel).format('DDMMYY') });
                        date = date.add(1, 'days');
                        counter += 1;
                    }
                });
            };

            scope.setDate = function (date) {
                hidePopup();
                var day = moment(date);
                var mom = moment(scope.glooDateModel);
                mom.date(day.date());
                mom.month(day.month());
                $timeout(function () {
                    scope.glooDateModel = mom.toDate();
                });
            };

            scope.updateDate = function () {
                var mom = moment(scope.displayDate);
                $timeout(function () {
                    scope.glooDateModel = mom.isValid() ? mom.toDate() : null;
                });
            };

            scope.clear = function () {
                $timeout(function () {
                    scope.displayDate = null;
                    scope.glooDateModel = '';
                    hidePopup();
                });
            };

            //

            var formatDate = function () {
                var mom = new moment(scope.glooDateModel);
                scope.displayDate = mom.format('D MMMM YYYY');
            };

            //

            anchors[0].onclick = function () {
                setMonth(scope.calendarDate.add(-1, 'months'));
            };

            anchors[1].onclick = function () {
                setMonth(scope.calendarDate.add(1, 'months'));
            };

            icon.onclick = showPopup;

            input.onfocus = function () {
                input.blur();
                showPopup();
            };

            input.onclick = function () {
                input.blur();
                showPopup();
            };

            //

            scope.$watch('year', function () {
                if (scope.year == undefined)
                    return;
                scope.calendarDate.year(scope.year);
                scope.glooDateModel = scope.calendarDate.toDate();
                setMonth(scope.calendarDate);
                formatDate();
            });

            scope.$watch('glooDateModel', function () {
                if (!(scope.glooDateModel instanceof Date))
                    return;
                scope.calendarDate = moment(scope.glooDateModel);
                scope.year = scope.glooDateModel.getFullYear();
                formatDate();
            });

        }
    };
});