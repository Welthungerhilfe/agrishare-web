/* Title: Gloo Framework
 * Author: Bradley Searle (C2 Digital)
 * Source: www.c2.co.zw
 * License: CC BY 4.0 (https://creativecommons.org/licenses/by/4.0/legalcode) */

agrishareApp.directive('glUpload', function ($timeout, App, Upload, Utils) {
    return {
        restrict: 'E',
        replace: true,

        scope: {
            required: '@glRequired',
            label: '@glLabel',
            ngModel: '='
        },

        template: function (element, attrs) {
            var label = attrs.glLabel || '';
            var required = attrs.glRequired;
            var name = required ? attrs.glName || attrs.glLabel || 'Upload' : '';
            if (required && label)
                label += ' *';
            if (attrs.glMultiple) {
                return '<div class="upload" name="' + name + '">' +
                    (label ? '<label>' + label + '</label>' : '') +
                    '<div ngf-drop="upload($files, ngModel)" ng-model="files" ngf-drag-over-class="\'dragover\'" ngf-multiple="' + attrs.glMultiple + '" ngf-allow-dir="' + attrs.glMultiple + '">' +
                    '<ul ng-show="ngModel && ngModel.length>0">' +
                    '<li ng-repeat="item in ngModel">' +
                    '<span ng-hide="item.IsImage"><i class="material-icons">attachment</i></span>' +
                    '<span ng-show="item.IsImage" style="background-image:url({{item.ThumbPath}})"></span>' +
                    '<strong>{{item.Title}}</strong>' +
                    '<gl-icon-button gl-icon="delete_outline" ng-click="ngModel.splice($index, 1);validity(ngModel.length)"></gl-icon-button>' +
                    '</li>' +
                    '</ul>' +
                    '<strong ngf-select="upload($files, ngModel)" ngf-multiple="' + attrs.glMultiple + '" ngf-allow-dir="' + attrs.glMultiple + '"><i class="material-icons">cloud_upload</i> Choose files or drop here</strong>' +
                    '<span class="progress" style="width: {{progress}}%" ng-show="progress > 0 && progress < 100"></span>' +
                    '</div>' +
                    '</div>';
            } else {
                return '<div class="upload" name="' + name + '">' +
                    (label ? '<label>' + label + '</label>' : '') +
                    '<div ngf-drop="upload($files, ngModel, \'' + attrs.glModelAttribute + '\')" ng-model="files" ngf-drag-over-class="\'dragover\'" ngf-multiple="' + attrs.glMultiple + '" ngf-allow-dir="' + attrs.glMultiple + '">' +
                    '<ul ng-show="ngModel[\'' + attrs.glModelAttribute + '\']">' +
                    '<li>' +
                    '<span ng-hide="ngModel[\'' + attrs.glModelAttribute + '\'].IsImage"><i class="material-icons">attachment</i></span>' +
                    '<span ng-show="ngModel[\'' + attrs.glModelAttribute + '\'].IsImage" style="background-image:url({{ngModel[\'' + attrs.glModelAttribute + '\'].Thumb}})"></span>' +
                    '<strong>{{ngModel[\'' + attrs.glModelAttribute + '\'].Filename}}</strong>' +
                    '<gl-icon-button gl-icon="delete_outline" ng-click="ngModel[\'' + attrs.glModelAttribute + '\']=null;validity(0)"></gl-icon-button>' +
                    '</li>' +
                    '</ul>' +
                    '<strong ngf-select="upload($files, ngModel, \'' + attrs.glModelAttribute + '\')" ngf-multiple="' + attrs.glMultiple + '" ngf-allow-dir="' + attrs.glMultiple + '"><i class="material-icons">cloud_upload</i> Choose file or drop here</strong>' +
                    '<span class="progress" style="width: {{progress}}%" ng-show="progress > 0 && progress < 100"></span>' +
                    '</div>' +
                    '</div>';
            }
        },

        require: ['ngModel'],

        link: function (scope, element, attrs, ctrl) {

            if (!attrs.glMultiple && (attrs.glModelAttribute == null || attrs.glModelAttribute == ''))
                alert("glUpload: model attribute must be defined");

            if (attrs.glRequired)
                ctrl[0].$setValidity('required', false);

            scope.$watch('ngModel', function (newValue) {
                if (attrs.glMultiple && newValue && newValue.length > 0)
                    ctrl[0].$setValidity('required', true);
                if (!attrs.glMultiple && newValue && newValue.Id > 0)
                    ctrl[0].$setValidity('required', true);
            });

            scope.progress = 0;

            scope.upload = function (files, model, modelAttribute) {

                if (files && files.length) {
                    for (var i = 0; i < files.length; i++) {
                        var file = files[i];
                        if (!file.$error) {
                            Upload.upload({
                                url: App.apiUrl + '/upload',
                                headers: App.authorizationHeader(),
                                data: { file: file }
                            }).then(function (response) {
                                $timeout(function () {
                                    ctrl[0].$setValidity('required', true);
                                    ctrl[0].$setTouched();
                                    if (attrs.glMultiple) {                                        
                                        for (var j = 0; j < response.data.length; j++)
                                            model.push(response.data[j]);
                                    } else if (response.data.length == 1)
                                        model[modelAttribute] = response.data[0];
                                });
                            }, function (response) {
                                alert(response.data.Message);
                            }, function (evt) {
                                evt.config.data.file.progress = Math.min(100, parseInt(100.0 * evt.loaded / evt.total));
                                var p = 0;
                                for (var k = 0; k < files.length; k++)
                                    p += files[k].progress;
                                scope.progress = p / files.length;
                            });
                        }
                    }
                }
            };

            scope.validity = function (count) {
                ctrl[0].$setValidity('required', count > 0);
            };

        }
    };
});
