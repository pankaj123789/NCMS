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
                id: 'billTable',
                headerTemplate: 'bill-header-template',
                rowTemplate: 'bill-row-template'
            }
        };

        vm.tableDefinition.dataTable = {
            source: vm.source,
            columnDefs: [
                {
                    targets: [3, 4],
                    render: $.fn.dataTable.render.moment(moment.ISO_8601, CONST.settings.shortDateDisplayFormat)
                }
            ]
        };

        vm.total = ko.computed(function () {
            var total = 0;

            ko.utils.arrayForEach(vm.source(), function (s) {
                total += s.Total;
            });

            return total;
        });

        vm.amountDue = ko.computed(function () {
            var total = 0;

            ko.utils.arrayForEach(vm.source(), function (s) {
                total += s.AmountDue;
            });

            return total;
        });

        vm.amountPaid = ko.computed(function () {
            return vm.total() - vm.amountDue();
        });

        vm.amountPaidPercent = ko.computed(function () {
            if (!vm.total()) return 0;
            return parseInt(vm.amountPaid() / vm.total() * 100);
        });

        vm.amountDuePercent = ko.computed(function () {
            if (!vm.total()) return 0;
            return 100 - vm.amountPaidPercent();
        });

        vm.sparklineValues = ko.computed(function () {
            return [vm.amountPaidPercent(), vm.amountDuePercent()];
        });

        vm.sparklineOptions = {
            type: 'pie',
            height: 135,
            sliceColors: ['#27c24c', '#f05050'],
            tooltipFormat: '{{offset:offset}} {{value}}%',
            tooltipValueLookups: {
                offset: {
                    0: ko.Localization('Naati.Resources.Finance.resources.PaymentTotal'),
                    1: ko.Localization('Naati.Resources.Finance.resources.Balance'),
                }
            }
        };

        return vm;
    }
});
