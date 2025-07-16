define([
        'plugins/router',
        'modules/enums',
        'services/testsession-data-service',
        'views/test-session/test-session-wizard/test-session-wizard-details',
        'views/test-session/test-session-wizard/test-session-wizard-skills',
        'views/test-session/test-session-wizard/test-session-wizard-matching-applicants',
        'views/test-session/test-session-wizard/test-session-wizard-notes',
        'views/test-session/test-session-wizard/test-session-wizard-email',
        'views/test-session/test-session-wizard/test-session-wizard-check-confirmation',
        'services/screen/date-service',
        'views/test-session/test-session-wizard/test-session-wizard-rehearsal-details',
    ],
    function (router, enums, testSessionService, details, skills, matchingApplicants, notes, email, checkConfirmation, dateService, rehearsalDetails) {
        var serverModel = {
            Id: ko.observable(),
            Name: ko.observable(),
            TestLocationId: ko.observable(),
            VenueId: ko.observable(),
            Capacity: ko.observable(),
            ApplicationTypeId: ko.observable(),
            CredentialTypeId: ko.observable(),
            AllowSelfAssign: ko.observable(false),
            NewCandidatesOnly: ko.observable(false),
            OverrideVenueCapacity: ko.observable(false),
            IsActive: ko.observable(false),
            TestTime: ko.observable(),
            TestDate: ko.observable(),
            PreparationTime: ko.observable().extend({
                maxLength: 3,
                number: true
            }),
            SessionDuration: ko.observable().extend({
                maxLength: 3,
                number: true
            }),
            PublicNote: ko.observable(),
            RehearsalDate: ko.observable(),
            RehearsalTime: ko.observable(),
            RehearsalNotes: ko.observable(),
            DefaultTestSpecificationId: ko.observable()
        };



        var emptyModel = ko.toJS(serverModel);
        var vm = {
            session: serverModel,
            action: ko.observable(),
            steps: ko.observableArray(),
            visibleSteps: ko.observableArray(),
        };

        var allSteps = {};

        allSteps[enums.TestSessionWizardSteps.Details] = {
            id: 'wizardDetails',
            contentCss: 'block',
            label: ko.Localization('Naati.Resources.TestSession.resources.Details'),
            model: details,
            compose: {
                view: 'views/test-session/test-session-wizard/test-session-wizard-details',
            }
        };

        allSteps[enums.TestSessionWizardSteps.Skills] = {
            id: 'wizardSkills',
            contentCss: 'block',
            label: ko.Localization('Naati.Resources.TestSession.resources.Skills'),
            model: skills,
            compose: {
                view: 'views/test-session/test-session-wizard/test-session-wizard-skills',
            }
        };

        allSteps[enums.TestSessionWizardSteps.MatchingApplicants] = {
            id: 'wizardMatchingApplicants',
            contentCss: 'block',
            label: ko.Localization('Naati.Resources.TestSession.resources.MatchingApplicants'),
            model: matchingApplicants,
            compose: {
                view: 'views/test-session/test-session-wizard/test-session-wizard-matching-applicants',
            }
        };

        allSteps[enums.TestSessionWizardSteps.Notes] = {
            id: 'wizardNotes',
            contentCss: 'block',
            label: ko.Localization('Naati.Resources.TestSession.resources.Notes'),
            model: notes,
            compose: {
                view: 'views/test-session/test-session-wizard/test-session-wizard-notes',
            }
        };

        allSteps[enums.TestSessionWizardSteps.PreviewEmail] = {
            id: 'wizardPreviewEmail',
            contentCss: 'block',
            label: ko.Localization('Naati.Resources.TestSession.resources.PreviewEmail'),
            model: email,
            compose: {
                view: 'views/test-session/test-session-wizard/test-session-wizard-email',
            }
        };

        allSteps[enums.TestSessionWizardSteps.CheckOption] = {
            id: 'wizardCheckOption',
            contentCss: 'block',
            label: ko.Localization('Naati.Resources.TestSession.resources.NotifiyTestSessionDetails'),
            model: checkConfirmation,
            compose: {
                view: 'views/test-session/test-session-wizard/test-session-wizard-check-confirmation',
            }
        };
        allSteps[enums.TestSessionWizardSteps.RehearsalDetails] = {
            id: 'wizardRehearsalDetails',
            contentCss: 'block',
            label: ko.Localization('Naati.Resources.TestSession.resources.RehearsalDetails'),
            model: rehearsalDetails,
            compose: {
                view: 'views/test-session/test-session-wizard/test-session-wizard-rehearsal-details',
            }
        };


        vm.title = ko.computed(function () {
            if (serverModel.Id()) {
                return ko.Localization('Naati.Resources.TestSession.resources.EditTestSession');
            }
            return ko.Localization('Naati.Resources.TestSession.resources.CreateTestSession');
        });

        vm.canActivate = function (testSessionId) {
            ko.viewmodel.updateFromModel(serverModel, emptyModel);

            if (testSessionId) {
                return load(testSessionId);
            }

            return getSteps();
        };

        vm.activate = function () {
            if (vm.wizardOptions.component) {
                vm.wizardOptions.component.activate();
            }
        };

        vm.deactivate = function () {
            vm.visibleSteps.removeAll();
            vm.steps.removeAll();
            vm.wizardOptions.component.deactivate();
        };

        vm.wizardOptions = {
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
                    if (!s.Skipped) {
                        promises.push(vm.wizardOptions.component.validateStep(s));
                    }
                   
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

        function load(testSessionId) {
            return testSessionService.getFluid(testSessionId + '/topsection').then(function (data) {
                data.TestDate = moment(data.TestDate).toDate();
                if (data.RehearsalDate) {
                    data.RehearsalDate = moment(data.RehearsalDate).toDate();
                }
               
                ko.viewmodel.updateFromModel(serverModel, data);
                return getSteps();
            });
        }

        function getStep(stepId) {
            return ko.utils.arrayFirst(vm.visibleSteps(), function (s) {
                return s.id === stepId;
            });
        }

        function gotoNextStep() {
            if (vm.wizardOptions.showFinish()) {
                return;
            }
            vm.visibleSteps()[vm.visibleSteps().length - 1].success(true);
            var newStep = vm.steps()[vm.visibleSteps().length];
            vm.visibleSteps.push(newStep);
            canShowStep(newStep.id).then(function(value) {
                if (value) {
                    newStep.current(true);
                    if (newStep.compose.model.load) {
                        newStep.compose.model.load();
                    }

                    vm.wizardOptions.component.scrollToStep(newStep);
                } else {
                    newStep.Skipped = true;
                    gotoNextStep();
                }
            });
            
        }

        function scrollStep(stepId) {
            var step = getStep(stepId);
            vm.wizardOptions.component.scrollToStep(step);
        }

        function request(includeSteps) {
            var req = ko.toJS(serverModel);
            req.TestDate = req.TestDate ? dateService.toPostDate(req.TestDate) : null;
            req.RehearsalDate = req.RehearsalDate ? dateService.toPostDate(req.RehearsalDate) : null;
            req.Steps = [];

            if (includeSteps) {

                ko.utils.arrayForEach(vm.visibleSteps(),
                    function (s) {
                        if (s.compose.model.postData) {
                            var step = {
                                Id: enums.TestSessionWizardSteps[s.id.replace('wizard', '')],
                                Data: s.compose.model.postData()
                            };

                            req.Steps.push(step);
                        }
                    });
            }

            return req;
        }

        function saveWizard() {

            var req = request(true);
            testSessionService.post(req, 'wizard').then(function () {
                toastr.success(ko.Localization('Naati.Resources.Application.resources.ActionTakenSuccessfully'));
                router.navigateBack();
            });
        }

        function canShowStep(stepId) {
            var defer = Q.defer();
            testSessionService.getFluid('wizard/canShowStep', { StepId: enums.TestSessionWizardSteps[stepId.replace('wizard', '')],  TestSessionId: vm.session.Id(), CredentialTypeId :vm.session.CredentialTypeId() }).then(function(data) {
                defer.resolve(data);
            });

            return defer.promise;
        }

        function getSteps() {
            vm.visibleSteps.removeAll();
            vm.steps.removeAll();

            return testSessionService.getFluid('wizard/steps', { TestSessionId: vm.session.Id() }).then(function (data) {
                var steps = [];
                ko.utils.arrayForEach(data, function (stepId) {
                    var step = allSteps[stepId];
                    var model = step.model;
                    var modelParams = {
                        session: serverModel,
                        request: request,
                        visibleSteps: vm.visibleSteps,
                        scrollStep: scrollStep,
                        getStep: getStep,
                        wizardService: testSessionService
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