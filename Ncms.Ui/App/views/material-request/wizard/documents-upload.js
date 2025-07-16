define([
    'services/file-data-service',
    'views/document/document-form',
    'services/util',
    'services/screen/date-service',
], function (fileService, documentForm, util, dateService) {
    return {
        getInstance: getInstance
    };

    function getInstance(params) {
        var defaultParams = {
            materialRequest: null,
            outputMaterial:null,
            action: null,
            materialRound: null,
            stepId: null,
            wizardService: null
        };

        $.extend(defaultParams, params);

        var vm = {
            materialRequest: defaultParams.materialRequest,
            materialRound: defaultParams.materialRound,
            wizardInstanceId: ko.observable('MaterialRequest' + util.guid()),
            action: defaultParams.action,
            outputMaterial: defaultParams.outputMaterial,
            documentInstance: null,
        };

        vm.generalDocumentsInstance = documentForm.getInstance();
        setupTestDocument(vm.generalDocumentsInstance);

        var validation = ko.validatedObservable(vm.materialRound);

        vm.postData = function () {
            return {
                WizardInstanceId: vm.wizardInstanceId(),
                Documents: vm.generalDocumentsInstance.documents
            };
        };

        vm.load = function() {
           
            defaultParams.wizardService.getFluid('wizard/showMergeOption/' +vm.action().Id +'/' + vm.outputMaterial.TestMaterialTypeId()).then(function(enableMerge) {
              
                defaultParams.wizardService.getFluid('wizard/rounddocumenttypes/' + vm.action().Id + '/' + vm.outputMaterial.TestMaterialTypeId()).then(function (data) {
                    var types = ko.utils.arrayMap(data, function (d) {
                        return d.Name;
                    });
                    vm.documentInstance.types(types);
                    vm.documentInstance.showMergeDocument(enableMerge);
                    vm.documentInstance.eportalDownload(true);
                    vm.generalDocumentsInstance.search();
                });
            });
        }

        vm.isValid = function () {
            var defer = Q.defer();
            var req = {
                MaterialRequest: vm.materialRequest,
                Documents: vm.generalDocumentsInstance.documents
            };
            defaultParams.wizardService.post(ko.toJS(req), 'wizard/documentsupload').then(function (data) {
                defer.resolve(data == null || data.Errors == null || data.Errors.length == 0);
            });
            return defer.promise;
        };

        function setupTestDocument(documentInstance) {
            vm.documentInstance = documentInstance;
            documentInstance.types([]);
            documentInstance.fileUpload.url = defaultParams.wizardService.url() + '/wizard/documentsupload/upload';
            documentInstance.fileUpload.formData = ko.computed(function () {
                return {
                    id: documentInstance.currentDocument.Id() || 0,
                    file: documentInstance.currentDocument.File(),
                    type: documentInstance.currentDocument.Type(),
                    wizardInstanceId: vm.wizardInstanceId(),
                    title: documentInstance.currentDocument.Title(),
                    tempFileId: documentInstance.currentDocument.StoredFileId() || 0,
                    eportalDownload: documentInstance.eportalDownload(),
                    mergeDocument: documentInstance.mergeDocument()
            }
            });

            $.extend(documentInstance, {
                getDocumentsPromise: function () {

                    return defaultParams.wizardService.getFluid('wizard/documentsupload/' + vm.wizardInstanceId());
                },
                postDocumentPromise: function () {
                    var data = ko.toJS(documentInstance.currentDocument);
                    data.MaterialRequestId = vm.materialRequest.MaterialRequestId();
                    return defaultParams.wizardService.post(data, 'document');
                },
                transformDocuments: function (data) {
                    return ko.utils.arrayMap(data, function (d) {
                        var tmp = d.FileName.split('.');
                        d.FileType = tmp[tmp.length - 1];
                        d.UploadedByName = d.UploadedByPersonName || d.UploadedByUserName;
                        d.DocumentType = d.DocumentTypeDisplayName;
                        d.StoredFileId = d.TempFileId;
                        d.Id = d.TempFileId;
                        d.EportalDownload = d.ExaminersAvailable;
                        d.AvailableForExaminersDisplayValue = d.ExaminersAvailable ? 'Yes' : 'No';
                        return d;
                    });
                },
                removeDocumentPromise: function (document) {

                    var request = {
                        WizardInstanceId: vm.wizardInstanceId(),
                        TempFileId: document.TempFileId
                    };
                    return defaultParams.wizardService.post(request, 'wizard/documentsupload/delete');
                },

                downloadDocumentUrlFunc: function (document) {

                    return defaultParams.wizardService.url() +
                        '/wizard/download/' +
                        document.StoredFileId +
                        '/' +
                        vm.wizardInstanceId();
                }
            });
        }

        return vm;
    }
});