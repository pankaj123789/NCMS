define([
    'plugins/router',
    'modules/enums',
    'services/application-data-service',
],
    function (router, enums, applicationService) {
        return {
            getInstance: getInstance
        };

        function getInstance() {
            var vm = {
                applications: ko.observableArray(),
                tableDefinition: {
                    id: 'prerequisiteApplicationsTable',
                    headerTemplate: 'prerequisite-applications-header-template',
                    rowTemplate: 'prerequisite-applications-row-template',
                },
            };

            if (vm.tableDefinition.dataTable == undefined) {
                vm.tableDefinition.dataTable = {
                    source: vm.applications,
                    columnDefs: [
                    ],
                    order: [
                        [0, "desc"]
                    ]
                };
            }

            vm.load = function (applicationId) {
                vm.applications([]);
                applicationService.getFluid('relatedApplications/{0}'.format(applicationId)).then(function (data) {
                    ko.utils.arrayForEach(data.PrerequisiteApplicationsModels, function (item) {
                        var appStatus = item.ExistingApplicationStatusTypeId;
                        item.appStatus = getAppStatus(appStatus);

                        var requestStatus = item.ExistingCredentialRequestStatusTypeId;
                        item.requestStatus = getRequestStatus(requestStatus);

                        if (item.CreatedDate != null) {
                            item.CreatedDate = new Date(item.CreatedDate).toLocaleDateString('en-AU', {
                                day: '2-digit',
                                month: '2-digit',
                                year: 'numeric',
                            });
                        }
                        else if (item.CreatedDate == null) {
                            item.CreatedDate = '-';
                        }

                        if (item.ExistingApplicationAutoCreated == true) {
                            item.ExistingApplicationAutoCreated = 'Yes';
                        }
                        else if (item.ExistingApplicationAutoCreated == false) {
                            item.ExistingApplicationAutoCreated = 'No';
                        }

                        if (item.ExistingCredentialRequestAutoCreated == true) {
                            item.ExistingCredentialRequestAutoCreated = 'Yes';
                        }
                        if (item.ExistingCredentialRequestAutoCreated == false) {
                            item.ExistingCredentialRequestAutoCreated = 'No'
                        }
                        if (item.ExistingCredentialRequestAutoCreated == null) {
                            item.ExistingCredentialRequestAutoCreated = '-';
                        }

                        item.url = '#application/{0}'.format(item.ExistingApplicationId);


                        vm.applications.push(item);
                    });
                });
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
        }
    }
);