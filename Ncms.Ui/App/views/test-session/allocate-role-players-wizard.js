define([
    'plugins/router',
    'modules/enums',
    'services/testsession-data-service',
    'views/test-session/allocate-role-players-wizard/test-session-specifications',
    'views/test-session/allocate-role-players-wizard/test-session-skill',
    'views/test-session/allocate-role-players-wizard/allocate-role-players',
    'views/test-session/allocate-role-players-wizard/preview-email',
    'views/test-session/allocate-role-players-wizard/send-email',
    'services/screen/date-service',
],
    function (router, enums, testSessionService, testSessionSpecifications, testSessionSkills, allocateRolePlayers, previewEmail, sendEmail, dateService) {
        var serverModel = {
            TestSessionId: ko.observable(),
            TestSpecificationId: ko.observable(),
            SkillId: ko.observable(),
            TestDate: ko.observable(),
        };

        serverModel.Request = function () {
            var req = $.extend({}, serverModel);
            delete req.Request;
            return ko.toJS(req);
        };

        var emptyModel = ko.toJS(serverModel);
        var vm = {
            summary: serverModel,
            action: ko.observable(),
            steps: ko.observableArray(),
            visibleSteps: ko.observableArray(),
        };

        var allSteps = {};

        allSteps[enums.AllocateRolePlayersSteps.TestSpecification] = {
            id: 'wizardTestSpecification',
            contentCss: 'block',
            label: ko.Localization('Naati.Resources.TestSession.resources.TestSpecification'),
            model: testSessionSpecifications,
            compose: {
                view: 'views/test-session/allocate-role-players-wizard/test-session-specifications',
            }
        };

        allSteps[enums.AllocateRolePlayersSteps.Skill] = {
            id: 'wizardSkill',
            contentCss: 'block',
            label: ko.Localization('Naati.Resources.TestSession.resources.Skill'),
            model: testSessionSkills,
            compose: {
                view: 'views/test-session/allocate-role-players-wizard/test-session-skill',
            }
        };

        allSteps[enums.AllocateRolePlayersSteps.AllocateRolePlayers] = {
            id: 'wizardAllocateRolePlayers',
            contentCss: 'block',
            label: ko.Localization('Naati.Resources.TestSession.resources.AllocateRolePlayers'),
            model: allocateRolePlayers,
            compose: {
                view: 'views/test-session/allocate-role-players-wizard/allocate-role-players',
            }
        };

        allSteps[enums.AllocateRolePlayersSteps.EmailPreview] = {
            id: 'wizardPreviewEmail',
            contentCss: 'block',
            label: ko.Localization('Naati.Resources.TestSession.resources.PreviewEmail'),
            model: previewEmail,
            compose: {
                view: 'views/test-session/allocate-role-players-wizard/preview-email',
            }
        };

        allSteps[enums.AllocateRolePlayersSteps.SendEmail] = {
            id: 'wizardSendEmail',
            contentCss: 'block',
            label: ko.Localization('Naati.Resources.TestSession.resources.SendEmail'),
            model: sendEmail,
            compose: {
                view: 'views/test-session/allocate-role-players-wizard/send-email',
            }
        };

        vm.title = ko.computed(function () {
            return ko.Localization('Naati.Resources.TestSession.resources.AllocateRolePlayers');
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
            vm.wizardOptions.component.deactivate();
        };

        vm.showPreviewEmail = ko.observable(true);

        vm.wizardOptions = {
            steps: vm.visibleSteps,
            showFinish: ko.computed(function () {
                return vm.visibleSteps().length === (vm.steps().length - (!vm.showPreviewEmail() ? 1 : 0));
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

        function load(testSessionId) {
            serverModel.TestSessionId(testSessionId);
            return getSteps();
        }

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

        function scrollStep(stepId) {
            var step = getStep(stepId);
            vm.wizardOptions.component.scrollToStep(step);
        }

        function request(includeSteps) {
            var req = ko.toJS(serverModel);
            req.TestDate = req.TestDate ? dateService.toPostDate(req.TestDate) : null;
            req.Steps = [];

            if (includeSteps) {

                ko.utils.arrayForEach(vm.visibleSteps(),
                    function (s) {
                        if (s.compose.model.postData) {
                            var step = {
                                Id: enums.AllocateRolePlayersSteps[s.id.replace('wizard', '')],
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
            testSessionService.post(req, 'allocateroleplayerswizard').then(function () {
                toastr.success(ko.Localization('Naati.Resources.Application.resources.ActionTakenSuccessfully'));
                router.navigate('test-session/' + vm.summary.TestSessionId());
            });
        }

        function getSteps() {
            vm.visibleSteps.removeAll();
            vm.steps.removeAll();

            return testSessionService.getFluid('allocateroleplayerswizard/steps', { TestSessionId: vm.summary.TestSessionId() }).then(function (data) {
                var steps = [];
                ko.utils.arrayForEach(data, function (stepId) {
                    var step = allSteps[stepId];
                    var model = step.model;
                    var modelParams = {
                        summary: serverModel,
                        request: request,
                        visibleSteps: vm.visibleSteps,
                        scrollStep: scrollStep,
                        getStep: getStep,
                        wizardService: testSessionService
                    };

                    var instance = model.getInstance(modelParams);

                    steps.push($.extend(stepId, step, {
                        current: ko.observable(false),
                        success: ko.observable(false),
                        cancel: ko.observable(false),
                        css: 'animated fadeIn',
                        compose: {
                            view: step.compose.view,
                            model: instance
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