/* Title: Gloo Framework
 * Author: Bradley Searle (C2 Digital)
 * Source: www.c2.co.zw
 * License: CC BY 4.0 (https://creativecommons.org/licenses/by/4.0/legalcode)
 */

agrishareApp.directive('glTextbox', function () {
    return {
        restrict: 'E',
        replace: true,

        scope: {
            label: '@glLabel',
            icon: '@glIcon',
            glRequired: '=',
            glDisabled: '=',
            glHint: '@',
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

            var customAttributes = [];
            customAttributes.push('ng-model="ngModel"');
            customAttributes.push('type="' + type + '"');
            customAttributes.push('name="' + name + '"');
            customAttributes.push('placeholder="' + placeholder + '"');
            if (attrs.glMin)
                customAttributes.push('min="' + attrs.glMin + '"');
            if (attrs.glMax)
                customAttributes.push('max="' + attrs.glMax + '"');
            if (attrs.glPattern)
                customAttributes.push('pattern="' + attrs.glPattern + '"');

            return '<div class="row">' +
                '<label ng-show="label">' + label + '</label>' +
                '<div ng-class="{\'icon\':icon}">' +
                '<i class="material-icons" ng-if="icon">{{icon}}</i>' +
                '<input ' + customAttributes.join(' ') + ' ng-required="glRequired" ng-disabled="glDisabled" />' +
                '</div>' +
                '<em class="hint" ng-if="glHint">{{glHint}}</em>' +
                '</div>';
        },

        link: function (scope, element, attrs) {

            var label = element.find('label').css({ opacity: 0 });
            var input = element.find('input');

            scope.$watch('ngModel', function (newValue) {
                if (newValue || newValue === 0)
                    label.stop(true, true).animate({ opacity: 1 });
            });

            input.on('keyup', function () {
                if (input.val() != '')
                    label.stop(true, true).animate({ opacity: 1 });
                else
                    label.stop(true, true).animate({ opacity: 0 });
            });
        }
    };
});