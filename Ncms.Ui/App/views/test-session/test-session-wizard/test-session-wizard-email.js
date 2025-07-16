define([
    'services/testsession-data-service',
    'modules/enums',
], function (testsessionService, enums) {
    return {
        getInstance: getInstance
    };

    function getInstance(params) {
        var defaultParams = {
            session: null
        };

        $.extend(defaultParams, params);

        var vm = {
            emails: ko.observableArray(),
            loaded: ko.observable(false),
            session: defaultParams.session,
            request: defaultParams.request,
            visibleSteps: defaultParams.visibleSteps
        };

        vm.emailsPreviewOptions = {
            loaded: vm.loaded,
            emails: vm.emails
        };

        vm.load = function () {
            vm.loaded(false);

            var emails = [];
            vm.emails([]);
         
            testsessionService.post(vm.request(true), 'wizard/previewemail').then(function (result) {
                ko.utils.arrayForEach(result, function (data) {
                    ko.utils.arrayForEach(data.Attachments, function (a) {
                        a.DownloadUrl = '';
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