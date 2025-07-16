define([],
    function () {
        return {
            getInstance: getInstance
        };

        function getInstance(options) {
            var credentialApplication = options.credentialApplication;

            var vm = {
                photoUrl: ko.observable(),
                applicationId: options.applicationId,
                applicationFormId: options.applicationFormId,
                naatiNumber: options.naatiNumber,
                token: options.token,
                question: options.question,
                enableActions: options.enableActions,
                fileValid: ko.observable(false),
                fileSelected: ko.observable(),
                fileOptions: {
                    multiple: false,
                    previewClass: 'single-file h-xxl',
                    uploadUrl: window.baseUrl + 'credentialapplication/personphoto',
                    maxFilePreviewSize: 10240,
                    maxFileSize: 10240,
                    fileActionSettings: {
                        showUpload: false,
                        showZoom: false
                    },
                    uploadExtraData: addExtraData,
                    browseOnZoneClick: true,
                    showRemove: false,
                    showCancel: false,
                    showClose: false,
                    showUpload: false,
                    showBrowse: false,
                    showCaption: false,
                    dropZoneTitle: 'Click or drag to preview a new photo',
                    dropZoneClickTitle: '',
                    allowedFileExtensions: ["jpg", "jpeg", "png"],
                    allowedPreviewTypes: ["image"],
                    layoutTemplates: {
                        preview: '<div class="file-preview {class}">\n' +
        '    <div class="{dropClass}">\n' +
        '    <div class="file-preview-thumbnails">\n' +
        '    </div>\n' +
        '    <div class="clearfix"></div>' +
        '    </div>\n' +
        '</div>'
                    },
                    previewTemplates: {
                        image: '<div class="file-preview-frame {frameClass}" id="{previewId}" data-fileindex="{fileindex}" data-template="{template}">\n' +
                        '   <div class="kv-file-content">' +
                        '        <div class="photo-upload h-xxl">\n' +
                        '            <span></span><img src="{data}" class="kv-preview-data file-preview-image img-rounded" title="{caption}" alt="{caption}" {style}>\n' +
                        '        </div>\n' +
                        '   </div>\n' +
                        '   <div class="m-t-sm text-center">Click on the photo to upload a different image.</div>' +
                        '</div>\n',
                        other: '<div class="file-preview-frame {frameClass}" id="{previewId}" data-fileindex="{fileindex}" data-template="{template}">\n' +
                        '   <div class="kv-file-content">' +
                        '        <div class="photo-upload h-xxl">\n' +
                        '           <div class="text-muted">Only "image" files less than 10MB are supported</div>\n' +
                        '        </div>\n' +
                        '   </div>\n' +
                        '</div>\n',
                    }
                },
                fileEvent: {
                    fileuploaderror: function (vm, event, data, msg) {
                        vm.fileValid(false);
                        vm.fileSelected(null);
                        $('#' + data.id).on('click', function () {
                            vm.fileOptions.element.click();
                        });
                    },
                    fileloaded: function (vm, event, file, previewId, index, reader) {
                        vm.fileValid(true);
                        vm.fileSelected(previewId);
                        $('#' + previewId).on('click', function () {
                            vm.fileOptions.element.click();
                        });
                    },
                    filebatchuploadcomplete: filebatchuploadcomplete
                }
            };

            vm.fileSelected.extend({
                required: {
                    onlyIf: ko.pureComputed(function () {
                        return !vm.photoUrl();
                    }),
                    message: 'File not selected'
                }
            });

            var validatedObservable = ko.validatedObservable([vm.fileSelected]);
            var validation = $.extend(true, {}, validatedObservable, { isValid: isValid });

            vm.validation = validation;

            vm.load = function () {
                vm.enableActions(false);
                vm.fileValid(false);
                vm.fileSelected(null);
                resetValidation();
                credentialApplication.personPhoto({ NAATINumber: vm.naatiNumber() || 0, Token: vm.token() || 0 }).then(function (data) {
                    vm.enableActions(true);
                    vm.photoUrl(data.PhotoUrl);
                });
            };

            vm.isValid = isValid;

            function addExtraData(previewId, index) {
                return {
                    applicationId: vm.applicationId() || 0,
                    applicationFormId: vm.applicationFormId() || 0,
                    naatiNumber: vm.naatiNumber() || 0,
                    token : vm.token() || 0
                };
            }

            function resetValidation() {
                if (vm.validation.errors) {
                    vm.validation.errors.showAllMessages(false);
                }
            }

            var filesDefer = Q.defer();
            function isValid() {
                if (vm.photoUrl()) {
                    return true;
                }

                if (!validatedObservable.isValid()) {
                    vm.validation.errors.showAllMessages();
                    return false;
                }

                if (!vm.fileValid()) {
                    return false;
                }

                filesDefer = Q.defer();
                $(vm.fileOptions.element).fileinput('upload');
                vm.enableActions(false);
                return filesDefer.promise;
            }

            function filebatchuploadcomplete() {
                vm.load();
                filesDefer.resolve(true);
                vm.enableActions(true);
            }

            return vm;
        }
    });