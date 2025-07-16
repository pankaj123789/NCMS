define([
    'services/application-data-service',
    'modules/enums'
], function (applicationService, enums) {
    return {
        getInstance: getInstance
    };

    function getInstance(params) {
        var defaultParams = {
            application: null,
            action: null,
            credentialRequest: null,
            visibleSteps: null
        };

        $.extend(defaultParams, params);

        var vm = {
            emailSubject: ko.observable(),
            emailBody: ko.observable(),
            loaded: ko.observable(false),
            application: defaultParams.application,
            action: defaultParams.action,
            credentialRequest: defaultParams.credentialRequest,
            visibleSteps: defaultParams.visibleSteps,
        };

        vm.serverModel = {
            subject: vm.emailSubject,
            body: vm.emailBody
        };

        vm.load = function () {
            vm.loaded(false);

            var request = {
                ApplicationId: vm.application.ApplicationId(),
                ActionId: Number(vm.action().Id),
                CredentialRequestId: vm.credentialRequest.Id(),
                Steps: []
            };

            ko.utils.arrayForEach(vm.visibleSteps(),
                function (s) {
                    if (s.compose.model.postData) {
                        request.Steps.push({
                            Id: enums.ApplicationWizardSteps[s.id.replace('wizard', '')],
                            Data: s.compose.model.postData()
                        });
                    }
                });

            applicationService.post(request, 'wizard/emailTemplates').then(function (result) {
                if (result.length == 0) {
                    toastr.error('There are no email templates mapped to this action.');
                } else {
                    if (result.length > 1) {
                        toastr.warning('There are multiple email templates mapped to this action. Using the first one.');
                    }

                    let template = result[0];
                    vm.emailSubject(template.Subject);
                    vm.emailBody(template.Content);
                }
                vm.loaded(true);
            });
        };

        vm.postData = function () {
            return ko.toJS(vm.serverModel);
        };

        return vm;
    }
});