define([
	'modules/custom-validator',
	'modules/enums',
	'services/util',
	'services/application-data-service',
], function (customValidator, enums, util, applicationService) {
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
			NaatiNumber: defaultParams.application.NaatiNumber,
			CategoryId: ko.observable(),
			CredentialTypeId: ko.observable(),
			SkillId: ko.observable(),
			CredentialTypes: ko.observable(),
		};

		var validator = customValidator.create(serverModel);

		var vm = {
			loading: ko.observable(true),
			request: serverModel,
			application: defaultParams.application,
			credentialRequest: defaultParams.credentialRequest,
			action: defaultParams.action,
			credentials: ko.observableArray(),
			credentialRequestId: ko.observable()
		};

		vm.credentialRequestId.subscribe(selectCredentialRequest);

		vm.load = function () {
			vm.loading(true);
			validator.reset();

			applicationService.getFluid('wizard/selectcredentialrecertification', vm.request).then(function (data) {
				vm.loading(false);
				vm.credentials(data);
			});
		};

		vm.isValid = function () {
			var defer = Q.defer();

			if (vm.loading()) {
				return false;
			}

			if (!vm.credentials().length) {
				return false;
			}

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

		vm.displayName = function (credential) {
			return '{0} {1} - {2} {3}'.format(
				credential.CredentialTypeInternalName,
				credential.SkillDisplayName,
				ko.Localization('Naati.Resources.Application.resources.ExpiringOn'),
				moment(credential.CertificationPeriod.EndDate).format('L'));
		};

		function selectCredentialRequest(credentialRequestId) {
			var credentialRequest = ko.utils.arrayFirst(vm.credentials(), function (c) {
				return c.Id == credentialRequestId;
			});

			credentialRequest = credentialRequest || {};

			serverModel.CategoryId(credentialRequest.CategoryId);
			serverModel.CredentialTypeId(credentialRequest.CredentialTypeId);
			serverModel.SkillId(credentialRequest.SkillId);
		}

		return vm;
	}
});