define([
    'plugins/router',
    'views/shell',
    'modules/enums',
    'services/screen/date-service',
    'services/screen/message-service',
    'services/panel-data-service',
    'services/panel-member-data-service',
    'views/panels/create'
], function (router, shell, enums, dateService, messageService, panelDataService, panelMemberDataService, createPanelModel) {
    var tableId = 'panelTable';

    var vm = {
        searchComponentOptions: {
            title: shell.titleWithSmall,
            filters: [
                { id: 'NAATINumber' },
                { id: 'Language', isDefault: true },
                { id: 'EndDate' },
                { id: 'PanelType' },
                { id: 'MaterialRequest' },        
            ],
            searchType: enums.SearchTypes.Panel,
            tableDefinition: {
                id: tableId,
                headerTemplate: 'panel-header-template',
                rowTemplate: 'panel-row-template'
            }
        },
        searchTerm: ko.observable({}),
        panels: ko.observableArray([]),
        showOutOfDateAlert: ko.observable(false),
        createPanelInstance: createPanelModel.getInstance(),
        parseSearchTerm: function (searchTerm) {
            return JSON.stringify({
                NAATINumber: searchTerm.NAATINumber ? searchTerm.NAATINumber.Data.NAATINumber : null,
                LanguageId: searchTerm.Language ? searchTerm.Language.Data.Options : null,
                EndDate: searchTerm.EndDate ? dateService.format(searchTerm.EndDate.Data.value) : null,
                PanelTypeId: searchTerm.PanelType ? searchTerm.PanelType.Data.Options : null,
                MaterialRequestTitle: searchTerm.MaterialRequest ? searchTerm.MaterialRequest.Data.Title.Data.value : null,
                MaterialRequestStatusId: searchTerm.MaterialRequest ? searchTerm.MaterialRequest.Data.Status.Data.selectedOptions : null,
                MaterialRequestCredentialTypeId: searchTerm.MaterialRequest ? searchTerm.MaterialRequest.Data.CredentialType.Data.selectedOptions : null,
                MaterialRequestTaskTypeId: searchTerm.MaterialRequest ? searchTerm.MaterialRequest.Data.TaskType.Data.selectedOptions : null
            });
        },
        getPanelList: function () {
            panelDataService.get({ request: vm.parseSearchTerm(vm.searchTerm()) }).then(function (data) {
                ko.utils.arrayForEach(data, function (panel) {
                    panel.panelMembers = ko.observableArray([]);
                    panel.ShowDetails = ko.observable(false);
                    panel.Chevron = ko.pureComputed(function () {
                        return panel.ShowDetails() ? 'fa-chevron-down' : 'fa-chevron-right';
                    });
                });

                vm.showOutOfDateAlert(false);
                vm.panels(data);

                if (data.length == 1) {
                    router.navigate('panel/' + data[0].PanelId);
                }
            });
        },
        refreshPanelList: function () {
            if (vm.panels().length > 0) {
                vm.getPanelList();
            }
        },
        create: function () {
            vm.createPanelInstance.createPanel().then(function (panel) {
                vm.showOutOfDateAlert(true);
                router.navigate('panel/' + panel.PanelId);
            });
        },
        toggleDetails: function (panel, e) {
            var parsedRequest = JSON.stringify({
                PanelId: panel.PanelId,
                StartDate: null,
                EndDate: dateService.today()
            });

            panelMemberDataService.get({ request: parsedRequest }).then(function (data) {
                ko.utils.arrayForEach(data, function (membership) {
                    membership.StartDateFormatted = moment(membership.StartDate).format(CONST.settings.shortDateDisplayFormat);
                    membership.EndDateFormatted = moment(membership.EndDate).format(CONST.settings.shortDateDisplayFormat);
                });

                panel.panelMembers(data);
                panel.ShowDetails(!panel.ShowDetails());

                var tableRow = $(e.target).closest('tr');
                var dataTable = tableRow.closest('#' + tableId).DataTable();
                var dataTableRow = dataTable.row(tableRow);

                if (panel.ShowDetails()) {
                    var panelDetailsTemplate = ko.generateTemplate('panel-details-template', { panelMembers: panel.panelMembers });
                    dataTableRow.child(panelDetailsTemplate).show();
                } else {
                    dataTableRow.child.hide();
                }
            });
        }
    };

    $.extend(true, vm, {
        searchComponentOptions: {
            searchCallback: vm.getPanelList,
            additionalButtons: [{
                'class': 'btn btn-success',
                icon: 'glyphicon glyphicon-plus',
                resourceName: 'Naati.Resources.Shared.resources.NewPanel',
                click: vm.create,
                enableWithPermission: 'Panel.Create'
                
            }],
            showOutOfDateAlert: vm.showOutOfDateAlert,
            searchTerm: vm.searchTerm,
            tableDefinition: {
                dataTable: {
                    source: vm.panels,
                    columnDefs: [
                        { targets: 4, orderable: false },
                        { targets: 3, render: $.fn.dataTable.render.moment(moment.ISO_8601, CONST.settings.shortDateDisplayFormat) }
                    ]
                }
            },
            idSearchOptions: {
                search: function (value) {
                    var filter = JSON.stringify({ PanelId: value });
                    panelDataService.get({ request: filter, supressResponseMessages: true })
                        .then(function (data) {
                            if (!data || !data.length) {
                                return toastr.success(ko.Localization('Naati.Resources.Shared.resources.NotFound').format('Panel', value));
                            }
                            router.navigate('panel/' + value);
                        });
                }
            }
        }
    });

    return vm;
});
