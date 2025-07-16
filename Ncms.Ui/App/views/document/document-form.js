define([
    'services/file-data-service',
    'services/screen/message-service',
    'services/servercallbackprocessor'
], function (fileService, message, servercallbackprocessor) {
    var documentTypes = ko.observableArray();

    fileService.getFluid('documentTypes').then(function (data) {
        documentTypes(data);
    });

    var vm = {
        getInstance: getInstance
    };

    return vm;

    function getInstance() {
        var serverModel = {
            Id: ko.observable(),
            Title: ko.observable().extend({ required: true, maxLength: 100 }),
            File: ko.observable().extend({ required: true }),
            StoredFileId: ko.observable(),
            UnmodifiedTitle: ko.observable(),
            Type: ko.observable()
        };

        var emptyModel = ko.toJS(serverModel);

        var vm = {
            formData: function () { },
            getDocumentsPromise: null,
            transformDocuments: null,
            postDocumentPromise: null,
            removeDocumentPromise: null,
            downloadDocumentUrlFunc: null,
            populateDocuments: null,
            currentDocument: serverModel,
            replaceFile: ko.observable(false),
            typeOptions: ko.observableArray(),
            documentTypes: documentTypes,
            childDocuments: ko.observableArray(),
            allowCreate: ko.observable(true),
            allowRemove: ko.observable(true),
            allowEdit: ko.observable(true),
            allowDownload: ko.observable(true),
            fileUpload: {
                events: {
                    fileuploaddone: function () {
                        vm.search();
                        vm.cancel();
                    }
                },
                files: ko.observableArray()
            },
            save: save,
            tableDefinition: {
                headerTemplate: 'document-header-template',
                rowTemplate: 'document-row-template'
            },

            childtableDefinition: {
                headerTemplate: 'child-document-header-template',
                rowTemplate: 'child-document-row-template'
            },

            parseFileName: null,
            fileName: ko.observable(),
            inEditMode: ko.observable(false),

            showEportalDownload: ko.observable(null),
            showEportalDownloadLabel: ko.observable(
                ko.Localization('Naati.Resources.Document.resources.MyNAATIDownload')),

            showMergeDocument: ko.observable(null),
            showMergeDocumentLabel: ko.observable(
                ko.Localization('Naati.Resources.Document.resources.MergeDocument')),

            editShowWithPermission: ko.observable(''),
            deleteShowWithPermission: ko.observable(''),
            uploadShowWithPermission: ko.observable(''),
            eportalDownload: ko.observable(false),
            mergeDocument: ko.observable(false),

            isTestMaterialDocument: ko.observable(false),
            tableTitle: ko.observable(),

            types: ko.observableArray([]),
            relatedId: ko.observable(),
            documents: ko.observableArray([]),
            currentDocumentType: ko.observable(),
            enableDocumentType: ko.pureComputed(function () {
                return vm.types().length > 1 && vm.inEditMode() && !vm.currentDocument.StoredFileId();
            }),

            enableMergeDocument: ko.observable(false),
            enableEportalDownload: ko.observable(false)
        };

        vm.setMergeDocument = function (viewModel) {

            if (viewModel.mergeDocument() === true) {

                if (hasMergedDocument(viewModel.currentDocument.Id())) {

                    message.confirm({
                        title: ko.Localization('Naati.Resources.Shared.resources.Warning'),
                        content: ko.Localization('Naati.Resources.Document.resources.ReplaceMergeDocumentFlag')
                    }).then(function (result) {
                        if (result === 'yes') {

                            $.grep(vm.documents(), function (n) {
                                return n.MergeDocument === true;
                            });
                        }

                        if (result === 'no') {

                            $.grep(vm.documents(), function (n) {
                                return n.MergeDocument === false;
                            });
                            vm.mergeDocument(false);
                        }

                    });

                }
            }
        };

        function hasMergedDocument(currentDocumentId) {
            var hasMergedFlag = false;

            if (vm.documents().length > 0) {

                if (currentDocumentId === undefined) {
                    ko.utils.arrayFirst(vm.documents(), function (d) {
                        if (d.MergeDocument) {
                            hasMergedFlag = true;
                        }
                    });
                } else {
                    ko.utils.arrayFirst(vm.documents(), function (d) {
                        if (d.Id !== currentDocumentId && d.MergeDocument) {
                            hasMergedFlag = true;
                        }
                    });
                }

            }
            return hasMergedFlag;
        }

        var dirtyFlag = new ko.DirtyFlag([vm.documents], false);

        vm.typeOptions.subscribe(function () {
            var showOptions = vm.typeOptions().length > 1;
            if (vm.showEportalDownload() == null) {
                vm.showEportalDownload(showOptions);
            }
            if (vm.showMergeDocument() == null) {
                vm.showMergeDocument(showOptions);
            }
        });

        vm.currentDocumentType.subscribe(function () {
            clearTitle(vm.currentDocument.Title());
        }, null, 'beforeChange');

        vm.currentDocumentType.subscribe(checkMergeAndEPortalDownload);

        vm.isDirty = function () {
            return dirtyFlag().isDirty();
        };

        vm.hideForm = function () {
            return !(vm.allowCreate() || vm.currentDocument.StoredFileId()) || vm.typeOptions().length <= 0;
        };

        vm.title = function () {
            if (vm.currentDocument.StoredFileId()) {
                return ko.Localization('Naati.Resources.Shared.resources.Editing') + ': ' + vm.currentDocument.UnmodifiedTitle();
            }
            else {
                return ko.Localization('Naati.Resources.Shared.resources.Create');
            }
        };

        vm.currentDocument.File.subscribe(function (oldValue) {
            clearTitle(oldValue);
        }, null, 'beforeChange');

        vm.currentDocument.File.subscribe(function () {
            processFileName();
            checkMergeAndEPortalDownload();
        });

        vm.fileUpload.files.subscribe(function (newValue) {
            vm.replaceFile(vm.currentDocument.StoredFileId() && newValue && newValue.length);
            vm.inEditMode(newValue && newValue.length > 0);
            if (newValue && newValue.length > 0 && newValue[0].files && newValue[0].files.length > 0) {
                vm.currentDocument.File(newValue[0].files[0].name);
            }
        });


        vm.types.subscribe(setDocumentTypes);
        vm.documentTypes.subscribe(setDocumentTypes);

        vm.tableDefinition.dataTable = {
            source: vm.documents,
            columnDefs: [
                { targets: [-1, 5], orderable: false },
                { targets: 7, render: $.fn.dataTable.render.moment(moment.ISO_8601, CONST.settings.shortDateTimeDisplayFormat) }
            ]
        };

        vm.childtableDefinition.dataTable = {
            source: vm.childDocuments,
            columnDefs: [
                { targets: -1, orderable: false },
                { targets: 4, render: $.fn.dataTable.render.moment(moment.ISO_8601, CONST.settings.shortDateTimeDisplayFormat) }
            ]
        };

        vm.search = function () {
            vm.documents([]);

            clearEdit();

            vm.getDocumentsPromise().then(function (data) {
                ko.utils.arrayForEach(data, function (d) {
                    d.ExaminerMarksRemoved = d.ExaminerMarksRemoved || false;
                    d.AvailableForExaminersDisplayValue = d.EportalDownload ? 'Yes' : 'No';
                    d.MergeDocumentDisplayValue = d.MergeDocument ? 'Yes' : 'No';
                    if (d.SoftDeleteDate) {
                        d.SoftDeleteDateMessage = 'Deleted on ' + moment(d.SoftDeleteDate).format('DD/MM/YYYY');
                    }
                });

                if (vm.transformDocuments) {
                    data = vm.transformDocuments(data);
                }

                if (vm.populateDocuments) {
                    vm.populateDocuments(data);
                }
                else {
                    vm.documents(data);
                }


                dirtyFlag().reset();
            });
        };

        vm.edit = function (data) {
            var title = data.Title;
            var titleAndExtension = vm.titleAndExtension(data);

            var type = ko.utils.arrayFirst(vm.typeOptions(), function (t) {
                return t.name === data.DocumentType;
            });

            data.Type = type.id;

            vm.currentDocument.Id(data.WorkPracticeAttachmentId || data.TestAttendanceId || data.MaterialId || data.TestSittingDocumentId || data.TestSpecificationAttachmentId);
            vm.currentDocument.File(titleAndExtension);
            vm.currentDocument.StoredFileId(data.StoredFileId);
            vm.currentDocument.Title(title);
            vm.currentDocument.UnmodifiedTitle(title);

            vm.currentDocumentType(data.Type);
            vm.eportalDownload(data.EportalDownload);
            vm.mergeDocument(data.MergeDocument);

            vm.fileUpload.component.fileName(data.FileName);

            vm.inEditMode(true);

            vm.enableMergeDocument(checkWordDocument(data.FileName));
            vm.enableEportalDownload(true);
        };

        vm.download = function (document) {
            if (vm.downloadDocumentUrlFunc) {
                return vm.downloadDocumentUrlFunc(document);
            } else {

                return fileService.getFluid('downloadWithToken/' + document.StoredFileId);
            }
        };

        vm.remove = function (document) {
            message.confirm({ title: ko.Localization('Naati.Resources.Shared.resources.Warning'), content: ko.Localization('Naati.Resources.Shared.resources.ThisRecordWillBeDeleted') }).then(function (answer) {
                if (answer === 'yes') {
                    vm.removeDocumentPromise(document).then(function () {
                        toastr.success(ko.Localization('Naati.Resources.Shared.resources.DeletedSuccessfully'));
                        vm.search();
                        vm.cancel();
                    });
                }
            });
        };

        var validation = ko.validatedObservable(vm.currentDocument);


        vm.resetfilecompnent = function () {
            vm.fileUpload.files([]);
        };

        vm.cancel = function () {

            vm.replaceFile(false);
            vm.fileUpload.files([]);

            if (vm.fileUpload.component !== undefined) {
                vm.fileUpload.component.reset();
            }

            vm.inEditMode(false);
            vm.eportalDownload(false);
            vm.mergeDocument(false);
            vm.enableMergeDocument(false);
            vm.enableEportalDownload(false);

            ko.viewmodel.updateFromModel(vm.currentDocument, emptyModel, true).onComplete(function () {
                var types = vm.types();
                if (types && types.length === 1) {
                    vm.currentDocument.Type(types[0]);
                }
                validation.errors.showAllMessages(false);
            });
        };

        vm.titleAndExtension = function (file) {
            if (file.Title.match(new RegExp(file.FileType + "$"))) {
                return file.Title;
            }

            return file.Title + "." + file.FileType;
        };

        vm.fileId = function (document) {
            return document.StoredFileId;
        };

        vm.fileType = function (fileName) {
            return fileName.split('.').pop();
        };

        vm.canRemoveDocument = function (doc) {
            return vm.allowRemove() &&
                vm.documentTypes().find(function (d) {
                    return d.DisplayName === doc.DocumentType;
                });
        }

        vm.canEditDocument = function (doc) {
            return vm.allowEdit() &&
                vm.documentTypes().find(function (d) {
                    return d.DisplayName === doc.DocumentType;
                });
        }

        function checkMergeAndEPortalDownload() {
            vm.currentDocument.Type(vm.currentDocumentType());
            if (vm.documentTypes().length > 0) {
                var type = $.grep(vm.documentTypes(), function (n) {
                    return n.DocumentType === vm.currentDocumentType();
                });

                vm.eportalDownload(type.length ? type[0].EportalDownload : false);
                vm.mergeDocument(type.length ? type[0].MergeDocument : false);
            }

            processFileName();
        }

        function clearEdit() {
            ko.viewmodel.updateFromModel(serverModel, emptyModel);

            vm.inEditMode(false);
            vm.eportalDownload(false);
            vm.mergeDocument(false);
            vm.enableMergeDocument(false);
            vm.enableEportalDownload(false);

            try {
                vm.fileUpload.component.fileName(null);
            } catch (e) { }

            if (validation.errors) {
                validation.errors.showAllMessages(false);
            }
        }

        function setDocumentTypes() {

            if (!vm.documentTypes().length || !vm.types().length) {
                return;
            }

            var types = vm.types();
            var mapped = $.map(types, function (d) {

                var type = $.grep(vm.documentTypes(), function (n) {
                    return n.DocumentType === d;
                });

                if (!type.length) {
                    return null;
                }

                return { id: d, name: type[0].DisplayName };
            });

            vm.typeOptions(mapped);

            if (types && types.length === 1) {
                vm.currentDocumentType(types[0]);
            }
        }

        function clearTitle(oldValue) {
            if (!oldValue) return;

            if (vm.currentDocument.Title() !== oldValue) {
                vm.currentDocument.Title(null);
            }
        }

        function processFileName() {
            var fileName = vm.currentDocument.File();

            if (!fileName) return;

            if (!vm.currentDocumentType()) {
                vm.currentDocumentType(vm.typeOptions()[0].id);
            }

            if (!vm.currentDocument.Title()) {
                if (vm.parseFileName) {
                    var documentType = ko.utils.arrayFirst(vm.typeOptions(),
                        function (t) {
                            return t.id === vm.currentDocumentType();
                        });

                    var newFileName = vm.parseFileName({
                        FileName: fileName,
                        DocumentTypeName: documentType.name
                    });
                    vm.currentDocument.Title(newFileName);
                } else {
                    vm.currentDocument.Title(fileName);
                }
            } else {
                if (vm.parseFileName) {
                    var editFileName = vm.parseFileName({
                        FileName: fileName,
                        DocumentTypeName: vm.currentDocumentType()
                    });
                    vm.currentDocument.Title(editFileName);
                } else {
                    vm.currentDocument.Title(fileName);
                }
            }

            if (vm.documentTypes().length > 0) {
                var type = $.grep(vm.documentTypes(), function (n) {
                    return n.DocumentType === vm.currentDocumentType();
                });

                if (!vm.inEditMode()) {
                    vm.eportalDownload(type[0].EportalDownload);
                    vm.mergeDocument(type[0].MergeDocument);
                }
            }

            vm.currentDocument.Type(vm.currentDocumentType());

            vm.enableMergeDocument(checkWordDocument(fileName));
            vm.enableEportalDownload(true);
        }

        function checkWordDocument(uploadfilename) {

            var resField = uploadfilename;

            var extension = resField.substr(resField.lastIndexOf('.') + 1).toLowerCase();
            var allowedExtensions = ['doc', 'docx'];
            if (resField.length > 0) {
                if (allowedExtensions.indexOf(extension) === -1) {
                    return false;
                }
            }
            return true;
        }

        function save() {
            var isValid = validation.isValid();
            validation.errors.showAllMessages(!isValid);

            if (!isValid) return;

            var files = vm.fileUpload.files();
            if (files.length) {
                files[0].submit()
                    .error(function (jqXhr, textStatus, errorThrown) {
                        servercallbackprocessor.showError(jqXhr, textStatus, errorThrown);
                    });
            }
            else {
                saveRelated();
            }
        }

        function saveRelated() {
            vm.postDocumentPromise().then(function () {
                vm.search();
                vm.cancel();
            });
        }

        return vm;
    }
});
