define([
	'services/application-data-service',
	'services/util',
	'modules/enums'
],
    function (applicationService, util, enums) {
		return {
			getInstance: getInstance
		};

		function getInstance(skill, isReadOnly) {
			isReadOnly = isReadOnly || ko.observable();

			var serverModel = $.extend({
				SkillId: ko.observable(),
				SkillTypeId: ko.observable(),
				Language1Id: ko.observable(),
				Language2Id: ko.observable(),
                DirectionTypeId: ko.observable(),
				CredentialApplicationTypeId: ko.observableArray(),
				Note: ko.observable().extend({ maxLength: 500 }),
            }, skill);

            serverModel.SkillTypeId.subscribe(skillTypeChange);

			var vm = {
				skill: serverModel,
				languages: ko.observableArray(),
                skillTypes: ko.observableArray(),
                directions: ko.observableArray(),
				credentialApplicationTypes: ko.observableArray(),
			};

			var disableLanguage2 = ko.observable();
			serverModel.DirectionTypeId.subscribe(function (value) {
				disableLanguage2(value == enums.DirectionTypes.Language1);
				if (disableLanguage2()) {
					serverModel.Language2Id(null);
				}
			});

			vm.skillTypeOptions = {
				value: serverModel.SkillTypeId,
				multiple: false,
				options: vm.skillTypes,
				optionsValue: 'Id',
				optionsText: 'DisplayName',
				disable: isReadOnly
			};

			vm.language1Options = {
				value: serverModel.Language1Id,
				multiple: false,
				options: vm.languages,
				optionsValue: 'Id',
				optionsText: 'DisplayName',
				disable: isReadOnly
			};

			vm.language2Options = {
				value: serverModel.Language2Id,
				multiple: false,
				options: vm.languages,
				optionsValue: 'Id',
				optionsText: 'DisplayName',
				disable: ko.computed(function () {
					return isReadOnly() || disableLanguage2();
				})
            };

            vm.directionOptions = {
                value: serverModel.DirectionTypeId,
                multiple: false,
                options: vm.directions,
                optionsValue: 'Id',
                optionsText: 'DisplayName',
                disable: isReadOnly
            };

            vm.credentialApplicationTypeOptions = {
                selectedOptions: serverModel.CredentialApplicationTypeId,
                multiple: true,
                options: vm.credentialApplicationTypes,
                optionsValue: 'Id',
                optionsText: 'DisplayName'
            };

			vm.name = ko.computed(function () {
				var direction = ko.utils.arrayFirst(vm.directions(), function (d) {
					return d.Id === serverModel.DirectionTypeId();
				});

				var language1 = ko.utils.arrayFirst(vm.languages(), function (d) {
					return d.Id === serverModel.Language1Id();
				});

				var language2 = ko.utils.arrayFirst(vm.languages(), function (d) {
					return d.Id === serverModel.Language2Id();
				});

				if (!direction || !language1) {
					return '';
				}

				return direction.DisplayName
					.replace('[Language 1]', language1.DisplayName || '[Language 1]')
					.replace('[Language 2]', (language2 || {}).DisplayName || '[Language 2]');
            });

            vm.activate = function () {
                if (serverModel.SkillTypeId()) {
                   applicationService.getFluid('credentialapplicationtype', { skillTypeIds: [serverModel.SkillTypeId()] }).then(vm.credentialApplicationTypes);
                }
            }

			vm.load = function () {
				applicationService.getFluid('lookuptype/SkillType').then(vm.skillTypes);
                applicationService.getFluid('lookuptype/Languages').then(function (data) {
                    vm.languages(data.sort(util.sortBy('DisplayName')));
                });
				applicationService.getFluid('lookuptype/DirectionType').then(vm.directions);
            }

            function skillTypeChange() {
                serverModel.CredentialApplicationTypeId([]);
                applicationService.getFluid('credentialapplicationtype', { skillTypeIds: [serverModel.SkillTypeId()] }).then(vm.credentialApplicationTypes);
            }

			return vm;
		}
	});

