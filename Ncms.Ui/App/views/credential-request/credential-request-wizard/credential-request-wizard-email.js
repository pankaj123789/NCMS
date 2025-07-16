define([
    'services/credentialrequest-data-service'
], function (credentialrequestService) {
    return {
        getInstance: getInstance
    };

    function getInstance(params) {
        var defaultParams = {
            summary: null
        };

        $.extend(defaultParams, params);

        var vm = {
            emails: ko.observableArray(),
            loaded: ko.observable(false),
            summary: defaultParams.summary,
        };

        vm.emailsPreviewOptions = {
            loaded: vm.loaded,
            emails: vm.emails
        };

        vm.load = function () {
            vm.loaded(false);

            var emails = [];
            vm.emails([]);

            credentialrequestService.post(vm.summary.Request(true), 'emailpreview').then(function (result) {
                ko.utils.arrayForEach(result, function (data) {
                    ko.utils.arrayForEach(data.Attachments, function (a) {
                        a.DownloadUrl = '{0}/downloademailattachment/{1}/{2}/{3}/{4}/{5}/{6}'.format(
                            credentialrequestService.url(),
                            vm.summary.CredentialApplicationTypeId(),
                            vm.summary.CredentialTypeId(),
                            vm.summary.SkillId(),
                            vm.summary.CredentialRequestStatusTypeId(),
                            vm.summary.Action().Id,
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