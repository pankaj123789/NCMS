define(['services/application-data-service'],
    function (applicationService) {
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
                files: ko.observableArray(),
                application: defaultParams.application,
                action: defaultParams.action,
                credentialRequest: defaultParams.credentialRequest,
                visibleSteps: defaultParams.visibleSteps,
                enable: ko.observable(true),
                tableDefinition: {
                    dom: "<'row'<'col-sm-12'tr>>" +
                    "<'row'<'col-sm-4'l><'col-sm-3'i><'col-sm-5'p>>",
                    searching: false,
                    paging: false,
                    oLanguage: { sInfoEmpty: '', sInfo: '' }
                }
            };

            vm.load = function () {
                vm.enable(true);


                var request = {
                    ActionId: vm.action().Id,
                    ApplicationId: vm.application.ApplicationId(),
                    CredentialRequestId: vm.credentialRequest.Id(),
                };
                var issueCredentialStep = ko.utils.arrayFilter(vm.visibleSteps(), function (step) {
                    return step.id === 'wizardIssueCredential';
                })[0];
                if (issueCredentialStep) {
                    request.CredentialStartDate = issueCredentialStep.compose.model.postData().StartDate;
                    request.CredentialExpiryDate = issueCredentialStep.compose.model.postData().ExpiryDate;
                    request.PractitionerNumber = issueCredentialStep.compose.model.postData().PractitionerNumber;
                }

                applicationService.getFluid('wizard/credentialpreview', request).then(function (data) {

                    ko.utils.arrayForEach(data,
                        function (f) {
                            var name = encodeURIComponent(f.FileName);
                            f.DownloadUrl = 'api/file/downloadTempFile/{0}'.format(name);
                        });

                    vm.files(data);
                });
            };

            vm.isValid = function () {
                vm.enable(false);
                return true;
            };

            vm.postData = function () {
                return ko.toJS(vm.files);
            };

            return vm;
        }
    });