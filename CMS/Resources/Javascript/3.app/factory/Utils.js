/* Title: Gloo Framework
 * Author: Bradley Searle (C2 Digital)
 * Source: www.c2.co.zw
 * License: CC BY 4.0 (https://creativecommons.org/licenses/by/4.0/legalcode)
 */

agrishareApp.factory('Utils', function ($ngConfirm, $timeout, $window, ngToast) {
    var utils = {

        focus: function (id) {
            $timeout(function () {
                var element = $window.document.getElementById(id);
                if (element)
                    element.focus();
            });
        },

        confirm: function(title, content, callback) {
            $ngConfirm({
                theme: 'light',
                useBootstrap: false,
                type: 'blue',
                title: title,
                content: content,
                buttons: {
                    close: function () { },
                    continue: {
                        text: 'Continue',
                        btnClass: 'btn-orange',
                        action: callback
                    }
                }
            });
        },

        alert: function (title, content) {
            $ngConfirm({
                theme: 'light',
                useBootstrap: false,
                type: 'blue',
                title: title,
                content: content,
                buttons: {
                    close: function () { }
                }
            });
        },

        toast: {

            info: function(message) {
                ngToast.info({ content: message });
            },

            error: function (message) {
                ngToast.danger({ content: message });
            },

            success: function (message) {
                ngToast.success({ content: message });
            }

        },

        cookie: {

            create: function (name, value, days) {
                if (days) {
                    var date = new Date();
                    date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
                    var expires = "; expires=" + date.toGMTString();
                    document.cookie = name + "=" + value + expires + "; path=/";
                }
                else {
                    document.cookie = name + "=" + value + "; path=/";
                }
            },

            read: function (name) {
                var nameEQ = name + "=";
                var ca = document.cookie.split(';');
                for (var i = 0; i < ca.length; i++) {
                    var c = ca[i];
                    while (c.charAt(0) == ' ') c = c.substring(1, c.length);
                    if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
                }
                return null;
            },

            erase: function (name) {
                create(name, "", -1);
            }

        }

    };

    return utils;
});