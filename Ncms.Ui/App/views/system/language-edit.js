define([
	'services/application-data-service',
	'services/setup-data-service',
    'views/system/language-details',
    'modules/enums'
],
	function (applicationService, setupService, languageDetails, enums) {

		var serverModel = {
			LanguageId: ko.observable(),
			Name: ko.observable().extend({ required: true, maxLength: 50 }),
			Code: ko.observable().extend({ required: true, maxLength: 10 }),
			GroupLanguageId: ko.observable(),
			SkillsAttached: ko.observableArray(),
			Note: ko.observable().extend({ maxLength: 500 })
		};

		var emptyModel = ko.toJS(serverModel);

		var vm = {
			language: serverModel,
			title: ko.pureComputed(function () {
				return '{0} - #{1}'.format(ko.Localization('Naati.Resources.Language.resources.EditLanguage'), serverModel.LanguageId());
			}),
			groups: ko.observableArray(),
		};

		var languageDetailsInstance = languageDetails.getInstance(serverModel);

		vm.languageDetailsOptions = {
			view: 'views/system/language-details.html',
			model: languageDetailsInstance
		};

		var dirtyFlag = new ko.DirtyFlag([serverModel], false);

		vm.canSave = ko.pureComputed(function () {
            return dirtyFlag().isDirty() && currentUser.hasPermissionSync(enums.SecNoun.Language, enums.SecVerb.Update) ;
		});

		var validation = ko.validatedObservable(serverModel);

		vm.save = function () {
			var defer = Q.defer();

			if (!validation.isValid()) {
				validation.errors.showAllMessages();
				return defer.promise;
			}

			var request = ko.toJS(serverModel);

			setupService.post(request, 'language')
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

			ko.viewmodel.updateFromModel(serverModel, emptyModel);

			serverModel.LanguageId(id);

			return loadLanguage();
		};

		function resetValidation() {
			if (validation.errors) {
				return validation.errors.showAllMessages(false);
			}
		};

		function loadLanguage() {
			applicationService.getFluid('lookuptype/LanguageGroup').then(vm.groups);

			return setupService.getFluid('language/' + serverModel.LanguageId()).then(function (data) {
				ko.viewmodel.updateFromModel(serverModel, data);

				resetValidation();
				dirtyFlag().reset();

				return true;
			});
		}

		return vm;
	});

