define(['durandal/app',
        'plugins/router',
        'services/requester'],
    function (app, router, requester) {

        var invoices = new requester('unraisedinvoices');

        var vm = {
            loaded: ko.observable(false),
            source: ko.observableArray(),
            unHandledException: ko.observable(false),
            unHandledExceptionMessage: ko.observable(),
            getInvoices: ko.observableArray(),
            tableDefinition: {
                id: 'invoiceAndCreditNotesTable',
                headerTemplate: 'invoicecredit-header-template',
                rowTemplate: 'invoicecredit-row-template'
            }
        };


        vm.loaderOptions = {
            name: 'loader',
            params: {
                show: ko.observable(true),
                text: ko.observable('Loading bills...')
            }
        };

        vm.activate = function () {
            loadInvoices();
        };

        function loadInvoices() {

            var getUrl = window.location;
            var baseUrl = window.baseUrl + '/';
            vm.unHandledException(false);

            invoices.getFluid('GetUnraisedInvoices').then(function (data) {

                vm.loaded(true);
                vm.loaderOptions.params.show(false);

                vm.unHandledException(data.UnHandledException);
                vm.unHandledExceptionMessage(data.UnHandledExceptionMessage);

                ko.utils.arrayForEach(data.Data, function (d) {
                    var tempDate = moment(d.Date.slice(0, 10), 'YYYY-MM-DD');
                    d.Date = tempDate.format('DD-MM-YYYY');     
                    // will only be true when application is sponsored by NAATI - as per backend implementation. 
                    if (d.IsNaatiSponsored) {
                        d.AmountDue = 0;
                        d.Balance = 0;
                    }
                    d.downloadUrl = null;
                    d.payOnline = null;
                    d.payPal = null;
                    d.hasBalanceToPay = null;
                    d.hasBalanceToPay = (d.Balance && d.Balance > 0) || d.IsNaatiSponsored === true; // for the make payment button to show up in case app is sponsored. 
                    d.payOnline = baseUrl + 'UnraisedInvoices/RaiseAndPayInvoice/?applicationId=' + d.CredentialApplicationId;

                    vm.source.push(d);
                });
                
            });
        }


        vm.tableDefinition.dataTable = {
            source: vm.source,
            columnDefs: [
                {
                    targets: 0,
                    //render: $.fn.dataTable.render.moment(moment.ISO_8601, CONST.settings.shortDateDisplayFormat)
                }
            ],
            order: [[0, 'desc']]
        };

        vm.totalAmount = ko.computed(function () {
            var total = 0;

            ko.utils.arrayForEach(vm.source(), function (s) {
                total += s.AmountDue;
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
                    0: 'Payment Total',
                    1: 'Balance'
                }
            }
        };

        return vm;
    }
);