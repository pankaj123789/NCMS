define([
    'services/screen/message-service',
    'services/finance-data-service'
], function (message, financeService) {
    return {
        getInstance: getInstance
    };

    function getInstance() {
        var vm = {
            naatiNumber: ko.observable(),
            source: ko.observableArray(),
            tableDefinition: {
                id: 'invoiceAndCreditNotesTable',
                headerTemplate: 'invoicecredit-header-template',
                rowTemplate: 'invoicecredit-row-template'
            }
        };

        vm.tableDefinition.dataTable = {
            source: vm.source,
            columnDefs: [
                {
                    targets: 0,
                    render: $.fn.dataTable.render.moment(moment.ISO_8601, CONST.settings.shortDateDisplayFormat)
                }
            ],
            order: [[0, 'desc']]
        };

        vm.totalAmount = ko.computed(function () {
            var total = 0;

            ko.utils.arrayForEach(vm.source(), function (s) {
                total += s.Total;
            });

            return total;
        });

        vm.totalPayments = ko.computed(function () {
            var total = 0;

            ko.utils.arrayForEach(vm.source(), function (s) {
                total += s.Payment;
            });

            return total;
        });

        vm.totalBalance = ko.computed(function () {
            var total = 0;

            ko.utils.arrayForEach(vm.source(), function (s) {
                total += s.Balance;
            });

            return total;
        });

        vm.paymentsPercent = ko.computed(function () {
            if (!vm.totalAmount()) return 0;
            return parseInt(vm.totalPayments() / vm.totalAmount() * 100);
        });

        vm.balancePercent = ko.computed(function () {
            if (!vm.totalAmount()) return 0;
            return 100 - vm.paymentsPercent();
        });

        vm.sparklineValues = ko.computed(function () {
            return [vm.paymentsPercent(), vm.balancePercent()];
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
