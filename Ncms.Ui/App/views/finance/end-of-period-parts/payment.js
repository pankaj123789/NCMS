define([], function () {
    return {
        getInstance: getInstance
    };

    function getInstance() {
        var vm = {
            details: ko.observableArray(),
            tableDefinition: {
                id: 'paymentsTable',
                headerTemplate: 'end-of-period-payment-header-template',
                rowTemplate: 'end-of-period-payment-row-template'
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
                    title: 'Payments {0}'.format(moment().format('YYYY-MM-DD'))
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
        }

        return vm;
    }
});
