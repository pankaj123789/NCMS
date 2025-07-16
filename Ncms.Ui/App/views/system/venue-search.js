define([
	'plugins/router',
    'services/setup-data-service',
    'modules/enums',
    'views/system/venue-create',
    'services/venue-data-service',
    'modules/enums'
], function (router, setupService, enums, venueCreate, venueService) {
	var searchTerm = ko.observable({});
	var tableId = 'venueTable';
	var tableDefinition = {
		id: tableId,
		headerTemplate: 'venues-header-template',
		rowTemplate: 'venues-row-template'
	};

	var vm = {
		venues: ko.observableArray([]),
        canEdit: ko.observable(false),
        computedVenues: ko.observableArray([]),
		tableDefinition: tableDefinition,
		create: create,
        idSearchValue: ko.observable(),
        showInactive: ko.observable(false)
    };

    ko.computed(function () {
        vm.computedVenues([]);
        var venues = ko.utils.arrayFilter(vm.venues(), function (v) {
            return !v.Inactive || (v.Inactive && vm.showInactive());
        });
        vm.computedVenues(venues);
    });

    vm.activate = function () {
        vm.showInactive(false);
        venueService.getFluid('venueSearch').then(function (data) {
            vm.venues(data);
        });

    };

	var venueCreateInstance = venueCreate.getInstance();

	vm.venueCreateOptions = {
		view: 'views/system/venue-create.html',
		model: venueCreateInstance
	};

	function create() {
		venueCreateInstance.show().then(function (data) {
			venueCreateInstance.hide();
			router.navigate('system/venue/' + data.Id);
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

        setupService.getFluid('venue', { request: { Skip: null, Take: 500, Filter: parseSearchTerm(searchTerm()) } })
            .then(vm.venues);

        currentUser.hasPermission(enums.SecNoun.Venue, enums.SecVerb.Update).then(vm.canEdit);
    };

    vm.idSearchClick = function () {
        var filter = JSON.stringify({ VenueIdIntList: [vm.idSearchValue()] });
        setupService.getFluid('venue', { request: { Skip: null, Take: 2, Filter: filter }, supressResponseMessages: true })
            .then(function (data) {
                if (!data || !data.length) {
                    return toastr.success(ko.Localization('Naati.Resources.Shared.resources.NotFound').format('Venue', vm.idSearchValue()));
                }
                router.navigate('system/venue/' + vm.idSearchValue());
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

	vm.activeMessage = function (venue) {
		if (!venue.Inactive) {
			return ko.Localization('Naati.Resources.Shared.resources.Active');
		}

		return ko.Localization('Naati.Resources.Shared.resources.NotActive');
		};

    vm.tableDefinition.dataTable = {
        source: vm.computedVenues,
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