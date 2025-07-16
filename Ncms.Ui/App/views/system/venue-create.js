define([
	'services/setup-data-service',
    'views/system/venue-details',
    'modules/enums'
],
	function (setupService, venueDetails, enums) {
		return {
			getInstance: getInstance
		};

		function getInstance() {
			var serverModel = {
				VenueId: ko.observable(),
				Name: ko.observable().extend({ required: true, maxLength: 255 }),
				Address: ko.observable().extend({ required: true, maxLength: 255 }),
				Latitude: ko.observable(),
				Longitude: ko.observable(),
				Coordinates: ko.observable(),
				Capacity: ko.observable().extend({ required: true, maxLength: 255, number: true }),
				TestLocationId: ko.observable().extend({ required: true }),
				PublicNotes: ko.observable().extend({ maxLength: 500 }),
				Active: ko.observable()
			};

			var emptyModel = ko.toJS(serverModel);

			var defer = null;

			var vm = {
				venue: serverModel
			};

			var shown = false;
			var venueDetailsInstance = venueDetails.getInstance(serverModel, true);
			vm.venueDetailsOptions = {
				view: 'views/system/venue-details.html',
				model: venueDetailsInstance
			};

			var dirtyFlag = new ko.DirtyFlag([serverModel], false);

			vm.canSave = ko.pureComputed(function () {
				return dirtyFlag().isDirty();
			});

			var validation = ko.validatedObservable(serverModel);

		    vm.show = function() {
		        defer = Q.defer();

		        currentUser.hasPermission(enums.SecNoun.Venue, enums.SecVerb.Update).then(
		            function(canUpdate) {
		                if (!canUpdate) {
		                    return false;
		                }

		                if (!shown) {
		                    venueDetailsInstance.load();
		                    shown = true;
		                }

		                ko.viewmodel.updateFromModel(serverModel, emptyModel);

		                resetValidation();
		                dirtyFlag().reset();

		                $('#createVenueModal').modal('show');
		            });

		        return defer.promise;
		    };

			vm.hide = function () {
				$('#createVenueModal').modal('hide');
			};

			vm.save = function () {
				if (!validation.isValid()) {
					return validation.errors.showAllMessages();
				}

				var request = ko.toJS(serverModel);

				setupService.post(request, 'venue')
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
