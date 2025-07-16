define([],
function () {
    return {
        getInstance: getInstance
    };

    function getInstance() {
        var vm = {
            payments: ko.observableArray(),
            details: ko.observableArray(),
            tableDefinition: {
                id: 'invoicesTable',
                headerTemplate: 'invoice-header-template',
                rowTemplate: 'invoice-row-template'
            }
        };

        vm.tableDefinition.dataTable = {
            source: vm.details,
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
                    text: '<span class="glyphicon glyphicon-export"></span><span>' +
                        ko.Localization('Naati.Resources.Shared.resources.ExportToCSV') + '</span>',
                    className: 'btn btn-default',
                    enabled: false,
                    extend: 'csv',
                    title: 'Invoices {0}'.format(moment().format('YYYY-MM-DD'))
                }]
            },
            initComplete: enableDisableDataTableButtons
        };

        function enableDisableDataTableButtons() {
            var $table = $('#' + vm.tableDefinition.id);

            if (!$.fn.DataTable.isDataTable($table)) {
                setTimeout(enableDisableDataTableButtons, 100);
                return;
            }

            var buttons = $table.DataTable().buttons('*');
            if (!buttons.length) {
                setTimeout(enableDisableDataTableButtons, 100);
                return;
            }

            var disable = vm.details().length === 0;
            if (disable) {
                buttons.disable();
            }
            else {
                buttons.enable();
            }
        };

        vm.selectInvoice = function (i, e) {
            var target = $(e.target);
            var tr = target.closest('tr');
            var dt = tr.closest('#' + vm.tableDefinition.id).DataTable();
            var row = dt.row(tr);

            if (row.child.isShown()) {
                target.removeClass('fa-chevron-down').addClass('fa-chevron-right');
                tr.removeClass('details');
                row.child.hide();
            }
            else {
                target.removeClass('fa-chevron-right').addClass('fa-chevron-down');
                tr.addClass('details');
                vm.payments(i.Payments);
                setTimeout(function () {
                    row.child($('#invoiceRowDetail').html()).show();
                }, 100);
            }
        };

        return vm;
    }
});
