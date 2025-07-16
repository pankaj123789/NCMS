define([
    'services/application-data-service',
        'services/screen/message-service'
],
    function (applicationService, messageService) {
        return {
            getInstance: getInstance
        };

        function getInstance(venue, isCreate) {
            
            var mapUrl = "https://maps.googleapis.com/maps/api/staticmap?center={0},{1}&zoom=16&size=550x505&maptype=roadmap&markers=color:red|{0},{1}&key=AIzaSyCwRmBZUnlm7MMoIwCVT1uOxx611lfXdSY"
            var mapDefaultAddress = '2 King Street, Deakin ACT 2600';
            var composed = false;

            var serverModel = $.extend({
                    VenueId: ko.observable(),
                    Name: ko.observable().extend({ required: true, maxLength: 255 }),
                    Address: ko.observable().extend({ required: true, maxLength: 255 }),
                    Latitude: ko.observable(),
                    Longitude: ko.observable(),
                    Capacity: ko.observable().extend({ required: true, maxLength: 255, number: true }),
                    TestLocationId: ko.observable().extend({ required: true }),
                    PublicNotes: ko.observable().extend({ maxLength: 500 }),
                    Active: ko.observable()
            }, venue);

            var vm = {
                venue: serverModel,
                requireCoordinates: ko.computed(function () {

                if ((venue.Latitude() === "undefined" || venue.Longitude() === "undefined") || (venue.Latitude() === "" || venue.Longitude() === ""))
                        return false;

                if (venue.Latitude() !== undefined || venue.Longitude() !== undefined)
                        return true;

                return false;
            }),

                title: ko.pureComputed(function() {
                    return '{0} - {1}'.format(ko.Localization('Naati.Resources.Venue.resources.EditVenue'),
                        serverModel.Name());
                }),
                locations: ko.observableArray(),
                isCreate: isCreate,

                compositionComplete: compositionComplete,

                mapSource: ko.observable(),
                mapTitle: ko.observable()
            };

            vm.activate = function() {
                vm.venue.Latitude.extend({ required: vm.requireCoordinates(), maxLength: 10, number: true, max: 90, min: -90 });
                vm.venue.Longitude.extend({ required: vm.requireCoordinates(), maxLength: 11, number: true, max: 180, min: -180 });
            };

            vm.locationOptions = {
                value: serverModel.TestLocationId,
                multiple: false,
                options: vm.locations,
                optionsValue: 'Id',
                optionsText: 'DisplayName'
            };

            vm.updateMap = function () {
                compositionComplete();
            }

            function setMapbyCordinate() {
                var latitude = vm.venue.Latitude();
                var longitude = vm.venue.Longitude();

                var url = mapUrl.replace(/\{0\}/g, latitude).replace(/\{1\}/g, longitude);
                vm.mapSource(url);
                vm.mapTitle(venue.Name());
            }

            vm.load = function() {
                applicationService.getFluid('lookuptype/TestLocation').then(function(data) {

                    ko.viewmodel.updateFromModel(vm.locations, data);
                });
            };

            //address auto com

            function compositionComplete() {
                composed = true;
                initialiseGoogle();

                if (vm.requireCoordinates() === true) {
                    setMapbyCordinate();
                }
                else {
                    if (venue.Address() !== '') {
                        var address = venue.Address();
                        setGoogleMapLocation(address);
                        if (address && !venue.Longitude() && !venue.Latitude()) {
                            loadMapDetails(address);
                        }
                    } else {
                        setGoogleMapLocation(mapDefaultAddress);
                    }
                }
                
            }

            function loadMapDetails(address) {
                var service = new google.maps.places.AutocompleteService();
                console.log("loadmapdetails");
                return new Promise(function (resolve, reject) {
                    function setModel(data, status) {
                        console.log("set model");
                        if (status != google.maps.places.PlacesServiceStatus.OK) {
                            console.log("status:" + status);
                            return resolve([]);
                        }
                        console.log("getPredictionDetails");
                        getPredictionDetails(data[0].place_id).then(function (data) {
                            console.log("onAddressSelected");
                            onAddressSelected(data);
                        });
                        return resolve(data);
                    };
                    var ausBounds = new google.maps.LatLngBounds({ lat: -54.83376579999999, lng: 110.95103389999997 },
                        { lat: -9.187026399999999, lng: 159.28722229999994 });
                    var defaultOptions = { bounds: ausBounds, types: ['geocode'] };
                    var input = $.extend({ input: address }, defaultOptions);
                    console.log("getPlacePredictions");
                    service.getPlacePredictions(input, setModel);
                });
            }

            function initialiseAddressAutocomplete() {
                if (typeof(google) == "undefined" || !google.maps) return;

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

                        var input = $.extend({ input: query }, defaultOptions);
                        service.getPlacePredictions(input, displaySuggestions);
                    });
                }
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
            function onAddressSelected(selectedAddress) {
                if (selectedAddress) {

                    if (!vm.requireCoordinates()) {
                        setGoogleMapLocation(selectedAddress.formatted_address);
                    }

                    var place = parseAutocompletePlace(selectedAddress);
                    var streetDetails = [];

                    if (place.street_number) {
                        if (place.subpremise) {
                            streetDetails.push(place.subpremise + '/' + place.street_number);
                        } else {
                            streetDetails.push(place.street_number);
                        }
                    }

                    if (place.route) {
                        streetDetails.push(place.route);
                    }

                    var locality = place.sublocality || place.locality || place.postal_town || '';
                    var partialStreetDetails = '';
                    var fullAddress = '';

                    if (place.country !== 'Australia') {

                        if (place.administrative_area_level_1 !== null || place.administrative_area_level_1 !== '') {
                            partialStreetDetails = ", " + locality + ", " + place.administrative_area_level_1;
                        } else {
                            partialStreetDetails = ", " + locality;
                        }
                        fullAddress = streetDetails.join(' ').concat(partialStreetDetails) + ', ' + locality + ', ' + place.postal_code + ', ' + place.country;

                    } else {
                        fullAddress = streetDetails.join(' ') + ', ' + locality + ', ' + place.postal_code + ', ' + place.country;
                    }

                    vm.venue.Address(fullAddress);
                    if (!vm.venue.Latitude()) {
                        var lattitude = selectedAddress.geometry.location.lat();
                        vm.venue.Latitude(lattitude.toString().substring(0, 10));
                    }
                    if (!vm.venue.Longitude()) {
                        var longitude = selectedAddress.geometry.location.lng();
                        vm.venue.Longitude(longitude.toString().substring(0, 10));
                    }
                }
            }

            function parseAutocompletePlace(place) {
                var addressComponents = place.address_components;
                var components = {};

                $.each(addressComponents,
                    function (k, v1) {
                        $.each(v1.types,
                            function (k2, v2) {
                                components[v2] = v1.long_name;
                            });
                    });

                return components;
            }

            function initialiseGoogle() {
                initialiseAddressAutocomplete();
            }

            function setGoogleMapLocation(address) {

                vm.mapSource(mapUrl.format(encodeURIComponent(address)));
                vm.mapTitle(address);
            }

            return vm;
        }
    });

