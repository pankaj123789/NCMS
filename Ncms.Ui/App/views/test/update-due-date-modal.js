define([
    'services/testresult-data-service',
    'services/screen/date-service'
], function (testResultService, dateService) {
    var vm = {
        getInstance: getInstance
    };

    return vm;

    function getInstance() {
        var defer = null;
        var vm = {
            show: show,
            close: close,
            save: save,
            dueDate: ko.observable().extend({ required: true }),
            jobList: ko.observableArray()
        };

        vm.dueDateOptions = {
            value: vm.dueDate,
            resattr: {
                placeholder: 'Naati.Resources.Test.resources.DueDate'
            }
        };

        var validation = ko.validatedObservable(vm.dueDateOptions);

        return vm;

        function save() {
            if (!validation.isValid()) {
                validation.errors.showAllMessages();
                return;
            }

            var data = {
                JobIds: vm.jobList(),
                DueDate: dateService.toPostDate(vm.dueDate())
            };

            testResultService.post(data, 'updateduedate').then(function () {
                toastr.success(ko.Localization('Naati.Resources.Shared.resources.SavedSuccessfully'));
                defer.resolve(vm.dueDate());
            });
        }

        function show(jobList) {
            defer = Q.defer();
            vm.dueDate(null);

            validation.errors.showAllMessages(false);

            vm.jobList(jobList);
            $('#updateDueDateModal').modal('show');

            return defer.promise;
        }

        function close() {
            $('#updateDueDateModal').modal('hide');
        }
    }
});