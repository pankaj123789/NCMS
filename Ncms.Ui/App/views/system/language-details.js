define([
	'services/application-data-service'
],
	function (applicationService) {
		return {
			getInstance: getInstance
		};

		function getInstance(language) {
			var serverModel = $.extend({
				LanguageId: ko.observable(),
				Name: ko.observable(),
				Code: ko.observable(),
				GroupLanguageId: ko.observable(),
				Note: ko.observable().extend({ maxLength: 500 })
			}, language);

			var vm = {
				language: serverModel,
				groups: ko.observableArray(),
			};

			vm.groupOptions = {
				value: serverModel.GroupLanguageId,
				multiple: false,
				options: vm.groups,
				optionsValue: 'Id',
				optionsText: 'DisplayName',
			};

			function init() {
				applicationService.getFluid('lookuptype/LanguageGroup').then(vm.groups);
			}

			init();

			return vm;
		}
	});

