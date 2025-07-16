define([
        'services/screen/date-service',
        'modules/custom-validator',
        'modules/enums',
        'services/application-data-service'
    ],
    function(dateService, customValidator, enums, applicationService) {
        return {
            getInstance: getInstance
        };

        function getInstance(params) {
            var defaultParams = {
                application: null,
                action: null,
                credentialRequest: null
            };

            $.extend(defaultParams, params);

            var serverModel = {
                PractitionerNumber: ko.observable(),
                StartDate: ko.observable().extend({ dateGreaterThan: moment('1/1/1753').format('l') }),
                ExpiryDate: ko.observable().extend({ dateGreaterThan: moment('1/1/1753').format('l') }),
                SelectedCertificationPeriodId: ko.observable(),
                CertificationPeriodStart: ko.observable(),
                CertificationPeriodEnd: ko.observable(),
                ShowInOnlineDirectory: ko.observable(),
                AllowEditCertificationPeriodStartDate: ko.observable(),
                DisallowEditCredentialStartDate: ko.observable(false)
            };

            serverModel.SelectedCertificationPeriodId.subscribe(setCertificationPeriodDates);

            var validator = customValidator.create(serverModel);

            var certificationPeriodDefaultDuration = null;

            var vm = {
                issueCredential: serverModel,
                application: defaultParams.application,
                action: defaultParams.action,
                credentialRequest: defaultParams.credentialRequest,
                certificationPeriods: ko.observableArray(),
                allCertificationPeriods: ko.observableArray(),
                disableCertificationPeriodEnd: ko.observable(true),
            };

            var requiredIfNewCertificationPeriod = {
                required: {
                    onlyIf: ko.pureComputed(function() {
                        return serverModel.SelectedCertificationPeriodId() === 0;
                    })
                }
            };

            serverModel.CertificationPeriodEnd.extend({
                dateGreaterThan: ko.pureComputed(function () {
                    return serverModel.SelectedCertificationPeriodId() === 0 ? serverModel.CertificationPeriodStart : null;
                }),
            });

            serverModel.StartDate.extend({
                dateLessThan: ko.pureComputed(function () {
                    
                    if (serverModel.SelectedCertificationPeriodId() === 0) {
                        return serverModel.CertificationPeriodEnd();
                    }

                    var selectedPeriod = ko.utils.arrayFilter(vm.certificationPeriods(),
                        function(period) {
                            return period.Id === serverModel.SelectedCertificationPeriodId();
                        });

                    if (selectedPeriod.length) {
                        return moment(selectedPeriod[0].EndDate);
                    }

                    return null;
                })
            });

           

            serverModel.CertificationPeriodStart.extend(requiredIfNewCertificationPeriod);
            serverModel.CertificationPeriodEnd.extend(requiredIfNewCertificationPeriod);

            serverModel.StartDate.subscribe(function(value) {
                if (serverModel.AllowEditCertificationPeriodStartDate()) {
                    //var endDate = moment(value).add({ year: certificationPeriodDefaultDuration, day: -1 }).toDate();
                    endDate = GetLeapYearAdjustedEndDate(value, certificationPeriodDefaultDuration);
                    serverModel.CertificationPeriodStart(value);
                    serverModel.CertificationPeriodEnd(endDate);
                }
            });

            function GetLeapYearAdjustedEndDate(startDate, policyYears) {
                // a 3 year certification is actually 1095 days
                var policyDays = 365 * policyYears;
                return moment().add(policyDays, 'days');
            }

            serverModel.CertificationPeriodEnd.subscribe(function(value) {
                serverModel.ExpiryDate(value);
            });

            vm.startDateOptions = {
                value: serverModel.StartDate,
                resattr: {
                    placeholder: 'Naati.Resources.Application.resources.StartDate'
                },
                css: 'form-control w-sm',
                disable: ko.pureComputed(function() {
                    return serverModel.DisallowEditCredentialStartDate();
                })
            };

            vm.expiryDateOptions = {
                value: serverModel.ExpiryDate,
                resattr: {
                    placeholder: 'Naati.Resources.Application.resources.ExpiryDate'
                },
                css: 'form-control w-sm'
            };

            vm.certificationPeriodOptions = {
                value: serverModel.SelectedCertificationPeriodId,
                multiple: false,
                options: vm.certificationPeriods,
                optionsValue: 'Id',
                optionsText: 'DisplayName',
                disable: ko.pureComputed(function() {
                    return !vm.certificationPeriods().length;
                })
            };

            vm.certificationPeriodStartOptions = {
                value: serverModel.CertificationPeriodStart,
                css: 'form-control w-sm',
                disable: true
            };

            vm.certificationPeriodEndOptions = {
                value: serverModel.CertificationPeriodEnd,
                css: 'form-control w-sm',
                disable: vm.disableCertificationPeriodEnd
            };

            vm.load = function() {
                validator.reset();

                var request = {
                    ApplicationId: vm.application.ApplicationId(),
                    ActionId: vm.action().Id,
                    CredentialRequestId: vm.credentialRequest.Id()
                };

                applicationService.getFluid('wizard/issuecredential', request).then(function(data) {
                    processCertificationPeriods(data.CertificationPeriods);
                    processIssueCredential(data);
                });
            };

            var validation = ko.validatedObservable(serverModel);
            vm.isValid = function() {
                var defer = Q.defer();

                if (!validation.isValid()) {
                    defer.resolve(false);
                    return defer.promise;
                }

                var request = ko.toJS(serverModel);
                request.StartDate = dateService.toPostDate(request.StartDate);
                request.ExpiryDate = request.ExpiryDate ? dateService.toPostDate(request.ExpiryDate) : null;
                request.CertificationPeriodStart = dateService.toPostDate(request.CertificationPeriodStart);
                request.CertificationPeriodEnd = dateService.toPostDate(request.CertificationPeriodEnd);
                request.Certification = vm.credentialRequest.CredentialType.Certification();

                $.extend(request,
                {
                    ApplicationId: vm.application.ApplicationId(),
                    Action: vm.action().Id,
                    CredentialRequestId: vm.credentialRequest.Id()
                });
                applicationService.post(request, 'wizard/issuecredential').then(function(data) {
                    validator.reset();

                    ko.utils.arrayForEach(data.InvalidFields,
                        function(i) {
                            validator.setValidation(i.FieldName, false, i.Message);
                        });

                    validator.isValid();

                    defer.resolve(!data.InvalidFields.length);
                });

                return defer.promise;
            };

            vm.postData = function() {
                var json = ko.toJS(serverModel);
                json.StartDate = dateService.toPostDate(json.StartDate);
                json.ExpiryDate = json.ExpiryDate ? dateService.toPostDate(json.ExpiryDate) : null;
                json.CertificationPeriodStart = dateService.toPostDate(moment(json.CertificationPeriodStart));
                json.CertificationPeriodEnd = dateService.toPostDate(moment(json.CertificationPeriodEnd));
                return json;
            };

            function processCertificationPeriods(data) {
                ko.utils.arrayForEach(data,
                    function(d) {
                        if (d.Id === 0) {
                            d.DisplayName = ko
                                .Localization('Naati.Resources.Application.resources.NewCertificationPeriod');
                        } else {
                            d.DisplayName = "{0}: {1} to {2}".format(d.Id,
                                moment(d.StartDate).format('l'),
                                moment(d.EndDate).format('l'));
                        }
                    });

                vm.certificationPeriods(data);
            }

            function processIssueCredential(data) {
                if (data.StartDate) {
                    data.StartDate = moment(data.StartDate).toDate();
                }
                if (data.ExpiryDate) {
                    data.ExpiryDate = moment(data.ExpiryDate).toDate();
                }

                ko.viewmodel.updateFromModel(serverModel, data);
                certificationPeriodDefaultDuration = data.CertificationPeriodDefaultDuration;
            }

            function setCertificationPeriodDates(certificationPeriod) {
                vm.disableCertificationPeriodEnd(true);

                var periods = vm.certificationPeriods();
                for (var i = 0; i < periods.length; i++) {

                    var period = periods[i];
                    if (period.Id === certificationPeriod) {
                        serverModel.CertificationPeriodStart(moment(period.StartDate).toDate());
                        serverModel.CertificationPeriodEnd(moment(period.OriginalEndDate).toDate());
                        serverModel.ExpiryDate(moment(period.EndDate).toDate());
                        break;
                    }
                }

                vm.disableCertificationPeriodEnd(false);
            }

            return vm;
        }
    });