define([
    'plugins/router',
    'modules/enums',
    'services/material-request-data-service',
    'views/material-request/wizard/test-material',
    'views/material-request/wizard/test-material-source',
    'views/material-request/wizard/round-details',
    'views/material-request/wizard/documents-upload',
    'views/material-request/wizard/members',
    'views/material-request/wizard/notes',
    'views/material-request/wizard/send-email-check-option',
    'views/material-request/wizard/email-preview',
    'views/material-request/wizard/existing-documents',
    'views/material-request/wizard/coordinator',
    'views/material-request/wizard/panel',
    'views/material-request/wizard/round-links'
], function (router, enums, materialRequestService, testMaterial, testMaterialSource, roundDetails, documentsUpload, members, notes, sendEmailCheckOption, emailPreview, existingDocuments, coordinator, panel, roundLinks) {
    var materialRequest = {
        MaterialRequestId: ko.observable(0).extend({ required: true }),
        PanelId: ko.observable(),
        SourceTestMaterialId: ko.observable(),
        ProductSpecificationId: ko.observable().extend({ required: true }),
        MaxBillableHours: ko.observable().extend({ required: true, number: true, min: 0.0, max: 1000 }),
    };

    var outputMaterial = {
        Title: ko.observable().extend({ required: true }),
        CredentialTypeId: ko.observable().extend({ required: true }),
        TestComponentTypeId: ko.observable().extend({ required: true }),
        TestMaterialTypeId: ko.observable().extend({ required: true }),
        isSkillRequired: ko.observable(true),
        isLanguageRequired: ko.observable(false),
        TestMaterialDomainId: ko.observable().extend({ required: true }),

    };

    outputMaterial.SkillId = ko.observable().extend({ required: outputMaterial.isSkillRequired });
    outputMaterial.LanguageId = ko.observable().extend({ required: outputMaterial.isLanguageRequired });


    var materialRound = {
        MaterialRoundId: ko.observable(),
        Round: ko.observable(),
        DueDate: ko.observable().extend({ required: true, dateGreaterThan: moment().format('DD/MM/YYYY') })
    };

    var emptyMaterialRequest = ko.toJS(materialRequest);
    var emptyOutputMaterial = ko.toJS(outputMaterial);
    var emptyMaterialRound = ko.toJS(materialRound);

    var vm = {
        materialRequest: materialRequest,
        materialRound: materialRound,
        outputMaterial: outputMaterial,
        action: ko.observable(),
        steps: ko.observableArray(),
        visibleSteps: ko.observableArray(),
        saveAllowed: ko.observable(true)
    };

    var allSteps = {};

    allSteps[enums.MaterialRequestWizardStep.TestMaterial] = {
        id: 'wizardTestMaterial',
        label: ko.Localization('Naati.Resources.MaterialRequest.resources.TestMaterial'),
        model: testMaterial,
        compose: {
            view: 'views/material-request/wizard/test-material',
        },
        contentCss: 'w-full'
    };

    allSteps[enums.MaterialRequestWizardStep.TestMaterialSource] = {
        id: 'wizardTestMaterialSource',
        label: ko.Localization('Naati.Resources.MaterialRequest.resources.TestMaterialSource'),
        model: testMaterialSource,
        compose: {
            view: 'views/material-request/wizard/test-material-source',
        },
        contentCss: 'w-full'
    };

    allSteps[enums.MaterialRequestWizardStep.RoundDetails] = {
        id: 'wizardRoundDetails',
        label: ko.Localization('Naati.Resources.MaterialRequest.resources.RoundDetails'),
        model: roundDetails,
        compose: {
            view: 'views/material-request/wizard/round-details',
        }
    };

    allSteps[enums.MaterialRequestWizardStep.DocumentsUpload] = {
        id: 'wizardDocumentsUpload',
        label: ko.Localization('Naati.Resources.MaterialRequest.resources.DocumentsUpload'),
        model: documentsUpload,
        compose: {
            view: 'views/material-request/wizard/documents-upload',
        }
    };

    allSteps[enums.MaterialRequestWizardStep.Members] = {
        id: 'wizardMembers',
        label: ko.Localization('Naati.Resources.MaterialRequest.resources.Members'),
        model: members,
        compose: {
            view: 'views/material-request/wizard/members',
        },
        contentCss: 'w-full'
    };

    allSteps[enums.MaterialRequestWizardStep.Notes] = {
        id: 'wizardNotes',
        label: ko.Localization('Naati.Resources.MaterialRequest.resources.Notes'),
        model: notes,
        compose: {
            view: 'views/material-request/wizard/notes',
        },
        contentCss: 'w-full'
    };

    allSteps[enums.MaterialRequestWizardStep.SendEmailCheckOption] = {
        id: 'wizardSendEmailCheckOption',
        label: ko.Localization('Naati.Resources.MaterialRequest.resources.SendEmailCheckOption'),
        model: sendEmailCheckOption,
        compose: {
            view: 'views/material-request/wizard/send-email-check-option',
        },
        contentCss: 'w-full'
    };

    allSteps[enums.MaterialRequestWizardStep.EmailPreview] = {
        id: 'wizardEmailPreview',
        label: ko.Localization('Naati.Resources.MaterialRequest.resources.EmailPreview'),
        model: emailPreview,
        compose: {
            view: 'views/material-request/wizard/email-preview'
        },
        contentCss: 'w-full'
    };

    allSteps[enums.MaterialRequestWizardStep.ExistingDocuments] = {
        id: 'wizardExistingDocuments',
        label: ko.Localization('Naati.Resources.MaterialRequest.resources.ExistingDocuments'),
        model: existingDocuments,
        compose: {
            view: 'views/material-request/wizard/existing-documents'
        },
        contentCss: 'w-full'
    };

    allSteps[enums.MaterialRequestWizardStep.Coordinator] = {
        id: 'wizardCoordinator',
        label: ko.Localization('Naati.Resources.MaterialRequest.resources.Coordinator'),
        model: coordinator,
        compose: {
            view: 'views/material-request/wizard/coordinator'
        },
        contentCss: 'w-full'
    };

    allSteps[enums.MaterialRequestWizardStep.Panel] = {
        id: 'wizardPanel',
        label: ko.Localization('Naati.Resources.MaterialRequest.resources.Panel'),
        model: panel,
        compose: {
            view: 'views/material-request/wizard/panel'
        },
        contentCss: 'w-full'
    };

    allSteps[enums.MaterialRequestWizardStep.RoundLinks] = {
        id: 'wizardRoundLinks',
        label: ko.Localization('Naati.Resources.MaterialRequest.resources.RoundLinks'),
        model: roundLinks,
        compose: {
            view: 'views/material-request/wizard/round-links'
        },
        contentCss: 'w-full'
    };

    vm.title = ko.computed(function () {
        if (!vm.action()) {
            return ko.Localization('Naati.Resources.Application.resources.TakeAction');
        }

        return vm.action().Name;
    });

    vm.subtitle = ko.computed(function () {
        return ko.Localization('Naati.Resources.Shared.resources.MaterialRequest');
    });

    vm.canActivate = function (action, panelId, materialRequestId, materialRoundId) {

        var actionName = null;
        for (var a in enums.MaterialRequestWizardActions) {
            if (enums.MaterialRequestWizardActions[a] == action) {
                actionName = ko.Localization('Naati.Resources.MaterialRequest.resources.' + a);
                break;
            }
        }

        vm.action({
            Id: action,
            Name: actionName
        });

        emptyMaterialRequest.PanelId = panelId;
        ko.viewmodel.updateFromModel(materialRequest, emptyMaterialRequest);
        ko.viewmodel.updateFromModel(outputMaterial, emptyOutputMaterial);
        ko.viewmodel.updateFromModel(materialRound, emptyMaterialRound);

        if (!materialRequestId) {
            vm.visibleSteps.removeAll();
            return getSteps();
        }

        return materialRequestService.getFluid(materialRequestId).then(function (data) {

            if (!data) {
                return;
            }

            materialRequest.MaterialRequestId(data.MaterialRequestId);
            materialRequest.ProductSpecificationId(data.ProductSpecificationId);
            materialRequest.PanelId(data.PanelId);
            materialRequest.MaxBillableHours(data.MaxBillableHours);
            materialRequest.SourceTestMaterialId(data.SourceTestMaterial ? data.SourceTestMaterial.Id : 0);
            outputMaterial.Title(data.OutputTestMaterial.Title);
            outputMaterial.CredentialTypeId(data.OutputTestMaterial.CredentialTypeId);
            outputMaterial.TestComponentTypeId(data.OutputTestMaterial.TypeId);
            outputMaterial.SkillId(data.OutputTestMaterial.SkillId);
            outputMaterial.LanguageId(data.OutputTestMaterial.LanguageId);
            outputMaterial.TestMaterialTypeId(data.OutputTestMaterial.TestMaterialTypeId);
            outputMaterial.TestMaterialDomainId(data.OutputTestMaterial.TestMaterialDomainId);

            vm.visibleSteps.removeAll();

            if (!materialRoundId) {
                return getSteps();
            }
            else {
                return materialRequestService.getFluid('round/' + materialRoundId).then(function (data) {
                    ko.viewmodel.updateFromModel(materialRound, data);
                    materialRound.MaterialRoundId(data.MaterialRoundId);
                    materialRound.Round(data.RoundNumber);
                    return getSteps();
                });
            }
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
        var request = {
            MaterialRequestId: vm.materialRequest.MaterialRequestId(),
            MaterialRequestRoundId: vm.materialRound.MaterialRoundId(),
            OutputMaterial: ko.toJS(outputMaterial),
            ActionId: Number(vm.action().Id),
            Steps: []
        };

        var valid = true;

        if (valid) {
            ko.utils.arrayForEach(vm.visibleSteps(),
                function (s) {
                    if (s.compose.model.postData) {
                        request.Steps.push({
                            Id: enums.MaterialRequestWizardStep[s.id.replace('wizard', '')],
                            Data: s.compose.model.postData()
                        });
                    }
                });

            //check if save should be allowed
            if (vm.saveAllowed()) {
                //prevent extra saves from occurring
                vm.saveAllowed(false);
                materialRequestService.post(request, 'wizard').then(function(data) {
                    toastr.success(ko.Localization('Naati.Resources.Application.resources.ActionTakenSuccessfully'));

                    //re enable saves after the save has completed
                    vm.saveAllowed(true);

                    return router.navigate('material-request/' + data.MaterialRequestIds[0]);
                    //router.navigateBack();
                });
            }
        }
    }

    function getSteps() {
        vm.steps.removeAll();

        var promise = vm.materialRound.MaterialRoundId()
            ? materialRequestService.getFluid('roundSteps/' + materialRound.MaterialRoundId() + '/' + vm.action().Id)
            : materialRequestService.getFluid('steps/' + materialRequest.MaterialRequestId() + '/' + vm.action().Id + '/' + vm.materialRequest.PanelId());

        return promise.then(function (data) {
            var steps = [];
            ko.utils.arrayForEach(data, function (stepId) {
                var step = allSteps[stepId];
                var model = step.model;
                var modelParams = {
                    materialRequest: materialRequest,
                    materialRound: materialRound,
                    outputMaterial: outputMaterial,
                    action: vm.action,
                    visibleSteps: vm.visibleSteps,
                    stepId: ko.observable(stepId),
                    wizardService: materialRequestService,
                    nextStepAction: vm.wizardOptions.next
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