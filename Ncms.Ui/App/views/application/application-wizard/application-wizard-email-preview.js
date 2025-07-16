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
            emails: ko.observableArray(),
            loaded: ko.observable(false),
            application: defaultParams.application,
            action: defaultParams.action,
            credentialRequest: defaultParams.credentialRequest,
            visibleSteps: defaultParams.visibleSteps,
        };

        vm.emailsPreviewOptions = {
            loaded: vm.loaded,
            emails: vm.emails
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

            var emails = [];
            vm.emails([]);

            applicationService.post(request, 'wizard/emailpreview').then(function (result) {
                ko.utils.arrayForEach(result, function (data) {
                    ko.utils.arrayForEach(data.Attachments, function (a) {
                        a.DownloadUrl = '{0}/downloademailattachment/{1}/{2}/{3}'.format(
                            applicationService.url(),
                            vm.application.ApplicationId(),
                            vm.action().Id,
                            a.FileName);
                    });

                    emails.push(ko.viewmodel.fromModel(data));
                });

                vm.emails(emails);
                vm.loaded(true);
            });
        };

        return vm;
    }
});