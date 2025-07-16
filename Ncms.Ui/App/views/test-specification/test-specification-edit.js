define([
    'modules/enums',
    'views/document/document-form',
    'services/testspecification-data-service',
    'views/test-specification/test-specification-upload'
],
    function (enums, documentForm, testSpecificationService, testSpecificationUpload) {

        var serverModel = {
            Id: ko.observable(),
            CredentialTypeId: ko.observable(),
            CredentialType: ko.observable(),
            OverallPassMark: ko.observable(),
            Description: ko.observable().extend({ maxLength: 1000 }),
            Active: ko.observable(false),
            ResultAutoCalculation: ko.observable(true),
            IsRubric: ko.observable(false)            
        };

        var emptyModel = ko.toJS(serverModel);

        var vm = {
            enums: enums,
            testSpecification: serverModel,
            enableRubricCalculations: ko.computed({
                read: function() {
                    return serverModel.ResultAutoCalculation();
                },
                write: function(data) {
                    serverModel.ResultAutoCalculation(data);
                }
            }),
            title: ko.pureComputed(function() {
                return '{0} - #{1} - {2}'.format(
                    ko.Localization('Naati.Resources.TestSpecification.resources.EditTestSpecification'),
                    serverModel.Id(),
                    serverModel.CredentialType());
            }),
            components: ko.observableArray(),
            tableDefinition: {
                headerTemplate: 'specificationcomponennts-header-template',
                rowTemplate: 'specificationcomponennts-row-template'
            },
            testSpecificationUploadOptions: ko.observable(),
            canUpload: ko.observable(false),
            testSpecificationUploadInstance: ko.observable(),
        };

        vm.testSpecificationUploadInstance = testSpecificationUpload.getInstance(0);

        vm.tableDefinition.dataTable = {
            source: vm.components,
            order: [
                [0, "asc"]
            ]
        };

        vm.download = function () {
            return testSpecificationService.getFluid('testSpecifications', { TestSpecificationName: vm.testSpecification.Description() });
        }

        vm.upload = function () {
            vm.testSpecificationUploadInstance.show();
        }

        vm.testSpecification.Active.subscribe(function(newValue) {
            dirtyFlag(true);
        });

        vm.testSpecification.Id.subscribe(function (id) {
            vm.testSpecificationUploadInstance = testSpecificationUpload.getInstance(id);
            vm.testSpecificationUploadOptions({
                view: 'views/test-specification/test-specification-upload',
                model: vm.testSpecificationUploadInstance
            });
        });

        var dirtyFlag = new ko.DirtyFlag([serverModel], false);

        vm.canSave = ko.pureComputed(function () {
            return dirtyFlag().isDirty();
        });

        var validation = ko.validatedObservable(serverModel);
        configDocumentForm();

        vm.save = function () {
            var defer = Q.defer();

            if (!validation.isValid()) {
                validation.errors.showAllMessages();
                return defer.promise;
            }

            var request = ko.toJS(serverModel);

            testSpecificationService.post(request)
                .then(function () {
                    dirtyFlag().reset();
                    defer.resolve('fulfilled');
                });

            return defer.promise;
        };

        vm.canActivate = function (id, query) {
            queryString = query || {};
            id = parseInt(id);

            ko.viewmodel.updateFromModel(serverModel, emptyModel);

            serverModel.Id(id);

            return loadTestSpecification();
        };

        vm.testSpecificationUploadInstance.event.progress(function (progress) {
            if (progress.name == 'UploadCompletedSuccessfully') {
                Console.Log('Upload Completed Successfully: ' + progress.data);
                loadTestSpecification();
            }
        });

        function resetValidation() {
            if (validation.errors) {
                return validation.errors.showAllMessages(false);
            }
        };

        function loadTestSpecification() {
            vm.documentsInstance.relatedId(serverModel.Id());
            vm.documentsInstance.search();

            testSpecificationService.getFluid('documentTypes').then(function (data) {
                vm.documentsInstance.types(data);

                vm.documentsInstance.showEportalDownload(true);
                vm.documentsInstance.showEportalDownloadLabel(ko.Localization('Naati.Resources.TestMaterial.resources.AvailableForExaminers'));

                vm.documentsInstance.showMergeDocument(true);
                vm.documentsInstance.showMergeDocumentLabel(ko.Localization('Naati.Resources.TestSpecification.resources.MergeDocument'));
            });

            testSpecificationService.getFluid('testComponents/{0}'.format(serverModel.Id()), {
                supressResponseMessages: true
            }).then(vm.components);

            canUpload();

            return testSpecificationService.get({ id: serverModel.Id() }).then(function (data) {
                if (data.length > 0) {
                    ko.viewmodel.updateFromModel(serverModel, data[0]);
                }
                resetValidation();
                dirtyFlag().reset();

                return true;
            });
        }


        function canUpload() {
            testSpecificationService.getFluid('canUpload/{0}'.format(serverModel.Id()), {
                suppressResponseMessages: true
            }).then(vm.canUpload);       
        }

        function configDocumentForm() {
            vm.documentsInstance = documentForm.getInstance();

            vm.documentsInstance.parseFileName = function (fileName) {
                var tmp = fileName.FileName.split('.');
                return tmp.slice(0, tmp.length - 1).join('.');
            };

            vm.documentsInstance.showEportalDownload(true);
            vm.documentsInstance.showEportalDownloadLabel(ko.Localization('Naati.Resources.TestMaterial.resources.AvailableForExaminers'));

            vm.documentsInstance.fileUpload.url = testSpecificationService.url() + '/documents/upload';
            vm.documentsInstance.showMergeDocument(true);
            vm.documentsInstance.showMergeDocumentLabel(ko.Localization('Naati.Resources.TestSpecification.resources.MergeDocument'));

            vm.documentsInstance.fileUpload.formData = ko.computed(function () {
                return {
                    id: vm.documentsInstance.currentDocument.Id() || 0,
                    file: vm.documentsInstance.currentDocument.File(),
                    type: vm.documentsInstance.currentDocument.Type(),
                    testSpecificationId: vm.documentsInstance.relatedId(),
                    title: vm.documentsInstance.currentDocument.Title(),
                    storedFileId: vm.documentsInstance.currentDocument.StoredFileId() || 0,

                    availableForExaminers: vm.documentsInstance.eportalDownload(),
                    mergeDocument: vm.documentsInstance.mergeDocument()
                };
            });

            $.extend(vm.documentsInstance, {
                getDocumentsPromise: function () {
                    return testSpecificationService.getFluid('documents/' + vm.documentsInstance.relatedId(), {
                        supressResponseMessages: true
                    });
                },
                postDocumentPromise: function () {
                    var data = ko.toJS(vm.documentsInstance.currentDocument);
                    data.TestSpecificationId = vm.documentsInstance.relatedId();
                    data.EportalDownload = vm.documentsInstance.eportalDownload();
                    data.MergeDocument = vm.documentsInstance.mergeDocument();

                    return testSpecificationService.post(data, 'documents');
                },
                removeDocumentPromise: function (document) {
                    return testSpecificationService.remove('documents/' + document.StoredFileId);
                }
            });
        }

        return vm;

    });