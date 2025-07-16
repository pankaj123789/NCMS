define([
	'views/system/skill-details',
    'services/setup-data-service',
    'modules/enums'
], function (skillDetails, setupService, enums) {
	return {
		getInstance: getInstance
	};

	function getInstance() {
		var serverModel = {
			SkillId: ko.observable(),
			SkillTypeId: ko.observable().extend({ required: true }),
			Language1Id: ko.observable().extend({ required: true }),
			Language2Id: ko.observable(),
            DirectionTypeId: ko.observable().extend({ required: true }),
			CredentialApplicationTypeId: ko.observableArray()
		};

		var emptyModel = ko.toJS(serverModel);

		var defer = null;

		var vm = {
			skill: serverModel
		};

		var shown = false;
		var skillDetailsInstance = skillDetails.getInstance(serverModel);
		vm.skillDetailsOptions = {
			view: 'views/system/skill-details.html',
			model: skillDetailsInstance
		};

		var dirtyFlag = new ko.DirtyFlag([serverModel], false);

		vm.canSave = ko.pureComputed(function () {
            return dirtyFlag().isDirty() && currentUser.hasPermissionSync(enums.SecNoun.Skill, enums.SecVerb.Create);
		});

		var validation = ko.validatedObservable(serverModel);

		vm.show = function () {
			if (!shown) {
				skillDetailsInstance.load();
				shown = true;
			}

			defer = Q.defer();

			ko.viewmodel.updateFromModel(serverModel, emptyModel);

			resetValidation();
			dirtyFlag().reset();

			$('#createSkillModal').modal('show');

			return defer.promise;
		};

		vm.hide = function () {
			$('#createSkillModal').modal('hide');
		};

		vm.save = function () {
			if (!validation.isValid()) {
				return validation.errors.showAllMessages();
			}

			var request = ko.toJS(serverModel);

			setupService.post(request, 'skill')
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
