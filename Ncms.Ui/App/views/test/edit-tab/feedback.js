define(['services/test-data-service'], function (testService) {
    var vm = {
        getInstance: getInstance,
    };

    return vm;

    function getInstance() {
        var vm = {
            load: load,
            dataLoaded: ko.observable(false),
            tableDefinition: {
                searching: false,
                paging: false,
                ordering: false,
                source: ko.observableArray([])
            }
        };

        function load(testAttendanceId, feedbackTabIcon) {
            vm.dataLoaded(false);
            vm.tableDefinition.source([]);

            testService.getFluid('feedback/' + testAttendanceId).then(function (data) {
                if (data != null && data.ExaminerFeedback.length) {
                    feedbackTabIcon('fas fa-asterisk fa-spin red-spinner-asterisk');
                    vm.tableDefinition.source(data.ExaminerFeedback);
                }
                vm.dataLoaded(true);
            });
        }

        return vm;
    }
});