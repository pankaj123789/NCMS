define([
    'modules/common',
    'modules/custom-validator',
    'services/credentialrequest-data-service',
    'services/testsession-data-service',
    'services/screen/date-service',
    'modules/enums'
], function (common, customValidator, credentialrequestService, testSessionService, dateService, enums) {
    var functions = common.functions();

    return {
        getInstance: getInstance
    };

    function getInstance(params) {
        var defaultParams = {
            session: null
        };

        $.extend(defaultParams, params);

        var vm = {
            session: defaultParams.session,
            testLocations: ko.observableArray(),
            venues: ko.observableArray(),
            credentialTypes: ko.observableArray(),
            testSpecifications: ko.observableArray(),
            applicants: ko.observableArray([]),
            hasRolePlayers: ko.observable(),
            canManageTestSession: ko.observable(false)
        };

        currentUser.hasPermission(enums.SecNoun.TestSession, enums.SecVerb.Manage).then(vm.canManageTestSession);

        var validator = customValidator.create(vm.session);

        vm.session.TestLocationId.subscribe(loadVenues);
        vm.session.CredentialTypeId.subscribe(loadTestSpecifications);

        vm.zCapacity = ko.computed({
            read: function() {
                if (vm.session.OverrideVenueCapacity() || vm.session.OverrideVenueCapacity() === null) {
                    return vm.session.Capacity();
                }

                var venueId = vm.session.VenueId();

                if (!venueId) {
                    return 0;
                }

                var venue = ko.utils.arrayFirst(vm.venues(),
                    function(v) {
                        return v.VenueId === venueId;
                    });

                if (!venue) {
                    return 0;
                }

                return (venue.Capacity);
            },
            write: function(value) {
                vm.session.Capacity(value);
            },
            owner: this
        });

        vm.session.VenueId.subscribe(function(value) {
            if (vm.session.OverrideVenueCapacity()) {
                return;
            }
            var venue = ko.utils.arrayFirst(vm.venues(),
                function (v) {
                    vm.session.PublicNote(v.PublicNotes);
                    return v.VenueId === value;
                });

            if (!venue) {
                return 0;
            }

            vm.session.Capacity(venue.Capacity);
        });

    vm.testDateOptions = {
            value: vm.session.TestDate,
            css: 'form-control'
        };

        vm.testLocationOptions = {
            value: vm.session.TestLocationId,
            multiple: false,
            options: vm.testLocations,
            optionsValue: 'Id',
            optionsText: 'DisplayName'
        };

        vm.venueOptions = {
            value: vm.session.VenueId,
            multiple: false,
            options: vm.venues,
            optionsValue: 'VenueId',
            optionsText: 'NamePlusCapacity',
            disable: ko.pureComputed(function () {
                return !vm.session.TestLocationId();
            })
        };
        vm.testSpecificationOptions = {
            value: vm.session.DefaultTestSpecificationId,
            multiple: false,
            options: vm.testSpecifications,
            optionsValue: 'Id',
            optionsText: 'DisplayName',
            disable: ko.pureComputed(function () {
                return !vm.session.CredentialTypeId() || vm.hasRolePlayers() || vm.hasTestMaterials() || !vm.canManageTestSession();
            })
        };
        vm.credentialTypeOptions = {
            value: vm.session.CredentialTypeId,
            multiple: false,
            options: vm.credentialTypes,
            disable: ko.computed(function () {
                return vm.applicants().length !== 0 || (vm.session.Id() && vm.session.AllowSelfAssign());
            }),
            optionsValue: 'Id',
            optionsText: 'DisplayName'
        };

        vm.isValid = function () {
            var defer = Q.defer();
            var request = ko.toJS(vm.session);
            request.TestDate = request.TestDate ? dateService.toPostDate(request.TestDate) : null;
            testSessionService.post(request, 'wizard/details').then(function (data) {
                validator.reset();

                ko.utils.arrayForEach(data.InvalidFields,
                    function (i) {
                        validator.setValidation(i.FieldName, false, i.Message);
                    });

                validator.isValid();

                var isValid = !data.InvalidFields.length;
                defer.resolve(isValid);
            });

            return defer.promise;
        };

        vm.hasTestMaterials = ko.computed(function () {
            return ko.utils.arrayFirst(vm.applicants(), function (d) {
                return d.HasTestMaterials;
            });
        });

        ko.computed(function () {
            if (!vm.session.Id()) {
                return;
            }

            testSessionService.getFluid(vm.session.Id()).then(function (data) {
                vm.hasRolePlayers(data.HasRolePlayers);
            });
        });

        vm.load = function () {
            validator.reset();
            vm.applicants([]);
            if (vm.session.Id()) {
                testSessionService.getFluid(vm.session.Id() + '/applicants').then(vm.applicants);
            }
            functions.getLookup('CredentialType').then(vm.credentialTypes);
            functions.getLookup('TestLocation').then(vm.testLocations);

            vm.session.TestLocationId.valueHasMutated();
            vm.session.CredentialTypeId.valueHasMutated();
        };

        function loadVenues() {
            if (!vm.session.Id()) {
                credentialrequestService.getFluid('activeVenues', { TestLocationId: vm.session.TestLocationId() })
                    .then(function (data) {
                        vm.venues(data);
                        var venue = ko.utils.arrayFilter(vm.venues(),
                            function (item) {
                                return item.VenueId === vm.session.VenueId();
                            });

                        if (!venue.length) {
                            vm.session.VenueId(null);
                        }

                    });
            }
            else {
                credentialrequestService.getFluid('venuesShowingInactive', { TestLocationId: vm.session.TestLocationId() })
                    .then(function (data) {
                        vm.venues(data);
                        var venue = ko.utils.arrayFilter(vm.venues(),
                            function (item) {
                                return item.VenueId === vm.session.VenueId();
                            });

                        if (!venue.length) {
                            vm.session.VenueId(null);
                        }

                    });
            }

        }
        function loadTestSpecifications() {

            testSessionService.getFluid('testSpecificationByCredentialType/' + (vm.session.CredentialTypeId() || 0))
                .then(function (data) {
                    vm.testSpecifications(data);

                    var testSpecification = ko.utils.arrayFilter(vm.testSpecifications(),
                        function (item) {
                            return item.Id === vm.session.DefaultTestSpecificationId();
                        });
                    if (!testSpecification.length) {
                        var activeSpecification = ko.utils.arrayFilter(vm.testSpecifications(),
                            function (item) {
                                return item.Active;
                            });
                        if (activeSpecification.length) {
                            vm.session.DefaultTestSpecificationId(activeSpecification[0].Id);
                        } else {
                            vm.session.DefaultTestSpecificationId(null);
                        }
                    }

                });

        }

        return vm;
    }
});