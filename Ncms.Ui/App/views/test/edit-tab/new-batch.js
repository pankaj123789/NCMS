define([
    'services/screen/message-service',
    'services/letter-data-service'
], function (message, letterService) {
    var viewModel = {
        getInstance: getInstance
    };

    return viewModel;

    function getInstance() {
        var defer = null;

        var vm = {
            name: ko.observable(),
            close: close,
            compositionComplete: compositionComplete,
        };

        vm.dirtyFlag = new ko.DirtyFlag([vm.name], false);

        vm.canSave = ko.computed(function () {
            return vm.dirtyFlag().isDirty();
        });

        vm.add = function () {
            vm.name(null);
            defer = Q.defer();
            $('#newBatchModal').modal('show');
            return defer.promise;
        };

        vm.save = function () {
            if (!validation.isValid()) {
                return validation.errors.showAllMessages();
            }

            letterService.post(vm.name(), 'batch').then(function(data) {
                defer.resolve(data);
            });
        };

        var canClose = false;
        var validation = ko.validatedObservable(vm.name);

        return vm;

        function compositionComplete() {
            $('#newBatchModal')
                .on('hide.bs.modal',
                    function (e) {
                        tryClose(e);
                    });

            $(window).on('newBatchCancel', function () {
                close();
            });
        }

        function tryClose(event) {
            if (event.target.id !== 'newBatchModal') {
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
                })
                    .then(
                        function (answer) {
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
            $('#newBatchModal').modal('hide');
            canClose = false;
        }
    }
});
