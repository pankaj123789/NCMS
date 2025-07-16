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
            FileName: ko.observable().extend({ required: true, maxLength: 100 }),
            File: ko.observable().extend({ required: true }),
            StoredFileId: ko.observable(),
            UnmodifiedTitle: ko.observable(),
            Type: ko.observable(),
            MergeDocument: ko.observable(),
            EportalDownload: ko.observable(),
            CurrentDocumentType: ko.observable(),
            EnableMergeDocument: ko.observable(),
            EnableEportalDownload: ko.observable()
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
            currentDocuments: ko.observableArray(),
            replaceFile: ko.observable(false),
            typeOptions: ko.observableArray(),
            documentTypes: documentTypes,
            childDocuments: ko.observableArray(),
            allowCreate: ko.observable(true),
            allowRemove: ko.observable(true),
            allowEdit: ko.observable(true),
            allowDownload: ko.observable(true),
            showLabels: ko.observable(false),
            hasMergedDocument: ko.observable(false),
            fileUpload: {
                files: ko.observableArray(),
                currentFile: ko.observable()
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
            inUploadMode: ko.observable(true),
            isMoreThanOneFile: ko.observable(false),

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
            isTestAsset: ko.observable(false),
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
            enableEportalDownload: ko.observable(false),
        };

        vm.setMergeDocument = function (currentDocument) {
            // the following checks are to stop the user getting prompted to replace the current merge document
            // if the document they are editing is the existing merged document

            // only check if the current value of merge document for the current document is true
            if (currentDocument.MergeDocument() === true) {
                // find the current merge document if it exists in documents
                var currentMergeDocument = vm.documents().find(function (item) {
                    return item.MergeDocument == true;
                });
                // if it does exist then check if the current merge document is the same as the current document
                // and if it is then do not do the validation check
                if (typeof currentMergeDocument != 'undefined') {
                    if (currentMergeDocument.StoredFileId == currentDocument.StoredFileId()) {
                        return;
                    }
                }
                // otherwise prompt the user as normal
                if (vm.hasMergedDocument()) {
                    message.confirm({
                        title: ko.Localization('Naati.Resources.Shared.resources.Warning'),
                        content: ko.Localization('Naati.Resources.Document.resources.ReplaceMergeDocumentFlag')
                    }).then(function (result) {
                        if (result === 'yes') {
                            $.grep(vm.documents(), function (item, index) {
                                if (item.MergeDocument) {
                                    item.MergeDocument = false;
                                }
                                vm.documents()[index] = item;
                            });
                            $.grep(vm.currentDocuments(), function (item, index) {
                                if (item == currentDocument) {
                                    return;
                                }
                                if (item.MergeDocument()) {
                                    item.MergeDocument(false);
                                }
                            });
                        }

                        if (result === 'no') {
                            currentDocument.MergeDocument(false);
                        }
                    });
                }
            }

            if (vm.currentDocuments().some(cd => cd.MergeDocument()) || vm.documents().some(d => d.MergeDocument)) {
                vm.hasMergedDocument(true);
            }
            else {
                vm.hasMergedDocument(false);
            }
        };

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

            if (vm.inEditMode()) {
                processFileName();
                return;
            }

            vm.inUploadMode(newValue && newValue.length > 0);
            vm.showLabels(true);
            if (newValue && newValue.length > 0 && newValue[0].files && newValue[0].files.length > 0) {
                vm.currentDocuments([]);
                ko.utils.arrayForEach(newValue, function (item) {
                    var newDocument = {
                        Id: ko.observable(),
                        Title: ko.observable().extend({ required: true, maxLength: 100 }),
                        FileName: ko.observable().extend({ required: true, maxLength: 100 }),
                        File: ko.observable().extend({ required: true }),
                        StoredFileId: ko.observable(),
                        UnmodifiedTitle: ko.observable(),
                        Type: ko.observable(),
                        MergeDocument: ko.observable(),
                        EportalDownload: ko.observable(),
                        CurrentDocumentType: ko.observable(),
                        EnableMergeDocument: ko.observable(),
                        EnableEportalDownload: ko.observable()
                    };
                    newDocument.File(item.files[0].name);
                    processFileNameMulti(newDocument);
                    vm.currentDocuments.push(newDocument);
                });

                if (vm.currentDocuments().some(cd => cd.EnableMergeDocument() === true) && !vm.isTestAsset()) {
                    vm.enableMergeDocument(true);
                }

                if (vm.currentDocuments().length > 1) {
                    vm.isMoreThanOneFile(true);
                }
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

                if (data.some(d => d.MergeDocument === true)) {
                    vm.hasMergedDocument(true);
                }
                else {
                    vm.hasMergedDocument(false);
                }

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
            vm.inUploadMode(false);

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
            vm.currentDocument.EportalDownload(data.EportalDownload);
            vm.currentDocument.MergeDocument(data.MergeDocument);

            vm.fileUpload.component.fileNames(data.FileName);

            vm.inEditMode(true);

            vm.enableMergeDocument(checkWordDocument(data.FileName) && !vm.isTestAsset());
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

        // var validation = ko.validatedObservable(vm.currentDocuments);


        vm.resetfilecompnent = function () {
            vm.fileUpload.files([]);
        };

        vm.cancel = function () {
            cleanAfterSaveOrCancel()
        };

        vm.titleAndExtension = function (file) {
            if (file.Title.match(new RegExp(file.FileType + "$"))) {
                return file.Title;
            }

            return file.Title + file.FileType;
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

        // used in html event to update eportaldownload switch based off document type
        vm.checkMergeAndEPortalDownloadMulti = function (currentDocument) {
            vm.currentDocument.Type(currentDocument.Type());
            if (vm.documentTypes().length > 0) {
                var type = $.grep(vm.documentTypes(), function (n) {
                    return n.DocumentType === currentDocument.Type();
                });

                currentDocument.EportalDownload(type.length ? type[0].EportalDownload : false);
                currentDocument.MergeDocument(type.length ? type[0].MergeDocument : false);
            }
        }

        function clearEdit() {
            ko.viewmodel.updateFromModel(serverModel, emptyModel);

            vm.inEditMode(false);
            vm.eportalDownload(false);
            vm.mergeDocument(false);
            vm.enableMergeDocument(false);
            vm.enableEportalDownload(false);

            try {
                vm.fileUpload.component.fileNames(null);
            } catch (e) { }

            //if (validation.errors) {
            //    validation.errors.showAllMessages(false);
            //}
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

            vm.currentDocument.FileName = fileName;

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

            vm.enableMergeDocument(checkWordDocument(fileName) && !vm.isTestAsset());
            vm.enableEportalDownload(true);
        }

        function processFileNameMulti(currentDocument) {
            var fileName = currentDocument.File();

            if (!fileName) return;

            if (!currentDocument.CurrentDocumentType()) {
                currentDocument.CurrentDocumentType(vm.typeOptions()[0].id);
            }

            if (!currentDocument.Title()) {
                if (vm.parseFileName) {
                    var documentType = ko.utils.arrayFirst(vm.typeOptions(),
                        function (t) {
                            return t.id === currentDocument.CurrentDocumentType();
                        });

                    var newFileName = vm.parseFileName({
                        FileName: fileName,
                        DocumentTypeName: documentType.name
                    });
                    if (fileNameAlreadyUsed(newFileName)) {
                        newFileName = makeNewFileNameUnique(newFileName);
                    }
                    currentDocument.Title(newFileName);
                    currentDocument.FileName(fileName);
                } else {
                    currentDocument.Title(fileName);
                }
            } else {
                if (vm.parseFileName) {
                    var editFileName = vm.parseFileName({
                        FileName: fileName,
                        DocumentTypeName: vm.currentDocumentType()
                    });
                    currentDocument.Title(editFileName);
                } else {
                    currentDocument.Title(fileName);
                }
            }

            if (vm.documentTypes().length > 0) {
                var type = $.grep(vm.documentTypes(), function (n) {
                    return n.DocumentType === currentDocument.CurrentDocumentType();
                });

                if (!vm.inEditMode()) {
                    currentDocument.EportalDownload(type[0].EportalDownload);
                    currentDocument.MergeDocument(type[0].MergeDocument);
                }
            }

            currentDocument.Type(currentDocument.CurrentDocumentType());

            vm.enableEportalDownload(true);

            currentDocument.EnableMergeDocument(checkWordDocument(fileName) && !vm.isTestAsset());
            currentDocument.EnableEportalDownload(true);
        }

        function makeNewFileNameUnique(newFileName) {
            var i = 1;
            var splitFileName = newFileName.split('.');
            var suffix = splitFileName[splitFileName.length - 1];
            var fileNamePart = newFileName.replace(/\.[^/.]+$/, "");
            newFileName = fileNamePart + i.toString() + "." + suffix;
            while (fileNameAlreadyUsed(newFileName))
            {
                newFileName = fileNamePart + (++i).toString() + "." + suffix;
            }
            return newFileName;
        }

        function fileNameAlreadyUsed(newFileName) {
            var foundDuplicate = false;
            ko.utils.arrayForEach(vm.currentDocuments(), function (currentDocument) {
                if (currentDocument.Title() == newFileName) {
                    foundDuplicate = true;
                }
            });
            return foundDuplicate;
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
            //var isValid = validation.isValid();
            //validation.errors.showAllMessages(!isValid);

            //if (!isValid) return;
            var files = vm.fileUpload.files();
            if (files.length) {
                var fileCount = files.length;
                var finishedCount = 0;
                $.each(files, function (index, file) {
                    vm.fileUpload.currentFile(file.files[0].name);
                    file.submit()
                        .done(function () {
                            finishedCount++;

                            if (finishedCount == fileCount) {
                                vm.search();
                                vm.cancel();
                            }
                        })
                        .error(function (jqXhr, textStatus, errorThrown) {
                            servercallbackprocessor.showError(jqXhr, textStatus, errorThrown);
                        });
                });
            }
            else {
                saveRelated();
            }
        }

        function cleanAfterSaveOrCancel() {
            vm.replaceFile(false);
            vm.fileUpload.files([]);
            vm.currentDocuments([]);

            if (vm.fileUpload.component !== undefined) {
                vm.fileUpload.component.reset();
            }

            vm.inEditMode(false);
            vm.inUploadMode(true);
            vm.eportalDownload(false);
            vm.mergeDocument(false);
            vm.enableMergeDocument(false);
            vm.enableEportalDownload(false);
            vm.showLabels(false);

            ko.viewmodel.updateFromModel(vm.currentDocument, emptyModel, true).onComplete(function () {
                var types = vm.types();
                if (types && types.length === 1) {
                    vm.currentDocument.Type(types[0]);
                }
            });
        }

        function saveRelated() {
            vm.postDocumentPromise().then(function () {
                vm.search();
                vm.cancel();
            });
        }

        function removeExtension(filename) {
            return filename.substring(0, filename.lastIndexOf('.')) || filename;
        }

        return vm;
    }
});
