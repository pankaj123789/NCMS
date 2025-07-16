define(['services/test-material-data-service',
    'services/messenger-service'],
    function (testMaterialService, messenger) {

        function getInstance() {

            var vm = {
                includeExaminer: ko.observable(),
                testSessionId: ko.observable(),
                pendingTestTasks: ko.observableArray([]),
                allCustomerAttendanceIds: ko.observableArray(),
                availablePendingTestTasks: ko.observable(),

                downloadAllMaterial: function (testSessionId) {
                    vm.pendingTestTasks([]);

                    vm.testSessionId(testSessionId);
                    vm.includeExaminer(false);
                    testMaterialService.getFluid('getTestTaskPendingToAssign/' + vm.testSessionId()).then(setPendingTestTasks);
                },

                download: function () {

                    testMaterialService.getFluid('bulkDownloadTestMaterial/' + vm.testSessionId() + '/' + vm.includeExaminer())
                        .then(function (data) {
                            messenger.checkMessages();
                        });

                    $('#downloadMaterialModal').modal('hide');

                    return false;
                }

            };

            vm.showWarningMessage = ko.pureComputed(function () {
                return vm.pendingTestTasks().length;
            });

            vm.limitMessage = ko.pureComputed(function () {
                return ko.Localization('Naati.Resources.TestMaterial.resources.PendingMaterialsLimitMessage').format(vm.pendingTestTasks().length, vm.availablePendingTestTasks());
            });


            vm.displayLimitMesssage = ko.pureComputed(function () {
                return vm.availablePendingTestTasks() > vm.pendingTestTasks().length;
            });

            function setPendingTestTasks(data) {
                vm.availablePendingTestTasks(data.length);
                if (vm.availablePendingTestTasks() > 5) {
                    data = data.splice(0, 5);
                }
                vm.pendingTestTasks(data);
                
                $('#downloadMaterialModal').modal('show');
            }

            return vm;
        }

        return {
            getInstance: getInstance
        };
    });

