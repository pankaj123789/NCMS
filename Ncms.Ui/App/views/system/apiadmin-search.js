define([
	'plugins/router',
    'services/setup-data-service',
    'modules/enums',
    'views/system/apiadmin-create',
    'services/apiadmin-data-service',
    'modules/enums'
], function (router, setupService, enums, apiAdminCreate, apiAdminService) {
	var searchTerm = ko.observable({});
	var tableId = 'apiClientTable';
	var tableDefinition = {
		id: tableId,
		headerTemplate: 'apiclient-header-template',
		rowTemplate: 'apiclient-row-template'
	};

	var vm = {
		apiClients: ko.observableArray([]),
        canEdit: ko.observable(false),
        computedApiClients: ko.observableArray([]),
		tableDefinition: tableDefinition,
		create: create,
        idSearchValue: ko.observable(),
        showInactive: ko.observable(false)
    };

    ko.computed(function () {
        vm.computedApiClients([]);
        var apiClients = ko.utils.arrayFilter(vm.apiClients(), function (v) {
            return v.Active || (!v.Active && vm.showInactive());
        });
        vm.computedApiClients(apiClients);
    });

    vm.activate = function () {
        vm.showInactive(false);
        apiAdminService.getFluid('apiAdminSearch').then(function (data) {
            vm.apiClients(data);
        });

    };

    var apiAdminCreateInstance = apiAdminCreate.getInstance();

	vm.apiAdminCreateOptions = {
		view: 'views/system/apiadmin-create.html',
		model: apiAdminCreateInstance
	};

	function create() {
		apiAdminCreateInstance.show().then(function (data) {
			apiAdminCreateInstance.hide();
			router.navigate('system/apiadmin/' + data.Id);
		});
	}

	function parseSearchTerm(searchTerm) {
		var json = {
		};

		return JSON.stringify(json);
	}

    vm.activate = function() {
        vm.canEdit(false);
        $.fn.dataTable.moment(CONST.settings.shortDateDisplayFormat);

        setupService.getFluid('apiadmin', { request: { Skip: null, Take: 500, Filter: parseSearchTerm(searchTerm()) } })
            .then(vm.apiClients);

        currentUser.hasPermission(enums.SecNoun.System, enums.SecVerb.Manage).then(vm.canEdit);
    };

    vm.idSearchClick = function () {
        var filter = JSON.stringify({ ApiAdminIdIntList: [vm.idSearchValue()] });
        setupService.getFluid('apiadmin', { request: { Skip: null, Take: 2, Filter: filter }, supressResponseMessages: true })
            .then(function (data) {
                if (!data || !data.length) {
                    return toastr.success(ko.Localization('Naati.Resources.Shared.resources.NotFound').format('Api Admin', vm.idSearchValue()));
                }
                router.navigate('system/apiadmin/' + vm.idSearchValue());
            });
    };

    vm.checkEnterPressed = function (data, e) {
        var keyCode = e.which || e.keyCode;
        if (keyCode == 13) {
            vm.idSearchClick();
            return false;
        }
        return true;
    };

	vm.activeMessage = function (apiAdmin) {
		if (!apiAdmin.Inactive) {
			return ko.Localization('Naati.Resources.Shared.resources.Active');
		}

		return ko.Localization('Naati.Resources.Shared.resources.NotActive');
		};

    vm.tableDefinition.dataTable = {
        source: vm.computedApiClients,
        columnDefs: [
            { targets: -2, orderable: false }
        ],
        order: [
            [1, "asc"],
            [2, "asc"]
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
                text: '<input type="checkbox" /><i></i><span>' + ko.Localization('Naati.Resources.SystemResources.resources.ShowInactive') + '</span>',
                className: 'i-switch i-switch-md bg-info m-l',
                init: function (dt, node, config) {
                    node.children('input[type="checkbox"]').prop('checked', vm.showInactive());
                },
                action: function (e, dt, node, config) {
                    var checked = !node.children('input[type="checkbox"]').prop('checked');
                    node.children('input[type="checkbox"]').prop('checked', checked);
                    vm.showInactive(checked);
                }
            }]
        }
    };

	return vm;
});