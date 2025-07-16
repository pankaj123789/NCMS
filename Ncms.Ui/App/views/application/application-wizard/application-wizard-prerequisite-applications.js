define([
    'services/application-data-service',
    'services/util',
    'modules/enums',
    'modules/custom-validator'
], function (applicationService, util, enums, customValidator) {
    return {
        getInstance: getInstance
    };

    function getInstance(params) {

        var defaultParams = {
            application: null,
            credentialRequest: null,
            action: null,
            credentialRequest: null,
        };

        $.extend(defaultParams, params);

        var serverModel = {

        };

        var vm = {
            session: params.preRequisiteSession,
            application: defaultParams.application,
            action: defaultParams.action,
            credentialRequest: defaultParams.credentialRequest,
            credentialPrerequisiteRequest: params.credentialPrerequisiteRequest,
            visibleSteps: defaultParams.visibleSteps,
            option: serverModel,
            optionPromise: null,
            prerequisiteApplications: ko.observableArray()
        };

        vm.tableDefinition = {
            dom: "<'row'<'col-sm-12'tr>>" +
                "<'row'<'col-sm-4'l><'col-sm-3'i><'col-sm-5'p>>",
            searching: false,
            paging: false,
            oLanguage: { sInfoEmpty: '', sInfo: '' },
            columnDefs: [
                {
                    orderable: false,
                    className: 'select-checkbox',
                    targets: 0
                }
            ],
            select: {
                style: 'multi+shift'
            },
            drawCallback: drawTable,
            events: {
                select: selectTable,
                deselect: selectTable
            }
        };

        vm.load = function () {
            vm.session.selectedApplications([]);
            applicationService.getFluid('prerequisiteApplicationsNullableApplications/{0}'.format(vm.credentialRequest.Id())).then(function (data) {
                console.log("Load Data Start");
                ko.utils.arrayForEach(data,
                    function (item) {

                        // property for selection on data table
                        item.Selected = false;

                        // for href
                        item.url = '';
                        item.ApplicationExists = true;

                        //for status
                        item.appStatus = '';
                        item.requestStatus = '';

                        console.log("Iterate Data 100");

                        if (item.ExistingApplicationStatusTypeId != null || item.MatchingCredentialStatusTypeId != null) {
                            var appStatus = item.ExistingApplicationStatusTypeId;
                            item.appStatus = getAppStatus(appStatus);
                            var requestStatus = item.MatchingCredentialStatusTypeId;
                            item.requestStatus = getRequestStatus(requestStatus);
                        }

                        if (item.ExistingApplicationId != null) {
                            item.url = '#application/{0}'.format(item.ExistingApplicationId)
                            item.ExistingApplicationId = 'APP{0}'.format(item.ExistingApplicationId);
                        }
                        else {
                            item.ExistingApplicationId = '-'
                            item.ApplicationExists = false;
                        }


                        if (item.ExistingApplicationTypeName == null) {
                            item.ExistingApplicationTypeName = '-'
                        }

                    });

                console.log("Load Data");
                console.log("Count:", data.length);

                ko.viewmodel.updateFromModel(vm.prerequisiteApplications, data);
            });
        };

        vm.alterSteps = function (steps, allSteps, currentStep) {
            if (vm.session.selectedApplications().length == 0) {
                steps(steps.slice(0, steps.indexOf(currentStep) + 1));
                var finalStepId = enums.ApplicationWizardSteps.NoNeedToContinue;
                var finalStep = allSteps[finalStepId];
                var finalStepType = 'NoSelectedNewApplications'
                var model = finalStep.model;

                var modelParams = {
                    preRequisiteSession: vm.session,
                    credentialPrerequisiteRequest: vm.credentialPrerequisiteRequest,
                    finalStepType: finalStepType
                };

                steps.push($.extend(finalStepId, finalStep, {
                    current: ko.observable(false),
                    success: ko.observable(false),
                    cancel: ko.observable(false),
                    label: ko.Localization('Naati.Resources.Application.resources.NoNewApplications'),
                    css: 'animated fadeIn',
                    compose: {
                        view: finalStep.compose.view,
                        model: model.getInstance(modelParams)
                    }
                }));

                return steps;
            }

            return steps;
        }

        vm.isValid = function () {
            return true;
        };

        vm.activate = function () {
        };

        var bypassSelect = false;
        function selectTable(e, dt) {
            if (bypassSelect || !dt) {
                return;
            }

            ko.utils.arrayForEach(vm.prerequisiteApplications(), function (s) {
                s.Selected(false);
            });

            var indexes = dt.rows('.selected').indexes();

            if (!indexes.length) {
                // if nothing selected, set selectedApplications to empty array
                vm.session.selectedApplications([])
                return;
            }

            for (var i = 0; i < indexes.length; i++) {
                vm.prerequisiteApplications()[indexes[i]].Selected(true);
            }

            var selectedPrerequisiteApplications = ko.observableArray(vm.prerequisiteApplications().filter(function (item) {
                return item.Selected() == true;
            }));

            var selectedPrerequisiteApplicationsForCreation = ko.observableArray([]);

            ko.utils.arrayForEach(selectedPrerequisiteApplications(), function (item) {
                var obj = {
                    PrerequisiteCredentialName: item.PrerequisiteCredentialName,
                    PrerequisiteCredentialLanguage1: item.PrerequisiteCredentialLanguage1,
                    PrerequisiteCredentialLanguage2: item.PrerequisiteCredentialLanguage2,
                    PrerequisiteApplicationTypeId: item.PrerequisiteApplicationTypeId,
                    PrerequisiteSkillId: item.PrerequisiteSkillId,
                    ApplicationTypePrerequisiteId: item.ApplicationTypePrerequisiteId,
                    hasValidationError: false
                }

                selectedPrerequisiteApplicationsForCreation.push(obj);
            });

            vm.session.selectedApplications(selectedPrerequisiteApplicationsForCreation());

        }

        function drawTable() {

            console.log("Draw Table Call Back");
            console.log("Count:", vm.prerequisiteApplications().length);

            bypassSelect = true;

            var $table = $(this).DataTable();
            var prerequisiteApplications = vm.prerequisiteApplications();

            for (var i = 0; i < prerequisiteApplications.length; i++) {
                if (prerequisiteApplications[i].Selected()) {
                    $table.row(i).select();
                }
            }

            bypassSelect = false;
        }

        function getAppStatus(status) {
            var statuses = enums.ApplicationStatusTypes;

            if (status === statuses.Draft) {
                return 'label-gray';
            }

            if (status === statuses.ProcessingSubmission || status === statuses.ProcessingApplicationInvoice) {
                return 'label-dark-gray';
            }
            if (status === statuses.Entered) {
                return 'label-info';
            }
            if (status === statuses.BeingChecked) {
                return 'label-orange';
            }
            if (status === statuses.Rejected) {
                return 'label-danger';
            }
            if (status === statuses.InProgress) {
                return 'label-orange';
            }
            if (status === statuses.Completed) {
                return 'label-success';
            }
            if (status === statuses.AwaitingAssessmentPayment || status === statuses.AwaitingApplicationPayment) {
                return 'label-dark-yellow';
            }
            else {
                return 'label-success';
            }
        }

        function getRequestStatus(status) {
            var statuses = enums.CredentialRequestStatusTypes;

            if (status === statuses.Draft) {
                return 'label-gray';
            }

            if (status === statuses.ProcessingSubmission) {
                return 'label-dark-gray';
            }

            if (status === statuses.RequestEntered) {
                return 'label-info';
            }
            if (status === statuses.ReadyForAssessment) {
                return 'label-info';
            }
            if (status === statuses.BeingAssessed || status === statuses.CertificationOnHold) {
                return 'label-orange';
            }
            if (status === statuses.Pending) {
                return 'label-dark-yellow';
            }
            if (status === statuses.AssessmentFailed) {
                return 'label-danger';
            }
            if (status === statuses.AssessmentPaidReview) {
                return 'label-orange';
            }
            if (status === statuses.EligibleForTesting) {
                return 'label-info';
            }
            if (status === statuses.AssessmentSuccessful) {
                return 'label-orange';
            }
            if (status === statuses.Rejected) {
                return 'label-danger';
            }
            if (status === statuses.TestFailed) {
                return 'label-danger';
            }
            //if (status === statuses.CredentialIssued) {
            //    return 'label-success';
            //}
            if (status === statuses.Withdrawn) {
                return 'label-purple';
            }
            if (status === statuses.Canceled) {
                return 'label-purple';
            }
            if (status === statuses.Deleted) {
                return 'label-purple';
            }
            if (status === statuses.AwaitingTestPayment || status === statuses.AwaitingApplicationPayment || status === statuses.AwaitingSupplementaryTestPayment) {
                return 'label-dark-yellow';
            }
            if (status === statuses.TestAccepted) {
                return 'label-info';
            }
            if (status === statuses.TestSat) {
                return 'label-orange';
            }
            if (status === statuses.CheckIn) {
                return 'label-orange';
            }
            if (status === statuses.UnderPaidTestReview) {
                return 'label-orange';
            }
            if (status === statuses.IssuedPassResult) {
                return 'label-orange';
            }
            if (status === statuses.ProcessingPaidReviewInvoice) {
                return 'label-dark-gray';
            }
            if (status === statuses.AwaitingPaidReviewPayment) {
                return 'label-dark-yellow';
            }
            if (status === statuses.SupplementaryTestInvoiceProcessed) {
                return 'label-dark-gray';
            }
            if (status === statuses.TestInvalidated) {
                return 'label-purple';
            }
            if (status === statuses.ToBeIssued) {
                return 'label-dark-gray';
            }
            if (status === statuses.ProcessingTestInvoice) {
                return 'label-dark-gray';
            }
            if (status === statuses.ProcessingRequest) {
                return 'label-dark-gray';
            }
            if (status === statuses.RefundRequested) {
                return 'label-orange';
            }
            if (status === statuses.ProcessingCreditNote) {
                return 'label-dark-gray';
            }
            if (status === statuses.AwaitingCreditNotePayment) {
                return 'label-orange';
            }
            if (status === statuses.RefundRequestApproved) {
                return 'label-dark-gray';
            }
            if (status === statuses.RefundFailed) {
                return 'label-orange';
            }
            else {
                return 'label-success';
            }
        }
     
        return vm;
    };
});