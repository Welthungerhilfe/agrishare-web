/* Title: Gloo Framework
 * Author: Bradley Searle (C2 Digital)
 * Source: www.c2.co.zw
 * License: CC BY 4.0 (https://creativecommons.org/licenses/by/4.0/legalcode) */

agrishareApp.directive('glEditor', function ($timeout, App) {
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
            var requiredAttr = required ? 'required' : '';

            return '' +
                '<div class="row gl-editor">' +
                '<label>' + label + '</label>' +
                '<div>' +
                '<textarea ng-model="ngModel" ' + requiredAttr + ' name="' + name + '"></textarea>' +
                '</div>' +
                '</div>';
        },

        link: function (scope, element, attrs) {

            var textarea = element.find('textarea');

            var listener;

            var ckEditor;
            ClassicEditor
                .create(textarea.get(0), {
                    simpleUpload: {
                        uploadUrl: App.apiUrl + '/ckeditor/upload/image'
                    }
                })
                .then(editor => {

                    editor.model.document.on('change', (e) => {
                        if (editor.model.document.differ.getChanges().length > 0) {
                            $timeout(function () {
                                listener();
                                scope.ngModel = editor.getData();
                            });
                        }
                    });

                    scope.$on('$destroy', function () {
                        editor.destroy()
                            .catch(error => {
                                console.log(error);
                            });
                    });

                    listener = scope.$watch('ngModel', function (newValue) {
                        if (!newValue)
                            return;
                        editor.setData(newValue);
                    }, true);
                })
                .catch(error => {
                    console.error(error);
                    alert('There was an error starting the editor: ' + error);
                });

        }
    };
});