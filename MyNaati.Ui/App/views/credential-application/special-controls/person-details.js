define([],
	function () {
		return {
			getInstance: getInstance
		};

		function getInstance(options) {
			var AustraliaCountryCode = 13;
			var credentialApplication = options.credentialApplication;

			var address = {
				Id: ko.observable(),
				Latitude: ko.observable(),
				Longitude: ko.observable(),
				StreetDetails: ko.observable().extend({ maxLength: 500 }),
				CountryName: ko.observable('Australia'),
				CountryId: ko.observable(AustraliaCountryCode),
				IsFromAustralia: ko.observable(),
				SuburbName: ko.observable(),
				PostCodeId: ko.observable(),
				Postcode: ko.observable(),
				State: ko.observable(),
				ValidateInExternalTool: ko.observable(true),
				Validated: ko.observable(false),
				FormattedAddress: ko.observable(),
				InvalidNumber: ko.observable(false),
				InvalidSuburb: ko.observable(false),
				ShowGoogleSuburbDropDown: ko.observable(false)
			};

			var cleanAddress = ko.toJS(address);

			address.StreetDetails.extend({
				required: {
					message: function () {
						if (address.InvalidNumber()) {
							return 'The street number is required.';
						}
						if (address.InvalidSuburb()) {
							return 'A suburb is required';
						}
						return 'This field is required.';
					}
				}
			});

			address.StreetDetails.extend({
				validation: {
					validator: function () {
						return !address.ValidateInExternalTool() || address.Validated();
					},
					message: "Select a validated address"
				}
			});

			var serverModel = {
				PhoneNumber: ko.observable().extend
					({
						pattern: /^(?=.*[0-9])[- +()0-9]+$/,
						required: true,
						maxLength: 50
					})
			};

			var cleanModel = ko.toJS(serverModel);
			var personValidation = ko.validatedObservable(serverModel);
			var validation = $.extend(true, {}, personValidation, { isValid: isValid });
			var mapUrl = 'https://maps.googleapis.com/maps/api/staticmap?center={0},{1}&zoom=16&size=640x300&maptype=roadmap&markers=color:red|{0},{1}&key=AIzaSyCwRmBZUnlm7MMoIwCVT1uOxx611lfXdSY';

			var vm = {
				person: serverModel,
				address: address,
				addressValue: ko.observable(),
				countries: ko.observableArray(),
				validation: validation,
				naatiNumber: options.naatiNumber,
				token: options.token,
				question: options.question,
				overseas: ko.observable(false),
				canContinue: ko.observable(false),
				mapId: ko.observable("map"),
				addressInvalidSuburb: ko.observable('Please select a suburb from the list. If your suburb is not in the list, please contact info@naati.com.au'),
				suburbOptions: {
					minLength: 3,
					source: function (request, response) {
						$.ajax({
							type: 'GET',
							data: {
								term: request.term
							},
							cache: false,
							url: window.baseUrl + 'credentialapplication/postcodes/',
							success: function (data) {
								address.PostCodeId(null);
								response($.map(data,
									function (item) {
										return { label: item.DisplayText, value: item.DisplayText, id: item };
									}));
							}
						});
					},
					select: function (event, ui) {
						var postCode = ui.item.id;
						if (postCode) {
							address.CountryId(AustraliaCountryCode);
							address.PostCodeId(postCode.SamId);
							//address.SuburbName(postCode.Suburb);
							address.InvalidSuburb(false);
						}
						else {
							address.PostCodeId(null);
							//address.SuburbName(null);
						}
					}
				},
			};

			address.InvalidSuburb.subscribe(function (newValue) {
				address.Validated(!newValue);
				address.ShowGoogleSuburbDropDown(newValue && address.ValidateInExternalTool())
			});


			vm.addressAutoCompleteOptions = {
				value: vm.addressValue,
				minlength: 3,
				mapId: vm.mapId,
				parserPromise: function (request) {
					return credentialApplication.parseAddress(request);
				},
			};

			address.SuburbName.subscribe(function (newValue) {
				if (address.PostCodeId() &&
					!address.ValidateInExternalTool() &&
					vm.overseas && newValue.length < vm.suburbOptions.minLength) {
					address.PostCodeId(null);
				}
			});

			address.CountryName.subscribe(function (value) {
				var selectedCountry = ko.utils.arrayFirst(vm.countries(), function (s) {
					return s.DisplayText === value;
				});

				if (selectedCountry) {
					address.CountryId(selectedCountry.SamId);
				}
				else {
					address.CountryId(null);
				}
			});

			address.StreetDetails.subscribe(invalidateAddress);
			address.FormattedAddress.subscribe(invalidateAddress);

			vm.addressValue.subscribe(function (selectedAddress) {
				if (selectedAddress) {
					if (!selectedAddress.StreetNumber) {
						ko.viewmodel.updateFromModel(vm.address, cleanAddress);
						return vm.address.InvalidNumber(true);
					}
					if (!selectedAddress.Suburb && selectedAddress.Postcode) {
						//google hasnt found a suburb and we need it.
						ko.viewmodel.updateFromModel(vm.address, cleanAddress);
						setAddressFields(selectedAddress);
						return vm.address.InvalidSuburb(true);
					}

					vm.address.InvalidNumber(false);
					setGoogleMapsLocation(selectedAddress.FormattedAddress, selectedAddress.Latitude, selectedAddress.Longitude);
					setAddressFields(selectedAddress);
				}
			});

			address.PostCodeId.extend({
				required: {
					onlyIf: ko.pureComputed(function () {
						return !vm.overseas() && !address.ValidateInExternalTool();
					}),
					message: 'Please select a suburb from the list'
				}
			});

			address.CountryId.extend({
				required: {
					onlyIf: ko.pureComputed(function () { return !address.ValidateInExternalTool(); })
				}
			});

			address.ValidateInExternalTool.subscribe(function (newValue) {
				if (newValue) {
					vm.overseas(false);
				}
				address.ShowGoogleSuburbDropDown(newValue && address.InvalidSuburb());
			});

			address.PostCodeId.subscribe(function () {
				if (!address.ValidateInExternalTool() && !vm.overseas()) {
					updateResponse();
				}
			});

			address.SuburbName.subscribe(function (data) {
				if (data && !vm.overseas()) {
					updateResponse();
				}
			});

			address.CountryId.subscribe(function () {
				if (!address.ValidateInExternalTool() && vm.overseas()) {
					updateResponse();
				}
			});

			vm.overseas.subscribe(function (value) {
				if (value) {
					address.CountryId(undefined);
				}
				else {
					address.CountryId(AustraliaCountryCode);
				}
			});

			var addressValidation = ko.validatedObservable(address);

			vm.countryOptions = {
				value: address.CountryId,
				multiple: false,
				options: vm.countries,
				optionsValue: 'SamId',
				optionsText: 'DisplayText',
				addChooseOption: false
			};

			vm.load = function () {
				vm.overseas(false);
				vm.canContinue(false);

				ko.viewmodel.updateFromModel(serverModel, cleanModel);
				ko.viewmodel.updateFromModel(address, cleanAddress);

				resetValidations();

				credentialApplication.customerDetails({ NAATINumber: vm.naatiNumber(), Token: vm.token() }).then(function (data) {
					var addr = data.Address;

					ko.viewmodel.updateFromModel(serverModel, data);
					ko.viewmodel.updateFromModel(address, addr);

					resetValidations();
					vm.canContinue(true);

					if (addr) {
						vm.overseas(addr.CountryName && !addr.IsFromAustralia);

                        if (!addr.Latitude && !addr.Longitude) {
                            var interval = setInterval(function () {
                                try {
                                    reverseGeocodeAndSetGoogleMapLocation(addr.FullAddress).then(function () {
                                        address.Validated(true);
                                    });
                                    clearInterval(interval);
                                }
                                catch (err) { }
                            }, 500);
						}

						if (!addr.SuburbName) {
							InvalidSuburb(true);
						}
                    }
                });
            };

			init();

			function invalidateAddress() {
				address.Validated(false);
				if (!address.ValidateInExternalTool()) {
					updateResponse();
				}
			}

			function init() {
				//credentialApplication.countries().then(vm.countries);

				credentialApplication.countries().then(function (countries) {
					var index = countries.findIndex(x => x.DisplayText == 'Australia');
					if (index > -1) {
						countries.splice(index, 1);
					}
						vm.countries(countries);
				});

				for (var propertyName in serverModel) {
					var property = serverModel[propertyName];
					if (ko.isObservable(property)) {
						property.subscribe(updateResponse);
					}
				}
			}

			function resetValidation(validation) {
				if (validation.errors) {
					validation.errors.showAllMessages(false);
				}
			}

			function resetValidations() {
				resetValidation(addressValidation);
				resetValidation(vm.validation);
			}

			function updateResponse() {

				if (!personValidation.isValid() || !addressValidation.isValid()) {
					return;
				}
				var request = ko.toJS(serverModel);
				request.Address = ko.toJS(address);
				vm.question.Response(JSON.stringify(request));
			}

			function isValid() {
				if (!personValidation.isValid()) {
					personValidation.errors.showAllMessages();
				}
				if (!addressValidation.isValid()) {
					addressValidation.errors.showAllMessages();
				}
				return personValidation.isValid() && addressValidation.isValid();
			}

            function reverseGeocodeAndSetGoogleMapLocation(address) {
                var defer = Q.defer();
                var geocoder = new google.maps.Geocoder();

				geocoder.geocode({ address: address },
					function (results, status) {
						if (status == google.maps.GeocoderStatus.OK) {
							var place = results[0];
							vm.address.FormattedAddress(place.formatted_address);
							setGoogleMapsLocation(place.formatted_address,
								place.geometry.location.lat(),
								place.geometry.location.lng());

							defer.resolve(place);
						} else {
							toastr.warning('This address cannot be located on the map.');
						}
					});

				return defer.promise;
			}

            function setGoogleMapsLocation(selectedAddress, latitude, longitude) {
                var url = mapUrl.replace(/\{0\}/g, latitude).replace(/\{1\}/g, longitude);
                $('#map').attr('src', url).attr('title', selectedAddress);
            }

			function setAddressFields(addr) {
				var country = addr.CountryName;

				ko.viewmodel.updateFromModel(address, cleanAddress);

				address.IsFromAustralia(country === 'Australia');
				address.CountryName(country);
				vm.overseas(address.CountryName() && !address.IsFromAustralia());


				address.SuburbName(addr.Suburb);

				address.StreetDetails(addr.StreetDetails);
				address.FormattedAddress(addr.FormattedAddress);

				if (country === 'Australia') {
					address.Postcode(addr.Postcode);
					address.State(addr.State);
					address.SuburbName(addr.Suburb);
					address.CountryId(AustraliaCountryCode);
				}

				if (!vm.overseas() && !address.ValidateInExternalTool()) {
					address.CountryId(AustraliaCountryCode); // default to Australia
				}

				if (vm.overseas()) {
					address.SuburbName(null);
					address.PostCodeId(null);
					if (address.CountryId == AustraliaCountryCode) { address.CountryId = undefined}
				}

				address.Validated(true);
				updateResponse();
			}

			return vm;
		}
	});