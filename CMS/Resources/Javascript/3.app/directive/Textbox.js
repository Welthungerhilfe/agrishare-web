/* Title: Gloo Framework
 * Author: Bradley Searle (C2 Digital)
 * Source: www.c2.co.zw
 * License: CC BY 4.0 (https://creativecommons.org/licenses/by/4.0/legalcode)
 */

agrishareApp.directive('agTextbox', function () {
    return {
        restrict: 'E',

        replace: true,

        scope: {
            label: '@agLabel',
            icon: '@agIcon',
            agRequired: '=',
            agDisabled: '=',
            agHint: '@',
            ngModel: '='
        },

        template: function (element, attrs) {
            var type = attrs.agType || 'text';
            var required = attrs.agRequired || false;
            var label = attrs.agLabel || '';
            var name = attrs.agRequired ? attrs.agName || label : '';
            if (required && label)
                label += ' *';
            var placeholder = attrs.agPlaceholder || label;
            var requiredAttr = required ? 'required' : '';

            var customAttributes = [];
            customAttributes.push('ng-model="ngModel"');
            customAttributes.push('type="' + type + '"');
            customAttributes.push('name="' + name + '"');
            customAttributes.push('placeholder="' + placeholder + '"');
            if (attrs.agMin)
                customAttributes.push('min="' + attrs.agMin + '"');
            if (attrs.agMax)
                customAttributes.push('max="' + attrs.agMax + '"');
            if (attrs.agPattern)
                customAttributes.push('pattern="' + attrs.agPattern + '"');

            return '<div class="row">' +
                '<label ng-show="label">' + label + '</label>' +
                '<div ng-class="{\'icon\':icon}">' +
                '<i class="material-icons" ng-if="icon">{{icon}}</i>' +
                '<input ' + customAttributes.join(' ') + ' ng-required="agRequired" ng-disabled="agDisabled" />' +
                '</div>' +
                '<em class="hint" ng-if="agHint">{{agHint}}</em>' +
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