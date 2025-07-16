define([
    'services/material-request-data-service'
],
    function (materialRequestService) {
        return {
            getInstance: getInstance
        };

        function getInstance() {
            var vm = {
                relations: ko.observableArray(),
                materialId: ko.observable(),
                tableDefinition: {
                    headerTemplate: 'material-relations-header-template',
                    rowTemplate: 'material-relations-row-template'
                }
            };

            vm.tableDefinition.dataTable = {
                source: vm.relations,
                columnDefs: [
                ],
                order: [
                    [0, "desc"]
                ]
            };

            vm.load = function (materialId) {
                vm.materialId(materialId);
                materialRequestService.getFluid("relations/" + materialId).then(function (data) {
                    vm.relations(data);
                });
            };

            return vm;
        }
    });
