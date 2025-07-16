define(['services/material-request-data-service',],
    function (materialRequestService) {
        return {
            getInstance: getInstance
        };

        function getInstance(options) {
            var defaultOptions = {
                Members: ko.observableArray(),
                MaterialRequestId: ko.observable(),
            };

            $.extend(defaultOptions, options);

            var vm = {
                MaterialRequestId: defaultOptions.MaterialRequestId,
                tableDefinition: {
                    headerTemplate: 'material-participants-header-template',
                    rowTemplate: 'material-participants-row-template',
                },
                showTotalCost: ko.observable()
            };

            vm.tableDefinition.dataTable = {
                source: defaultOptions.Members,
                order: [
                    [0, "asc"]
                ]
            };

            vm.load = function () {
                materialRequestService.getFluid('showTotalCost/' + vm.MaterialRequestId()).then(vm.showTotalCost);
            }

            return vm;
        }
    });
