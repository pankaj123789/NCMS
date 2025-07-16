(function () {
    ko.bindingHandlers.addressAutoComplete = {
        init: function (element, valueAccessor, allBindingsAccessor, viewModel) {
            var accessor = ko.unwrap(valueAccessor()) || {};
			var $searchInput = $(element);
            
            // bounds of australia which we will use to  bias autocomplete results to australia
            var defaultOptions = {
                bounds: [],
                types: ['geocode']
            };

            defaultOptions.bounds = new google.maps.LatLngBounds({ lat: -54.83376579999999, lng: 110.95103389999997 },
                { lat: -9.187026399999999, lng: 159.28722229999994 });

            if (accessor.bounds) {
                defaultOptions.bounds = new google.maps.LatLngBounds(accessor.bounds);
			}

			if (accessor.minlength) {
				setupCustom();
			}
			else {
				setupDefault();
			}
            accessor.parserPromise = accessor.parserPromise || function (data) { return Q.Promise.resolve({}) }

			function setupDefault() {
				var autocomplete = new google.maps.places
					.Autocomplete($searchInput[0], defaultOptions);

				autocomplete.addListener('place_changed', function () {
                    onAddressSelected(autocomplete.getPlace());
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

            function onAddressSelected(selectedAddress) {
                if (!selectedAddress) {
                    return;
                }
                var request = getParseAddressRequest(selectedAddress);
                accessor.parserPromise(request).then(function(parsedAddress) {
                    var address = {
                        FormattedAddress: selectedAddress.formatted_address,
                        Latitude: selectedAddress.geometry.location.lat(),
                        Longitude: selectedAddress.geometry.location.lng()
                    };

                    address.StreetDetails = parsedAddress.StreetDetails;
                    address.Suburb = parsedAddress.Suburb;
                    address.Postcode = parsedAddress.Postcode;
                    address.CountryName = parsedAddress.Country;
                    address.StreetNumber = parsedAddress.StreetNumber;
                    address.State = parsedAddress.State;
                    if (!accessor.value) {
                        return;
                    }
                    accessor.value(address);
                    
                });
			}

			function setupCustom() {
				$searchInput.autocomplete({
					minLength: accessor.minlength,
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

			function getPredictionDetails(placeId) {
                var placeService = new google.maps.places.PlacesService(document.getElementById(accessor.mapId()));
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
        }
    };
})();