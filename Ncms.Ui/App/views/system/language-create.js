define([
	'views/system/language-details',
    'services/setup-data-service',
    'modules/enums'
], function (languageDetails, setupService, enums) {
	return {
		getInstance: getInstance
	};

	function getInstance() {
		var serverModel = {
			LanguageId: ko.observable(),
			Name: ko.observable().extend({ required: true, maxLength: 50 }),
			Code: ko.observable().extend({ required: true, maxLength: 10 }),
			GroupLanguageId: ko.observable(),
			SkillsAttached: ko.observableArray()
		};

		var emptyModel = ko.toJS(serverModel);

		var defer = null;

		var vm = {
			language: serverModel
		};

		var languageDetailsInstance = languageDetails.getInstance(serverModel);
		vm.languageDetailsOptions = {
			view: 'views/system/language-details.html',
			model: languageDetailsInstance
		};

		var dirtyFlag = new ko.DirtyFlag([serverModel], false);

		vm.canSave = ko.pureComputed(function () {
            return dirtyFlag().isDirty() && currentUser.hasPermissionSync(enums.SecNoun.Language, enums.SecVerb.Create) ;
		});

		var validation = ko.validatedObservable(serverModel);

		vm.show = function () {
			defer = Q.defer();

			ko.viewmodel.updateFromModel(serverModel, emptyModel);

			resetValidation();
			dirtyFlag().reset();

			$('#createLanguageModal').modal('show');

			return defer.promise;
		};

		vm.hide = function () {
			$('#createLanguageModal').modal('hide');
		};

		vm.save = function () {
			if (!validation.isValid()) {
				return validation.errors.showAllMessages();
			}

			var request = ko.toJS(serverModel);

			setupService.post(request, 'language')
				.then(function (response) {
					dirtyFlag().reset();
					defer.resolve(response);
					toastr.success(ko.Localization('Naati.Resources.Shared.resources.SavedSuccessfully'));
				});
		};

		function resetValidation() {
			if (validation.errors) {
				return validation.errors.showAllMessages(false);
			}
		};

		return vm;
	}
});
