/* Title: Gloo Framework
 * Author: Bradley Searle (C2 Digital)
 * Source: www.c2.co.zw
 * License: CC BY 4.0 (https://creativecommons.org/licenses/by/4.0/legalcode)
 */

var agrishareApp = angular

    .module('agrishareApp', [])

    .controller('AppController', function ($attrs, $scope) {
        $scope.apiUrl = $attrs.ngApiUrl;
        $scope.menu = {};

        $scope.authToken = function () {
            var nameEQ = "agrishare=";
            var ca = document.cookie.split(';');
            for (var i = 0; i < ca.length; i++) {
                var c = ca[i];
                while (c.charAt(0) === ' ') c = c.substring(1, c.length);
                if (c.indexOf(nameEQ) === 0) return c.substring(nameEQ.length, c.length);
            }
            return null;
        };
    })

    .controller('SearchController', function ($scope, $timeout) {

        $scope.search = {
            step: 1,
            next: function () {
                if (Page_ClientValidate('Step' + $scope.search.step))
                    $scope.search.step += 1;
            },
            previous: function () {
                $scope.search.step -= 1;
            }
        };

    })

    .controller('SlideshowController', function ($scope, $timeout) {

        $scope.index = -1;

        var loop = function () {
            if ($scope.index === 2)
                $scope.index = 0;
            else
                $scope.index += 1;            
            $timeout(loop, 5000);
        };
        loop();

    })

    .controller('PollController', function ($attrs, $http, $scope) {

        $scope.transactions = [];

        var poll = function () {

            var url = $scope.apiUrl + '/transactions/ecocash/poll?BookingId=' + $attrs.agBookingId;
            $http({
                url: url,
                headers: { "Authorization": $scope.authToken() }
            }).then(function (response) {
                if (response.data.Message)
                    location.href = location.href;
                else {
                    $scope.transactions = response.data.Transactions;
                    setTimeout(poll, 1000);
                }
            }, function () {
                setTimeout(poll, 1000);
            });

        };

        poll();


    })

    .controller('AvailabilityController', function ($http, $scope) {

        $scope.calendar = {
            busy: false,
            visible: false,
            dates: [],
            listingId: 0,
            startDate: null,
            days: 1,
            currentDate: new Date(),
            months: ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'],

            show: function () {
                var calendar = this;
                calendar.currentDate = new Date();
                calendar.visible = true;
                calendar.fetch();
            },

            hide: function () {
                this.visible = false;
            },

            previous: function () {
                var calendar = this;
                calendar.currentDate.setDate(1);
                if (calendar.currentDate.getMonth() === 0) {
                    calendar.currentDate.setMonth(11);
                    calendar.currentDate.setYear(calendar.currentDate.getFullYear() - 1);
                }
                else
                    calendar.currentDate.setMonth(calendar.currentDate.getMonth() - 1);

                if (calendar.currentDate < new Date())
                    calendar.currentDate = new Date();

                calendar.fetch();
            },

            next: function () {
                var calendar = this;
                calendar.currentDate.setDate(1);
                if (calendar.currentDate.getMonth() === 11) {
                    calendar.currentDate.setMonth(0);
                    calendar.currentDate.setYear(calendar.currentDate.getFullYear() + 1);
                }
                else
                    calendar.currentDate.setMonth(calendar.currentDate.getMonth() + 1);
                calendar.fetch();
            },

            fetch: function () {
                var calendar = this;
                if (calendar.busy)
                    return;
                calendar.busy = true;

                var startDate = calendar.currentDate;

                var endDate = new Date(startDate);
                while (endDate.getMonth() === startDate.getMonth()) {
                    endDate.setDate(endDate.getDate() + 1);
                }
                endDate.setDate(endDate.getDate() - 1);

                calendar.month = calendar.months[startDate.getMonth()];

                var url = $scope.apiUrl + '/listings/availability?ListingId=' + calendar.listingId + '&StartDate=' + calendar.formatDate(startDate) + '&EndDate=' + calendar.formatDate(endDate) + '&Days=' + calendar.days;
                $http({
                    url: url,
                    headers: { "Authorization": $scope.authToken() }
                }).then(function (response) {
                    calendar.dates = response.data.Calendar;
                    calendar.busy = false;
                }, function () {
                    calendar.busy = false;
                });
            },

            formatDate: function (date) {
                var year = date.getFullYear();
                var month = date.getMonth() + 1;
                if (month < 10) month = '0' + month;
                var day = date.getDate();
                if (day < 10) day = '0' + day;
                return year + '-' + month + '-' + day;
            },

            setStartDate: function (available, date) {
                if (!available) return;
                var calendar = this;
                var nqs = '?';
                var qs = location.search.replace(/^\?/gi, '').split('&');
                for (var i = 0; i < qs.length; i++) {
                    var pair = qs[i].split('=');
                    if (pair[0] === 'std')
                        nqs += 'std=' + calendar.formatDate(new Date(date)) + '&';
                    else
                        nqs += qs[i] + '&';
                }
                location.href = location.origin + location.pathname + nqs;
            },

            date: function (arg) {
                return new Date(arg);
            },

            month: 'January'
        };

    });

