/* Title: Gloo Framework
 * Author: Bradley Searle (C2 Digital)
 * Source: www.c2.co.zw
 * License: CC BY 4.0 (https://creativecommons.org/licenses/by/4.0/legalcode) */

agrishareApp.directive('glTime', function ($timeout) {
    return {
        restrict: 'E',
        replace: true,

        scope: {
            label: '@glLabel',
            icon: '@glIcon',
            ngModel: '='
        },

        template: function (element, attrs) {
            var type = attrs.glType || 'text';
            var required = attrs.glRequired || false;
            var label = attrs.glLabel || '';
            var name = attrs.glRequired ? attrs.glName || label : '';
            if (required && label)
                label += ' *';
            var placeholder = attrs.glPlaceholder || label;
            var requiredAttr = required ? 'required' : '';

            return '' +
                '<div class="row time-row" name="' + name + '">' +
                '<label>' + label + '</label>' +
                '<div class="icon">' +
                '<i class="material-icons">access_time</i>' +
                '<div class="placeholder" ng-hide="editing">' + placeholder + '</div>' +
                '<input type="text" placeholder="--" ng-show="editing" maxlength="2" />' +
                '<div ng-show="editing">:</div>' +
                '<input type="text" placeholder="--" ng-show="editing" maxlength="2" />' +
                '<span ng-show="editing">am</span>' +
                '</div>' +
                '</div>';
        },

        link: function (scope, element, attrs) {

            scope.editing = false;

            var label = element.find('label').css({ opacity: 0 });
            var placeholder = element.find('.placeholder');

            placeholder.on('click', function () {
                label.stop(true, true).animate({ opacity: 1 });
                $timeout(function () { scope.editing = true; });
                $timeout(function () { element.find('input').eq(0).focus(); }, 250);                
            });

            //

            var span = element.find('span');
            var hourInput = element.find('input').eq(0);
            var minuteInput = element.find('input').eq(1);

            span.on('click', function () {
                if (span.text().toLowerCase() == 'am')
                    span.text('pm');
                else
                    span.text('am');

                updateModel();
            });

            hourInput.on('keydown', function (e) {

                var val = hourInput.val();
                if (val == '')
                    return;

                var hrs = parseInt(val);
                if (isNaN(hrs)) {
                    hourInput.val('');
                    return;
                }

                if (e.keyCode == 40)
                    hrs -= 1;

                if (e.keyCode == 38)
                    hrs += 1;

                hrs = Math.min(12, hrs);
                hrs = Math.max(1, hrs);
                hourInput.val(hrs);

                updateModel();
            });

            minuteInput.on('keydown', function (e) {

                var val = minuteInput.val();
                if (val == '')
                    return;

                var mins = parseInt(val);
                if (isNaN(mins)) {
                    minuteInput.val('');
                    return;
                }

                if (e.keyCode == 40)
                    mins -= 1;

                if (e.keyCode == 38)
                    mins += 1;

                mins = Math.min(59, mins);
                mins = Math.max(0, mins);
                minuteInput.val(mins);

                updateModel();
            });

            minuteInput.on('blur', function (e) {

                var val = minuteInput.val();
                if (val == '')
                    return;

                var mins = parseInt(val);
                if (isNaN(mins)) {
                    minuteInput.val('');
                    return;
                }

                minuteInput.val(formatMinutes(mins));
            });

            //

            var updateModel = function () {

                var hrs = parseInt(hourInput.val());
                var mins = parseInt(minuteInput.val());
                if (span.text().toLowerCase() == 'pm' && hrs != 12) hrs += 12;
                if (span.text().toLowerCase() == 'am' && hrs == 12) hrs = 0;

                if (isNaN(hrs) || isNaN(mins))
                    return;

                $timeout(function () {
                    var date = scope.ngModel;
                    if (!(date instanceof Date))
                        date = new Date();

                    console.log(date);
                    date.setHours(hrs, mins);

                    scope.ngModel = date;
                });
            };

            scope.$watch('ngModel', function () {
                
                if (!(scope.ngModel instanceof Date)) {
                    scope.editing = false;
                    hourInput.val('');
                    minuteInput.val('');
                    label.stop(true, true).animate({ opacity: 0 });                    
                    return;
                }

                var hours = scope.ngModel.getHours();
                var minutes = scope.ngModel.getMinutes();

                if (hours > 11)
                    span.text('pm');

                if (hours > 12)
                    hours -= 12;

                if (hours == 0) {
                    span.text('am');
                    hours = 12;
                }

                hourInput.val(hours);
                minuteInput.val(formatMinutes(minutes));

                label.stop(true, true).animate({ opacity: 1 });
                scope.editing = true;
            });

            var formatMinutes = function (mins) {
                if (mins < 10)
                    return '0' + mins;
                return mins;
            };
        }
    };
});