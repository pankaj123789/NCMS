define([
    'views/shell',
    'modules/enums',
    'services/screen/date-service',
    'services/finance-data-service',
], function (shell, enums, dateService, financeService) {
    var tableId = 'financeQueueTable';

    var vm = {
        searchComponentOptions: {
            title: shell.titleWithSmall,
            filters: [
                {
                    id: 'TestDateFromAndTo',
                    name: 'Naati.Resources.Finance.resources.DateRequested',
                },
                { id: 'AccountingOperationStatus' }
            ],
            searchType: enums.SearchTypes.FinanceQueue,
            tableDefinition: {
                id: tableId,
                headerTemplate: 'finance-queue-header-template',
                rowTemplate: 'finance-queue-row-template'
            }
        },
        searchTerm: ko.observable({}),
        queue: ko.observableArray([]),
        parseSearchTerm: function (searchTerm) {
            var accountingOperationStatus = [];

            if (!searchTerm.AccountingOperationStatus && (!searchTerm.TestDateFromAndTo || !searchTerm.TestDateFromAndTo.Data.From && !searchTerm.TestDateFromAndTo.Data.To)) {
                accountingOperationStatus = ['Failed', 'Requested'];
            }

            return JSON.stringify({
                Statuses: searchTerm.AccountingOperationStatus ? searchTerm.AccountingOperationStatus.Data.Options : accountingOperationStatus,
                RequestedFrom: searchTerm.TestDateFromAndTo && searchTerm.TestDateFromAndTo.Data.From ? searchTerm.TestDateFromAndTo.Data.From : null,
                RequestedTo: searchTerm.TestDateFromAndTo && searchTerm.TestDateFromAndTo.Data.To ? searchTerm.TestDateFromAndTo.Data.To : null
            });
        },
        getQueue: function () {
            financeService.getFluid('financequeue', { request: vm.parseSearchTerm(vm.searchTerm()) }).then(function (data) {
                ko.utils.arrayForEach(data, function (d) {
                    d.CanReprocess = d.StatusId === enums.AccountingOperationStatusId.Failed || d.StatusId === enums.AccountingOperationStatusId.Requested;
                });
                vm.queue(data);
            });
        },
        submitToWiise: function (operation) {
            var type = operation.TypeId === enums.AccountingOperationTypeId.CreateInvoice ? 'retryInvoice' : 'retryPayment';
            financeService.post({ OperationId: operation.Id }, type).then(function (data) {
                if (!data.Error) {
                    toastr.success(ko.Localization('Naati.Resources.Finance.resources.OperationReprocessedSuccessfully'));
                    vm.getQueue();
                }
            });
        }
    };

    $.extend(true, vm, {
        searchComponentOptions: {
            searchCallback: vm.getQueue,
            searchTerm: vm.searchTerm,
            tableDefinition: {
                dataTable: {
                    source: vm.queue,
                    order: [
                        [0, "desc"]
                    ],
                    columnDefs: [
                        { targets: -1, orderable: false },
                        { targets: [3, 4], render: $.fn.dataTable.render.nullableDate(CONST.settings.shortDateTimeDisplayFormat) }
                    ]
                }
            }
        }
    });

    return vm;
});
