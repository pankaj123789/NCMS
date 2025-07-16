define([
    'modules/enums',
    'views/document/document-form-multi',
    'services/test-material-data-service',
    'services/application-data-service',
    'services/material-request-data-service',
    'views/test-material/test-material-relations',
    'views/test-material/source-test-material-relations'
],
    function (enums, documentForm, testMaterialService, applicationService, materialRequestService, relationsForm, sourceRelationsForm) {

        var serverModel = {
            Id: ko.observable(),
            Title: ko.observable().extend({ required: true, maxLength: 255 }),
            CredentialTypeId: ko.observable().extend({ required: true }),
            TestComponentTypeId: ko.observable().extend({ required: true }),
            TestMaterialDomainId: ko.observable().extend({ required: true }),
            LanguageId: ko.observable(),
            SkillId: ko.observable(),
            Notes: ko.observable().extend({ maxLength: 1000 }),
            Available: ko.observable(),
            IsTestMaterialTypeSource: ko.observable(),
            SourceTestMaterialId: ko.observable(),
            SourceTestMaterialTitle: ko.observable()
        };

        var emptyModel = ko.toJS(serverModel);

        var vm = {
            isWriter: ko.observable(false),
            isDownloader: ko.observable(false),
            testMaterial: serverModel,
            title: ko.pureComputed(function () {
                return '{0} - #{1}'.format(ko.Localization('Naati.Resources.TestMaterial.resources.EditTestMaterial'), serverModel.Id());
            }),
            languages: ko.observableArray(),
            credentialTypes: ko.observableArray(),
            skills: ko.observableArray(),
            testComponentTypes: ko.observableArray(),
            domains: ko.observableArray(),
            baseOnSkill: ko.observable(),
            baseOnLanguage: ko.observable(),
            candidateBriefRequired: ko.observable(),
            documentTypes: ko.observableArray()
        };

        vm.disable = ko.computed(function () {
            return !vm.isWriter();
        });

        ko.computed(function () {
            var selectedTestComponentType = ko.utils.arrayFirst(vm.testComponentTypes(), function (t) {
                return t.Id === serverModel.TestComponentTypeId();
            }) || {};

            vm.baseOnSkill(selectedTestComponentType.TestComponentBaseTypeId === enums.TestComponentBaseType.Skill);
            vm.baseOnLanguage(selectedTestComponentType.TestComponentBaseTypeId === enums.TestComponentBaseType.Language);

            if (vm.baseOnLanguage()) {
                serverModel.SkillId(null);
            }

            if (vm.baseOnSkill()) {
                serverModel.LanguageId(null);
            }
        });

        serverModel.SkillId.extend({
            required: {
                onlyIf: vm.baseOnSkill
            }
        });

        serverModel.LanguageId.extend({
            required: {
                onlyIf: vm.baseOnLanguage
            }
        });

        vm.languageOptions = {
            value: serverModel.LanguageId,
            multiple: false,
            options: vm.languages,
            optionsValue: 'Id',
            optionsText: 'DisplayName',
            disable: vm.disable
        };

        vm.credentialTypeOptions = {
            value: serverModel.CredentialTypeId,
            multiple: false,
            options: vm.credentialTypes,
            optionsValue: 'Id',
            optionsText: 'DisplayName',
            disable: vm.disable
        };

        vm.skillOptions = {
            value: serverModel.SkillId,
            multiple: false,
            options: vm.skills,
            optionsValue: 'Id',
            optionsText: 'DisplayName',
            disable: vm.disable
        };

        configDocumentForm();
        vm.relationsInstance = relationsForm.getInstance();
        vm.sourceRelationsInstance = sourceRelationsForm.getInstance();

        ko.computed(checkDocumentAccess);

        vm.testMaterial.TestComponentTypeId.subscribe(updateDocumentTypes);

        function updateDocumentTypes(newValue) {
            if (newValue === '') {
                return;
            }

            var testComponentType = $.grep(vm.testComponentTypes(), function (e) {
                return e.Id === newValue;
            })[0];

            if (testComponentType !== undefined) {

                vm.candidateBriefRequired(testComponentType !== null ? testComponentType.CandidateBriefRequired : false);

                if (!vm.candidateBriefRequired()) {
                    var array = vm.documentTypes();

                    for (var i = 0; i < array.length; i++) {

                        if (array[i] === 'CandidateBrief') {
                            array.splice(i, 1);
                        }
                    }
                    vm.documentsInstance.types(array);
                }
                else {
                    testMaterialService.getFluid('documentTypes').then(function (data) {
                        vm.documentsInstance.types(data);
                    });
                }

            }

        }

        vm.testComponentTypeOptions = {
            value: serverModel.TestComponentTypeId,
            multiple: false,
            options: vm.testComponentTypes,
            optionsValue: 'Id',
            optionsText: 'DisplayName',
            disable: vm.disable
        };


        vm.testMaterialDomainOptions = {
            value: serverModel.TestMaterialDomainId,
            multiple: false,
            options: vm.domains,
            optionsValue: 'Id',
            optionsText: 'DisplayName',
            disable: vm.disable
        };

        ko.computed(function () {
            var credentialTypeId = serverModel.CredentialTypeId();
            if (!credentialTypeId) {
                vm.testComponentTypes([]);
                vm.domains([]);
                return vm.skills([]);
            }

            var request = { CredentialTypeIds: [credentialTypeId] };
            applicationService.getFluid('testtask', request).then(function (data) {

                $.each(data,
                    function (i, d) {
                        var availability = !d.Active ? '(Unavailable)' : '';
                        d.DisplayName =
                            '{0} - {1}{2} {3}'.format(d.DisplayName, 'SPEC', d.TestSpecificationId, availability);
                    });

                vm.testComponentTypes(data);
                vm.setDocumentTypes();
            });
            materialRequestService.getFluid('wizard/domains/' + credentialTypeId).then(function (data) {
                vm.domains(data);
                if (data.length === 1) {
                    vm.testMaterial.TestMaterialDomainId(data[0].Id);
                } else if (vm.testMaterial.TestMaterialDomainId()) {

                    var foundDomain = ko.utils.arrayFilter(data,
                        function(d) {
                            return d.Id === vm.testMaterial.TestMaterialDomainId();
                        });

                    if (!foundDomain.length) {

                        vm.testMaterial.TestMaterialDomainId(undefined);
                        resetValidation();
                    }
                }
            });
            applicationService.getFluid('skill', request).then(vm.skills);
        }).extend({ deferred: true });;

        vm.setDocumentTypes = function () {

            if (vm.testComponentTypes().length > 0) {
                var selectedTestComponentType = $.grep(vm.testComponentTypes(), function (e) {
                    return e.Id === vm.testMaterial.TestComponentTypeId();
                })[0];

                vm.candidateBriefRequired(selectedTestComponentType.CandidateBriefRequired);
                if (!vm.candidateBriefRequired()) {

                    var array = vm.documentTypes();

                    for (var i = 0; i < array.length; i++) {

                        if (array[i] === 'CandidateBrief') {
                            array.splice(i, 1);
                        }
                    }
                    vm.documentsInstance.types(array);
                } else {
                    vm.documentsInstance.types(vm.documentTypes());
                }
            }

        };

        vm.documentType = function (type) {
            vm.documentTypes().remove(type);
        }

        var dirtyFlag = new ko.DirtyFlag([serverModel], false);

        vm.canSave = ko.pureComputed(function () {
            return dirtyFlag().isDirty();
        });

        var validation = ko.validatedObservable(serverModel);

        vm.save = function () {
            var defer = Q.defer();

            if (!validation.isValid()) {
                validation.errors.showAllMessages();
                return defer.promise;
            }

            var request = ko.toJS(serverModel);

            testMaterialService.post(request, 'createOrUpdateTestMaterial')
                .then(function () {
                    dirtyFlag().reset();
                    defer.resolve('fulfilled');
                    toastr.success(ko.Localization('Naati.Resources.Shared.resources.SavedSuccessfully'));
                });

            return defer.promise;
        };

        vm.canActivate = function (id, query) {
            queryString = query || {};
            id = parseInt(id);

            materialRequestService.getFluid('testmaterial/allowEdit/' + id).then(function(data) {
                currentUser.hasPermission(enums.SecNoun.TestMaterial, enums.SecVerb.Update).then(function(permission) {
                    vm.isWriter(data && permission);
                });
            });
            ko.viewmodel.updateFromModel(serverModel, emptyModel);

            serverModel.Id(id);

            currentUser.hasPermission(enums.SecNoun.TestMaterial, enums.SecVerb.Download).then(vm.isDownloader);

            return true;
        };

        vm.activate = function() {
            return loadTestMaterial();
        };

        function resetValidation() {
            if (validation.errors) {
                return validation.errors.showAllMessages(false);
            }
        };

        function loadTestMaterial() {
            applicationService.getFluid('lookuptype/Languages').then(vm.languages);
            applicationService.getFluid('lookuptype/CredentialType').then(vm.credentialTypes);

            vm.documentsInstance.relatedId(serverModel.Id());
            vm.documentsInstance.isTestMaterialDocument(true);
            vm.documentsInstance.search();
            vm.documentsInstance.currentDocumentType('');

            testMaterialService.getFluid('documentTypes').then(function (data) {
                vm.documentTypes(data);
                vm.documentsInstance.showEportalDownload(true);
                vm.documentsInstance.showEportalDownloadLabel(ko.Localization('Naati.Resources.TestMaterial.resources.AvailableForExaminers'));
                vm.documentsInstance.showMergeDocument(true);
                vm.documentsInstance.showMergeDocumentLabel(ko.Localization('Naati.Resources.TestSpecification.resources.MergeDocument'));
            });

            return testMaterialService.getFluid('getTestMaterials/' + serverModel.Id()).then(function (data) {

                ko.viewmodel.updateFromModel(serverModel, data);
                resetValidation();
                dirtyFlag().reset();

                loadRelationsForm();
                loadSourceRelationsForm();

                return true;
            });
        }

        function checkDocumentAccess() {
            vm.documentsInstance.allowCreate(vm.isWriter());
            vm.documentsInstance.allowEdit(vm.isWriter());
            vm.documentsInstance.allowRemove(vm.isWriter());
            vm.documentsInstance.allowDownload(vm.isDownloader() || vm.isWriter());
        }



        function configDocumentForm() {
            vm.documentsInstance = documentForm.getInstance();
            vm.documentsInstance.populateDocuments = populateDocuments;
            vm.documentsInstance.showEportalDownload(true);
            vm.documentsInstance.showEportalDownloadLabel(ko.Localization('Naati.Resources.TestMaterial.resources.AvailableForExaminers'));
            vm.documentsInstance.showMergeDocument(true);
            vm.documentsInstance.showMergeDocumentLabel(ko.Localization('Naati.Resources.TestSpecification.resources.MergeDocument'));
            checkDocumentAccess();


            vm.documentsInstance.parseFileName = function (fileName) {
                var tmp = fileName.FileName.split('.');
                return tmp.slice(0, tmp.length - 1).join('.');
            };

            vm.documentsInstance.fileUpload.url = testMaterialService.url() + '/documents/upload';
            vm.documentsInstance.fileUpload.formData = ko.computed(function () {
                if (vm.documentsInstance.inUploadMode()) {
                    if (!vm.documentsInstance.fileUpload.currentFile()) {
                        return null;
                    }
                    var foundDocument = ko.utils.arrayFirst(vm.documentsInstance.currentDocuments(), function (item) {
                        return item.File() == vm.documentsInstance.fileUpload.currentFile();
                    });
                    if (!foundDocument) {
                        return null;
                    }
                    return {
                        id: foundDocument.Id() || 0,
                        file: foundDocument.File(),
                        type: foundDocument.Type(),
                        testMaterialId: vm.documentsInstance.relatedId(),
                        title: foundDocument.Title(),
                        storedFileId: foundDocument.StoredFileId() || 0,

                        availableForExaminers: foundDocument.EportalDownload(),
                        mergeDocument: foundDocument.MergeDocument()
                    };
                }
                if (vm.documentsInstance.inEditMode()) {
                    return {
                        id: vm.documentsInstance.currentDocument.Id() || 0,
                        file: vm.documentsInstance.currentDocument.File(),
                        type: vm.documentsInstance.currentDocument.Type(),
                        testMaterialId: vm.documentsInstance.relatedId(),
                        title: vm.documentsInstance.currentDocument.Title(),
                        storedFileId: vm.documentsInstance.currentDocument.StoredFileId() || 0,

                        availableForExaminers: vm.documentsInstance.currentDocument.EportalDownload(),
                        mergeDocument: vm.documentsInstance.currentDocument.MergeDocument()
                    };
                }
                return null;
            });

            function populateDocuments(data) {
                vm.documentsInstance.documents(ko.utils.arrayFilter(data, function (item) { return item.DocumentTypeId !== enums.DocumentType.CandidateBrief; }));
                vm.documentsInstance.childDocuments(ko.utils.arrayFilter(data, function (item) { return item.DocumentTypeId === enums.DocumentType.CandidateBrief; }));
            }


            $.extend(vm.documentsInstance, {
                getDocumentsPromise: function () {
                    return testMaterialService.getFluid('documents/' + vm.documentsInstance.relatedId(), {
                        supressResponseMessages: true
                    });
                },
                postDocumentPromise: function () {
                    var data = ko.toJS(vm.documentsInstance.currentDocument);
                    data.TestMaterialId = vm.documentsInstance.relatedId();
                    data.AvailableForExaminers = data.EportalDownload;

                    return testMaterialService.post(data, 'documents');
                },
                removeDocumentPromise: function (document) {
                    return testMaterialService.remove('documents/' + document.StoredFileId);
                }
            });
        }

        function loadRelationsForm() {
            vm.relationsInstance.load(vm.testMaterial.Id());
        }

        function loadSourceRelationsForm() {
            vm.sourceRelationsInstance.load(vm.testMaterial.SourceTestMaterialId(), vm.testMaterial.Id());
        }

        return vm;

    });