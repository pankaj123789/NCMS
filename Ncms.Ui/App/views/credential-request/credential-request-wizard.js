define([
    'plugins/router',
    'modules/enums',
    'services/credentialrequest-data-service',
    'views/credential-request/credential-request-wizard/credential-request-wizard-header',
    'views/credential-request/credential-request-wizard/credential-request-wizard-test-session',
    'views/credential-request/credential-request-wizard/credential-request-wizard-notes',
    'views/credential-request/credential-request-wizard/credential-request-wizard-email',
    'views/credential-request/credential-request-wizard/credential-request-wizard-existing-applicants',
    'views/credential-request/credential-request-wizard/credential-request-wizard-new-applicants',
    'views/shared/wizard-check-confirmation',
    'views/credential-request/credential-request-wizard/credential-request-wizard-invoice',
    'views/credential-request/credential-request-wizard/credential-request-wizard-message'
], function (router, enums, credentialrequestService, wizardHeader, wizardTestSession, wizardNotes, wizardEmail, wizardExistingApplicants, wizardNewApplicants, checkConfirmation, wizardInvoice, wizardMessage) {
    var serverModel = {
        CredentialApplicationTypeId: ko.observable(),
        CredentialTypeId: ko.observable(),
        SkillId: ko.observable(),
        CredentialRequestStatusTypeId: ko.observable(),
        ApplicationTypeId: ko.observable(),
        ApplicationTypeName: ko.observable(),
        CredentialTypeName: ko.observable(),
        Skill: ko.observable(),
        PreferredTestLocation: ko.observable(),
        TestLocationId: ko.observable(),
        Applicants: ko.observableArray(),
        EarliestApplication: ko.observable(),
        LastApplication: ko.observable(),
        Action: ko.observable()
    };

    var testSessionModel = {
        Id: ko.observable(),
        SessionName: ko.observable(),
        VenueId: ko.observable(),
        Notes: ko.observable(),
        TestDate: ko.observable(),
        TestTime: ko.observable(),
        PreparationTime: ko.observable(),
        SessionDuration: ko.observable(),
        VenueCapacity: ko.observable(),
        AllowSelfAssign: ko.observable(),
        Skills: ko.observable()
    };

    var vm = {
        selectedTestSession: testSessionModel,
        summary: serverModel,
        action: ko.observable(),
        steps: ko.observableArray(),
        visibleSteps: ko.observableArray(),
    };

    serverModel.Request = function (includeSteps) {
        var testSessionId = null;
        var venueCapacity = 0;

        var testSessionStep = getStep('wizardTestSession');
        if (testSessionStep) {
            var testSession = testSessionStep.compose.model.session;
            testSessionId = testSession.Id();
            venueCapacity = testSession.Capacity();
        }

        var credentialRequestIds = 0;
        var newApplicantsStep = getStep('wizardNewApplicants');
        if (newApplicantsStep) {
            var newApplicants = newApplicantsStep.compose.model;
            credentialRequestIds = newApplicants.selectedIds();
        }

        var req = {
            CredentialApplicationTypeId: serverModel.CredentialApplicationTypeId(),
            CredentialTypeId: serverModel.CredentialTypeId(),
            SkillId: serverModel.SkillId(),
            CredentialRequestStatusTypeId: serverModel.CredentialRequestStatusTypeId(),
            TestLocationId: serverModel.TestLocationId(),
            Action: serverModel.Action().Id,
            TestSessionId: testSessionId,
            VenueCapacity: venueCapacity,
            CredentialRequestIds: credentialRequestIds,
            Steps: []
        };

        if (includeSteps) {
            ko.utils.arrayForEach(vm.visibleSteps(),
                function (s) {
                    if (s.compose.model.postData) {
                        req.Steps.push({
                            Id: enums.CredentialRequestWizardSteps[s.id.replace('wizard', '')],
                            Data: s.compose.model.postData()
                        });
                    }
                });
        }

        return req;
    };

    var wizardHeaderInstance = wizardHeader.getInstance({ summary: serverModel });

    var allSteps = {};

    allSteps[enums.CredentialRequestWizardSteps.TestSession] = {
        id: 'wizardTestSession',
        contentCss: 'block',
        label: ko.Localization('Naati.Resources.CredentialRequestSummary.resources.TestSession'),
        model: wizardTestSession,
        compose: {
            view: 'views/credential-request/credential-request-wizard/credential-request-wizard-test-session',
        }
    };

    allSteps[enums.CredentialRequestWizardSteps.Notes] = {
        id: 'wizardNotes',
        contentCss: 'block',
        label: ko.Localization('Naati.Resources.CredentialRequestSummary.resources.Notes'),
        model: wizardNotes,
        compose: {
            view: 'views/credential-request/credential-request-wizard/credential-request-wizard-notes',
        }
    };

    allSteps[enums.CredentialRequestWizardSteps.ViewEmailAttachments] = {
        id: 'wizardEmail',
        contentCss: 'block',
        label: ko.Localization('Naati.Resources.CredentialRequestSummary.resources.ViewEmailAttachments'),
        model: wizardEmail,
        compose: {
            view: 'views/credential-request/credential-request-wizard/credential-request-wizard-email',
        }
    };

    allSteps[enums.CredentialRequestWizardSteps.ExistingApplicants] = {
        id: 'wizardExistingApplicants',
        contentCss: 'block',
        label: ko.Localization('Naati.Resources.CredentialRequestSummary.resources.ExistingApplicants'),
        model: wizardExistingApplicants,
        compose: {
            view: 'views/credential-request/credential-request-wizard/credential-request-wizard-existing-applicants',
        }
    };

    allSteps[enums.CredentialRequestWizardSteps.NewApplicants] = {
        id: 'wizardNewApplicants',
        contentCss: 'block',
        label: ko.Localization('Naati.Resources.CredentialRequestSummary.resources.NewApplicants'),
        model: wizardNewApplicants,
        compose: {
            view: 'views/credential-request/credential-request-wizard/credential-request-wizard-new-applicants',
        }
    };

    allSteps[enums.CredentialRequestWizardSteps.CheckOption] = {
        id: 'wizardCheckOption',
        contentCss: 'block',
        label: ko.Localization('Naati.Resources.Application.resources.NotifiyToApplicant'),
        model: checkConfirmation,
        compose: {
            view: 'views/shared/wizard-check-confirmation',
        }
    };

    allSteps[enums.CredentialRequestWizardSteps.ViewInvoice] = {
        id: 'wizardViewInvoice',
        contentCss: 'block',
        label: ko.Localization('Naati.Resources.Application.resources.ViewInvoice'),
        model: wizardInvoice,
        compose: {
            view: 'views/credential-request/credential-request-wizard/credential-request-wizard-invoice'
        }
    };

    allSteps[enums.CredentialRequestWizardSteps.ViewMessage] = {
        id: 'wizardViewMessage',
        contentCss: 'block',
        label: ko.Localization('Naati.Resources.Application.resources.Message'),
        model: wizardMessage,
        compose: {
            view: 'views/credential-request/credential-request-wizard/credential-request-wizard-message'
        }
    };


    vm.title = ko.computed(function () {
        var takeAction = ko.Localization('Naati.Resources.CredentialRequestSummary.resources.TakeAction');

        if (!serverModel.Action()) {
            return takeAction;
        }

        return '{0} - {1}'.format(takeAction, serverModel.Action().Name);
    });

    vm.canActivate = function (credentialApplicationTypeId, credentialTypeId, skillId, credentialRequestStatusTypeId, testLocationId, action, query) {
        serverModel.CredentialApplicationTypeId(credentialApplicationTypeId);
        serverModel.CredentialTypeId(credentialTypeId);
        serverModel.SkillId(skillId);
        serverModel.CredentialRequestStatusTypeId(credentialRequestStatusTypeId);
        serverModel.TestLocationId(testLocationId);
        serverModel.Action(action);

        return credentialrequestService.post(serverModel.Request(), 'summary').then(function (data) {
            ko.viewmodel.updateFromModel(serverModel, data);

            var actionName = null;
            for (var a in enums.ApplicationWizardActions) {
                if (enums.ApplicationWizardActions[a] == action) {
                    actionName = ko.Localization('Naati.Resources.CredentialRequestSummary.resources.' + a + 'ActionLabel');
                    break;
                }
            }

            serverModel.Action({
                Id: action,
                Name: actionName
            });

            vm.visibleSteps.removeAll();
            return getSteps();
        });
    };

    vm.activate = function () {
        if (vm.wizardOptions.component) {
            vm.wizardOptions.component.activate();
        }
    };

    vm.deactivate = function () {
        vm.wizardOptions.component.deactivate();
    };

    vm.wizardOptions = {
        header: {
            model: wizardHeaderInstance,
            view: 'views/credential-request/credential-request-wizard/credential-request-wizard-header'
        },
        steps: vm.visibleSteps,
        showFinish: ko.computed(function () {
            return vm.visibleSteps().length === vm.steps().length;
        }),
        cancel: function () {
            router.navigateBack();
        },
        next: function () {
            var currentStep = vm.visibleSteps()[vm.visibleSteps().length - 1];
            vm.wizardOptions.component.validateStep(currentStep).then(function (result) {
                vm.wizardOptions.component.setStepState(currentStep, result);
                if (result) {
                    gotoNextStep();
                }
            });
        },
        finish: function () {
            // re-validate all steps - maybe something was changed
            var promises = [];
            ko.utils.arrayForEach(vm.visibleSteps(), function (s) {
                promises.push(vm.wizardOptions.component.validateStep(s));
            });

            Q.allSettled(promises).done(function (results) {
                for (var i = 0; i < results.length; i++) {
                    var r = results[i];
                    var step = vm.visibleSteps()[i];
                    vm.wizardOptions.component.setStepState(step, r.value);
                    if (!r.value) {
                        vm.wizardOptions.component.scrollToStep(step);
                        return;
                    }
                }

                saveWizard();
            });
        }
    };

    function getStep(stepId) {
        return ko.utils.arrayFirst(vm.visibleSteps(), function (s) {
            return s.id === stepId;
        });
    }

    function gotoNextStep() {
        vm.visibleSteps()[vm.visibleSteps().length - 1].success(true);
        var newStep = vm.steps()[vm.visibleSteps().length];
        newStep.current(true);
        vm.visibleSteps.push(newStep);
        if (newStep.compose.model.load) {
            newStep.compose.model.load();
        }

        vm.wizardOptions.component.scrollToStep(newStep);
    }

    function saveWizard() {
        var req = serverModel.Request(true);
        var credentialRequestIds = req.CredentialRequestIds;
        saveInBatch(req, credentialRequestIds);
    }

    function saveInBatch(req, credentialRequestIds) {
        if (!credentialRequestIds || !credentialRequestIds.length) {
            toastr.success(ko.Localization('Naati.Resources.Application.resources.ActionTakenSuccessfully'));
            router.navigateBack();
            return;
        }

        var batchSize = 10;
        var length = credentialRequestIds.length;
        req.CredentialRequestIds = [];

        for (var i = 0; i < length && i < batchSize; i++) {
            req.CredentialRequestIds.push(credentialRequestIds.pop());
        }

        credentialrequestService.post(req, 'wizard').then(function () {
            saveInBatch(req, credentialRequestIds);
        });
    }

    function getSteps() {
        vm.steps.removeAll();

        return credentialrequestService.getFluid('steps', serverModel.Request()).then(function (data) {
            var steps = [];
            ko.utils.arrayForEach(data, function (stepId) {
                var step = allSteps[stepId];
                var model = step.model;
                var modelParams = {
                    summary: serverModel,
                    visibleSteps: vm.visibleSteps,
                    selectedTestSession: vm.selectedTestSession,
                    wizardService: credentialrequestService,
                };

                steps.push($.extend(stepId, step, {
                    current: ko.observable(false),
                    success: ko.observable(false),
                    cancel: ko.observable(false),
                    css: 'animated fadeIn',
                    compose: {
                        view: step.compose.view,
                        model: model.getInstance(modelParams)
                    }
                }));
            });

            vm.steps(steps);

            if (!steps.length) {
                return true;
            }

            var newStep = steps[0];
            newStep.current(true);
            vm.visibleSteps.push(newStep);

            if (newStep.compose.model.load) {
                newStep.compose.model.load();
            }

            return true;
        });
    }

    return vm;
});