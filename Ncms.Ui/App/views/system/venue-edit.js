define([
    'services/setup-data-service',
    'views/system/venue-details',
    'modules/enums'
],
    function (setupService, venueDetails, enums) {

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

        var vm = {
            venue: serverModel,
            title: ko.pureComputed(function () {
                return '{0} - {1}'.format(ko.Localization('Naati.Resources.Venue.resources.EditVenue'), serverModel.Name());
            }),
            locations: ko.observableArray()
        };

        vm.locationOptions = {
            value: serverModel.TestLocationId,
            multiple: false,
            options: vm.locations,
            optionsValue: 'Id',
            optionsText: 'DisplayName'
        };

        var venueDetailsInstance = venueDetails.getInstance(serverModel, false);
        venueDetailsInstance.load();

        vm.venueDetailsOptions = {
            view: 'views/system/venue-details.html',
            model: venueDetailsInstance
        };

        var dirtyFlag = new ko.DirtyFlag([serverModel], false);

        vm.canSave = ko.pureComputed(function () {
            return true;
        });

        var validation = ko.validatedObservable(serverModel);

        vm.save = function () {
            var defer = Q.defer();

            if (serverModel.Latitude() === "undefined" || serverModel.Latitude() === "")
                serverModel.Latitude(undefined);

            if (serverModel.Longitude() === "undefined" || serverModel.Longitude() === "")
                serverModel.Longitude(undefined);

            if (!validation.isValid()) {
                validation.errors.showAllMessages();
                return defer.promise;
            }

            serverModel.Coordinates(null);
            if (serverModel.Latitude() && serverModel.Latitude()) {
                serverModel.Coordinates(serverModel.Latitude() + ',' + serverModel.Longitude());
            }

            var request = ko.toJS(serverModel);

            setupService.post(request, 'venue')
                .then(function () {
                    dirtyFlag().reset();
                    defer.resolve('fulfilled');
                    toastr.success(ko.Localization('Naati.Resources.Venue.resources.SavedSuccessfully'));
                });

            return defer.promise;
        };

        vm.canActivate = function(id, query) {

            queryString = query || {};
            id = parseInt(id);

            ko.viewmodel.updateFromModel(serverModel, emptyModel);

            serverModel.VenueId(id);

            return currentUser.hasPermission(enums.SecNoun.Venue, enums.SecVerb.Update).then(
                function(canUpdate) {
                    if (!canUpdate) {
                        return false;
                    }

                    return loadVenue();
                });
        };

        function resetValidation() {
            if (validation.errors) {
                return validation.errors.showAllMessages(false);
            }
        };

        function setLatitudeLongitude() {
            if (serverModel.Coordinates()) {
                var splitCoordinates = serverModel.Coordinates().split(',');
                serverModel.Latitude(splitCoordinates[0]);
                serverModel.Longitude(splitCoordinates[1]);
            }
        };

        function loadVenue() {

            return setupService.getFluid('venue/' + serverModel.VenueId()).then(function (data) {

                ko.viewmodel.updateFromModel(serverModel, data);
                setLatitudeLongitude();

                venueDetailsInstance.compositionComplete();

                resetValidation();
                dirtyFlag().reset();

                return true;
            });
        }

        return vm;

    });

