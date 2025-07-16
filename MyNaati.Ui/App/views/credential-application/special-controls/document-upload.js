define([],
    function () {
        return {
            getInstance: getInstance
        };

        function getInstance(options) {
            var credentialApplication = options.credentialApplication;

            var vm = {
                documents: ko.observableArray(),
                documentTypes: ko.observableArray(),
                applicationId: options.applicationId,
                applicationFormId: options.applicationFormId,
                naatiNumber: options.naatiNumber,
                token: options.token,
                question: options.question,
                sections: options.sections,
                enableActions: options.enableActions,
                isUploadingCount: ko.observable(0),
                documentsLoaded: ko.observable(false),
                documentTypesLoaded: ko.observable(false),
                fileEvent: {
                    filesuccessremove: filesuccessremove,
                    fileuploaded: fileuploaded,
                    filebatchselected: filebatchselected,
                    fileuploaderror: fileuploaderror,
                    filepreremove: filepreremove
                }
            };

            ko.computed(function () {
                var enableActions = !vm.question.current() || (vm.isUploadingCount() === 0 && vm.documentsLoaded() && vm.documentTypesLoaded());
                vm.enableActions(enableActions);
            });

            vm.getFileOptions = function (documentType) {
                var options = {
                    uploadUrl: window.baseUrl + 'credentialapplication/upload',
                    deleteUrl: window.baseUrl + 'credentialapplication/deletedocument',
                    maxFilePreviewSize: 10240,
                    maxFileSize: 10240,
                    fileActionSettings: {
                        showUpload: false,
                        showZoom: false
                    },
                    uploadExtraData: {
                        ApplicationId: vm.applicationId(),
                        ApplicationFormId: vm.applicationFormId(),
                        TypeId: documentType.Id,
                        Token: vm.token()
                    },
                    browseOnZoneClick: true,
                    showRemove: false,
                    showCancel: false,
                    showClose: false,
                    showUpload: false,
                    showBrowse: false,
                    showCaption: false,
                    dropZoneTitle: 'Click or drag a file',
                    dropZoneClickTitle: '',
                    allowedFileExtensions: ['bmp', 'gif', 'jpg', 'jpeg', 'png', 'doc', 'csv', 'docx', 'pdf', 'ppt', 'pptx', 'rtf', 'tiff', 'txt', 'xls', 'xlsx'],
                    layoutTemplates: {
                        main2: '{preview}\n{remove}\n{cancel}\n{upload}\n{browse}\n'
                    },
					previewTemplates: {
						pdf: '<div class="file-preview-frame {frameClass}" id="{previewId}" data-fileindex="{fileindex}" data-template="{template}">\n' +
							'	<div class="kv-file-content">\n' +
							'		<div class="kv-preview-data file-preview-other-frame" {style}>\n' +
							'			<div class="file-preview-other">\n' +
							'				<span class="file-other-icon"><i class="glyphicon glyphicon-file"></i></span>\n' +
							'			</div>\n' +
							'		</div>\n' +
							'	</div>\n' +
							'	{footer} \n' +
							'</div>\n'
					}
                };

                return options;
            };

            vm.rows = ko.pureComputed(function () {
                var mandatory = vm.mandatory();
                var rows = [];
                var i = 0;
                var row = null;

                while (i < mandatory.length) {
                    if (i % 2 === 0) {
                        row = { cols: [] };
                        rows.push(row);
                    }

                    row.cols.push(mandatory[i]);
                    i++;
                }

                return rows;
            });

            vm.mandatory = ko.pureComputed(function () {
                return ko.utils.arrayFilter(vm.documentTypes(), function (d) {
                    return d.Required;
                });
            });

            vm.documentTypeOptions = {
                value: vm.documentType,
                multiple: false,
                options: vm.documentTypes,
                optionsValue: 'Id',
                optionsText: 'DisplayName',
            };

            vm.documents.extend({
                validation: {
                    validator: function (val) {
                        var mandatoryDocuments = vm.mandatory();

                        for (var i = 0; i < mandatoryDocuments.length; i++) {
                            if (!mandatoryDocuments[i].uploaded()) {
                                return false;
                            }
                        }

                        return true;
                    },
                    message: 'You need to add all mandatory documents.'
                }
            });

            var validatedObservable = ko.validatedObservable([vm.documents]);
            var validation = $.extend(true, {}, validatedObservable, { isValid: isValid });

            vm.validation = validation;

            vm.load = function () {
                vm.enableActions(false);
                vm.documentTypesLoaded(false);
                credentialApplication.documentTypes({ ApplicationId: vm.applicationId(), ApplicationFormId: vm.applicationFormId(), Token: vm.token() }).then(function (data) {
                    ko.utils.arrayForEach(data, function (documentType) {
                        documentType.uploaded = ko.pureComputed(function () {
                            return ko.utils.arrayFirst(vm.documents(), function (d) {
                                return d.TypeId === documentType.Id;
                            });
                        });
                    });
                    vm.documentTypes(data);
                    vm.documentTypesLoaded(true);
                });
                loadDocuments();
                resetValidation();
            };

            vm.removeDocument = function (document) {
                var defer = Q.defer();

                mbox.remove({ title: 'Remove document', content: 'Are you sure that you want to remove this document?' }).then(function (answer) {
                    if (answer === 'yes') {
                        credentialApplication.deleteDocument({ DocumentId: document.Id, Token: vm.token(), ApplicationId: vm.applicationId() }).then(function () {
                            vm.documents.remove(document);
                            resetValidation();
                            defer.resolve();
                        });
                    }
                });

                return defer.promise;
            };

            function loadDocuments() {
                vm.documentsLoaded(false);
                return credentialApplication.documents({ ApplicationFormId: vm.applicationFormId(), ApplicationId: vm.applicationId(), Token: vm.token() }).then(function (data) {
                    vm.documents(data);
                    vm.documentsLoaded(true);
                    resetValidation();
                });
            }

            function resetValidation() {
                if (vm.validation.errors) {
                    vm.validation.errors.showAllMessages(false);
                }
            }

            function isValid() {
                return ko.utils.arrayFirst(vm.mandatory(), function (m) {
                    return !m.uploaded();
                }) === null;
            }

            function filesuccessremove(model, event, id) {
                var $preview = $('#' + id);
                var canRemove = $preview.data('canRemove');
                var document = $preview.data('document');
                if (canRemove) {
                    return true;
                }

                if (!vm.question.section.current()) {
                    mbox.confirm({
                        title: 'Change Answer',
                        content:
                            'The next sections will reload if you change the answer. Are you sure that you want to change the answer?'
                    }).then(function(argument) {
                        if (argument === 'yes') {
                            credentialApplication
                                .deleteDocument({
                                    DocumentId: document.Id,
                                    Token: vm.token(),
                                    ApplicationId: vm.applicationId()
                                }).then(function() {
                                    vm.documents.remove(document);
                                    resetValidation();
                                    $preview.data('canRemove', true);
                                    $preview.find('.kv-file-remove')[0].click();
                                });
                            vm.question.Response(null);
                        }
                    });
                    return false;
                }

                vm.removeDocument(document).then(function () {
                    $preview.data('canRemove', true);
                    $preview.find('.kv-file-remove')[0].click();
                });

                return false;
            }

            function filepreremove(model, event, id, error) {
                if (error === "0") {
                    return true;
                }

                var $preview = $('#' + id);
                var document = $preview.data('document');
                var canRemove = $preview.data('canRemove');

                if (canRemove) {
                    return true;
                }

                mbox.remove({ title: 'Remove document', content: 'Are you sure that you want to remove this document?' }).then(function (answer) {
                    if (answer === 'yes') {
                        credentialApplication.deleteDocument({ DocumentId: document.Id, Token: vm.token() }).then(function () {
                            vm.documents.remove(document);
                            resetValidation();
                            $preview.data('canRemove', true);
                            $preview.find('.kv-file-remove')[0].click();
                        });
                    }
                });

                return false;
            }

            function fileuploaderror(model, event, previewId, errorMsg) {
                $('#' + previewId.id).find('.kv-file-remove')[0].click();
                mbox.alert({ title: 'Remove document', content: errorMsg });
            }

            function filebatchselected(model, event, files) {
                for (var i = 0; i < files.length; i++) {
                    if (files[i].name.length > 100) {
                        mbox.alert({ title: 'Invalid file', content: 'You cannot upload a file with more than 100 characters.' });
                        $(event.currentTarget).fileinput("clear");
                        return;
                    }
                }
                vm.isUploadingCount(vm.isUploadingCount() + files.length);
                $(event.currentTarget).fileinput("upload");
            }

            function fileuploaded(model, event, data, previewId, index) {
                var file = data.files[index];
                var document = {
                    Id: data.response.ids[0],
                    Name: file.name,
                    Size: file.size,
                    TypeId: data.extra.TypeId
                };
                
                vm.question.Response(document.Id);
                $('#' + previewId).data('document', document);
                vm.isUploadingCount(vm.isUploadingCount() - 1);
                vm.documents.push(document);

                resetValidation();
            }

            return vm;
        }
    });