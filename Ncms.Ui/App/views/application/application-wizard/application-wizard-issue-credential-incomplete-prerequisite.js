define([
    'services/application-data-service',
    'services/util',
    'modules/custom-validator',
    'modules/enums',
], function (applicationService, util, customValidator, enums) {
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

        var serverModel = {
            Checked: ko.observable(),
            Message: ko.observable(),
            OnDisableMessage: ko.observable()
        };

        var vm = {
            application: defaultParams.application,
            action: defaultParams.action,
            credentialRequest: defaultParams.credentialRequest,
            visibleSteps: defaultParams.visibleSteps,
            option: serverModel,
            optionPromise: null,
            checkBoxText: ko.Localization('Naati.Resources.Application.resources.ProceedWithCredentialIssue'),
            match: ko.observable(false),
        };

        vm.load = function () {
        };

        vm.isValid = function () {
            return true;
        };

        vm.postData = function () {
            return ko.toJS(serverModel);
        };

        vm.activate = function () {
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
                vm.match(data.Match);
            });


            applicationService.getFluid('incompletePrerequsiteCredentialCheckOptionMessage').then(function (data) {
                ko.viewmodel.updateFromModel(serverModel, data);
            });
        }

        vm.alterSteps = function (steps, allSteps, currentStep) {
            if (!vm.option.Checked()) {
                //get rid of remaining steps and OK button will show to allow completion of wizard steps
                steps(steps.slice(0, steps.indexOf(currentStep) + 1));
                var finalStepId = enums.ApplicationWizardSteps.NoNeedToContinue;
                var finalStep = allSteps[finalStepId];
                var finalStepType = 'CredentialWillBePutOnHold'
                var model = finalStep.model;

                var modelParams = {
                    finalStepType: finalStepType
                };

                steps.push($.extend(finalStepId, finalStep, {
                    current: ko.observable(false),
                    success: ko.observable(false),
                    cancel: ko.observable(false),
                    label: ko.Localization('Naati.Resources.Application.resources.CredentialWillBePutOnHold'),
                    css: 'animated fadeIn',
                    compose: {
                        view: finalStep.compose.view,
                        model: model.getInstance(modelParams)
                    }
                }));
            }
            return steps;
        }
     
        return vm;
    };
});