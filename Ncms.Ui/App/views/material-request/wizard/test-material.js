define([
    'services/application-data-service',
    'modules/enums',
], function (applicationService, enums) {
    return {
        getInstance: getInstance
    };

    function getInstance(params) {
        var defaultParams = {
            materialRequest: null,
            outputMaterial: null,
            action: null,
            materialRound: null,
            stepId: null,
            wizardService: null,
        };

        $.extend(defaultParams, params);

        var vm = {
            outputMaterial: defaultParams.outputMaterial,
            skills: ko.observableArray(),
            domains: ko.observableArray(),
            credentialTypes: ko.observableArray(),
            testComponentTypes: ko.observableArray(),
            productSpecifications: ko.observableArray(),
            testMaterialTypes: ko.observableArray(),
            materialRequest: defaultParams.materialRequest,
            wizardService: defaultParams.wizardService,
            action: defaultParams.action,
            materialRequestReadOnly: ko.observable(),
            materialRequestCostReadOnly: ko.observable(),
            domainRequired: ko.observable()
        };


        vm.selectedType = ko.pureComputed({
            read: function () {
                return vm.outputMaterial.TestComponentTypeId();
            },
            write: function (value) {
                vm.outputMaterial.TestComponentTypeId(value);
                if (value) {
                    vm.outputMaterial.TestComponentTypeId(value);
                    if (!vm.testComponentTypes().length) {
                        return;
                    }
                    var selectedType = ko.utils.arrayFilter(vm.testComponentTypes(),
                        function (item) {
                            return item.Id === value;
                        })[0];

                    var selectedBaseType = selectedType.TestComponentBaseTypeId;
                    
                    var defaultBillableHours = selectedType.DefaultMaterialRequestHours;
                    vm.materialRequest.MaxBillableHours(defaultBillableHours);

                    if (selectedBaseType === enums.TestComponentBaseType.Skill) {
                        vm.outputMaterial.isSkillRequired(true);
                        vm.outputMaterial.isLanguageRequired(false);
                    } else {
                        vm.outputMaterial.isSkillRequired(false);
                        vm.outputMaterial.isLanguageRequired(true);
                    }
                }
                else {
                    vm.outputMaterial.isSkillRequired(true);
                    vm.outputMaterial.isLanguageRequired(false);
                }

                vm.outputMaterial.SkillId(null);
                vm.outputMaterial.LanguageId(null);
            }
        });

        vm.selectedDomain = ko.pureComputed({
            read: function () {
                return vm.outputMaterial.TestMaterialDomainId();
            },
            write: function (value) {
                if (!value && !vm.domainRequired()) {
                    vm.outputMaterial.TestMaterialDomainId(enums.TestMaterialDomain.Undefined);
                } else {
                    vm.outputMaterial.TestMaterialDomainId(value);
                }
            }
        });

        vm.selectedSkill = ko.pureComputed({
            read: function () {
                return vm.outputMaterial.LanguageId() || vm.outputMaterial.SkillId();
            },
            write: function (value) {
                if (vm.outputMaterial.isLanguageRequired()) {
                    vm.outputMaterial.LanguageId(value);
                    vm.outputMaterial.SkillId(null);
                } else {
                    vm.outputMaterial.SkillId(value);
                    vm.outputMaterial.LanguageId(null);
                }
            }
        });

        vm.selectedCredentialType = ko.pureComputed({
            read: function () {
                return vm.outputMaterial.CredentialTypeId();
            },
            write: function (value) {

                vm.outputMaterial.CredentialTypeId(value);
                vm.selectedType(null);
            }
        });

        var validation = ko.validatedObservable([vm.outputMaterial, vm.materialRequest]);
        vm.skillOptions = {
            value: vm.selectedSkill,
            multiple: false,
            options: vm.skills,
            optionsValue: 'Id',
            optionsText: 'DisplayName',
            disable: vm.materialRequestReadOnly
        };

        vm.domainOptions = {
            value: vm.selectedDomain,
            multiple: false,
            options: vm.domains,
            optionsValue: 'Id',
            optionsText: 'DisplayName',
        };

        vm.languageOptions = {
            value: vm.selectedSkill,
            multiple: false,
            options: vm.skills,
            optionsValue: 'Language1Id',
            optionsText: 'DisplayName',
            disable: vm.materialRequestReadOnly
        };

        vm.testMaterialTypeOptions = {
            value: vm.outputMaterial.TestMaterialTypeId,
            multiple: false,
            options: vm.testMaterialTypes,
            optionsValue: 'Id',
            optionsText: 'DisplayName',
            disable: vm.materialRequestReadOnly
        };

        vm.productSpecificationOptions = {
            value: vm.materialRequest.ProductSpecificationId,
            multiple: false,
            options: vm.productSpecifications,
            optionsValue: 'Id',
            optionsText: 'DisplayName',
            disable: vm.materialRequestCostReadOnly
        };

        vm.credentialTypeOptions = {
            value: vm.selectedCredentialType,
            multiple: false,
            options: vm.credentialTypes,
            optionsValue: 'Id',
            optionsText: 'DisplayName',
            disable: vm.materialRequestReadOnly
        };

        vm.testComponentTypeOptions = {
            value: vm.selectedType,
            multiple: false,
            options: vm.testComponentTypes,
            optionsValue: 'Id',
            optionsText: 'DisplayName',
            disable: vm.materialRequestReadOnly
        };

        vm.outputMaterial.CredentialTypeId.subscribe(loadTasksAndSkills);

        vm.load = function () {
            loadTasksAndSkills();
            clearValidation();
        };

        vm.postData = function () {
            var data = ko.toJS(vm.outputMaterial);
            data.TaskTypeId = data.TestComponentTypeId;
            data.PanelId = vm.materialRequest.PanelId();
            data.ProductSpecificationId = vm.materialRequest.ProductSpecificationId();
            data.MaxBillableHours = vm.materialRequest.MaxBillableHours();
            data.TestMaterialDomainId = vm.outputMaterial.TestMaterialDomainId();
            return data;
        };

        vm.isValid = function () {
            var isValid = validation.isValid();

            if (!isValid) {
                validation.errors.showAllMessages();
            }

            return isValid;
        };

        vm.activate = function () {
            vm.wizardService.getFluid('wizard/testMaterial/isTestMaterialReadOnly/' + vm.action().Id).then(vm.materialRequestReadOnly);
            vm.wizardService.getFluid('wizard/testMaterial/isRequestCostReadOnly/' + vm.action().Id + '/' + vm.materialRequest.MaterialRequestId()).then(vm.materialRequestCostReadOnly);
            vm.wizardService.getFluid('wizard/materialDomainRequired/' + (vm.materialRequest.MaterialRequestId() || 0)).then(vm.domainRequired);
            applicationService.getFluid('lookuptype/Languages').then(vm.languages);
            applicationService.getFluid('lookuptype/CredentialType').then(vm.credentialTypes);
            applicationService.getFluid('lookuptype/TestMaterialType').then(vm.testMaterialTypes);
            applicationService.getFluid('lookuptype/MaterialRequestSpecification').then(vm.productSpecifications);

            var languageId = vm.outputMaterial.LanguageId();
            if (languageId) {
                vm.outputMaterial.isLanguageRequired(true);
                vm.outputMaterial.isSkillRequired(false);

            } else {
                vm.outputMaterial.isLanguageRequired(false);
                vm.outputMaterial.isSkillRequired(true);
            }

        }

        function clearValidation() {
            if (validation.errors) {
                validation.errors.showAllMessages(false);
            }
        }

        function loadTasksAndSkills() {
            var credentialTypeId = vm.outputMaterial.CredentialTypeId();
            if (!credentialTypeId) {
                vm.testComponentTypes([]);
                return clearValidation();
            }

            var request = { CredentialTypeIds: [credentialTypeId] };
            applicationService.getFluid('testtask', request).then(function (data) {
                $.each(data, function (i, d) {
                    var availability = !d.Active ? '(Unavailable)' : '';
                    d.DisplayName = '{0} - {1}{2} {3}'.format(d.DisplayName, 'SPEC', d.TestSpecificationId, availability);
                });

                vm.testComponentTypes(data);
                clearValidation();
            });

            var request2 = { CredentialTypeId: credentialTypeId, PanelId: vm.materialRequest.PanelId() }
            vm.wizardService.getFluid('skill', request2).then(vm.skills);

            vm.wizardService.getFluid('wizard/domains/' + credentialTypeId).then(function (data) {
                vm.domains(data);

                   var items = ko.utils.arrayFilter(data,
                        function (domain) {
                            return domain.Id === vm.outputMaterial.TestMaterialDomainId();
                        });

                    if (!items.length) {
                        if (vm.domainRequired()) {
                            vm.outputMaterial.TestMaterialDomainId(null);
                        } else {
                            vm.outputMaterial.TestMaterialDomainId(enums.TestMaterialDomain.Undefined);
                        }

                    }
               
                clearValidation();
            });

            clearValidation();
        }

        return vm;
    }
});