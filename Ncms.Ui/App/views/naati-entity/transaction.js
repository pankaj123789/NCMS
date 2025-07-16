define([
        'services/finance-data-service',
        'views/naati-entity/transaction/invoice-creditnote',
        'views/naati-entity/transaction/payment',
        'views/naati-entity/transaction/bill'
],
function (financeService, invoiceCreditNote, payment, bill) {
    return {
        getInstance: getInstance
    };

    function getInstance() {
        var vm = {
            naatiNumber: ko.observable(),
            source: ko.observableArray(),
            showOld: ko.observable(false),
            isLoading: ko.observable(false),
        };

        vm.naatiNumber.subscribe(loadInvoices);
        vm.showOld.subscribe(loadInvoices);
        vm.source.subscribe(loadChilds);

        var invoiceCreditNoteInstance = invoiceCreditNote.getInstance();
        var paymentInstance = payment.getInstance();
        var billInstance = bill.getInstance();

        vm.auditLogOptions = {};

        vm.tabOptions = {
            tabContainerId: 'transactionsTab',
            id: 'transactionsTab',
            tabs: ko.observableArray([{
                active: true,
                id: 'invoicecreditnote',
                name: 'Naati.Resources.Person.resources.InvoicesAndCreditNotes',
                type: 'compose',
                composition: {
                    model: invoiceCreditNoteInstance,
                    view: 'views/naati-entity/transaction/invoice-creditnote'
                },
                click: onTabClick
            }
            ])
        };

        var skipLoadInvoice = false;

        vm.load = function (naatiNumber) {
            skipLoadInvoice = true;
            vm.showOld(false);
            skipLoadInvoice = false;

            vm.naatiNumber(naatiNumber);
        };

        vm.refresh = function () {
            if (vm.isLoading()) return;
            loadInvoices();
        };

        vm.refreshCss = ko.pureComputed(function () {
            if (vm.isLoading()){
                return 'fa fa-circle-notch animated spin infinite text-info';
            }
            return 'fa fa-redo';
        });

        return vm;

        function onTabClick(tabOption) {
            function displaySkarline() {
                if ($('#' + tabOption.id).is(':visible')) {
                    $.sparkline_display_visible();
                    return;
                }
                setTimeout(displaySkarline, 100);
            }

            displaySkarline();
        }

        function loadInvoices() {
            if (skipLoadInvoice) {
                return;
            }

            vm.isLoading(true);
            var naatiNumber = vm.naatiNumber();

            var request = {
                NaatiNumber: [naatiNumber]
            };

            if (!vm.showOld()) {
                request.DateCreatedFrom = moment().add('day', -90).format();
            }

            financeService.getFluid('invoice', request).then(function (data) {
                ko.utils.arrayForEach(data, function (d) {
                    var typeName = 'Invoice';
                    if (d.Type === CONST.invoiceTypes.creditNote) {
                        typeName = 'CreditNote';
                    }
                    else if (d.Type === CONST.invoiceTypes.bill) {
                        typeName = 'Bill';
                    }

                    d.TypeName = ko.Localization('Naati.Resources.Finance.resources.' + typeName);
                    d.showLog = null;
                    d.downloadUrl = null;
                    d.StatusText = null;

                    for (var s in CONST.invoiceStatuses) {
                        if (CONST.invoiceStatuses[s] === d.Status) {
                            d.StatusText = ko.Localization('Naati.Resources.InvoiceStatus.resources.' + s[0].toUpperCase() + s.substr(1));
                        }
                    }

                    if (d.TransactionId) {
                        d.showLog = function () {
                            vm.auditLogOptions.component.show({ ParentId: d.TransactionId });
                        };
                    }

                    d.downloadUrl = financeService.url() +
                        '/downloadInvoice?number=' + d.InvoiceNumber +
                        '&type=' + typeName +
                        '&location=' + (d.TransactionId ? 'Sam' : 'Wiise');

                    d.showLog = function() {
                        vm.auditLogOptions.component.show({ ChangeDetailPart: d.InvoiceNumber });
                    };
                });

                vm.source(data);
                vm.isLoading(false);
            });
        }

        function loadChilds(newValue) {
            var invoices = ko.utils.arrayFilter(newValue, function (invoice) {
                return invoice.Type === CONST.invoiceTypes.invoice || invoice.Type === CONST.invoiceTypes.creditNote;
            });

            invoiceCreditNoteInstance.source(invoices);

            var payments = [];
            ko.utils.arrayForEach(newValue, function (invoice) {
                ko.utils.arrayForEach(invoice.Payments, function (payment) {
                    payment.TypeName = invoice.TypeName;
                    payment.InvoiceUrl = invoice.InvoiceUrl;
                    payment.showLog = invoice.showLog;
                    payment.downloadUrl = invoice.downloadUrl;
                    payments.push(payment);
                });
            });

            paymentInstance.source(payments);

            var bills = ko.utils.arrayFilter(newValue, function (invoice) {
                return invoice.Type === CONST.invoiceTypes.bill;
            });

            billInstance.source(bills);
        }
    }
});
