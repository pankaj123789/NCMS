define([
	'services/setup-data-service',
    'views/system/apiadmin-details',
    'modules/enums'
],
	function (setupService, apiadminDetails, enums) {
		return {
			getInstance: getInstance
		};

		function getInstance() {
			var serverModel = {
				ApiAccessId: ko.observable(),
				Name: ko.observable(),
				PublicKey: ko.observable().extend({ required: true, maxLength: 255 }),
				PrivateKey: ko.observable().extend({ required: true, maxLength: 255 }),
				Permissions: ko.observable(),
				Active: ko.observable(),
				InstitutionId: ko.observable().extend({ required: true }),
				PermissionIds: ko.observableArray([]).extend({ required: true }),
				MaskedPublicKey: ko.observable(),
				MaskedPrivateKey: ko.observable(),
			};

			serverModel.Active(true);

			var emptyModel = ko.toJS(serverModel);


			var defer = null;

			var vm = {
				apiadmin: serverModel
			};

			var shown = false;
			var apiAdminDetailsInstance = apiadminDetails.getInstance(serverModel, true);
			vm.apiAdminDetailsOptions = {
				view: 'views/system/apiadmin-details.html',
				model: apiAdminDetailsInstance
			};

			var dirtyFlag = new ko.DirtyFlag([serverModel], false);

			vm.canSave = ko.pureComputed(function () {
				return dirtyFlag().isDirty();
			});
			vm.generateApiKeys = function () {

				setupService.getFluid('NewGuid').then(function (data) {
					serverModel.PublicKey(data);
					serverModel.MaskedPublicKey(serverModel.PublicKey());
				});
				setupService.getFluid('NewGuid').then(function (data) {
					serverModel.PrivateKey(data);
					serverModel.MaskedPrivateKey(serverModel.PrivateKey());
				});
			};

			var validation = ko.validatedObservable(serverModel);

		    vm.show = function() {
		        defer = Q.defer();

		        currentUser.hasPermission(enums.SecNoun.System, enums.SecVerb.Manage).then(
		            function(canUpdate) {
		                if (!canUpdate) {
		                    return false;
		                }

		                if (!shown) {
		                    apiAdminDetailsInstance.load();
		                    shown = true;
		                }

		                ko.viewmodel.updateFromModel(serverModel, emptyModel);

		                resetValidation();
		                dirtyFlag().reset();

		                $('#createApiAdminModal').modal('show');
		            });

		        return defer.promise;
		    };

			vm.hide = function () {
				$('#createApiAdminModal').modal('hide');
			};

			vm.checkedChanged = function () {
				dirtyFlag().isDirty();
			}

			vm.save = function () {
				if (!validation.isValid()) {
					return validation.errors.showAllMessages();
				}

				var request = ko.toJS(serverModel);

				setupService.post(request, 'saveapiadmin')
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
