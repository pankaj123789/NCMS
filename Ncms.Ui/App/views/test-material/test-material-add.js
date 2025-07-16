define([
    'modules/enums',
    'services/util',
    'services/test-material-data-service',
    'services/application-data-service',
    'services/material-request-data-service',
], function (enums, util, testMaterialService, applicationService, materialRequestService) {
    var vm = {
        getInstance: getInstance
    };

    return vm;

    function getInstance() {
        var defer = null;

        var serverModel = {
            Title: ko.observable().extend({ required: true, maxLength: 255 }),
            CredentialTypeId: ko.observable().extend({ required: true }),
            TestComponentTypeId: ko.observable().extend({ required: true }),
            TestMaterialDomainId: ko.observable().extend({ required: true }),
            LanguageId: ko.observable(),
            SkillId: ko.observable(),
            IsTestMaterialTypeSource: ko.observable(false),
            Notes: ko.observable().extend({ maxLength: 1000 })
        };

        var emptyModel = ko.toJS(serverModel);
        var validation = ko.validatedObservable(serverModel);

        var vm = {
            isSaveClicked: ko.observable(false),
            testMaterial: serverModel,
            modalId: util.guid(),
            languages: ko.observableArray(),
            credentialTypes: ko.observableArray(),
            skills: ko.observableArray(),
            testComponentTypes: ko.observableArray(),
            domains: ko.observableArray(),
            baseOnSkill: ko.observable(),
            baseOnLanguage: ko.observable(),
            disableDomainOptions: ko.observable()
    };


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
        };

        vm.credentialTypeOptions = {
            value: serverModel.CredentialTypeId,
            multiple: false,
            options: vm.credentialTypes,
            optionsValue: 'Id',
            optionsText: 'DisplayName',
        };

        vm.skillOptions = {
            value: serverModel.SkillId,
            multiple: false,
            options: vm.skills,
            optionsValue: 'Id',
            optionsText: 'DisplayName'
        };

        vm.testComponentTypeOptions = {
            value: serverModel.TestComponentTypeId,
            multiple: false,
            options: vm.testComponentTypes,
            optionsValue: 'Id',
            optionsText: 'DisplayName'
        };

        vm.testMaterialDomainOptions = {
            value: serverModel.TestMaterialDomainId,
            multiple: false,
            options: vm.domains,
            optionsValue: 'Id',
            optionsText: 'DisplayName',
            disable: vm.disableDomainOptions
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

                $.each(data, function (i, d) {
                    var availability = !d.Active ? '(Unavailable)' : '';
                    d.DisplayName = '{0} - {1}{2} {3}'.format(d.DisplayName, 'SPEC', d.TestSpecificationId, availability);
                });

                vm.testComponentTypes(data);
            });

            materialRequestService.getFluid('wizard/domains/' + credentialTypeId).then(function(data) {
                vm.domains(data);
                if (data.length === 1) {
                    vm.testMaterial.TestMaterialDomainId(data[0].Id);
                }
            });
            applicationService.getFluid('skill', request).then(vm.skills);
        });

        vm.activate = function () {
            currentUser.hasPermission(enums.SecNoun.TestMaterial, enums.SecVerb.Create).then(function (data) { vm.disableDomainOptions(!data)});
            applicationService.getFluid('lookuptype/Languages').then(vm.languages);
            applicationService.getFluid('lookuptype/CredentialType').then(vm.credentialTypes);
        };

        vm.canSave = ko.pureComputed(function () {
            return !vm.isSaveClicked();
        });

        vm.save = function () {
            if (!validation.isValid()) {
                validation.errors.showAllMessages();
                return;
            }
            vm.isSaveClicked(true);

            var req = ko.toJS(serverModel);
            testMaterialService.post(req, 'createOrUpdateTestMaterial').then(function (data) {
                req.Id = data.Id;
                defer.resolve(req);
                vm.close();
            });
        };

        vm.show = function () {
            defer = Q.defer();

            vm.isSaveClicked(false);

            ko.viewmodel.updateFromModel(serverModel, emptyModel);

            if (validation.errors) {
                validation.errors.showAllMessages(false);
            }

            $('#' + vm.modalId).modal('show');

            return defer.promise;
        };

        vm.close = function () {
            $('#' + vm.modalId).modal('hide');
        };

        return vm;
    }
});