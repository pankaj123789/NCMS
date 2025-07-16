define([
    'views/shell',
    'services/application-data-service',
    'services/testsession-data-service',
    'modules/enums',
], function (shell, applicationService, testSessionService, enums) {

        var testSession = {
        Id: ko.observable(),
        Name: ko.observable(),
        TestDate: ko.observable(),
        CredentialTypeInternalName: ko.observable(),
        VenueName: ko.observable(),
        VenueAddress: ko.observable(),
        Skills: ko.observable()
    };

    var application = {
        ApplicantFamilyName: ko.observable(),
        ApplicantGivenName: ko.observable(),
        NaatiNumber: ko.observable(),
        ApplicationReference: ko.observable(),
        ApplicationId: ko.observable()
    };

    var serverModel = {
        ApplicationId: ko.observable().extend({ required: true }),
        CredentialRequestId: ko.observable().extend({ required: true }),
        TestSessionId: ko.observable().extend({ required: true, notify: 'always' })
    };

    var vm = {
        title: shell.titleWithSmall,
        serverModel: serverModel,
        credentialRequests: ko.observableArray(),
        testSession: testSession,
        application: application,
        showNoCredentialRequestsAlert: ko.observable(false),
        credentialrequest: ko.observable(),
        candidateName: ko.observable(),
        agreeForCondition: ko.observable(false),
        showDetails: ko.observable(false),
        showPastDateAlert: ko.observable(false),
        showCredentialRequests: ko.observable(false)
    };

    vm.serverModel.CredentialRequestId.subscribe(setCredentialRequestText);

    function setCredentialRequestText(credentialRequestId) {
        if (credentialRequestId) {
            var filteredCredentialRequest = ko.utils.arrayFilter(vm.credentialRequests(), function (item) {
                return item.Id === credentialRequestId;
            });
            if (filteredCredentialRequest) {
                vm.credentialrequest(filteredCredentialRequest[0].CredentialName + " " + filteredCredentialRequest[0].Direction + "(" + filteredCredentialRequest[0].Status + ")");
            }
        }
    }

    vm.applicationValidation = ko.validatedObservable([serverModel.ApplicationId]);
    vm.testSessionValidation = ko.validatedObservable([serverModel.CredentialRequestId, serverModel.TestSessionId]);
    vm.serverModelValidation = ko.validatedObservable([serverModel.CredentialRequestId, serverModel.TestSessionId]);


    function getApplicationId() {
        var applicationId = serverModel.ApplicationId();
        if (applicationId.toUpperCase().startsWith("APP")) {
            applicationId = applicationId.substring(3, applicationId.length);
        }
        return applicationId;
    }

    vm.getCredentialRequest = function () {
        vm.showNoCredentialRequestsAlert(false);
        clearTestSession();
        if (vm.validateApplication()) {
            var applicationId = getApplicationId();
            applicationService.getFluid('{0}/credentialrequests'.format(applicationId)).then(function (data) {
                var statuses = enums.CredentialRequestStatusTypes;

                var filteredCredentialRequests = ko.utils.arrayFilter(data, function (item) {
                    return item.StatusTypeId === statuses.TestAccepted ||
                        item.StatusTypeId === statuses.EligibleForTesting;
                });

                if (filteredCredentialRequests.length > 0) {
                    ko.utils.arrayForEach(filteredCredentialRequests,
                        function (item) {
                            item.DispalyName = item.CredentialName + " " + item.Direction;
                        });
                    vm.credentialRequests(filteredCredentialRequests);
                    vm.showCredentialRequests(true);
                }
                else {
                    vm.credentialRequests([]);
                    vm.showNoCredentialRequestsAlert(true);
                    vm.showCredentialRequests(false);
                }
            });
        }
    };

    function getTetSessionId() {
        var testSessionId = serverModel.TestSessionId();
        if (testSessionId.toUpperCase().startsWith("TS")) {
            testSessionId = testSessionId.substring(2, testSessionId.length);
        }
        return testSessionId;
    }

    vm.getTestSession = function () {
        if (vm.validateTestSession()) {
            var testSessionId = getTetSessionId();
            var applicationId = getApplicationId();
            testSessionService.getFluid(testSessionId + '/topsection').then(function (data) {
                if (data.IsPastTestSession) {
                    vm.showPastDateAlert(false);

                    ko.viewmodel.updateFromModel(testSession, data);
                    vm.testSession.TestDate(moment(data.TestDate).format(CONST.settings.shortDateTimeDisplayFormat));

                    testSessionService.getFluid(testSessionId + '/testSessionSkillDetails').then(function (data) {
                        ko.utils.arrayForEach(data,
                            function (item) {
                                item.DispalyText = item.Name;
                                if (item.MaximumCapacity) {
                                    item.DispalyText = item.DispalyText + "(" + item.MaximumCapacity + ")";
                                }
                            });
                        var text = data.map(e => e.DispalyText).join(", ");
                        vm.testSession.Skills(text);
                    });

                    applicationService.getFluid(applicationId).then(function (data) {
                        ko.viewmodel.updateFromModel(application, data);
                        vm.candidateName(data.ApplicantGivenName + " " + data.ApplicantFamilyName);
                    });

                    vm.showDetails(true);
                }
                else {
                    vm.showPastDateAlert(true);
                    vm.showDetails(false);
                }
            });


        }
    }

    vm.apply = function () {
        var testSessionId = getTetSessionId();
        if (vm.validateServerModel()) {
            var request = {
                CredentialRequestId: serverModel.CredentialRequestId(),
                TestSessionId: testSessionId
            };

            testSessionService.post(request, 'allocatePastTestSession').then(function (result) {
                toastr.success(ko.Localization('Naati.Resources.TestSession.resources.AssignedToSession'));
                clear();
            });
        }
    }

    vm.credentialRequestOptions = {
        value: serverModel.CredentialRequestId,
        multiple: false,
        options: vm.credentialRequests,
        optionsValue: 'Id',
        optionsText: 'DispalyName'
    }

    function clear() {
        vm.showDetails(false);
        vm.showCredentialRequests(false);
        vm.serverModel.ApplicationId(null);
        vm.serverModel.CredentialRequestId(null);
        vm.serverModel.TestSessionId(null);
        vm.credentialRequests([]);
        vm.agreeForCondition();

        clearValidation();
    }

    function clearTestSession() {
        vm.showCredentialRequests(false);
        vm.serverModel.CredentialRequestId(null);
        vm.serverModel.TestSessionId(null);
        vm.showDetails(false);
        vm.credentialRequests([]);
        vm.agreeForCondition();

        clearValidation();
    }

    function clearValidation() {
        vm.applicationValidation.isValid(true);
        vm.applicationValidation.errors.showAllMessages(false);

        vm.testSessionValidation.isValid(true);
        vm.testSessionValidation.errors.showAllMessages(false);

        vm.serverModelValidation.isValid(true);
        vm.serverModelValidation.errors.showAllMessages(false);
    };

    vm.validateApplication = function () {
        var isValid = vm.applicationValidation.isValid();

        vm.applicationValidation.errors.showAllMessages(!isValid);

        return isValid;
    };

    vm.validateTestSession = function () {
        var isValid = vm.testSessionValidation.isValid();

        vm.testSessionValidation.errors.showAllMessages(!isValid);

        return isValid;
    };

    vm.validateServerModel = function () {
        var isValid = vm.serverModelValidation.isValid();

        vm.serverModelValidation.errors.showAllMessages(!isValid);

        return isValid;
    };

    return vm;

});
