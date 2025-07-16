define([
    'modules/custom-validator',
    'services/util',
    'services/application-data-service',
], function (customValidator, util, applicationService) {
    return {
        getInstance: getInstance
    };

    function getInstance(params) {
        var defaultParams = {
            application: null,
            action: null,
            credentialRequest: null
        };

        $.extend(defaultParams, params);

        var serverModel = {
            ApplicationId: defaultParams.application.ApplicationId,
            CategoryId: ko.observable(),
            CredentialTypeId: ko.observable(),
            SkillId: ko.observable(),
            CredentialTypes: ko.observable(),
            CredentialApplicationTypeIds: defaultParams.application.ApplicationTypeId
        };

        serverModel.CategoryId.subscribe(selectCategory);
        serverModel.CredentialTypes.subscribe(selectCredentialTypes);

        var validator = customValidator.create(serverModel);
        serverModel.CategoryId.subscribe(validator.reset);
        serverModel.CredentialTypes.subscribe(validator.reset);
        serverModel.SkillId.subscribe(selectSkill);

        serverModel.CredentialTypeIds = ko.pureComputed(function () {
            return (serverModel.CredentialTypes() || '').toString().split(',');
        });

        var vm = {
            request: serverModel,
            application: defaultParams.application,
            credentialRequest: defaultParams.credentialRequest,
            action: defaultParams.action,
            categories: ko.observableArray(),
            credentialTypes: ko.observableArray(),
            skills: ko.observableArray(),
        };

        vm.computedCredentialTypes = ko.pureComputed(function () {
            var credentialTypes = vm.credentialTypes();
            var distinct = [];

            for (var i = 0; i < credentialTypes.length; i++) {
                var credentialType = credentialTypes[i];
                if (typeof (distinct[credentialType.DisplayName]) === 'undefined') {
                    distinct[credentialType.DisplayName] = credentialType.Id;
                }
                else {
                    distinct[credentialType.DisplayName] += ',' + credentialType.Id;
                }
            }

            var result = [];
            for (var displayName in distinct) {
                result.push({ Id: distinct[displayName], DisplayName: displayName })
            }

            return result;
        });

        vm.categories.subscribe(function (options) { setIfSingle(options, serverModel.CategoryId); });
        vm.computedCredentialTypes.subscribe(function (options) { setIfSingle(options, serverModel.CredentialTypes); });
        vm.skills.subscribe(function (options) { setIfSingle(options, serverModel.SkillId); });

        $.extend(vm, {
            categoryOptions: {
                value: serverModel.CategoryId,
                multiple: false,
                disable: ko.computed(function () { return !vm.categories().length; }),
                options: vm.categories,
                optionsValue: 'Id',
                optionsText: 'DisplayName',
            },
            credentialTypeOptions: {
                value: serverModel.CredentialTypes,
                multiple: false,
                disable: ko.computed(function () { return !vm.credentialTypes().length; }),
                options: vm.computedCredentialTypes,
                optionsValue: 'Id',
                optionsText: 'DisplayName',
            },
            skillOptions: {
                value: serverModel.SkillId,
                multiple: false,
                multiselect: { enableFiltering: true },
                disable: ko.computed(function () { return !vm.skills().length; }),
                options: vm.skills,
                optionsValue: 'Id',
                optionsText: 'DisplayName',
            },
        });

        vm.load = function () {
            validator.reset();
            applicationService.getFluid('categories', vm.request).then(vm.categories);
        };

        vm.isValid = function () {
            var defer = Q.defer();
           
            applicationService.post(ko.viewmodel.toModel(serverModel), 'wizard/selectcredential').then(function (data) {
                validator.reset();

                ko.utils.arrayForEach(data.InvalidFields, function (i) {
                    validator.setValidation(i.FieldName, false, i.Message);
                });

                validator.isValid();

                defer.resolve(!data.InvalidFields.length);
            });

            return defer.promise;
        };

        vm.postData = function () {
            return ko.toJS(serverModel);
        };

        function selectCategory(category) {
            serverModel.CredentialTypes(null);

            if (!category) {
                return vm.credentialTypes([]);
            }

            applicationService.getFluid('credentialtype', vm.request).then(vm.credentialTypes);
        }

        function selectCredentialTypes(credentialTypes) {
            if (!credentialTypes) {
                return;
            }

            applicationService.getFluid('skill', $.param(ko.toJS(vm.request))).then(function (data) {
                sortSkillByAlphabetically(data);
                vm.skills(data);
            });
        }        

        function setIfSingle(options, observable) {
            if (options.length === 1) {
                observable(options[0].Id);
            }
        }

        function sortSkillByAlphabetically(data) {
            var objArray = data;
            objArray.sort(util.sortBy('DisplayName'));
        }

        function selectSkill() {
            validator.reset();

            var skillId = serverModel.SkillId();
            if (!skillId) {
                return serverModel.CredentialTypeId(skillId);
            }

            var skill = ko.utils.arrayFirst(vm.skills(), function (s) {
                return s.Id === skillId;
            });

            if (!skill) {
                return serverModel.CredentialTypeId(skill);
            }

            serverModel.CredentialTypeId(skill.CredentialTypeId);
        }


        return vm;
    }
});