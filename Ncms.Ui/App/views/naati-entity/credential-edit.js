define([
    'services/util',
    'services/screen/message-service',
    'services/application-data-service',
], function (util, message, applicationService) {
    return {
        getInstance: getInstance
    };

    function getInstance() {
        var serverModel = {
            Id: ko.observable(),
            CredentialType: ko.observable(),
            ShowInOnlineDirectory: ko.observable()
        };

        var cleanModel = ko.toJS(serverModel);

        var vm = {
            originalCredential: null,
            modalId: util.guid(),
            credential: serverModel,
            compositionComplete: compositionComplete,
        };

        vm.dirtyFlag = new ko.DirtyFlag([serverModel], false);

        var validation = ko.validatedObservable(serverModel);

        vm.show = function (credential) {
            vm.originalCredential = credential;
            ko.viewmodel.updateFromModel(serverModel, ko.toJS(credential));

            if (validation.errors) {
                validation.errors.showAllMessages(false);
            }

            vm.dirtyFlag().reset();
            $('#' + vm.modalId).modal('show');
        };

        vm.isDirty = ko.computed(function () {
            return vm.dirtyFlag().isDirty();
        });

        vm.save = function () {
            var request = ko.toJS(serverModel);
            applicationService.post(request, 'credentialrequest').then(function () {
                ko.viewmodel.updateFromModel(vm.originalCredential, request);
                close();
            });
        };

        var canClose = false;

        function compositionComplete() {
            $('#' + vm.modalId).on('hide.bs.modal', function (e) {
                tryClose(e);
            });
        }

        function tryClose(event) {
            if (event.target.id !== vm.modalId) {
                return;
            }

            if (canClose) {
                return;
            }

            event.preventDefault();
            event.stopImmediatePropagation();

            if (vm.dirtyFlag().isDirty()) {
                message.confirm({
                    title: ko.Localization('Naati.Resources.Shared.resources.Confirm'),
                    content: ko.Localization('Naati.Resources.Shared.resources.AreYouSureYouWantToCancel')
                }).then(function (answer) {
                    if (answer === 'yes') {
                        close();
                    }
                });
            } else {
                close();
            }
        }

        function close() {
            canClose = true;
            $('#' + vm.modalId).modal('hide');
            canClose = false;
        }

        return vm;
    }
});