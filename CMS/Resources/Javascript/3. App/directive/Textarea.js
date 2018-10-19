/* Title: Gloo Framework
 * Author: Bradley Searle (C2 Digital)
 * Source: www.c2.co.zw
 * License: CC BY 4.0 (https://creativecommons.org/licenses/by/4.0/legalcode) */

agrishareApp.directive('glTextarea', function ($timeout) {
    return {
        restrict: 'E',
        replace: true,

        scope: {
            label: '@glLabel',
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

            return '' +
                '<div class="row">' +
                '<label ng-if="label">' + label + '</label>' +
                '<div>' +
                '<textarea placeholder="' + placeholder + '" ng-model="ngModel" ' + requiredAttr + ' name="' + name + '"></textarea>' +
                '</div>' +
                '</div>';
        },

        link: function (scope, element, attrs) {

            var label = element.find('label').css({ opacity: 0 });

            scope.$watch('ngModel', function (newValue) {              
                if (newValue)
                    label.stop(true, true).animate({ opacity: 1 });
                else
                    label.stop(true, true).animate({ opacity: 0 });
            });

            var textarea = element.find('textarea');
            var textareaElement = textarea.get(0);
            var offset = textareaElement.offsetHeight - textareaElement.clientHeight;            
            var updateHeight = function () {
                textarea.css('height', 'auto').css('height', textareaElement.scrollHeight + offset);
            };

            textarea.on('input change keyup', updateHeight);

            scope.$watch('ngModel', function (newValue) {
                if (newValue) {
                    $timeout(updateHeight);
                }
            });

        }
    };
});