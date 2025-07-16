define([
    'views/shell',
    'modules/enums',
    'views/finance/end-of-period-parts/payment',
    'services/finance-data-service',
    'services/setup-data-service'
],
function (shell, enums, paymentModel, financeService, setupService) {
    function plainObjectSelector(field) {
        return function (obj) {
            return obj[field];
        }
    }

    function getComputeTotal(array, selector) {
        return function () {
            var total = 0;

            ko.utils.arrayForEach(array(), function (data) {
                total += selector(data);
            });

            return total;
        }
    }

    function getComputedFilter(array, selector, value) {
        return function () {
            return ko.utils.arrayFilter(array(), function (data) {
                return ko.utils.arrayFirst(value, function (v) {
                    return v === selector(data);
                }) != null;
            });
        };
    }

    var vm = {
        title: shell.titleWithSmall,
        searchTerm: ko.observable({}),
        tests: ko.observableArray([]),
        selectedTestIndecies: ko.observableArray([]),
        paymentInstance: paymentModel.getInstance(),
        paidToAccount: ko.observable()
    };

    $.extend(vm, {
        payments: ko.pureComputed(function () {
            return vm.paymentInstance.details();
        })
    });

    $.extend(vm, {
        cashPayments: ko.pureComputed(getComputedFilter(vm.payments, plainObjectSelector('PaymentType'), ['Cash'])),
        chequePayments: ko.pureComputed(getComputedFilter(vm.payments, plainObjectSelector('PaymentType'), ['Cheque'])),
        eftPayments: ko.pureComputed(getComputedFilter(vm.payments, plainObjectSelector('PaymentType'), ['EFT', 'EFTPOS'])),
        amexPayments: ko.pureComputed(getComputedFilter(vm.payments, plainObjectSelector('PaymentType'), ['AMEX'])),
        paypalPayments: ko.pureComputed(getComputedFilter(vm.payments, plainObjectSelector('PaymentType'), ['PayPal'])),
        unknownPayments: ko.pureComputed(getComputedFilter(vm.payments, plainObjectSelector('PaymentType'), ['Unknown']))
    });

    var paymentsInfo = {
        cash: ko.pureComputed(getComputeTotal(vm.cashPayments, plainObjectSelector('Amount'))),
        cheque: ko.pureComputed(getComputeTotal(vm.chequePayments, plainObjectSelector('Amount'))),
        eft: ko.pureComputed(getComputeTotal(vm.eftPayments, plainObjectSelector('Amount'))),
        amex: ko.pureComputed(getComputeTotal(vm.amexPayments, plainObjectSelector('Amount'))),
        paypal: ko.pureComputed(getComputeTotal(vm.paypalPayments, plainObjectSelector('Amount'))),
        unknown: ko.pureComputed(getComputeTotal(vm.unknownPayments, plainObjectSelector('Amount')))
    };

    paymentsInfo.cashAndCheque = ko.pureComputed(function () {
        return paymentsInfo.cash() + paymentsInfo.cheque();
    });

    paymentsInfo.total = ko.pureComputed(function () {
        return paymentsInfo.cash() + paymentsInfo.cheque() + paymentsInfo.eft() + paymentsInfo.amex() + paymentsInfo.unknown() + paymentsInfo.paypal();
    });

    function getBarObject(fieldName, resourceName, className) {
        return {
            label: 'Naati.Resources.Finance.resources.{0}'.format(resourceName),
            barCss: 'bg-{0}'.format(className),
            textCss: 'text-{0}'.format(className),
            value: paymentsInfo[fieldName],
            percent: getPercent(fieldName)
        };
    }

    paymentsInfo.bars = function () {
        return [
            getBarObject('cash', 'CashTotal', 'warning'),
            getBarObject('cheque', 'ChequeTotal', 'info'),
            getBarObject('cashAndCheque', 'CashAndChequeTotal', 'primary'),
            getBarObject('eft', 'EFTTotal', 'success'),
            getBarObject('amex', 'AMEX', 'darkblue'),
            getBarObject('paypal', 'PayPal', 'darkblue'),
            getBarObject('unknown', 'UnknownTotal', 'dark'),
            getBarObject('total', 'Total', 'danger')
        ];
    };

    function getPercent(type) {
        return ko.computed(function () {
            var value = paymentsInfo[type]();
            return parseInt(paymentsInfo.total() === 0 ? 0 : value * 100 / paymentsInfo.total());
        });
    }

	var resultData = {
		paymentOptions: {
			model: vm.paymentInstance,
			view: 'views/finance/end-of-period-parts/payment'
		},
        showResult: ko.observable(false),
        paymentsInfo: paymentsInfo
    };

    vm.parsedSearchTerm = ko.pureComputed(function () {
        var searchTerm = vm.searchTerm();

        var parsedSearchTerm = {
            PaidToAccount: [vm.paidToAccount()]
        };

        if (searchTerm.TestDateFromAndTo) {
            $.extend(parsedSearchTerm, {
                DateCreatedFrom: moment(searchTerm.TestDateFromAndTo.Data.From).format(CONST.settings.yearMonthDayFormat),
                DateCreatedTo: moment(searchTerm.TestDateFromAndTo.Data.To).format(CONST.settings.yearMonthDayFormat)
            });
        }

        if (searchTerm.FinanceOffice) {
            $.extend(parsedSearchTerm, {
                Office: searchTerm.FinanceOffice.Data.Office,
                EftMachine: searchTerm.FinanceOffice.Data.EFTMachine
            });
        }

        if (searchTerm.PaymentType) {
            $.extend(parsedSearchTerm, {
                PaymentType: searchTerm.PaymentType.Data.Options
            });
        }

        if (searchTerm.InvoiceNumber) {
            $.extend(parsedSearchTerm, {
                InvoiceNumber: searchTerm.InvoiceNumber.Data.value
            });
        }

        return parsedSearchTerm;
    });

    vm.invoiceSearchRequest = ko.pureComputed(function () {
        return $.extend(vm.parsedSearchTerm(),
        {
            IncludeFullPaymentInfo: false,
            ExcludeCreditNotes: true
        });
    });

    vm.getExportUrl = function() {
        return financeService.url() + '/exportEndOfPeriod/?' + $.param(vm.invoiceSearchRequest());
    }

    vm.searchComponentOptions = {
        name: 'custom-search',
        params: {
            title: vm.title,
            filters: [
                {
                    id: 'TestDateFromAndTo',
                    name: 'Naati.Resources.Finance.resources.Date'
                },
                { id: 'FinanceOffice' },
                { id: 'InvoiceNumber' },
                { id: 'PaymentType' }
            ],
            searchType: enums.SearchTypes.EndOfPeriod,
            searchTerm: vm.searchTerm,
            searchCallback: function () {
                financeService.getFluid('payment', vm.parsedSearchTerm()).then(function(data) {
                    ko.utils.arrayForEach(data, function (p) {
                        p.downloadUrl = financeService.url() +
                            '/downloadInvoice?number=' + p.InvoiceNumber +
                            '&type=' + (p.InvoiceType === CONST.invoiceTypes.creditNote ? 'CreditNote' : 'Invoice') +
                            '&location=Wiise';
                    });

                    vm.paymentInstance.details(data);
                    resultData.showResult(true);
				});
            },
            resultTemplate: {
                name: 'invoicesAndPaymentsTemplate',
                data: resultData
            },
            additionalButtons: [{
                'class': 'btn btn-default',
                downloadFile: { url: vm.getExportUrl },
                icon: 'glyphicon glyphicon-export',
                resourceName: 'Naati.Resources.Shared.resources.ExportToExcel'
            }]
        }
    };

    vm.activate = function () {
        setupService.getFluid('xeroParameters').then(function (data) {
            vm.paidToAccount(data.WiisePaymentAccount);
        });
    };

    return vm;
});
