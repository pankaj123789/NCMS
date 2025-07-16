define([
	'plugins/router',
	'modules/enums',
	'services/test-material-data-service',
	'views/test-material/test-material-wizard/test-material-wizard-specifications',
	'views/test-material/test-material-wizard/test-material-wizard-skills',
	'views/test-material/test-material-wizard/test-material-wizard-materials',
	'views/test-material/test-material-wizard/test-material-wizard-applicants',
	'views/test-material/test-material-wizard/test-material-wizard-supplementary-tests',
	'views/test-material/test-material-wizard/test-material-wizard-examiners'
],
    function (router, enums, testMaterialService, specifications, skills, materials, applicants, supplementaryTestApplicants, examiners) {
		var serverModel = {
			TestSessionIds: ko.observableArray(),
			TestSpecificationId: ko.observable(),
			SkillId: ko.observable(),
			TestMaterialAssignments: ko.observableArray(),
			ShowIncludedValue: ko.observable(),
		};

		var emptyModel = ko.toJS(serverModel);
		var vm = {
			summary: serverModel,
			action: ko.observable(),
			steps: ko.observableArray(),
			visibleSteps: ko.observableArray(),
		};

		serverModel.Request = function () {
			var req = $.extend({}, serverModel);
			delete req.Request;
			return ko.toJS(req);
		};

		var allSteps = {};

		allSteps[enums.TestMaterialWizardSteps.SupplementaryTestApplicants] = {
			id: 'wizardSupplementaryTestApplicants',
			contentCss: 'block',
			label: ko.Localization('Naati.Resources.TestMaterial.resources.SupplementaryTestApplicants'),
			model: supplementaryTestApplicants,
			compose: {
				view: 'views/test-material/test-material-wizard/test-material-wizard-supplementary-tests',
			}
		};

		allSteps[enums.TestMaterialWizardSteps.TestSpecification] = {
			id: 'wizardTestSpecification',
			contentCss: 'block',
			label: ko.Localization('Naati.Resources.TestMaterial.resources.TestSpecificationSelection'),
			model: specifications,
			compose: {
				view: 'views/test-material/test-material-wizard/test-material-wizard-specifications',
			}
		};

		allSteps[enums.TestMaterialWizardSteps.Skills] = {
			id: 'wizardSkills',
			contentCss: 'block',
			label: ko.Localization('Naati.Resources.TestMaterial.resources.SkillSelection'),
			model: skills,
			compose: {
				view: 'views/test-material/test-material-wizard/test-material-wizard-skills',
			}
		};

		allSteps[enums.TestMaterialWizardSteps.TestMaterials] = {
			id: 'wizardTestMaterials',
			contentCss: 'block',
			label: ko.Localization('Naati.Resources.TestMaterial.resources.TestMaterials'),
			model: materials,
			compose: {
				view: 'views/test-material/test-material-wizard/test-material-wizard-materials',
			}
        };

        allSteps[enums.TestMaterialWizardSteps.Applicants] = {
            id: 'wizardApplicants',
            contentCss: 'block',
            label: ko.Localization('Naati.Resources.TestMaterial.resources.Applicants'),
            model: applicants,
            compose: {
                view: 'views/test-material/test-material-wizard/test-material-wizard-applicants',
            }
        };

        allSteps[enums.TestMaterialWizardSteps.ExaminersAndRolePlayers] = {
            id: 'wizardExaminersAndRolePlayers',
            contentCss: 'block',
            label: ko.Localization('Naati.Resources.TestMaterial.resources.ExaminersAndRolePlayers'),
            model: examiners,
            compose: {
                view: 'views/test-material/test-material-wizard/test-material-wizard-examiners',
            }
        };

		vm.canActivate = function (guid) {
            var testSessionIds = Cookies.getJSON(guid);
	

			if (!testSessionIds || !testSessionIds.length) {
				return router.navigate('test-session');
			}

			ko.viewmodel.updateFromModel(serverModel, emptyModel);

			if (testSessionIds) {
				serverModel.TestSessionIds(testSessionIds);
				return getSteps();
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

		function scrollStep(stepId) {
			var step = getStep(stepId);
			vm.wizardOptions.component.scrollToStep(step);
		}

		function request(includeSteps) {
			var req = ko.toJS(serverModel);
			req.Steps = [];

			if (includeSteps) {

				ko.utils.arrayForEach(vm.visibleSteps(),
					function (s) {
						if (s.compose.model.postData) {
							var step = {
								Id: enums.TestMaterialWizardSteps[s.id.replace('wizard', '')],
								Data: s.compose.model.postData()
							};

							req.Steps.push(step);
						}
					});
			}

			return req;
		}

		function saveWizard() {
			testMaterialService.post(serverModel.Request(), 'wizard').then(function () {
				toastr.success(ko.Localization('Naati.Resources.Application.resources.ActionTakenSuccessfully'));
				router.navigateBack();
			});
		}

		function getSteps() {
			vm.visibleSteps.removeAll();
			vm.steps.removeAll();

            return testMaterialService.post(serverModel.Request(),'getSteps').then(function (data) {
				var steps = [];
				ko.utils.arrayForEach(data, function (stepId) {
					var step = allSteps[stepId];
					var model = step.model;
					var modelParams = {
						summary: serverModel,
						request: request,
						visibleSteps: vm.visibleSteps,
						scrollStep: scrollStep,
						getStep: getStep
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