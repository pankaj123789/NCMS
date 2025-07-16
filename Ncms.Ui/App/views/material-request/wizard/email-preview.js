define([
    'modules/enums'
], function (enums) {
    return {
        getInstance: getInstance
    };

    function getInstance(params) {
        var defaultParams = {
            wizardService: null,
            materialRequest: null,
            action: null,
            materialRound: null,
            stepId: null
        };

        $.extend(defaultParams, params);

        var vm = {
            emails: ko.observableArray(),
            loaded: ko.observable(false),
            materialRequest: defaultParams.materialRequest,
            materialRound: defaultParams.materialRound,
            action: defaultParams.action,
            visibleSteps: defaultParams.visibleSteps,
        };

        vm.emailsPreviewOptions = {
            loaded: vm.loaded,
            emails: vm.emails
        };

        vm.load = function () {
            vm.loaded(false);

            var request = {
                MaterialRequestId: vm.materialRequest.MaterialRequestId(),
                MaterialRequestRoundId: vm.materialRound.MaterialRoundId(),
                ActionId: Number(vm.action().Id),
                Steps: []
            };

            ko.utils.arrayForEach(vm.visibleSteps(),
                function (s) {
                    if (s.compose.model.postData) {
                        request.Steps.push({
                            Id: enums.MaterialRequestWizardStep[s.id.replace('wizard', '')],
                            Data: s.compose.model.postData()
                        });
                    }
                });

            var emails = [];
            vm.emails([]);

            defaultParams.wizardService.post(request, 'wizard/email-preview').then(function (result) {
                ko.utils.arrayForEach(result, function (data) {
                    ko.utils.arrayForEach(data.Attachments, function (a) {
                        a.DownloadUrl = '{0}/downloademailattachment/{1}/{2}/{3}'.format(
                            defaultParams.wizardService.url(),
                            vm.materialRequest.MaterialRequestId(),
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