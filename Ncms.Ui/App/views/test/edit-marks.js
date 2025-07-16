define([
    'services/examiner-data-service',
    'services/testresult-data-service',
    'services/screen/message-service'
], function (examinerService, testResultService, message) {
    var viewModel = {
        getInstance: getInstance
    };

    return viewModel;

    function getInstance() {
        var vm = {
            testResultId: ko.observable(),
            jobExaminerId: ko.observable(),
            components: ko.observableArray(),
            overAllPassMark: ko.observable(),
            includePreviousMarks: ko.observable()
        };

        vm.totalMarks = ko.pureComputed(function () {
            var total = 0;

            $.each(vm.components(), function (i, c) {
                total += c.TotalMarks();
            });

            return total;
        });

        vm.sumTotalMarks = ko.pureComputed(function () {
            var total = 0;

            $.each(vm.components(), function (i, c) {
                var mark = parseFloat(c.Mark());

                if (isNaN(mark))
                    return;

                total += mark;
            });

            return total.toFixed(1);
        });

        var service = ko.pureComputed(function () {
            return vm.jobExaminerId() ? examinerService : testResultService;
        });

        var serviceAction = ko.pureComputed(function () {
            return (vm.jobExaminerId() ? vm.jobExaminerId() + '/' : '') + 'marks/' + vm.testResultId();
        });

        var defer = null;
        var validation = null;

        function getData() {
            return service().getFluid(serviceAction());
        }

        function postData(data) {
            return service().post(data, serviceAction());
        }

        vm.edit = function (testResultId, includePreviousMarks, jobExaminerId) {
            defer = Q.defer();

            vm.testResultId(testResultId);
            vm.includePreviousMarks(includePreviousMarks);
            vm.jobExaminerId(jobExaminerId);

            if (validation && validation.errors) {
                validation.errors.showAllMessages(false);
            }

            getData().then(function (data) {
                ko.viewmodel.updateFromModel(vm.components, data.Components, true).onComplete(function () {
                    var marks = [];

                    $.each(vm.components(), function (i, c) {
                        c.Mark.extend({ max: c.TotalMarks(), message: 'The mark must be less than or equal to {0}.' });
                        marks.push(c.Mark);
                    });

                    validation = ko.validatedObservable(marks);
                    vm.dirtyFlag().reset();
                });

                vm.overAllPassMark(data.OverAllPassMark.OverAllPassMark);
            });

            $('#editMarksModal').modal('show');

            return defer.promise;
        };

        vm.save = function () {
            if (!validation.isValid()) {
                validation.errors.showAllMessages();
                return;
            }

            var data = {
                Components: ko.toJS(vm.components()),
                IncludePreviousMarks: vm.includePreviousMarks()
            };

            postData(data).then(function () {
                toastr.success(ko.Localization('Naati.Resources.Test.resources.MarksSavedSuccessfully'));
                defer.resolve();
                close();
            });
        };

        vm.dirtyFlag = new ko.DirtyFlag([vm.components], false);

        vm.canSave = ko.computed(function () {
            return vm.dirtyFlag().isDirty();
        });

        var canClose = false;

        vm.compositionComplete = function () {
            $('#editMarksModal').on('hide.bs.modal', function (e) {
                tryClose(e);
            });

            $(window).on('editMarksCancel', function () {
                close();
            });
        };

        vm.close = close;

        function tryClose(event) {
            if (event.target.id !== 'editMarksModal') {
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
                .then(function (answer) {
                    if (answer === 'yes') {
                        close();
                    }
                });
            } else {
                close();
            }
        };

        function close() {
            canClose = true;
            $('#editMarksModal').modal('hide');
            canClose = false;
        };

        return vm;
    }
});
