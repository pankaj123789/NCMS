define([
    'services/application-data-service',
    'services/util',
    'modules/enums'
], function (applicationService, util, enums) {
    return {
        getInstance: getInstance
    };

    function getInstance(params) {
        var defaultParams = {
            application: null,
            action: null,
            credentialRequest: null,
        };

        $.extend(defaultParams, params);


        var vm = {
            session: params.preRequisiteSession,
            prerequisites: ko.observableArray(),
            application: defaultParams.application,
            action: defaultParams.action,
            credentialRequest: defaultParams.credentialRequest,
            visibleSteps: defaultParams.visibleSteps,
            tableDefinition: {
                dom: "<'row'<'col-sm-12'tr>>" +
                    "<'row'<'col-sm-4'l><'col-sm-3'i><'col-sm-5'p>>",
                searching: false,
                paging: false,
                oLanguage: { sInfoEmpty: '', sInfo: '' }
            },
            match: ko.observable(true)
        };

        // we want to check if the action is issue credential for the alter steps function so we can check the exemptions and prerequisites
        vm.isIssueCredentialAction = ko.pureComputed(function () {
            if (vm.action().Id == 1010) { // Issue Credential
                return true;
            }
            else {
                return false;
            }
        });

        vm.load = function () {
            vm.session.needsNewApplication(true);

            var request = {
                credentialRequestId: vm.credentialRequest.Id()
            };

            applicationService.getFluid('prerequisiteSummary', request).then(function (data) {
                ko.utils.arrayForEach(data.PrerequisiteSummaryModels,
                    function (item) {
                        if (item.Match) {
                            item.MatchButton = util.getIssueCredentialMatchStatusCss(1);
                            item.IsMatch = 'Match';
                        }
                        else if (!item.Match) {
                            item.MatchButton = util.getIssueCredentialMatchStatusCss(2);
                            item.IsMatch = 'Unmatch';
                            vm.match(false);
                        }

                        if (item.StartDate != null) {
                            item.StartDate = new Date(item.StartDate).toLocaleDateString('en-AU', {
                                day: '2-digit',
                                month: '2-digit',
                                year: 'numeric',
                            });
                        }
                        else if (item.Startdate == null) {
                            item.StartDate = '-';
                        }

                        if (item.EndDate != null) {
                            item.EndDate = new Date(item.EndDate).toLocaleDateString('en-AU', {
                                day: '2-digit',
                                month: '2-digit',
                                year: 'numeric',
                            });
                        }
                        else if (item.EndDate == null) {
                            item.EndDate = '-';
                        }

                        if (item.CertificationPeriodId == null) {
                            item.CertificationPeriodId = '-'
                        }
                    });

                ko.viewmodel.updateFromModel(vm.prerequisites, data.PrerequisiteSummaryModels);
                //vm.match(data.Match);

                vm.session.needsNewApplication(!vm.match());
            });
        };

        vm.isValid = function () {
            return true;
        };

        vm.alterSteps = function (steps, allSteps, currentStep) {
            if (vm.isIssueCredentialAction()) {
                // find exempted credentials
                var exemptedCredentials = vm.visibleSteps().find(x => x.id == 'wizardPrerequisiteExemptions').compose.model.prerequisiteExemptions().filter(exemption => {
                    return exemption.Checked == true;
                });

                // find any prerequisties from the summary that are not complete (unmatched)
                var nonCompletePrereqs = vm.prerequisites().filter(prereq => {
                    return prereq.Match() == false;
                });

                // for each of the exempted credentials, check if they are the same as the incomplete credential, if so remove it from
                // the incomplete credential list
                ko.utils.arrayForEach(exemptedCredentials, function (exemptedCredential) {
                    var nonCompletePrereq = ko.utils.arrayFirst(nonCompletePrereqs, function (nonCompletePrereq) {
                        return nonCompletePrereq.PreRequisiteCredential() == exemptedCredential.PrerequisiteCredentialName;
                    });

                    var nonCompletePrereqIndex = nonCompletePrereqs.indexOf(nonCompletePrereq);

                    if (nonCompletePrereqIndex > -1) {
                        nonCompletePrereqs.splice(nonCompletePrereqIndex, 1);
                    }
                });

                // If all prerequisites are met, then we want to remove the Incomplete Prerequisite Step (The step that allows credentials to be moved to On Hold)
                if (nonCompletePrereqs.length == 0) {
                    var stepsToSplice = [allSteps[enums.ApplicationWizardSteps.IncompletePrerequisiteCredentials]];
                    return spliceSteps(stepsToSplice, steps);
                }
            }

            if (vm.match() == true) {
                var stepsToSplice = [allSteps[enums.ApplicationWizardSteps.IncompletePrerequisiteCredentials], allSteps[enums.ApplicationWizardSteps.PrerequisiteExemptions]];
                return spliceSteps(stepsToSplice, steps);
            }
            if (vm.match() == false) {
                var stepsToSplice = allSteps[enums.ApplicationWizardSteps.PrerequisiteIssueOnHoldCredentials];
                return spliceSteps(stepsToSplice, steps);
            }

            return steps;
        }

        function spliceSteps(stepsToSplice, steps) {
            ko.utils.arrayForEach(stepsToSplice, function (s) {
                var stepToSpliceIndex = -1;
                var stepToSplice = ko.utils.arrayFirst(steps(), function (step) {
                    return step.id == s.id;
                });

                stepToSpliceIndex = steps.indexOf(stepToSplice);

                if (stepToSpliceIndex > -1) {
                    steps.splice(stepToSpliceIndex, 1);
                }
            });

            return steps;
        }
        return vm;
    }
});