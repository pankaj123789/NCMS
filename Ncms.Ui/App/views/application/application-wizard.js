define([
    'plugins/router',
    'modules/enums',
    'services/application-data-service',
    'views/application/application-wizard-header',
    'views/application/application-wizard/application-wizard-notes',
    'views/application/application-wizard/application-wizard-invoice',
    'views/application/application-wizard/application-wizard-issue-credential',
    'views/application/application-wizard/application-wizard-documents',
    'views/application/application-wizard/application-wizard-email-preview',
    'views/application/application-wizard/application-wizard-select-credential',
    'views/application/application-wizard/application-wizard-select-credential-recertification',
    'views/application/application-wizard/application-wizard-confirmation',
    'views/application/application-wizard/application-wizard-supplementary-test',
    'views/application/application-wizard/application-wizard-request-refund',
    'views/shared/wizard-check-confirmation',
    'views/application/application-wizard/application-wizard-message',
    'views/application/application-wizard/application-wizard-issue-credential-prerequisite-summary',
    'views/application/application-wizard/application-wizard-issue-credential-incomplete-prerequisite',
    'views/application/application-wizard/application-wizard-prerequisite-applications',
    'views/application/application-wizard/application-wizard-prerequisite-mandatory-fields',
    'views/application/application-wizard/application-wizard-prerequisite-mandatory-document-types',
    'views/application/application-wizard/application-wizard-prerequisite-confirm-application-creation',
    'views/application/application-wizard/application-wizard-no-need-to-continue',
    'views/application/application-wizard/application-wizard-prerequisite-exemptions',
    'views/application/application-wizard/application-wizard-prerequisites-issue-on-hold-credentials',
    'views/application/application-wizard/application-wizard-email-compose',


], function (router, enums, applicationService, wizardHeader, wizardNotes, wizardInvoice, wizardIssueCredential, wizardDocuments,
        wizardEmailPreview, wizardSelectCredential, wizardSelectCredentialRecertification, wizardConfirmation, wizardSupplementaryTest, wizardRequestRefund,
        checkConfirmation, wizardMessage, prerequisiteSummary, incompletePrerequisite, prerequisiteApplications, prerequisiteMandatoryFields,
        prerequisiteMandatoryDocumentTypes, prerequisiteConfirmCreation, noNeedToContinue, prerequisiteExemptions, prerequisiteIssueOnHoldCredentials,
        wizardEmailCompose) {
    var serverModel = {
        ApplicationId: ko.observable(),
        ApplicationTypeId: ko.observable(),
        ApplicationTypeName: ko.observable(),
        ApplicationOwner: ko.observable(),
        ApplicationStatus: ko.observable(),
        ApplicationStatusTypeId: ko.observable(),
        StatusModified: ko.observable(),
        StatusModifiedBy: ko.observable(),
        NaatiNumber: ko.observable(),
        ApplicantGivenName: ko.observable(),
        ApplicantFamilyName: ko.observable(),
        ApplicantPrimaryEmail: ko.observable(),
        ApplicationReference: ko.observable(),
        CredentialApplicationTypeCategoryId: ko.observable(),
    };

    var credentialRequest = {
        Id: ko.observable(),
        CredentialName: ko.observable(),
        CredentialType: {
            Certification: ko.observable(),
            CredentialApplicationTypeCredentialTypes: ko.observableArray(),
            DefaultExpiry: ko.observable(),
            ExternalName: ko.observable(),
            Id: ko.observable(),
            InternalName: ko.observable(),
            Simultaneous: ko.observable(),
            DisplayOrder: ko.observable(),
        },
        Direction: ko.observable(),
        StatusChangeDate: ko.observable(),
        StatusChangeUserId: ko.observable(),
        ModifiedBy: ko.observable()
    };

    var cleanCredentialRequest = ko.toJS(credentialRequest);

    //format for preRequisiteModel.selectedApplications
    //variables set in mandatory fields js
    var credentialPrerequisiteRequest = {
        ParentApplicationId: 0,
        ParentCredentialRequestId: 0,
        CredentialRequestTypes: [],
        CreateApplications: false
    };

    var preRequisiteModel = {
        personId: ko.observable(),
        selectedApplications: ko.observableArray(),
        needsNewApplication: ko.observable(),
        hasValidationError: ko.observable()
    };


    var vm = {
        application: serverModel,
        preRequisiteSession: preRequisiteModel,
        credentialPrerequisiteRequest: credentialPrerequisiteRequest,
        credentialRequest: credentialRequest,
        action: ko.observable(),
        steps: ko.observableArray(),
        visibleSteps: ko.observableArray()
    };

    var wizardHeaderInstance = wizardHeader.getInstance({ application: serverModel, credentialRequest: credentialRequest });

    var allSteps = {};

    allSteps[enums.ApplicationWizardSteps.Notes] = {
        id: 'wizardNotes',
        contentCss: 'block',
        label: ko.Localization('Naati.Resources.Application.resources.Notes'),
        model: wizardNotes,
        compose: {
            view: 'views/application/application-wizard/application-wizard-notes',
        }
    };
    allSteps[enums.ApplicationWizardSteps.ViewInvoice] = {
        id: 'wizardViewInvoice',
        contentCss: 'block',
        label: ko.Localization('Naati.Resources.Application.resources.ViewInvoice'),
        model: wizardInvoice,
        compose: {
            view: 'views/application/application-wizard/application-wizard-invoice',
        }
    };
    allSteps[enums.ApplicationWizardSteps.DocumentsPreview] = {
        id: 'wizardDocuments',
        contentCss: 'block',
        label: ko.Localization('Naati.Resources.Application.resources.PreviewDocuments'),
        model: wizardDocuments,
        compose: {
            view: 'views/application/application-wizard/application-wizard-documents',
        }
    };
    allSteps[enums.ApplicationWizardSteps.IssueCredential] = {
        id: 'wizardIssueCredential',
        label: ko.Localization('Naati.Resources.Application.resources.IssueCredentialActionLabel'),
        model: wizardIssueCredential,
        compose: {
            view: 'views/application/application-wizard/application-wizard-issue-credential',
        }
    };
    allSteps[enums.ApplicationWizardSteps.EmailPreview] = {
        id: 'wizardEmailPreviewAndAttachments',
        contentCss: 'block',
        label: ko.Localization('Naati.Resources.Application.resources.PreviewEmail'),
        model: wizardEmailPreview,
        regenerateIconVisible: true,
        compose: {
            view: 'views/application/application-wizard/application-wizard-email-preview',
        }
    };
    allSteps[enums.ApplicationWizardSteps.SelectCredential] = {
        id: 'wizardSelectCredential',
        label: ko.Localization('Naati.Resources.Application.resources.SelectCredential'),
        model: wizardSelectCredential,
        compose: {
            view: 'views/application/application-wizard/application-wizard-select-credential',
        }
    };
    allSteps[enums.ApplicationWizardSteps.DeleteConfirmation] = {
        id: 'wizardDeleteConfirmation',
        label: ko.Localization('Naati.Resources.Application.resources.DeleteConfirmation'),
        model: wizardConfirmation,
        compose: {
            view: 'views/application/application-wizard/application-wizard-confirmation',
        }
    };
    allSteps[enums.ApplicationWizardSteps.NotFoundConcededCredential] = {
        id: 'wizardNotFoundConcededCredential',
        label: ko.Localization('Naati.Resources.Application.resources.IssueConcededPass'),
        model: wizardConfirmation,
        compose: {
            view: 'views/application/application-wizard/application-wizard-confirmation',
        }
    };
    allSteps[enums.ApplicationWizardSteps.ExistingConcededCredential] = {
        id: 'wizardExistingConcededCredential',
        label: ko.Localization('Naati.Resources.Application.resources.IssueConcededPass'),
        model: wizardConfirmation,
        compose: {
            view: 'views/application/application-wizard/application-wizard-confirmation',
        }
    };

    allSteps[enums.ApplicationWizardSteps.IssueConcededPass] = {
        id: 'wizardIssueConcededPass',
        label: ko.Localization('Naati.Resources.Application.resources.IssueConcededPass'),
        model: wizardConfirmation,
        compose: {
            view: 'views/application/application-wizard/application-wizard-confirmation',
        }
    };
    allSteps[enums.ApplicationWizardSteps.SupplementaryTest] = {
        id: 'wizardSupplementaryTest',
        label: ko.Localization('Naati.Resources.Application.resources.SupplementaryTest'),
        model: wizardSupplementaryTest,
        compose: {
            view: 'views/application/application-wizard/application-wizard-supplementary-test',
        }
    };
    allSteps[enums.ApplicationWizardSteps.CheckOption] = {
        id: 'wizardCheckOption',
        label: ko.Localization('Naati.Resources.Application.resources.NotifiyToApplicant'),
        model: checkConfirmation,
        compose: {
            view: 'views/shared/wizard-check-confirmation',
        }
    };

    allSteps[enums.ApplicationWizardSteps.ConfigureRefund] = {
        id: 'wizardConfigureRefund',
        label: ko.Localization('Naati.Resources.Application.resources.RequestRefund'),
        model: wizardRequestRefund,
        compose: {
            view: 'views/application/application-wizard/application-wizard-request-refund',
        }
    };

    allSteps[enums.ApplicationWizardSteps.ApproveRefund] = {
        id: 'wizardApproveRefund',
        label: ko.Localization('Naati.Resources.Application.resources.ApproveRefundLabel'),
        model: wizardRequestRefund,
        compose: {
            view: 'views/application/application-wizard/application-wizard-request-refund',
        }
    };

    allSteps[enums.ApplicationWizardSteps.ViewMessage] = {
        id: 'wizardViewMessage',
        label: ko.Localization('Naati.Resources.Application.resources.Message'),
        model: wizardMessage,
        compose: {
            view: 'views/application/application-wizard/application-wizard-message',
        }
    };

    allSteps[enums.ApplicationWizardSteps.PrerequisiteSummary] = {
        id: 'wizardPrerequisiteSummary',
        label: ko.Localization('Naati.Resources.Application.resources.PrerequisiteSummary'),
        model: prerequisiteSummary,
        compose: {
            view: 'views/application/application-wizard/application-wizard-issue-credential-prerequisite-summary',
        }
     };

    allSteps[enums.ApplicationWizardSteps.IncompletePrerequisiteCredentials] = {
        id: 'wizardIncompletePrerequisite',
        label: ko.Localization('Naati.Resources.Application.resources.IncompletePrerequisite'),
        model: incompletePrerequisite,
        compose: {
            view: 'views/application/application-wizard/application-wizard-issue-credential-incomplete-prerequisite',
        }
    };

    allSteps[enums.ApplicationWizardSteps.PrerequisiteApplications ] = {
        id: 'wizardPrerequisiteApplications',
        label: ko.Localization('Naati.Resources.Application.resources.PrerequisiteApplications'),
        model: prerequisiteApplications,
        compose: {
            view: 'views/application/application-wizard/application-wizard-prerequisite-applications',
        }
    };

    allSteps[enums.ApplicationWizardSteps.PrerequisiteMandatoryFields ] = {
        id: 'wizardPrerequisiteMandatoryFields',
        label: ko.Localization('Naati.Resources.Application.resources.PrerequisiteMandatoryFields'),
        model: prerequisiteMandatoryFields,
        compose: {
            view: 'views/application/application-wizard/application-wizard-prerequisite-mandatory-fields',
        }
    };

    allSteps[enums.ApplicationWizardSteps.PrerequisiteMandatoryDocumentTypes ] = {
        id: 'wizardPrerequisiteMandatoryDocumentTypes',
        label: ko.Localization('Naati.Resources.Application.resources.PrerequisiteMandatoryDocumentTypes'),
        model: prerequisiteMandatoryDocumentTypes,
        compose: {
            view: 'views/application/application-wizard/application-wizard-prerequisite-mandatory-document-types',
        }
    };

    allSteps[enums.ApplicationWizardSteps.PrerequisiteConfirmApplicationCreation ] = {
        id: 'wizardPrerequisiteConfirmApplicationCreation',
        label: ko.Localization('Naati.Resources.Application.resources.PrerequisiteConfirmApplicationCreation'),
        model: prerequisiteConfirmCreation,
        compose: {
            view: 'views/application/application-wizard/application-wizard-prerequisite-confirm-application-creation',
        }
    };

    allSteps[enums.ApplicationWizardSteps.NoNeedToContinue] = {
        id: 'wizardNoNeedToContinue',
        label: '',
        model: noNeedToContinue,
        compose: {
            view: 'views/application/application-wizard/application-wizard-no-need-to-continue',
        }
    };

    allSteps[enums.ApplicationWizardSteps.PrerequisiteExemptions] = {
        id: 'wizardPrerequisiteExemptions',
        label: ko.Localization('Naati.Resources.Application.resources.PrerequisiteExemptions'),
        model: prerequisiteExemptions,
        compose: {
            view: 'views/application/application-wizard/application-wizard-prerequisite-exemptions'
        }
    };

    allSteps[enums.ApplicationWizardSteps.PrerequisiteIssueOnHoldCredentials] = {
        id: 'wizardPrerequisiteIssueOnHoldCredentials',
        label: ko.Localization('Naati.Resources.Application.resources.IssueOnHoldCredentialsLabel'),
        model: prerequisiteIssueOnHoldCredentials,
        compose: {
            view: 'views/application/application-wizard/application-wizard-prerequisites-issue-on-hold-credentials',
        }
    }

    allSteps[enums.ApplicationWizardSteps.ComposeEmail] = {
        id: 'wizardComposeEmail',
        contentCss: 'block',
        label: ko.Localization('Naati.Resources.Application.resources.ComposeEmail'),
        model: wizardEmailCompose,
        regenerateIconVisible: false,
        keepEditable: true,
        compose: {
            view: 'views/application/application-wizard/application-wizard-email-compose',
        }
    };


    vm.title = ko.computed(function () {
        if (!vm.action()) {
            return ko.Localization('Naati.Resources.Application.resources.TakeAction');
        }

        return vm.action().Name;
    });

    vm.subtitle = ko.computed(function () {
        return '{0} {1} - {2} {3} - {4}'.format(
            serverModel.ApplicationTypeName(),
            ko.Localization('Naati.Resources.Shared.resources.Application'),
            serverModel.ApplicantGivenName(),
            serverModel.ApplicantFamilyName(),
            serverModel.NaatiNumber()
        );
    });

    vm.canActivate = function (applicationId, action, credentialRequestId, query) {
        return applicationService.getFluid(applicationId).then(function (data) {
            ko.viewmodel.updateFromModel(serverModel, data);

            updateSteps();

            var actionName = null;
            for (var a in enums.ApplicationWizardActions) {
                if (enums.ApplicationWizardActions[a] == action) {
                    actionName = ko.Localization('Naati.Resources.Application.resources.' + a + 'ActionLabel');
                    break;
                }
            }

            vm.action({
                Id: action,
                Name: actionName
            });

            vm.visibleSteps.removeAll();

            if (!credentialRequestId) {
                ko.viewmodel.updateFromModel(credentialRequest, cleanCredentialRequest);
                return getSteps();
            }
            else {
                return applicationService.getFluid('credentialrequest/' + credentialRequestId).then(function (data) {
                    ko.viewmodel.updateFromModel(credentialRequest, data);
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
        header: {
            model: wizardHeaderInstance,
            view: 'views/application/application-wizard-header'
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

    function updateSteps() {
        if (serverModel.CredentialApplicationTypeCategoryId() === enums.CredentialApplicationTypeCategory.Recertification) {
            allSteps[enums.ApplicationWizardSteps.SelectCredential] = {
                id: 'wizardSelectCredential',
                label: ko.Localization('Naati.Resources.Application.resources.SelectCredential'),
                model: wizardSelectCredentialRecertification,
                compose: {
                    view: 'views/application/application-wizard/application-wizard-select-credential-recertification',
                }
            };
        }
    }

    function gotoNextStep() {
        vm.visibleSteps()[vm.visibleSteps().length - 1].success(true);
        var newStep = determineNextStep();
        newStep.current(true);
        vm.visibleSteps.push(newStep);
        if (newStep.compose.model.load) {
            newStep.compose.model.load();
        }

        vm.wizardOptions.component.scrollToStep(newStep);
    }

    function determineNextStep() {
        var currentStep = vm.visibleSteps()[vm.visibleSteps().length - 1];

        if (currentStep.compose.model.alterSteps) {
            var alteredSteps = currentStep.compose.model.alterSteps(vm.steps, allSteps, currentStep);

            vm.steps(alteredSteps());
        }

        return vm.steps()[vm.visibleSteps().length];
    }



    function saveWizard() {

        var request = {
            ApplicationId: vm.application.ApplicationId(),
            CredentialRequestId: vm.credentialRequest.Id(),
            ActionId: Number(vm.action().Id),
            Steps: []
        };

        //need to change action if issuing a credential and a prequesite is involved.
        if (vm.visibleSteps().find(x => x.id == 'wizardIncompletePrerequisite'))
        {
            if (vm.visibleSteps().find(x => x.id == 'wizardIncompletePrerequisite').compose.model.option.Checked())
            {
                request.ActionId = 1010 //Issue Credential
            }
            else {
                request.ActionId = 1062 //CertificationOnHold
            }
        }

        var valid = true;
        //ko.utils.arrayForEach(vm.visibleSteps(),
        //    function(s) {
        //        valid &= s.model.isValid();
        //    });

        if (valid) {
            ko.utils.arrayForEach(vm.visibleSteps(),
                function (s) {
                    if (s.compose.model.postData) {
                        request.Steps.push({
                            Id: enums.ApplicationWizardSteps[s.id.replace('wizard', '')],
                            Data: s.compose.model.postData()
                        });
                    }
                }
            );

            applicationService.post(request, 'wizard').then(function () {
                toastr.success(ko.Localization('Naati.Resources.Application.resources.ActionTakenSuccessfully'));
                router.navigateBack();
            });
        }
    }

    function getSteps() {
        vm.steps.removeAll();

        var request = {
            ApplicationId: serverModel.ApplicationId(),
            ApplicationStatusId: serverModel.ApplicationStatusTypeId(),
            ActionId: vm.action().Id,
            CredentialTypeId: credentialRequest.CredentialType.Id() || 0,
            ApplicationTypeId: serverModel.ApplicationTypeId(),
            CredentialRequestId: credentialRequest.Id() || 0,
        };

        return applicationService.getFluid('steps', request).then(function (data) {
            var steps = [];
            ko.utils.arrayForEach(data, function (stepId) {
                var step = allSteps[stepId];
                var model = step.model;
                var modelParams = {
                    preRequisiteSession: preRequisiteModel,
                    application: vm.application,
                    credentialRequest: vm.credentialRequest,
                    credentialPrerequisiteRequest: vm.credentialPrerequisiteRequest,
                    action: vm.action,
                    visibleSteps: vm.visibleSteps,
                    stepId: ko.observable(stepId),
                    wizardService: applicationService,
                    wizardOptions: vm.wizardOptions
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