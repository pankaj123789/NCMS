define([
    'views/shell',
    'services/screen/message-service',
    'services/finance-data-service',
    'services/office-data-service',
    'views/finance/add-payment-detail'
], function (shell, message, financeService, officeService, addDetail) {
    var serverModel = {
        AllowEdit: ko.observable(true),
        Type: ko.observable(),
        InvoiceNumber: ko.observable(),
        InvoiceUrl: ko.observable(),
        Customer: ko.observable(),
        Office: ko.observable(),
        DueDate: ko.observable(),
        WiiseReference: ko.observable(),
        Theme: ko.observable(),
        Total: ko.observable(),
        PaymentTotal: ko.observable(),
        Payments: ko.observableArray()
    };

    var cleanServerModel = ko.toJS(serverModel);

    var vm = {
        title: shell.titleWithSmall,
        invoice: serverModel,
        search: ko.observable(),
        dueDate: ko.observable(),
        addDetailInstance: addDetail.getInstance(),
        showInvoiceSelector: ko.observable(false),
        invoices: ko.observableArray(),
        invoiceId: ko.observable(),
        entityIdCandidate: ko.observable(),
        processing: ko.observable(false),
        tableDefinition: {
            id: 'paymentsTable',
            headerTemplate: 'payments-header-template',
            rowTemplate: 'payments-row-template'
        },
        paymentAccount: null
    };

    vm.isInvoice = ko.computed(function () {
        return vm.invoice.Type() === 1;
    });

    vm.typeName = ko.computed(function () {
        return ko.Localization('Naati.Resources.Finance.resources.' + (vm.isInvoice() ? 'Invoice' : 'CreditNote'));
    });

    vm.invoiceOrCreditNoteLabel = ko.computed(function () {
        return ko.Localization('Naati.Resources.Finance.resources.' + (vm.isInvoice() ? 'InvoiceNo' : 'CreditNoteNo'));
    });

    vm.paid = ko.computed(function () {
        var total = 0;

        $.each(serverModel.Payments(), function (i, d) {
            total += ko.unwrap(d.Amount);
        });

        return total;
    });

    vm.balance = ko.computed(function () {
        if (!vm.invoice.Total()) {
            return 0;
        }

        return vm.invoice.Total() - vm.paid();
    });

    vm.paidPercent = ko.computed(function () {
        if (!serverModel.Total()) return 0;
        return parseInt(vm.paid() / serverModel.Total() * 100);
    });

    vm.balancePercent = ko.computed(function () {
        return 100 - vm.paidPercent();
    });

    vm.search.subscribe(selectResult);
    vm.invoiceId.subscribe(selectEntity);
    vm.balance.subscribe(enableDisableDataTableButtons);

    vm.searchOptions = {
        source: function (query, callback) {
            financeService.getFluid('newpayment/search', { term: query }).then(function (data) {
                ko.utils.arrayForEach(data, function (d) {
                    d.Label = d.Type === 'I' ? d.EntityId : d.Name;

                });
                callback(data);
            });
        },
        multiple: false,
        valueProp: 'EntityId',
        labelProp: 'Label',
        template: 'entityto-template',
        dataValue: vm.search,
        textValue: ko.observable(),
        resattr: {
            placeholder: 'Naati.Resources.Finance.resources.Search'
        }
    };

    vm.invoicesOptions = {
        options: vm.invoices,
        value: vm.invoiceId,
        multiple: false,
        optionsValue: 'InvoiceNumber',
        optionsText: 'Text'
    };

    vm.sparklineValues = ko.computed(function () {
        return [vm.paidPercent(), vm.balancePercent()];
    });

    vm.sparklineOptions = {
        type: 'pie',
        height: 135,
        sliceColors: ['#27c24c', '#f05050'],
        tooltipFormat: '{{offset:offset}} {{value}}%',
        tooltipValueLookups: {
            offset: {
                0: ko.Localization('Naati.Resources.Finance.resources.PaymentTotal'),
                1: ko.Localization('Naati.Resources.Finance.resources.Balance')
            }
        }
    };

    vm.tableDefinition.dataTable = {
        source: serverModel.Payments,
        columnDefs: [
            {
                targets: 3,
                render: $.fn.dataTable.render.moment(moment.ISO_8601, CONST.settings.shortDateDisplayFormat)
            }
        ],
        buttons: {
            dom: {
                button: {
                    tag: 'label',
                    className: ''
                },
                buttonLiner: {
                    tag: null
                }
            },
            buttons: [{
                enabled: false,
                text: '<span class="glyphicon glyphicon-plus"></span><span>' +
                    ko.Localization('Naati.Resources.Finance.resources.AddPayment') + '</span>',
                className: 'btn btn-success',
                action: addPayment
            }]
        },
        initComplete: function () {
            enableDisableDataTableButtons();
        }
    };

    vm.edit = function (payment) {
        vm.addDetailInstance.edit(payment, vm.balance() + payment.Amount).then(function (newValue) {
            newValue = parsePayment(newValue);

            var details = serverModel.Payments();
            serverModel.Payments([]);

            var index = details.indexOf(payment);

            if (index !== -1) {
                details[index] = newValue;
            }

            serverModel.Payments(details);
        });
    };

    vm.remove = function (payment) {
        message.remove({
            yes: '<span class="glyphicon glyphicon-trash"></span><span>' +
                ko.Localization('Naati.Resources.Shared.resources.Yes') +
                '</span>',
            no: ko.Localization('Naati.Resources.Shared.resources.No'),
            content: ko.Localization('Naati.Resources.Finance.resources.DeletePaymentDetailConfirmation')
        }).then(function (answer) {
            if (answer === 'yes') {
                serverModel.Payments.remove(payment);
            }
        });
    };

    vm.canSave = ko.computed(function () {
        return vm.invoice.InvoiceNumber() && serverModel.Payments().length && !vm.processing() && vm.invoice.AllowEdit();
    });

    vm.trySave = function () {
        if (vm.balance() > 0) {
            message.confirm({
                title: ko.Localization('Naati.Resources.Shared.resources.Warning'),
                content: ko.Localization('Naati.Resources.Finance.resources.InvoiceNotPaidInFull')
            }).then(function (answer) {
                if (answer === 'yes') {
                    save();
                }
            });

            return;
        }

        save();
    };

    vm.successMessage = function () {
        return ko.Localization('Naati.Resources.Finance.resources.PaidSuccessfully').format(vm.invoiceId());
    };

    vm.activate = function (invoiceId) {
        vm.processing(false);

        financeService.getFluid('paymentoptions').then(function (data) {
            vm.paymentAccount = data.PaymentAccount;
        });

        if (invoiceId) {
            vm.showInvoiceSelector(false);
            vm.invoiceId(invoiceId);
            vm.searchOptions.textValue(invoiceId);
        }
        else {
            ko.viewmodel.updateFromModel(vm.invoice, cleanServerModel);
        }

        serverModel.Payments([]);
    };

    vm.clean = function () {
        vm.processing(false);
        ko.viewmodel.updateFromModel(vm.invoice, cleanServerModel);
        vm.searchOptions.textValue('');
    };

    vm.downloadUrl = function () {
        return financeService.url() +
            '/downloadInvoice?number=' + serverModel.InvoiceNumber() +
            '&type=' + (serverModel.Type() === CONST.invoiceTypes.creditNote ? 'CreditNote' : 'Invoice') +
            '&location=Wiise';
    };

    function save() {
        if (vm.processing()) {
            return;
        }

        var invoice = ko.toJS(vm.invoice);

        $.each(invoice.Payments, function (i, p) {
            p.InvoiceNumber = invoice.InvoiceNumber;
            p.AccountId = vm.paymentAccount.Id;
        });

        vm.processing(true);
        financeService.post(invoice.Payments, 'payment').then(checkFail);
    }

    function checkFail(data) {
        vm.processing(false);

        if (!data.Error) {
            serverModel.AllowEdit(false);
            $('#paidSuccessfullyModal').modal('show');

            return;
        }

        message.confirm({
            title: ko.Localization('Naati.Resources.Shared.resources.Warning'),
            content: ko.Localization('Naati.Resources.Finance.resources.ErrorOnPaymentCreation')
        }).then(function (answer) {
            if (answer === 'yes') {
                financeService.post({ OperationId: data.OperationId }, 'retryPayment').then(checkFail);
            }
            else {
                financeService.post({ OperationId: data.OperationId }, 'cancel');
            }
        });
    }

    function selectResult() {
        var result = vm.search();
        if (!result) {
            vm.invoiceId(null);
            return;
        }

        if (result.Type === 'I' || result.Type === 'C') {
            vm.invoiceId(result.EntityId);
            vm.showInvoiceSelector(false);
        } else {
            vm.invoiceId(null);
            vm.showInvoiceSelector(true);

            financeService.getFluid('invoice', { NaatiNumber: [result.NaatiNumber] }).then(
                function (data) {
                    if (data) {
                        $.each(data,
                            function (i, e) {
                                e.Text = '{0} #{1} - {2}'.format(ko.Localization('Naati.Resources.Finance.resources.' + (e.Type === 1 ? 'Invoice' : 'CreditNote')), e.InvoiceNumber, e.WiiseReference);
                                checkCreditNote(e);
                            });
                        vm.invoices(data);
                    }
                });
        }
    }

    function selectEntity() {
        var invoiceId = vm.invoiceId();
        if (!invoiceId) {
            ko.viewmodel.updateFromModel(vm.invoice, cleanServerModel);

            return;
        }

        financeService.getFluid('invoice', { InvoiceNumber: invoiceId }).then(function (data) {
            enableDisableDataTableButtons();

            if (data && data.length) {
                var invoice = data[0];
                checkCreditNote(invoice);
                invoice.AllowEdit = true;

                if (!invoice.InvoiceNumber) {
                    message.alert({
                        title: ko.Localization('Naati.Resources.Shared.resources.Warning'),
                        content: ko.Localization('Naati.Resources.Finance.resources.InvoiceNotInWiise')
                    });

                    invoice.AllowEdit = false;
                }

                if (invoice.Payments && invoice.Payments.length) {
                    message.alert({
                        title: ko.Localization('Naati.Resources.Shared.resources.Warning'),
                        content: ko.Localization('Naati.Resources.Finance.resources.InvoiceHasPaymens')
                    });

                    invoice.AllowEdit = false;
                }

                ko.utils.arrayForEach(invoice.Payments, function (p) {
                    p.OfficeObject = { Id: p.OfficeId, Name: p.Office };
                    p.PaymentTypeObject = { name: p.PaymentType };
                    p.DatePaidFormatted = moment(p.Date).format();
                });

                ko.viewmodel.updateFromModel(vm.invoice, invoice);
            }
        });
    }

    function retryEnableDisableDataTableButtons() {
        setTimeout(function () {
            enableDisableDataTableButtons();
        }, 100);
    }

    function enableDisableDataTableButtons() {
        if (!$.fn.DataTable.isDataTable($('#paymentsTable'))) {
            retryEnableDisableDataTableButtons();

            return;
        }

        var buttons = $('#paymentsTable').DataTable().buttons('*');
        if (!buttons.length) {
            retryEnableDisableDataTableButtons();

            return;
        }

        if (serverModel.InvoiceNumber() && vm.balance() && serverModel.AllowEdit()) {
            buttons.enable();
        }
        else {
            buttons.disable();
        }
    }

    function addPayment() {
        vm.addDetailInstance.add(vm.paymentAccount.Name, vm.balance()).then(function (payment) {
            payment = parsePayment(payment);
            serverModel.Payments.push(payment);
        });
    }

    function parsePayment(payment) {
        return $.extend({
            DatePaidFormatted: moment(payment.DatePaid).format()
        }, payment);
    }

    function checkCreditNote(invoice) {
        if (invoice.Type === 2) {
            invoice.Balance = -invoice.Balance;
            invoice.Total = -invoice.Total;
        }
    }

    return vm;
});
