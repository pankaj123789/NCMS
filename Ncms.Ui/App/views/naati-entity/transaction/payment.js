define([
    'services/screen/message-service',
    'services/finance-data-service'
], function (message, financeService) {
    return {
        getInstance: getInstance
    };

    function getInstance() {
        var vm = {
            source: ko.observableArray(),
            tableDefinition: {
                headerTemplate: 'transation-payment-header-template',
                rowTemplate: 'transation-payment-row-template'
            }
        };

        vm.tableDefinition.dataTable = {
            source: vm.source,
            columnDefs: [
                {
                    targets: 2,
                    render: $.fn.dataTable.render.moment(moment.ISO_8601, CONST.settings.shortDateDisplayFormat)
                }
            ]
        };

        vm.totalPayments = ko.computed(function () {
            var total = 0;

            ko.utils.arrayForEach(vm.source(), function (s) {
                total += s.Amount;
            });

            return total;
        });

        return vm;
    }
});
