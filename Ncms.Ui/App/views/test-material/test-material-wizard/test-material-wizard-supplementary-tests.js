define([
        'services/test-material-data-service'
],
    function (testMaterialService) {
        return {
            getInstance: getInstance
        };

        function getInstance(params) {
            var defaultParams = {
                summary: null
            };

            $.extend(defaultParams, params);

            var vm = {
                summary: defaultParams.summary,
                tests: ko.observableArray(),
                readOnly: ko.observable(false),
                tableDefinition: {
                    order: [
                        [0, "asc"]
                    ]
                },
            };

            vm.load = function () {
                vm.readOnly(false);
                testMaterialService.post(vm.summary.Request(),'getSupplementaryTests').then(function (data) {
					vm.tests(data.Results);
				});
            };

            return vm;
        }
    });