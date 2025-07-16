define([
	'modules/custom-validator',
	'services/screen/date-service',
], function (customValidator, dateService) {
	return {
		getInstance: getInstance
	};

	function getInstance(params) {
		var defaultParams = {
			application: null,
			action: null,
			credentialRequest: null,
			stepId: null,
			wizardService: null,
			summary: null
		};

		$.extend(defaultParams, params);

		var serverModel = {
			NotifyRolePlayersChecked: ko.observable(),
			NotifyRolePlayersMessage: ko.observable(),
			OnDisableRolePlayersMessage: ko.observable(),
			ReadOnly: ko.observable()
		};

		var vm = {
			option: serverModel,
			optionPromise: null
		};


		vm.load = function () {
		};

		vm.postData = function () {
			return ko.toJS(serverModel.NotifyRolePlayersChecked);
		};

		vm.isValid = function () {

			return true;
		};

		vm.activate = function () {
			var sessionDetails = ko.toJS(defaultParams.summary);

			sessionDetails.TestDate = sessionDetails.TestDate ? dateService.toPostDate(sessionDetails.TestDate) : null;
            defaultParams.wizardService.post(sessionDetails, 'getAllocateRolePlayersCheckOptionMessage').then(function (data) {
				ko.viewmodel.updateFromModel(serverModel, data);
			});

		}

		return vm;
	}
});