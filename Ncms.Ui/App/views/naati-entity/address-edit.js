define([
    'services/person-data-service',
    'plugins/router',
    'modules/common',
    'services/util',
    'services/screen/message-service',
    'services/address-data-service',
    'services/institution-data-service',
],
    function (personDataService, router, common, util, message, addressService, institutionService) {
        var australiaCountryCode = 13;
        var mapDefaultAddress = '2 King Street, Deakin ACT 2600';
        var mapUrl = 'https://maps.googleapis.com/maps/api/staticmap?center={0},{1}&zoom=16&size=640x300&maptype=roadmap&markers=color:red|{0},{1}&key=AIzaSyCwRmBZUnlm7MMoIwCVT1uOxx611lfXdSY';
        var serverModel = {
            AddressId: ko.observable(),
            EntityId: ko.observable(),
            Postcode: ko.observable(),
            PostcodeId: ko.observable(),
            StreetDetails: ko.observable().extend({ required: true }),
            Suburb: ko.observable(),
            CountryId: ko.observable(),
            CountryName: ko.observable(),
            StateAbbreviation: ko.observable(),
            Note: ko.observable(),
            PrimaryContact: ko.observable(),
            ValidateInExternalTool: ko.observable(),
            ExaminerCorrespondence: ko.observable(),
            IsExaminer: ko.observable(),
            IsOrganisation: ko.observable(),
            OdAddressVisibilityTypeId: ko.observable().extend({ required: true })
        };

        var suburbs;
        var composed = false;

        var vm = {
            canActivate: canActivate,
            compositionComplete: compositionComplete,
            activate: activate,
            address: serverModel,
            addresses: ko.observableArray(),
            addressText: ko.observable(),
            selectedAddress: ko.observable(),
            overseas: ko.observable(false),
            save: save,
            tryClose: tryClose,
            suburb: ko.observable(),
            editMode: ko.observable(),
            originalPrimaryContact: ko.observable(false),
            isAddressSelectedFromList: ko.observable(false),
            mapSource: ko.observable(),
            mapTitle: ko.observable(),
            IsOrganisation: ko.observable()
        };

        vm.windowTitle = ko.computed(function () {
            return vm.editMode() ? 'Edit Address' : 'Add Address';
        });

        serverModel.PostcodeId.extend({
            required: {
                params: ko.pureComputed(function () {
                    return !vm.overseas() && !serverModel.ValidateInExternalTool();
                }),
                message: 'Please select a suburb from the list'
            }
        });

        serverModel.CountryId.extend({ required: ko.pureComputed(function () { return !serverModel.ValidateInExternalTool(); }) });

        serverModel.PostcodeId.subscribe(selectSuburb);

        serverModel.ValidateInExternalTool.subscribe(function (newValue) {
            if (newValue) {
                vm.overseas(false);
                $("#map").css({ display: "block" });
                google.maps.event.trigger($("#map")[0], 'resize');
            } else {
                $("#map").css({ display: "none" });
            }
        });

        vm.dirtyFlag = new ko.DirtyFlag([vm.address], false);

        vm.canSave = ko.computed(function () {
            return vm.dirtyFlag().isDirty();
        });

        vm.setIsOrganisationFlag = function(flag) {
            vm.IsOrganisation(flag);
        };

        vm.disablePrimaryContact = ko.computed(function () {
            var isAdding = !vm.editMode();
            var addedCount = vm.addresses().length;

            return addedCount === 0 || addedCount === 1 && !isAdding && vm.originalPrimaryContact() || vm.originalPrimaryContact();
        });

        vm.countryOptions = {
            value: vm.address.CountryId,
            entity: 'Country',
            valueField: ko.observable('CountryId'),
            textField: ko.observable('Name'),
            multiple: false,
            dataCallback: function (data) {
                var countries = data.filter(
                    function (item) {
                        return item.Name !== 'Australia';
                    });
                return countries.sort(util.sortBy('Name'));
            }
        };

        vm.OdAddressVisibilityTypesParams = {
            multiple: false,
            value: vm.address.OdAddressVisibilityTypeId,
            optionsValue: 'Id',
            optionsText: 'DisplayName',
            options: ko.observableArray(),
            addChooseOption: false
        };

        serverModel.EntityId.subscribe(function (newValue) {
            if (!newValue) return vm.addresses([]);

            (vm.IsOrganisation() ? institutionService : personDataService).getFluid(newValue + '/contactdetails')
                .then(function (data) {
                    if (!serverModel.PrimaryContact()) {
                        serverModel.PrimaryContact(data.Addresses.length === 0);
                    }
                    vm.addresses(data.Addresses);
                });
        });

        vm.suburb.subscribe(selectSuburb);

        var validation = ko.validatedObservable(vm.address);
        var autocomplete;

        function selectSuburb(value) {
            if (suburbs) {
                for (var i = 0; i < suburbs.length; i++) {
                    if (suburbs[i].N === value) {
                        vm.address.Suburb(suburbs[i].N);
                        if (vm.address.PostcodeId() != suburbs[i].P) {
                            vm.address.PostcodeId(suburbs[i].P);
                        }
                        break;
                    }
                }
            }
        }

        function canActivate(entityId, addressId) {

            common.functions().getLookup('OdAddressVisibilityTypeBackend').then(function (data) {
                vm.OdAddressVisibilityTypesParams.options(data);
            });

            if (addressId !== undefined) {
                return loadAddress(entityId, addressId);
            } else {
                checkExaminerRole(entityId);
                vm.editMode(false);
                return true;
            }
        }

        function compositionComplete() {
            composed = true;
            initialiseGoogle();
            // due to view caching, we only compose the first time the view is used. In this instance setting the default location in the map can't be done in 
            // activate() (composition isn't finished), so it must be done here. subsequently it will be done in activate().
            if (!vm.editMode()) {
                setGoogleMapLocation(mapDefaultAddress);
            }

        }

        function activate(entityId) {
            if (!suburbs) {
                populateSuburbs();
            }

            if (vm.editMode()) {
                edit();
            } else {
                create(entityId);
            }

            if (vm.editMode()) {
                setGoogleMapLocation(vm.addressText());
            } else {
                if (composed) {
                    setGoogleMapLocation(mapDefaultAddress);
                }
            }
        }

        function populateSuburbs() {
            (vm.IsOrganisation() ? institutionService : personDataService).getFluid('suburbs')
                .then(
                function (data) {
                    suburbs = data;
                    var suburbSelect = document.getElementById('suburbSelect');
                    $('#suburbSelect').find('option').remove();
                    suburbs.forEach(
                        function (item) {
                            var option = document.createElement('option');
                            option.value = item.N;
                            suburbSelect.appendChild(option);
                        });
                });
        }

        function clear() {
            util.resetModel(serverModel);
            vm.addressText('');
            vm.suburb('');
            vm.overseas(false);
        }

        function create(entityId) {
            clear();
            serverModel.AddressId(null);
            serverModel.EntityId(entityId);
            serverModel.ValidateInExternalTool(true);
            serverModel.OdAddressVisibilityTypeId(1);
            vm.originalPrimaryContact(false);
            vm.dirtyFlag().reset();
            validation.errors.showAllMessages(false);
            return true;
        }

        function loadAddress(entityId, addressId) {
            return (vm.IsOrganisation() ? institutionService : personDataService).getFluid(entityId + '/address/' + addressId)
                .then(
                function (data) {
                    clear();
                    ko.viewmodel.updateFromModel(vm.address, data);
                    vm.IsOrganisation(data.IsOrganisation);
                    vm.editMode(true);
                    vm.originalPrimaryContact(data.PrimaryContact);
                    return true;
                },
                function () {
                    return false;
                });
        }

        function checkExaminerRole(entityId) {
            return (vm.IsOrganisation() ? institutionService : personDataService).getFluid('checkExaminerRole/' + entityId)
                .then(
                function (data) {
                    vm.address.IsExaminer(data);
                });
        }

        function edit() {
            vm.suburb(vm.address.Suburb());
            vm.addressText(util.addressToString(vm.address));
            vm.overseas(vm.address.CountryName() !== "" && vm.address.CountryName() !== 'Australia');
            vm.dirtyFlag().reset();
            validation.errors.showAllMessages(false);
        }

        function initialiseAddressAutocomplete() {
            // bounds of australia which we will use to  bias autocomplete results to australia
            var ausBounds = new google.maps.LatLngBounds({ lat: -54.83376579999999, lng: 110.95103389999997 },
                { lat: -9.187026399999999, lng: 159.28722229999994 });

            var defaultOptions = { bounds: ausBounds, types: ['geocode'] };

            var $searchInput = $('#addressInput');

            if ($searchInput.attr('address-search-minlength')) {
                setupCustom();
            }
            else {
                setupDefault();
            }

            function setupDefault() {
                var autocomplete = new google.maps.places
                    .Autocomplete($searchInput[0], defaultOptions);

                autocomplete.addListener('place_changed', function () {
                    onAddressSelected(autocomplete.getPlace());
                });
            }

            function setupCustom() {
                $searchInput.autocomplete({
                    minLength: $searchInput.attr('address-search-minlength'),
                    source: function (request, response) {
                        search(request.term).then(function (data) {
                            response(data);
                        });
                    },
                    select: function (event, ui) {
                        getPredictionDetails(ui.item.place_id).then(function (data) {
                            onAddressSelected(data);
                        });
                    }
                });
            }

            function search(query) {
                //var restrictions = { country: 'uk' };
                var service = new google.maps.places.AutocompleteService();

                return new Promise(function (resolve, reject) {
                    function displaySuggestions(predictions, status) {
                        if (status != google.maps.places.PlacesServiceStatus.OK) {
                            return resolve([]);
                        }

                        $.each(predictions, function (i, p) {
                            p.label = p.description;
                            p.value = p.description;
                        });

                        return resolve(predictions);
                    };

                    service.getPlacePredictions($.extend({ input: query }, defaultOptions), displaySuggestions);
                });
            }
        }

        function setGoogleMapLocation(address) {

            vm.mapSource(mapUrl.format(encodeURIComponent(address)));
            vm.mapTitle(address);
        }

        function onAddressSelected(selectedAddress) {

            if (!selectedAddress) {
                return;
            }
            var request = getParseAddressRequest(selectedAddress);
            addressService.getFluid('parse', request).then(function (parsedAddress) {
                setGoogleMapLocation(selectedAddress.formatted_address);
                vm.address.StreetDetails(parsedAddress.StreetDetails);
                vm.address.Suburb(parsedAddress.Suburb);
                vm.address.Postcode(parsedAddress.Postcode);
                vm.address.CountryName(parsedAddress.Country);
                vm.address.StateAbbreviation(parsedAddress.State)
                vm.isAddressSelectedFromList(true);
            });
        }

        function getPredictionDetails(placeId) {
            var placeService = new google.maps.places.PlacesService(document.getElementById("map"));
            return new Promise(function (resolve, reject) {
                placeService.getDetails({
                    placeId: placeId
                }, function (result, status) {
                    if (status === google.maps.places.PlacesServiceStatus.OK) {
                        resolve(result);
                    } else {
                        reject(status);
                    }
                });
            });
        }

        function getParseAddressRequest(address) {

            return {
                Geometry: {
                    Location: {
                        Lat: address.geometry.location.lat(),
                        Lng: address.geometry.location.lng()
                    }
                },
                Formatted_Address: address.formatted_address,
                Types: address.types,
                Address_Components: address.address_components.map(getAddressComponent)
            };
        }

        function getAddressComponent(component) {
            return {
                Long_Name: component.long_name,
                Short_Name: component.short_name,
                Types: component.types
            };
        }

        function resetSuburbPostcode() {
            vm.address.Suburb(null);
            vm.address.PostcodeId(null);
        }

        function save() {

            if (!vm.address.ValidateInExternalTool()) {
                if (!vm.overseas()) {
                    vm.address.CountryId(australiaCountryCode); // default to Australia
                } else {
                    resetSuburbPostcode();
                    if (vm.address.CountryId() === australiaCountryCode) {
                        vm.address.CountryId(null);
                    }
                }
            } else {

                vm.overseas((vm.address.CountryName() !== "" && vm.address.CountryName() !== 'Australia'));

                if (!vm.overseas()) {
                    if (vm.isAddressSelectedFromList()) {
                        vm.address.PostcodeId(null);
                        //once reseting PostcodeId then reset this flag again
                        vm.isAddressSelectedFromList(false);
                    }
                    vm.address.CountryId(australiaCountryCode); // default to Australia
                } else {
                    resetSuburbPostcode();
                    //since ValidateInExternalTool has been used here and set the address.CountryName(), so need to reset the address.CountryId()
                    vm.address.CountryId(null);
                }
            }

            var isValid = validation.isValid();
            validation.errors.showAllMessages(!isValid);

            if (isValid) {
                var json = ko.toJS(vm.address);
                (vm.IsOrganisation() ? institutionService : personDataService).post(json, 'address')
                    .then(
                    function () {
                        toastr.success('Address saved');
                        close();
                    });
            }
        }

        function tryClose() {
            if (vm.dirtyFlag().isDirty()) {
                message.confirm({
                    title: ko.Localization('Naati.Resources.Shared.resources.Confirm'),
                    content: ko.Localization('Naati.Resources.Shared.resources.AreYouSureYouWantToCancel')
                })
                    .then(
                    function (answer) {
                        if (answer === 'yes') {
                            close();
                        }
                    });
            } else {
                close();
            }
        }

        function close() {
            router.navigateBack();
        };

        function initialiseGoogle() {
            initialiseAddressAutocomplete();
        }

        return vm;
    });
