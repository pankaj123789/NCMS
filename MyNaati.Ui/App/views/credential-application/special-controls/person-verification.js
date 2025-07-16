define([],
    function () {
        return {
            getInstance: getInstance
        };

        function getInstance(options) {
            var credentialApplication = options.credentialApplication;

            var application = {
                Id: ko.observable(),
                Created: ko.observable(),
                StatusModified: ko.observable(),
                Status: ko.observable()
            };
            var cleanApplication = ko.toJS(application);

            var serverModel = {
                FirstName: ko.observable().extend({ required: true, maxLength: 100 }),
                FamilyName: ko.observable().extend({ maxLength: 100 }),
                DateOfBirth: ko.observable().extend({
                    required: true,
                    dateLessThan: moment().format('l'),
                    dateGreaterThan: moment('1/1/1900').format('l')
                }),
                Email: ko.observable().extend({ email: true, required: true, maxLength: 200 }),
                NAATINumber: ko.observable().extend({ min: 0, max: 999999 }),
                CountryOfBirth: ko.observable().extend({ required: true }),
                Gender: ko.observable().extend({ required: true }),
                MiddleNames: ko.observable().extend({ maxLength: 100 }),
                Title: ko.observable(),
            };

            serverModel.VerifyEmail = ko.observable()
                .extend({
                    email: true,
                    equal: {
                        params: serverModel.Email,
                        message: 'The field must equal to Email'
                    },
                    maxLength: 200
                });

            var cleanModel = ko.toJS(serverModel);
            var validatedObservable = ko.validatedObservable(serverModel);
            var validation = $.extend(true, {}, validatedObservable, { isValid: isValid });

            var vm = {
                person: serverModel,
                application: application,
                showApplication: ko.observable(),
                validation: validation,
                countries: ko.observableArray(),
                recaptchaToken: ko.observable(),
                applicationId: options.applicationId,
                naatiNumber: options.naatiNumber,
                token: options.token,
                question: options.question,
                applicationFormId: options.applicationFormId,
                next: options.next,
                titles: ko.observableArray(),
                validated: ko.observable(),
                canContinue: ko.observable(false),
                emailEvent: {
                    paste: function(ctx, e) {
                        if (window.kosettings.allowEmailPaste) {
                            return true;
                        }

                        e.preventDefault();
                    }
                },
                loaderOptions: options.loaderOptions,
                canEditDetails: ko.observable(false),
                canEditEmail: ko.observable(false)
        };

            vm.loaderOptions.params.text('Verifying your details...');

            vm.disableDetails = ko.computed(function () {
                return !vm.canEditDetails() || vm.validated();
            });

            vm.disableEmail = ko.computed(function () {
                return !vm.canEditEmail() || vm.validated();
            });


            vm.countryOfBirthOptions = {
                value: serverModel.CountryOfBirth,
                multiple: false,
                options: vm.countries,
                optionsValue: 'SamId',
                optionsText: 'DisplayText',
                disable: vm.disableDetails
            };

            vm.genderOptions = {
                value: serverModel.Gender,
                multiple: false,
                options: [
                    { value: 'M', text: 'Male' },
                    { value: 'F', text: 'Female' },
                    { value: 'X', text: 'Unspecified' },
                ],
                disable: vm.disableDetails
            };

            vm.titleOptions = {
                value: serverModel.Title,
                multiple: false,
                options: vm.titles,
                optionsValue: 'SamId',
                optionsText: 'DisplayText',
                disable: vm.disableDetails
            };

          
			var preventUpdateResponse = false;
            vm.load = function () {
                vm.validated(false);
                vm.naatiNumber(undefined);
                vm.canEditDetails(false);
                vm.canEditEmail(false);
				vm.showApplication(false);

				preventUpdateResponse = true;
                ko.viewmodel.updateFromModel(serverModel, cleanModel);
				preventUpdateResponse = false;

                ko.viewmodel.updateFromModel(application, cleanApplication);

                if (vm.validation.errors) {
                    vm.validation.errors.showAllMessages(false);
                }

                var interval = setInterval(function () {
                    try {
                        grecaptcha.render(
                            'recaptcha',
                            {
                                sitekey: "6LcAPjMUAAAAAO9MGOKhFfw6hEwmK_OjO8G_pH_z",
                                callback: recaptchaCallback,
                                size: "invisible"
                            }
                        );
                        clearInterval(interval);
                    }
                    catch (err) { }
                }, 500);

                vm.canContinue(false);
                vm.loaderOptions.params.show(true);

                credentialApplication.countries().then(vm.countries);
                credentialApplication.personTitles().then(vm.titles);
                credentialApplication.personDetails().then(function (data) {
                    vm.canContinue(true);
                    vm.loaderOptions.params.show(false);

                    if (data.Anonymous) {
                        return;
                    }

                    var personDetails = data.PersonDetails;

                    personDetails.FirstName = personDetails.GivenName;
                    personDetails.DateOfBirth = moment(personDetails.DateOfBirth).toDate();
                    personDetails.Email = personDetails.PrimaryEmail;
                    personDetails.VerifyEmail = personDetails.Email;
                    personDetails.NAATINumber = personDetails.NaatiNumber;
                    personDetails.MiddleNames = personDetails.OtherNames;

                    setPersonPropertyFromArray(personDetails,
                        'CountryOfBirth',
                        vm.countries());

                    setPersonPropertyFromArray(personDetails,
                        'Title',
                        vm.titles());

					preventUpdateResponse = true;
                    ko.viewmodel.updateFromModel(serverModel, personDetails);
					preventUpdateResponse = false;
					updateResponse();

                    credentialApplication.canEditPersonDetails().then(function (data) {
                        vm.canEditDetails(data.CanEditPersonDetails);
                        vm.canEditEmail(data.CanEditEmail);

                        if (!vm.canEditDetails() && !vm.canEditEmail()) {
                            vm.next();
                        }
                    });
                  
                });

                for (var propertyName in serverModel) {
                    serverModel[propertyName].subscribe(updateResponse);
                }
            }

            function setPersonPropertyFromArray(personDetails, propertyName, array) {
                var entity = ko.utils.arrayFirst(array, function (c) {
                    return c.DisplayText === personDetails[propertyName];
                });

                if (entity) {
                    personDetails[propertyName] = entity.SamId;
                }
                else {
                    personDetails[propertyName] = null;
                }
            }

			function updateResponse() {
				if (preventUpdateResponse) {
					return;
				}

                vm.question.Response(JSON.stringify(ko.toJS(serverModel)));
            }

            var recaptchaDefer = null;
            function recaptchaCallback(token) {
                recaptchaDefer.resolve(token);
                vm.recaptchaToken(token);
            }

            function isValid() {

                var isValid = validatedObservable.isValid();

                if (isValid) {
                    if (vm.validated()) {
                        return true;
                    }
                    recaptchaDefer = Q.defer();


                    if (!window.kosettings.skipRecaptcha && !vm.recaptchaToken()) {
                        grecaptcha.execute();
                    }
                    else {
                        recaptchaDefer.resolve(vm.recaptchaToken());
                    }

                    var defer = Q.defer();

                    recaptchaDefer.promise.then(function () {
                        vm.loaderOptions.params.show(true);
                        credentialApplication.personVerification(ko.toJS(serverModel)).then(function (data) {

                            var naatiNumber = data.NAATINumber;
                            var token = data.Token;
                            if (data.Message) {
                                mbox.alert({ content: data.Message });
                                vm.validated(false);
                                vm.loaderOptions.params.show(false);
                                return defer.resolve(false);
                            }

                            credentialApplication.customerApplication({ NAATINumber: naatiNumber, ApplicationFormId: vm.applicationFormId(), Token: token }).then(function (data) {
                                var app = data.Application;

                                if (data.Message) {
                                    mbox.alert({ content: data.Message });
                                    vm.loaderOptions.params.show(false);
                                    return defer.resolve(false);
                                }

                                if (app.Id) {
                                    vm.applicationId(app.Id);
                                    app.Created = moment(new Date(parseInt(app.Created.substr(6)))).format();
                                    app.StatusModified = moment(new Date(parseInt(app.StatusModified.substr(6)))).format();
                                    ko.viewmodel.updateFromModel(application, app);
                                    vm.naatiNumber(naatiNumber);
                                    vm.token(token);
                                    vm.person.NAATINumber(naatiNumber);
                                    vm.validated(true);
                                    vm.loaderOptions.params.show(false);
                                    // START: BYPASS TO APPLICATION PREVIEW - REMOVE IT AFTER APPLICATIONS DRAFT WAS DONE
                                    return defer.resolve(true);
                                    // END
                                    vm.showApplication(true);
                                } else {
                                    // creates an new application
                                    var request = {
                                        Sections: [],
                                        ApplicationId: 0,
                                        NaatiNumber: naatiNumber,
                                        ApplicationFormId: vm.applicationFormId(),
                                        Token: token
                                    };

                                    credentialApplication.createCredentialApplication(request).then(function (data) {
                                        vm.applicationId(data.ApplicationId);
                                        vm.naatiNumber(naatiNumber);
                                        vm.token(token);
                                        vm.person.NAATINumber(naatiNumber);
                                        vm.validated(true);
                                        vm.loaderOptions.params.show(false);
                                        return defer.resolve(true);
                                    });
                                }
                            });

                            // START: BYPASS TO APPLICATION PREVIEW - UNCOMMENT IT
                            //defer.resolve(vm.showApplication());
                            // END
                        }, function() {
                            vm.loaderOptions.params.show(false);
                            return defer.resolve(false);
                        });
                    });

                    return defer.promise;
                }
                vm.validated(false);
                return false;
            }

            return vm;
        }
    });